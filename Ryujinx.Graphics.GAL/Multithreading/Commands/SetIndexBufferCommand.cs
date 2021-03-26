namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetIndexBufferCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetIndexBuffer;
        private BufferRange _buffer;
        private IndexType _type;

        public void Set(BufferRange buffer, IndexType type)
        {
            _buffer = buffer;
            _type = type;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            BufferRange range = threaded.Buffers.MapBufferRange(_buffer);
            renderer.Pipeline.SetIndexBuffer(range, _type);
        }
    }
}
