using Ryujinx.Graphics.GAL.Multithreading.Commands.Shader;

namespace Ryujinx.Graphics.GAL.Multithreading.Resources
{
    class ThreadedShader : IShader
    {
        private ThreadedRenderer _renderer;
        public IShader Base;

        public ThreadedShader(ThreadedRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Dispose()
        {
            _renderer.QueueCommand(new ShaderDisposeCommand(this));
        }
    }
}
