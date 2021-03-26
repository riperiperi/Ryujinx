using Ryujinx.Graphics.GAL.Multithreading.Commands.Shader;
using Ryujinx.Graphics.GAL.Multithreading.Model;

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
            _renderer.New<ShaderDisposeCommand>().Set(new TableRef<ThreadedShader>(_renderer, this));
            _renderer.QueueCommand();
        }
    }
}
