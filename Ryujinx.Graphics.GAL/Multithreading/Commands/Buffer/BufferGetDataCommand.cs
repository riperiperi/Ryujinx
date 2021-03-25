namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Buffer
{
    class BufferGetDataCommand : IGALCommand
    {
        private BufferHandle _buffer;
        private int _offset;
        private int _size;

        public byte[] Result;

        public BufferGetDataCommand(BufferHandle buffer, int offset, int size)
        {
            _buffer = buffer;
            _offset = offset;
            _size = size;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            Result = renderer.GetBufferData(threaded.Buffers.MapBuffer(_buffer), _offset, _size);
        }
    }
}
