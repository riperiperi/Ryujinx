using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetVertexAttribsCommand : IGALCommand
    {
        private IMemoryOwner<VertexAttribDescriptor> _vertexAttribs;
        private int _vertexAttribsLength;

        public SetVertexAttribsCommand(IMemoryOwner<VertexAttribDescriptor> vertexAttribs, int vertexAttribsLength)
        {
            _vertexAttribs = vertexAttribs;
            _vertexAttribsLength = vertexAttribsLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ReadOnlySpan<VertexAttribDescriptor> vertexAttribs = _vertexAttribs.Memory.Span.Slice(0, _vertexAttribsLength);
            renderer.Pipeline.SetVertexAttribs(vertexAttribs);
            _vertexAttribs.Dispose();
        }
    }
}
