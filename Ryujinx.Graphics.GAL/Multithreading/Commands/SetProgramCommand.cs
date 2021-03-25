using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetProgramCommand : IGALCommand
    {
        private ThreadedProgram _program;

        public SetProgramCommand(ThreadedProgram program)
        {
            _program = program;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetProgram(_program.Base);
        }
    }
}
