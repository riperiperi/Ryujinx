using Ryujinx.Graphics.GAL.Multithreading.Resources;
using Ryujinx.Graphics.Shader;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class CreateBufferCommand : IGALCommand
    {
        private BufferHandle _threadedHandle;
        private int _size;

        public CreateBufferCommand(BufferHandle threadedHandle, int size)
        {
            _threadedHandle = threadedHandle;
            _size = size;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            threaded.Buffers.AssignBuffer(_threadedHandle, renderer.CreateBuffer(_size));
        }
    }
}
