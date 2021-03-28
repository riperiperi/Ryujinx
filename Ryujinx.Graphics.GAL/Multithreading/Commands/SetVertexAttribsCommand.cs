using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetVertexAttribsCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetVertexAttribs;
        private TableRef<ISpanRef> _vertexAttribs;
        private int _vertexAttribsLength;

        public void Set(TableRef<ISpanRef> vertexAttribs, int vertexAttribsLength)
        {
            _vertexAttribs = vertexAttribs;
            _vertexAttribsLength = vertexAttribsLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ISpanRef vertexOwner = _vertexAttribs.Get(threaded);

            ReadOnlySpan<VertexAttribDescriptor> vertexAttribs = vertexOwner.Get<VertexAttribDescriptor>(_vertexAttribsLength);
            renderer.Pipeline.SetVertexAttribs(vertexAttribs);
            vertexOwner.Dispose<VertexAttribDescriptor>(_vertexAttribsLength);
        }
    }
}
