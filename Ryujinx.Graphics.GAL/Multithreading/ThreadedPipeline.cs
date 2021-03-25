using Ryujinx.Graphics.GAL.Multithreading.Commands;
using Ryujinx.Graphics.GAL.Multithreading.Resources;
using Ryujinx.Graphics.Shader;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading
{
    public class ThreadedPipeline : IPipeline
    {
        private ThreadedRenderer _renderer;
        private IPipeline _impl;

        private SetGenericBuffersDelegate _setStorageBuffers;
        private SetGenericBuffersDelegate _setTransformFeedbackBuffers;
        private SetGenericBuffersDelegate _setUniformBuffers;

        public ThreadedPipeline(ThreadedRenderer renderer, IPipeline impl)
        {
            _renderer = renderer;
            _impl = impl;

            _setStorageBuffers = impl.SetStorageBuffers;
            _setTransformFeedbackBuffers = impl.SetTransformFeedbackBuffers;
            _setUniformBuffers = impl.SetUniformBuffers;
        }

        public void Barrier()
        {
            _renderer.QueueCommand(new BarrierCommand());
        }

        public void BeginTransformFeedback(PrimitiveTopology topology)
        {
            _renderer.QueueCommand(new BeginTransformFeedbackCommand(topology));
        }

        public void ClearBuffer(BufferHandle destination, int offset, int size, uint value)
        {
            _renderer.QueueCommand(new ClearBufferCommand(destination, offset, size, value));
        }

        public void ClearRenderTargetColor(int index, uint componentMask, ColorF color)
        {
            _renderer.QueueCommand(new ClearRenderTargetColorCommand(index, componentMask, color));
        }

        public void ClearRenderTargetDepthStencil(float depthValue, bool depthMask, int stencilValue, int stencilMask)
        {
            _renderer.QueueCommand(new ClearRenderTargetDepthStencilCommand(depthValue, depthMask, stencilValue, stencilMask));
        }

        public void CopyBuffer(BufferHandle source, BufferHandle destination, int srcOffset, int dstOffset, int size)
        {
            _renderer.QueueCommand(new CopyBufferCommand(source, destination, srcOffset, dstOffset, size));
        }

        public void DispatchCompute(int groupsX, int groupsY, int groupsZ)
        {
            _renderer.QueueCommand(new DispatchComputeCommand(groupsX, groupsY, groupsZ));
        }

        public void Draw(int vertexCount, int instanceCount, int firstVertex, int firstInstance)
        {
            _renderer.QueueCommand(new DrawCommand(vertexCount, instanceCount, firstVertex, firstInstance));
        }

        public void DrawIndexed(int indexCount, int instanceCount, int firstIndex, int firstVertex, int firstInstance)
        {
            _renderer.QueueCommand(new DrawIndexedCommand(indexCount, instanceCount, firstIndex, firstVertex, firstInstance));
        }

        public void EndHostConditionalRendering()
        {
            _renderer.QueueCommand(new EndHostConditionalRenderingCommand());
        }

        public void EndTransformFeedback()
        {
            _renderer.QueueCommand(new EndTransformFeedbackCommand());
        }

        public void SetAlphaTest(bool enable, float reference, CompareOp op)
        {
            _renderer.QueueCommand(new SetAlphaTestCommand(enable, reference, op));
        }

        public void SetBlendState(int index, BlendDescriptor blend)
        {
            _renderer.QueueCommand(new SetBlendStateCommand(index, blend));
        }

        public void SetDepthBias(PolygonModeMask enables, float factor, float units, float clamp)
        {
            _renderer.QueueCommand(new SetDepthBiasCommand(enables, factor, units, clamp));
        }

        public void SetDepthClamp(bool clamp)
        {
            _renderer.QueueCommand(new SetDepthClampCommand(clamp));
        }

        public void SetDepthMode(DepthMode mode)
        {
            _renderer.QueueCommand(new SetDepthModeCommand(mode));
        }

        public void SetDepthTest(DepthTestDescriptor depthTest)
        {
            _renderer.QueueCommand(new SetDepthTestCommand(depthTest));
        }

        public void SetFaceCulling(bool enable, Face face)
        {
            _renderer.QueueCommand(new SetFaceCullingCommand(enable, face));
        }

        public void SetFrontFace(FrontFace frontFace)
        {
            _renderer.QueueCommand(new SetFrontFaceCommand(frontFace));
        }

        public void SetImage(int binding, ITexture texture, Format imageFormat)
        {
            _renderer.QueueCommand(new SetImageCommand(binding, texture as ThreadedTexture, imageFormat));
        }

        public void SetIndexBuffer(BufferRange buffer, IndexType type)
        {
            _renderer.QueueCommand(new SetIndexBufferCommand(buffer, type));
        }

        public void SetLogicOpState(bool enable, LogicalOp op)
        {
            _renderer.QueueCommand(new SetLogicOpStateCommand(enable, op));
        }

        public void SetPointParameters(float size, bool isProgramPointSize, bool enablePointSprite, Origin origin)
        {
            _renderer.QueueCommand(new SetPointParametersCommand(size, isProgramPointSize, enablePointSprite, origin));
        }

        public void SetPrimitiveRestart(bool enable, int index)
        {
            _renderer.QueueCommand(new SetPrimitiveRestartCommand(enable, index));
        }

        public void SetPrimitiveTopology(PrimitiveTopology topology)
        {
            _renderer.QueueCommand(new SetPrimitiveTopologyCommand(topology));
        }

        public void SetProgram(IProgram program)
        {
            _renderer.QueueCommand(new SetProgramCommand(program as ThreadedProgram));
        }

        public void SetRasterizerDiscard(bool discard)
        {
            _renderer.QueueCommand(new SetRasterizerDiscardCommand(discard));
        }

        public void SetRenderTargetColorMasks(ReadOnlySpan<uint> componentMask)
        {
            _renderer.QueueCommand(new SetRenderTargetColorMasksCommand(_renderer.CopySpan(componentMask), componentMask.Length));
        }

        public void SetRenderTargets(ITexture[] colors, ITexture depthStencil)
        {
            _renderer.QueueCommand(new SetRenderTargetsCommand(colors, depthStencil));
        }

        public void SetRenderTargetScale(float scale)
        {
            _renderer.QueueCommand(new SetRenderTargetScaleCommand(scale));
        }

        public void SetSampler(int binding, ISampler sampler)
        {
            _renderer.QueueCommand(new SetSamplerCommand(binding, sampler as ThreadedSampler));
        }

        public void SetScissor(int index, bool enable, int x, int y, int width, int height)
        {
            _renderer.QueueCommand(new SetScissorCommand(index, enable, x, y, width, height));
        }

        public void SetStencilTest(StencilTestDescriptor stencilTest)
        {
            _renderer.QueueCommand(new SetStencilTestCommand(stencilTest));
        }

        public void SetStorageBuffers(ReadOnlySpan<BufferRange> buffers)
        {
            _renderer.QueueCommand(new SetGenericBuffersCommand(_renderer.CopySpan(buffers), buffers.Length, _setStorageBuffers));
        }

        public void SetTexture(int binding, ITexture texture)
        {
            _renderer.QueueCommand(new SetTextureCommand(binding, texture as ThreadedTexture));
        }

        public void SetTransformFeedbackBuffers(ReadOnlySpan<BufferRange> buffers)
        {
            _renderer.QueueCommand(new SetGenericBuffersCommand(_renderer.CopySpan(buffers), buffers.Length, _setTransformFeedbackBuffers));
        }

        public void SetUniformBuffers(ReadOnlySpan<BufferRange> buffers)
        {
            _renderer.QueueCommand(new SetGenericBuffersCommand(_renderer.CopySpan(buffers), buffers.Length, _setUniformBuffers));
        }

        public void SetUserClipDistance(int index, bool enableClip)
        {
            _renderer.QueueCommand(new SetUserClipDistanceCommand(index, enableClip));
        }

        public void SetVertexAttribs(ReadOnlySpan<VertexAttribDescriptor> vertexAttribs)
        {
            _renderer.QueueCommand(new SetVertexAttribsCommand(_renderer.CopySpan(vertexAttribs), vertexAttribs.Length));
        }

        public void SetVertexBuffers(ReadOnlySpan<VertexBufferDescriptor> vertexBuffers)
        {
            _renderer.QueueCommand(new SetVertexBuffersCommand(_renderer.CopySpan(vertexBuffers), vertexBuffers.Length));
        }

        public void SetViewports(int first, ReadOnlySpan<Viewport> viewports)
        {
            _renderer.QueueCommand(new SetViewportsCommand(first, _renderer.CopySpan(viewports), viewports.Length));
        }

        public void TextureBarrier()
        {
            _renderer.QueueCommand(new TextureBarrierCommand());
        }

        public void TextureBarrierTiled()
        {
            _renderer.QueueCommand(new TextureBarrierTiledCommand());
        }

        public bool TryHostConditionalRendering(ICounterEvent value, ulong compare, bool isEqual)
        {
            var evt = value as ThreadedCounterEvent;
            if (evt != null)
            {
                if (compare == 0 && evt.Type == CounterType.SamplesPassed && evt.ClearCounter)
                {
                    _renderer.QueueCommand(new TryHostConditionalRenderingCommand(evt, compare, isEqual));
                    return true;
                }
            }

            _renderer.QueueCommand(new TryHostConditionalRenderingFlushCommand(evt, null, isEqual));
            return false;
        }

        public bool TryHostConditionalRendering(ICounterEvent value, ICounterEvent compare, bool isEqual)
        {
            _renderer.QueueCommand(new TryHostConditionalRenderingFlushCommand(value as ThreadedCounterEvent, compare as ThreadedCounterEvent, isEqual));
            return false;
        }

        public void UpdateRenderScale(ShaderStage stage, float[] scales, int textureCount, int imageCount)
        {
            float[] scalesCopy = new float[scales.Length];
            scales.CopyTo(scalesCopy, 0);
            _renderer.QueueCommand(new UpdateRenderScaleCommand(stage, scalesCopy, textureCount, imageCount));
        }
    }
}
