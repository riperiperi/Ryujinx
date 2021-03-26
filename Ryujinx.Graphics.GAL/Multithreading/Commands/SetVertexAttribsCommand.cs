using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetVertexAttribsCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetVertexAttribs;
        private TableRef<IMemoryOwner<VertexAttribDescriptor>> _vertexAttribs;
        private int _vertexAttribsLength;

        public void Set(TableRef<IMemoryOwner<VertexAttribDescriptor>> vertexAttribs, int vertexAttribsLength)
        {
            _vertexAttribs = vertexAttribs;
            _vertexAttribsLength = vertexAttribsLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            IMemoryOwner<VertexAttribDescriptor> vertexOwner = _vertexAttribs.Get(threaded);

            ReadOnlySpan<VertexAttribDescriptor> vertexAttribs = vertexOwner.Memory.Span.Slice(0, _vertexAttribsLength);
            renderer.Pipeline.SetVertexAttribs(vertexAttribs);
            vertexOwner.Dispose();
        }
    }
}
