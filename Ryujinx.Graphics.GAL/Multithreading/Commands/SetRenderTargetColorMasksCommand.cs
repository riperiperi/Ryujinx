using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetRenderTargetColorMasksCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetRenderTargetColorMasks;
        private SpanRef<uint> _componentMask;

        public void Set(SpanRef<uint> componentMask)
        {
            _componentMask = componentMask;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ReadOnlySpan<uint> componentMask = _componentMask.Get(threaded);
            renderer.Pipeline.SetRenderTargetColorMasks(componentMask);
            _componentMask.Dispose(threaded);
        }
    }
}
