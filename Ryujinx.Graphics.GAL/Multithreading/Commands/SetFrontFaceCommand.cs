namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetFrontFaceCommand : IGALCommand
    {
        private FrontFace _frontFace;

        public SetFrontFaceCommand(FrontFace frontFace)
        {
            _frontFace = frontFace;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetFrontFace(_frontFace);
        }
    }
}
