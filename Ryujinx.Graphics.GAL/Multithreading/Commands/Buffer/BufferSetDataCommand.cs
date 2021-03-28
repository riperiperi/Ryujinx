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
        private TableRef<ISpanRef> _data;
        private int _dataLength;

        public void Set(BufferHandle buffer, int offset, TableRef<ISpanRef> data, int dataLength)
        {
            _buffer = buffer;
            _offset = offset;
            _data = data;
            _dataLength = dataLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ISpanRef owner = _data.Get(threaded);

            ReadOnlySpan<byte> data = owner.Get<byte>(_dataLength);
            renderer.SetBufferData(threaded.Buffers.MapBuffer(_buffer), _offset, data);
            owner.Dispose<byte>(_dataLength);
        }
    }
}
