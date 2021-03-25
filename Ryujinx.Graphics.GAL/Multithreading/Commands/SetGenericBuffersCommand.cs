using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    delegate void SetGenericBuffersDelegate(ReadOnlySpan<BufferRange> buffers);

    class SetGenericBuffersCommand : IGALCommand
    {
        private IMemoryOwner<BufferRange> _buffers;
        private int _buffersLength;
        private SetGenericBuffersDelegate _action;

        public SetGenericBuffersCommand(IMemoryOwner<BufferRange> buffers, int buffersLength, SetGenericBuffersDelegate action)
        {
            _buffers = buffers;
            _buffersLength = buffersLength;
            _action = action;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            Span<BufferRange> buffers = _buffers.Memory.Span.Slice(0, _buffersLength);
            _action(threaded.Buffers.MapBufferRanges(buffers));
            _buffers.Dispose();
        }
    }
}
