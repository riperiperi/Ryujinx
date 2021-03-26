using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Program
{
    struct ProgramGetBinaryCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.ProgramGetBinary;
        private TableRef<ThreadedProgram> _program;
        private TableRef<ResultBox<byte[]>> _result;

        public void Set(TableRef<ThreadedProgram> program, TableRef<ResultBox<byte[]>> result)
        {
            _program = program;
            _result = result;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            byte[] result = _program.Get(threaded).Base.GetBinary();

            _result.Get(threaded).Result = result;
        }
    }
}
