using Ryujinx.Graphics.GAL.Multithreading.Model;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Buffer
{
    struct BufferGetDataCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.BufferGetData;
        private BufferHandle _buffer;
        private int _offset;
        private int _size;
        private TableRef<ResultBox<byte[]>> _result;

        public void Set(BufferHandle buffer, int offset, int size, TableRef<ResultBox<byte[]>> result)
        {
            _buffer = buffer;
            _offset = offset;
            _size = size;
            _result = result;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            byte[] result = renderer.GetBufferData(threaded.Buffers.MapBuffer(_buffer), _offset, _size);

            _result.Get(threaded).Result = result;
        }
    }
}
