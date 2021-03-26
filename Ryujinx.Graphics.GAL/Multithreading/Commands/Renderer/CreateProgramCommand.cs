using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;
using System.Linq;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct CreateProgramCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.CreateProgram;
        private TableRef<ThreadedProgram> _program;
        private TableRef<IShader[]> _shaders;
        private TableRef<TransformFeedbackDescriptor[]> _transformFeedbackDescriptors;

        public void Set(TableRef<ThreadedProgram> program, TableRef<IShader[]> shaders, TableRef<TransformFeedbackDescriptor[]> transformFeedbackDescriptors)
        {
            _program = program;
            _shaders = shaders;
            _transformFeedbackDescriptors = transformFeedbackDescriptors;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ThreadedProgram program = _program.Get(threaded);

            IShader[] shaders = _shaders.Get(threaded).Select(shader => (shader as ThreadedShader)?.Base).ToArray();
            program.Base = renderer.CreateProgram(shaders, _transformFeedbackDescriptors.Get(threaded));
        }
    }
}
