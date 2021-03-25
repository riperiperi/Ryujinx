using Ryujinx.Graphics.GAL.Multithreading.Resources;
using System.Linq;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class CreateProgramCommand : IGALCommand
    {
        private ThreadedProgram _program;
        private IShader[] _shaders;
        private TransformFeedbackDescriptor[] _transformFeedbackDescriptors;

        public CreateProgramCommand(ThreadedProgram program, IShader[] shaders, TransformFeedbackDescriptor[] transformFeedbackDescriptors)
        {
            _program = program;
            _shaders = shaders;
            _transformFeedbackDescriptors = transformFeedbackDescriptors;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            IShader[] shaders = _shaders.Select(shader => (shader as ThreadedShader)?.Base).ToArray();
            _program.Base = renderer.CreateProgram(shaders, _transformFeedbackDescriptors);
        }
    }
}
