using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetViewportsCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetViewports;
        private int _first;
        private TableRef<IMemoryOwner<Viewport>> _viewports;
        private int _viewportsLength;

        public void Set(int first, TableRef<IMemoryOwner<Viewport>> viewports, int viewportsLength)
        {
            _first = first;
            _viewports = viewports;
            _viewportsLength = viewportsLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            IMemoryOwner<Viewport> viewportOwner = _viewports.Get(threaded);

            ReadOnlySpan<Viewport> viewports = viewportOwner.Memory.Span.Slice(0, _viewportsLength);
            renderer.Pipeline.SetViewports(_first, viewports);
            viewportOwner.Dispose();
        }
    }
}
