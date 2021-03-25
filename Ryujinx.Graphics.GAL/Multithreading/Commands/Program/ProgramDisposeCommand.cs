using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Program
{
    class ProgramDisposeCommand : IGALCommand
    {
        private ThreadedProgram _program;

        public ProgramDisposeCommand(ThreadedProgram program)
        {
            _program = program;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _program.Base.Dispose();
        }
    }
}
