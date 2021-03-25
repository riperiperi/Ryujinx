using Ryujinx.Common.Configuration;
using Ryujinx.Graphics.GAL.Multithreading.Commands;
using Ryujinx.Graphics.GAL.Multithreading.Commands.Buffer;
using Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer;
using Ryujinx.Graphics.GAL.Multithreading.Resources;
using Ryujinx.Graphics.Shader;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading;

namespace Ryujinx.Graphics.GAL.Multithreading
{
    /// <summary>
    /// The ThreadedRenderer is a layer that can be put in front of any Renderer backend to make
    /// its processing happen on a separate thread, rather than intertwined with the GPU emulation.
    /// A new thread is created to handle the GPU command processing, separate from the renderer thread.
    /// Calls to the renderer, pipeline and resources are queued to happen on the renderer thread.
    /// </summary>
    public class ThreadedRenderer : IRenderer
    {
        private IRenderer _baseRenderer;
        private Thread _gpuThread;
        private bool _disposed;
        private bool _running;

        private AutoResetEvent _frameComplete = new AutoResetEvent(true);

        private ManualResetEventSlim _galWorkAvailable;
        private ConcurrentQueue<IGALCommand> _galQueue;

        private IGALCommand _invokeCommand;
        private ManualResetEventSlim _invokeRun;

        private bool _lastSampleCounterClear = true;

        internal BufferMap Buffers { get; }
        internal SyncMap Sync { get; }

        public IPipeline Pipeline { get; }
        public IWindow Window { get; }

        public IRenderer BaseRenderer => _baseRenderer;

        public ThreadedRenderer(IRenderer renderer)
        {
            _baseRenderer = renderer;

            Pipeline = new ThreadedPipeline(this, renderer.Pipeline);
            Window = new ThreadedWindow(this, renderer.Window);
            Buffers = new BufferMap();
            Sync = new SyncMap();

            _galWorkAvailable = new ManualResetEventSlim(false);
            _galQueue = new ConcurrentQueue<IGALCommand>();
            _invokeRun = new ManualResetEventSlim();
        }

        public void RunLoop(Action gpuLoop)
        {
            _running = true;

            _gpuThread = new Thread(() => {
                gpuLoop();
                _running = false;
                _galWorkAvailable.Set();
                });

            _gpuThread.Start();

            RenderLoop();
        }

        public void RenderLoop()
        {
            // Power through the render queue until the Gpu thread work is done.

            while (_running && !_disposed)
            {
                _galWorkAvailable.Wait();
                _galWorkAvailable.Reset();

                while (_galQueue.TryDequeue(out IGALCommand command))
                {
                    command.Run(this, _baseRenderer);

                    if (command == _invokeCommand)
                    {
                        _invokeRun.Set();
                    }
                }
            }
        }

        internal IMemoryOwner<T> CopySpan<T>(ReadOnlySpan<T> data) where T : unmanaged
        {
            IMemoryOwner<T> memory = MemoryPool<T>.Shared.Rent(data.Length);

            data.CopyTo(memory.Memory.Span);

            return memory;
        }

        internal void QueueCommand(IGALCommand command)
        {
            _galQueue.Enqueue(command);
            _galWorkAvailable.Set();
        }

        internal void InvokeCommand(IGALCommand command)
        {
            _invokeRun.Reset();
            _invokeCommand = command;

            _galQueue.Enqueue(command);
            _galWorkAvailable.Set();

            // Wait for the command to complete.
            _invokeRun.Wait();
            _invokeCommand = null;
        }


        internal void WaitForFrame()
        {
            _frameComplete.WaitOne();
        }

        internal void SignalFrame()
        {
            _frameComplete.Set();
        }

        internal bool IsGpuThread()
        {
            return Thread.CurrentThread == _gpuThread;
        }

        public void BackgroundContextAction(Action action)
        {
            if (IsGpuThread())
            {
                // The action must be performed on the render thread.
                InvokeCommand(new ActionCommand(action));
            }
            else
            {
                _baseRenderer.BackgroundContextAction(action);
            }
        }

        public IShader CompileShader(ShaderStage stage, string code)
        {
            var shader = new ThreadedShader(this);
            var cmd = new CompileShaderCommand(shader, stage, code);
            QueueCommand(cmd);

            return shader;
        }

        public BufferHandle CreateBuffer(int size)
        {
            BufferHandle handle = Buffers.CreateBufferHandle();
            var cmd = new CreateBufferCommand(handle, size);
            QueueCommand(cmd);

            return handle;
        }

        public IProgram CreateProgram(IShader[] shaders, TransformFeedbackDescriptor[] transformFeedbackDescriptors)
        {
            var program = new ThreadedProgram(this);
            var cmd = new CreateProgramCommand(program, shaders, transformFeedbackDescriptors);
            QueueCommand(cmd);

            return program;
        }

        public ISampler CreateSampler(SamplerCreateInfo info)
        {
            var sampler = new ThreadedSampler(this);
            var cmd = new CreateSamplerCommand(sampler, info);
            QueueCommand(cmd);

            return sampler;
        }

        public void CreateSync(ulong id)
        {
            Sync.CreateSyncHandle(id);
            QueueCommand(new CreateSyncCommand(id));
        }

        public ITexture CreateTexture(TextureCreateInfo info, float scale)
        {
            if (IsGpuThread())
            {
                var texture = new ThreadedTexture(this, info, scale);
                var cmd = new CreateTextureCommand(texture, info, scale);
                QueueCommand(cmd);

                return texture;
            }
            else
            {
                var texture = new ThreadedTexture(this, info, scale);
                texture.Base = _baseRenderer.CreateTexture(info, scale);

                return texture;
            }
        }

        public void DeleteBuffer(BufferHandle buffer)
        {
            QueueCommand(new BufferDisposeCommand(buffer));
        }

        public byte[] GetBufferData(BufferHandle buffer, int offset, int size)
        {
            if (IsGpuThread())
            {
                var cmd = new BufferGetDataCommand(buffer, offset, size);
                InvokeCommand(cmd);

                return cmd.Result;
            }
            else
            {
                return _baseRenderer.GetBufferData(Buffers.MapBufferBlocking(buffer), offset, size);
            }
        }

        public Capabilities GetCapabilities()
        {
            var cmd = new GetCapabilitiesCommand();
            InvokeCommand(cmd);

            return cmd.Result;
        }

        /// <summary>
        /// Initialize the base renderer. Must be called on the render thread.
        /// </summary>
        /// <param name="logLevel">Log level to use</param>
        public void Initialize(GraphicsDebugLevel logLevel)
        {
            _baseRenderer.Initialize(logLevel);
        }

        public IProgram LoadProgramBinary(byte[] programBinary)
        {
            var program = new ThreadedProgram(this);
            var cmd = new LoadProgramBinaryCommand(program, programBinary);
            QueueCommand(cmd);

            return program;
        }

        public void PreFrame()
        {
            QueueCommand(new PreFrameCommand());
        }

        public ICounterEvent ReportCounter(CounterType type, EventHandler<ulong> resultHandler)
        {
            ThreadedCounterEvent evt = new ThreadedCounterEvent(this, type, _lastSampleCounterClear);
            QueueCommand(new ReportCounterCommand(evt, type, resultHandler));

            if (type == CounterType.SamplesPassed)
            {
                _lastSampleCounterClear = false;
            }

            return evt;
        }

        public void ResetCounter(CounterType type)
        {
            QueueCommand(new ResetCounterCommand(type));
            _lastSampleCounterClear = true;
        }

        public void SetBufferData(BufferHandle buffer, int offset, ReadOnlySpan<byte> data)
        {
            QueueCommand(new BufferSetDataCommand(buffer, offset, CopySpan(data), data.Length));
        }

        public void UpdateCounters()
        {
            QueueCommand(new UpdateCountersCommand());
        }

        public void WaitSync(ulong id)
        {
            Sync.WaitSyncAvailability(id);

            _baseRenderer.WaitSync(id);
        }

        public void Dispose()
        {
            // Dispose must happen from the render thread, after all commands have completed.

            // Stop the GPU thread.
            _disposed = true;
            _gpuThread.Join();

            // Dispose the renderer.
            _baseRenderer.Dispose();
        }
    }
}
