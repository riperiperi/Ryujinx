using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class LoadProgramBinaryCommand : IGALCommand
    {
        private ThreadedProgram _program;
        private byte[] _programBinary;

        public LoadProgramBinaryCommand(ThreadedProgram program, byte[] programBinary)
        {
            _program = program;
            _programBinary = programBinary;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _program.Base = renderer.LoadProgramBinary(_programBinary);
        }
    }
}
