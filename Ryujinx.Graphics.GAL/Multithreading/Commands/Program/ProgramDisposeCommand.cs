using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Program
{
    struct ProgramDisposeCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.ProgramDispose;
        private TableRef<ThreadedProgram> _program;

        public void Set(TableRef<ThreadedProgram> program)
        {
            _program = program;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _program.Get(threaded).Base.Dispose();
        }
    }
}
