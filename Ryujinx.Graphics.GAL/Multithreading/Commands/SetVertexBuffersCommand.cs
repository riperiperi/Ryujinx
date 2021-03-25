using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetVertexBuffersCommand : IGALCommand
    {
        private IMemoryOwner<VertexBufferDescriptor> _vertexBuffers;
        private int _vertexBuffersLength;

        public SetVertexBuffersCommand(IMemoryOwner<VertexBufferDescriptor> vertexBuffers, int vertexBuffersLength)
        {
            _vertexBuffers = vertexBuffers;
            _vertexBuffersLength = vertexBuffersLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            Span<VertexBufferDescriptor> vertexBuffers = _vertexBuffers.Memory.Span.Slice(0, _vertexBuffersLength);
            renderer.Pipeline.SetVertexBuffers(threaded.Buffers.MapBufferRanges(vertexBuffers));
            _vertexBuffers.Dispose();
        }
    }
}
