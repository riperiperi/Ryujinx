using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct LoadProgramBinaryCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.LoadProgramBinary;
        private TableRef<ThreadedProgram> _program;
        private TableRef<byte[]> _programBinary;

        public void Set(TableRef<ThreadedProgram> program, TableRef<byte[]> programBinary)
        {
            _program = program;
            _programBinary = programBinary;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ThreadedProgram program = _program.Get(threaded);
            program.Base = renderer.LoadProgramBinary(_programBinary.Get(threaded));
        }
    }
}
