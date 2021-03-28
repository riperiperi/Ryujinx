using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetVertexAttribsCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetVertexAttribs;
        private SpanRef<VertexAttribDescriptor> _vertexAttribs;

        public void Set(SpanRef<VertexAttribDescriptor> vertexAttribs)
        {
            _vertexAttribs = vertexAttribs;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ReadOnlySpan<VertexAttribDescriptor> vertexAttribs = _vertexAttribs.Get(threaded);
            renderer.Pipeline.SetVertexAttribs(vertexAttribs);
            _vertexAttribs.Dispose(threaded);
        }
    }
}
