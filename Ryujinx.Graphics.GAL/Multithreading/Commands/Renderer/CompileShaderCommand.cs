using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;
using Ryujinx.Graphics.Shader;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct CompileShaderCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.CompileShader;
        private TableRef<ThreadedShader> _shader;
        private ShaderStage _stage;
        private TableRef<string> _code;

        public void Set(TableRef<ThreadedShader> shader, ShaderStage stage, TableRef<string> code)
        {
            _shader = shader;
            _stage = stage;
            _code = code;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ThreadedShader shader = _shader.Get(threaded);
            shader.Base = renderer.CompileShader(_stage, _code.Get(threaded));
        }
    }
}
