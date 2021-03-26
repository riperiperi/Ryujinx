namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetFrontFaceCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetFrontFace;
        private FrontFace _frontFace;

        public void Set(FrontFace frontFace)
        {
            _frontFace = frontFace;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetFrontFace(_frontFace);
        }
    }
}
