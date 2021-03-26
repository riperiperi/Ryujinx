using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetRenderTargetColorMasksCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetRenderTargetColorMasks;
        private TableRef<IMemoryOwner<uint>> _componentMask;
        private int _componentMaskLength;

        public void Set(TableRef<IMemoryOwner<uint>> componentMask, int componentMaskLength)
        {
            _componentMask = componentMask;
            _componentMaskLength = componentMaskLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            IMemoryOwner<uint> maskOwner = _componentMask.Get(threaded);

            ReadOnlySpan<uint> componentMask = maskOwner.Memory.Span.Slice(0, _componentMaskLength);
            renderer.Pipeline.SetRenderTargetColorMasks(componentMask);
            maskOwner.Dispose();
        }
    }
}
