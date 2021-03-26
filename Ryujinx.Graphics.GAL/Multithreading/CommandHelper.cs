using Ryujinx.Graphics.GAL.Multithreading.Commands;
using Ryujinx.Graphics.GAL.Multithreading.Commands.Buffer;
using Ryujinx.Graphics.GAL.Multithreading.Commands.CounterEvent;
using Ryujinx.Graphics.GAL.Multithreading.Commands.Program;
using Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer;
using Ryujinx.Graphics.GAL.Multithreading.Commands.Sampler;
using Ryujinx.Graphics.GAL.Multithreading.Commands.Shader;
using Ryujinx.Graphics.GAL.Multithreading.Commands.Texture;
using Ryujinx.Graphics.GAL.Multithreading.Commands.Window;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ryujinx.Graphics.GAL.Multithreading
{
    static class CommandHelper
    {
        private static void RunTypedCommand<T>(Span<byte> memory, ThreadedRenderer threaded, IRenderer renderer) where T : unmanaged, IGALCommand
        {
            Unsafe.As<byte, T>(ref MemoryMarshal.GetReference(memory)).Run(threaded, renderer);
        }
        
        public static void RunCommand(Span<byte> memory, ThreadedRenderer threaded, IRenderer renderer)
        {
            switch ((CommandType)memory[memory.Length - 1])
            {
                case CommandType.Action:
                    RunTypedCommand<ActionCommand>(memory, threaded, renderer);
                    break;
                case CommandType.CompileShader:
                    RunTypedCommand<CompileShaderCommand>(memory, threaded, renderer);
                    break;
                case CommandType.CreateBuffer:
                    RunTypedCommand<CreateBufferCommand>(memory, threaded, renderer);
                    break;
                case CommandType.CreateProgram:
                    RunTypedCommand<CreateProgramCommand>(memory, threaded, renderer);
                    break;
                case CommandType.CreateSampler:
                    RunTypedCommand<CreateSamplerCommand>(memory, threaded, renderer);
                    break;
                case CommandType.CreateSync:
                    RunTypedCommand<CreateSyncCommand>(memory, threaded, renderer);
                    break;
                case CommandType.CreateTexture:
                    RunTypedCommand<CreateTextureCommand>(memory, threaded, renderer);
                    break;
                case CommandType.GetCapabilities:
                    RunTypedCommand<GetCapabilitiesCommand>(memory, threaded, renderer);
                    break;
                case CommandType.LoadProgramBinary:
                    RunTypedCommand<LoadProgramBinaryCommand>(memory, threaded, renderer);
                    break;
                case CommandType.PreFrame:
                    RunTypedCommand<PreFrameCommand>(memory, threaded, renderer);
                    break;
                case CommandType.ReportCounter:
                    RunTypedCommand<ReportCounterCommand>(memory, threaded, renderer);
                    break;
                case CommandType.ResetCounter:
                    RunTypedCommand<ResetCounterCommand>(memory, threaded, renderer);
                    break;
                case CommandType.UpdateCounters:
                    RunTypedCommand<UpdateCountersCommand>(memory, threaded, renderer);
                    break;

                case CommandType.BufferDispose:
                    RunTypedCommand<BufferDisposeCommand>(memory, threaded, renderer);
                    break;
                case CommandType.BufferGetData:
                    RunTypedCommand<BufferGetDataCommand>(memory, threaded, renderer);
                    break;
                case CommandType.BufferSetData:
                    RunTypedCommand<BufferSetDataCommand>(memory, threaded, renderer);
                    break;

                case CommandType.CounterEventDispose:
                    RunTypedCommand<CounterEventDisposeCommand>(memory, threaded, renderer);
                    break;
                case CommandType.CounterEventFlush:
                    RunTypedCommand<CounterEventFlushCommand>(memory, threaded, renderer);
                    break;

                case CommandType.ProgramDispose:
                    RunTypedCommand<ProgramDisposeCommand>(memory, threaded, renderer);
                    break;
                case CommandType.ProgramGetBinary:
                    RunTypedCommand<ProgramGetBinaryCommand>(memory, threaded, renderer);
                    break;

                case CommandType.SamplerDispose:
                    RunTypedCommand<SamplerDisposeCommand>(memory, threaded, renderer);
                    break;

                case CommandType.ShaderDispose:
                    RunTypedCommand<ShaderDisposeCommand>(memory, threaded, renderer);
                    break;

                case CommandType.TextureCopyTo:
                    RunTypedCommand<TextureCopyToCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureCopyToScaled:
                    RunTypedCommand<TextureCopyToScaledCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureCopyToSlice:
                    RunTypedCommand<TextureCopyToSliceCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureCreateView:
                    RunTypedCommand<TextureCreateViewCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureGetData:
                    RunTypedCommand<TextureGetDataCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureRelease:
                    RunTypedCommand<TextureReleaseCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureSetData:
                    RunTypedCommand<TextureSetDataCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureSetDataSlice:
                    RunTypedCommand<TextureSetDataSliceCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureSetStorage:
                    RunTypedCommand<TextureSetStorageCommand>(memory, threaded, renderer);
                    break;

                case CommandType.WindowPresent:
                    RunTypedCommand<WindowPresentCommand>(memory, threaded, renderer);
                    break;

                case CommandType.Barrier:
                    RunTypedCommand<BarrierCommand>(memory, threaded, renderer);
                    break;
                case CommandType.BeginTransformFeedback:
                    RunTypedCommand<BeginTransformFeedbackCommand>(memory, threaded, renderer);
                    break;
                case CommandType.ClearBuffer:
                    RunTypedCommand<ClearBufferCommand>(memory, threaded, renderer);
                    break;
                case CommandType.ClearRenderTargetColor:
                    RunTypedCommand<ClearRenderTargetColorCommand>(memory, threaded, renderer);
                    break;
                case CommandType.ClearRenderTargetDepthStencil:
                    RunTypedCommand<ClearRenderTargetDepthStencilCommand>(memory, threaded, renderer);
                    break;
                case CommandType.CopyBuffer:
                    RunTypedCommand<CopyBufferCommand>(memory, threaded, renderer);
                    break;
                case CommandType.DispatchCompute:
                    RunTypedCommand<DispatchComputeCommand>(memory, threaded, renderer);
                    break;
                case CommandType.Draw:
                    RunTypedCommand<DrawCommand>(memory, threaded, renderer);
                    break;
                case CommandType.DrawIndexed:
                    RunTypedCommand<DrawIndexedCommand>(memory, threaded, renderer);
                    break;
                case CommandType.EndHostConditionalRendering:
                    RunTypedCommand<EndHostConditionalRenderingCommand>(memory, threaded, renderer);
                    break;
                case CommandType.EndTransformFeedback:
                    RunTypedCommand<EndTransformFeedbackCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetAlphaTest:
                    RunTypedCommand<SetAlphaTestCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetBlendState:
                    RunTypedCommand<SetBlendStateCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetDepthBias:
                    RunTypedCommand<SetDepthBiasCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetDepthClamp:
                    RunTypedCommand<SetDepthClampCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetDepthMode:
                    RunTypedCommand<SetDepthModeCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetDepthTest:
                    RunTypedCommand<SetDepthTestCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetFaceCulling:
                    RunTypedCommand<SetFaceCullingCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetFrontFace:
                    RunTypedCommand<SetFrontFaceCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetGenericBuffers:
                    RunTypedCommand<SetGenericBuffersCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetImage:
                    RunTypedCommand<SetImageCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetIndexBuffer:
                    RunTypedCommand<SetIndexBufferCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetLogicOpState:
                    RunTypedCommand<SetLogicOpStateCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetPointParameters:
                    RunTypedCommand<SetPointParametersCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetPrimitiveRestart:
                    RunTypedCommand<SetPrimitiveRestartCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetPrimitiveTopology:
                    RunTypedCommand<SetPrimitiveTopologyCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetProgram:
                    RunTypedCommand<SetProgramCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetRasterizerDiscard:
                    RunTypedCommand<SetRasterizerDiscardCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetRenderTargetColorMasks:
                    RunTypedCommand<SetRenderTargetColorMasksCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetRenderTargetScale:
                    RunTypedCommand<SetRenderTargetScaleCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetRenderTargets:
                    RunTypedCommand<SetRenderTargetsCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetSampler:
                    RunTypedCommand<SetSamplerCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetScissor:
                    RunTypedCommand<SetScissorCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetStencilTest:
                    RunTypedCommand<SetStencilTestCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetTexture:
                    RunTypedCommand<SetTextureCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetUserClipDistance:
                    RunTypedCommand<SetUserClipDistanceCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetVertexAttribs:
                    RunTypedCommand<SetVertexAttribsCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetVertexBuffers:
                    RunTypedCommand<SetVertexBuffersCommand>(memory, threaded, renderer);
                    break;
                case CommandType.SetViewports:
                    RunTypedCommand<SetViewportsCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureBarrier:
                    RunTypedCommand<TextureBarrierCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TextureBarrierTiled:
                    RunTypedCommand<TextureBarrierTiledCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TryHostConditionalRendering:
                    RunTypedCommand<TryHostConditionalRenderingCommand>(memory, threaded, renderer);
                    break;
                case CommandType.TryHostConditionalRenderingFlush:
                    RunTypedCommand<TryHostConditionalRenderingFlushCommand>(memory, threaded, renderer);
                    break;
                case CommandType.UpdateRenderScale:
                    RunTypedCommand<UpdateRenderScaleCommand>(memory, threaded, renderer);
                    break;
            }
        }
    }
}
