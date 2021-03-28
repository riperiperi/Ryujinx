using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetVertexBuffersCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetVertexBuffers;
        private TableRef<ISpanRef> _vertexBuffers;
        private int _vertexBuffersLength;

        public void Set(TableRef<ISpanRef> vertexBuffers, int vertexBuffersLength)
        {
            _vertexBuffers = vertexBuffers;
            _vertexBuffersLength = vertexBuffersLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ISpanRef vertexOwner = _vertexBuffers.Get(threaded);

            Span<VertexBufferDescriptor> vertexBuffers = vertexOwner.Get<VertexBufferDescriptor>(_vertexBuffersLength);
            renderer.Pipeline.SetVertexBuffers(threaded.Buffers.MapBufferRanges(vertexBuffers));
            vertexOwner.Dispose<VertexBufferDescriptor>(_vertexBuffersLength);
        }
    }
}
