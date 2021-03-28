using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetVertexBuffersCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetVertexBuffers;
        private SpanRef<VertexBufferDescriptor> _vertexBuffers;

        public void Set(SpanRef<VertexBufferDescriptor> vertexBuffers)
        {
            _vertexBuffers = vertexBuffers;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            Span<VertexBufferDescriptor> vertexBuffers = _vertexBuffers.Get(threaded);
            renderer.Pipeline.SetVertexBuffers(threaded.Buffers.MapBufferRanges(vertexBuffers));
            _vertexBuffers.Dispose(threaded);
        }
    }
}
