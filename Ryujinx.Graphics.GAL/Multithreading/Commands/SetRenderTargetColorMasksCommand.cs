using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetRenderTargetColorMasksCommand : IGALCommand
    {
        private IMemoryOwner<uint> _componentMask;
        private int _componentMaskLength;

        public SetRenderTargetColorMasksCommand(IMemoryOwner<uint> componentMask, int componentMaskLength)
        {
            _componentMask = componentMask;
            _componentMaskLength = componentMaskLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ReadOnlySpan<uint> componentMask = _componentMask.Memory.Span.Slice(0, _componentMaskLength);
            renderer.Pipeline.SetRenderTargetColorMasks(componentMask);
            _componentMask.Dispose();
        }
    }
}
