using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Shader
{
    class ShaderDisposeCommand : IGALCommand
    {
        private ThreadedShader _shader;

        public ShaderDisposeCommand(ThreadedShader shader)
        {
            _shader = shader;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _shader.Base.Dispose();
        }
    }
}
