using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    delegate void SetGenericBuffersDelegate(ReadOnlySpan<BufferRange> buffers);

    struct SetGenericBuffersCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetGenericBuffers;
        private TableRef<ISpanRef> _buffers;
        private int _buffersLength;
        private TableRef<SetGenericBuffersDelegate> _action;

        public void Set(TableRef<ISpanRef> buffers, int buffersLength, TableRef<SetGenericBuffersDelegate> action)
        {
            _buffers = buffers;
            _buffersLength = buffersLength;
            _action = action;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ISpanRef buffersOwner = _buffers.Get(threaded);

            Span<BufferRange> buffers = buffersOwner.Get<BufferRange>(_buffersLength);
            _action.Get(threaded)(threaded.Buffers.MapBufferRanges(buffers));
            buffersOwner.Dispose<BufferRange>(_buffersLength);
        }
    }
}
