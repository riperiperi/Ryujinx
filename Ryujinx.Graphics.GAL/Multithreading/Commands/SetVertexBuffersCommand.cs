using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetVertexBuffersCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetVertexBuffers;
        private TableRef<IMemoryOwner<VertexBufferDescriptor>> _vertexBuffers;
        private int _vertexBuffersLength;

        public void Set(TableRef<IMemoryOwner<VertexBufferDescriptor>> vertexBuffers, int vertexBuffersLength)
        {
            _vertexBuffers = vertexBuffers;
            _vertexBuffersLength = vertexBuffersLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            IMemoryOwner<VertexBufferDescriptor> vertexOwner = _vertexBuffers.Get(threaded);

            Span<VertexBufferDescriptor> vertexBuffers = vertexOwner.Memory.Span.Slice(0, _vertexBuffersLength);
            renderer.Pipeline.SetVertexBuffers(threaded.Buffers.MapBufferRanges(vertexBuffers));
            vertexOwner.Dispose();
        }
    }
}
