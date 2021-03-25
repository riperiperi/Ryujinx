using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Program
{
    class ProgramGetBinaryCommand : IGALCommand
    {
        private ThreadedProgram _program;

        public byte[] Result;

        public ProgramGetBinaryCommand(ThreadedProgram program)
        {
            _program = program;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            Result = _program.Base.GetBinary();
        }
    }
}
