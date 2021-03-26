using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetProgramCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetProgram;
        private TableRef<ThreadedProgram> _program;

        public void Set(TableRef<ThreadedProgram> program)
        {
            _program = program;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetProgram(_program.Get(threaded).Base);
        }
    }
}
