namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct CopyBufferCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.CopyBuffer;
        private BufferHandle _source;
        private BufferHandle _destination;
        private int _srcOffset;
        private int _dstOffset;
        private int _size;

        public void Set(BufferHandle source, BufferHandle destination, int srcOffset, int dstOffset, int size)
        {
            _source = source;
            _destination = destination;
            _srcOffset = srcOffset;
            _dstOffset = dstOffset;
            _size = size;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.CopyBuffer(threaded.Buffers.MapBuffer(_source), threaded.Buffers.MapBuffer(_destination), _srcOffset, _dstOffset, _size);
        }
    }
}
