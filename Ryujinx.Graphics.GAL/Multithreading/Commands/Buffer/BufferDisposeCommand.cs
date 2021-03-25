namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Buffer
{
    class BufferDisposeCommand : IGALCommand
    {
        private BufferHandle _buffer;

        public BufferDisposeCommand(BufferHandle buffer)
        {
            _buffer = buffer;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.DeleteBuffer(threaded.Buffers.MapBuffer(_buffer));
            threaded.Buffers.UnassignBuffer(_buffer);
        }
    }
}
