namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetIndexBufferCommand : IGALCommand
    {
        private BufferRange _buffer;
        private IndexType _type;

        public SetIndexBufferCommand(BufferRange buffer, IndexType type)
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
