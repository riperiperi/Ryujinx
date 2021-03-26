namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Buffer
{
    struct BufferDisposeCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.BufferDispose;
        private BufferHandle _buffer;

        public void Set(BufferHandle buffer)
        {
            _buffer = buffer;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.DeleteBuffer(threaded.Buffers.MapBuffer(_buffer));
            threaded.Buffers.UnassignBuffer(_buffer);
        }
    }
}
