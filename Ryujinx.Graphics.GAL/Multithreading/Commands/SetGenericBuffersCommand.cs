using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    delegate void SetGenericBuffersDelegate(ReadOnlySpan<BufferRange> buffers);

    struct SetGenericBuffersCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetGenericBuffers;
        private SpanRef<BufferRange> _buffers;
        private TableRef<SetGenericBuffersDelegate> _action;

        public void Set(SpanRef<BufferRange> buffers, TableRef<SetGenericBuffersDelegate> action)
        {
            _buffers = buffers;
            _action = action;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            Span<BufferRange> buffers = _buffers.Get(threaded);
            _action.Get(threaded)(threaded.Buffers.MapBufferRanges(buffers));
            _buffers.Dispose(threaded);
        }
    }
}
