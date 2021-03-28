using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetViewportsCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetViewports;
        private int _first;
        private SpanRef<Viewport> _viewports;

        public void Set(int first, SpanRef<Viewport> viewports)
        {
            _first = first;
            _viewports = viewports;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ReadOnlySpan<Viewport> viewports = _viewports.Get(threaded);
            renderer.Pipeline.SetViewports(_first, viewports);
            _viewports.Dispose(threaded);
        }
    }
}
