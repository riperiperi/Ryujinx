using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetViewportsCommand : IGALCommand
    {
        private int _first;
        private IMemoryOwner<Viewport> _viewports;
        private int _viewportsLength;

        public SetViewportsCommand(int first, IMemoryOwner<Viewport> viewports, int viewportsLength)
        {
            _first = first;
            _viewports = viewports;
            _viewportsLength = viewportsLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ReadOnlySpan<Viewport> viewports = _viewports.Memory.Span.Slice(0, _viewportsLength);
            renderer.Pipeline.SetViewports(_first, viewports);
            _viewports.Dispose();
        }
    }
}
