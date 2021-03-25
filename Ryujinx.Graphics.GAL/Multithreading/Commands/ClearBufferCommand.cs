namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class ClearBufferCommand : IGALCommand
    {
        private BufferHandle _destination;
        private int _offset;
        private int _size;
        private uint _value;

        public ClearBufferCommand(BufferHandle destination, int offset, int size, uint value)
        {
            _destination = destination;
            _offset = offset;
            _size = size;
            _value = value;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.ClearBuffer(threaded.Buffers.MapBuffer(_destination), _offset, _size, _value);
        }
    }
}
