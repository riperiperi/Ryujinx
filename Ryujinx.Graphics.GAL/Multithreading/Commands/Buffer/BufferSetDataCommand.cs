using System;
using System.Buffers;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Buffer
{
    class BufferSetDataCommand : IGALCommand
    {
        private BufferHandle _buffer;
        private int _offset;
        private IMemoryOwner<byte> _data;
        private int _dataLength;

        public BufferSetDataCommand(BufferHandle buffer, int offset, IMemoryOwner<byte> data, int dataLength)
        {
            _buffer = buffer;
            _offset = offset;
            _data = data;
            _dataLength = dataLength;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ReadOnlySpan<byte> data = _data.Memory.Span.Slice(0, _dataLength);
            renderer.SetBufferData(threaded.Buffers.MapBuffer(_buffer), _offset, data);
            _data.Dispose();
        }
    }
}
