using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Buffer
{
    struct BufferSetDataCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.BufferSetData;
        private BufferHandle _buffer;
        private int _offset;
        private SpanRef<byte> _data;

        public void Set(BufferHandle buffer, int offset, SpanRef<byte> data)
        {
            _buffer = buffer;
            _offset = offset;
            _data = data;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ReadOnlySpan<byte> data = _data.Get(threaded);
            renderer.SetBufferData(threaded.Buffers.MapBuffer(_buffer), _offset, data);
            _data.Dispose(threaded);
        }
    }
}
