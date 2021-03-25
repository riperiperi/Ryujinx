using Ryujinx.Graphics.GAL.Multithreading.Resources;
using Ryujinx.Graphics.Shader;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class CompileShaderCommand : IGALCommand
    {
        private ThreadedShader _shader;
        private ShaderStage _stage;
        private string _code;

        public CompileShaderCommand(ThreadedShader shader, ShaderStage stage, string code)
        {
            _shader = shader;
            _stage = stage;
            _code = code;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _shader.Base = renderer.CompileShader(_stage, _code);
        }
    }
}
