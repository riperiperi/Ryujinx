using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Buffer
{
    struct BufferSetDataCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.BufferSetData;
        private BufferHandle _buffer;
        private int _offset;
        private TableRef<IMemoryOwner<byte>> _data;
        private int _dataLength;

        public void Set(BufferHandle buffer, int offset, TableRef<IMemoryOwner<byte>> data, int dataLength)
        {
            _buffer = buffer;
            _offset = offset;
            _data = data;
            _dataLength = dataLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            IMemoryOwner<byte> owner = _data.Get(threaded);

            ReadOnlySpan<byte> data = owner.Memory.Span.Slice(0, _dataLength);
            renderer.SetBufferData(threaded.Buffers.MapBuffer(_buffer), _offset, data);
            owner.Dispose();
        }
    }
}
