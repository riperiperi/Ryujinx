using Ryujinx.Graphics.GAL.Multithreading.Commands.Program;

namespace Ryujinx.Graphics.GAL.Multithreading.Resources
{
    class ThreadedProgram : IProgram
    {
        private ThreadedRenderer _renderer;
        public IProgram Base;

        public ThreadedProgram(ThreadedRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Dispose()
        {
            _renderer.QueueCommand(new ProgramDisposeCommand(this));
        }

        public byte[] GetBinary()
        {
            var cmd = new ProgramGetBinaryCommand(this);

            _renderer.InvokeCommand(cmd);

            return cmd.Result;
        }
    }
}
