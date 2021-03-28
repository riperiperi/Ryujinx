using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetRenderTargetColorMasksCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetRenderTargetColorMasks;
        private TableRef<ISpanRef> _componentMask;
        private int _componentMaskLength;

        public void Set(TableRef<ISpanRef> componentMask, int componentMaskLength)
        {
            _componentMask = componentMask;
            _componentMaskLength = componentMaskLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ISpanRef maskOwner = _componentMask.Get(threaded);

            ReadOnlySpan<uint> componentMask = maskOwner.Get<uint>(_componentMaskLength);
            renderer.Pipeline.SetRenderTargetColorMasks(componentMask);
            maskOwner.Dispose<uint>(_componentMaskLength);
        }
    }
}
