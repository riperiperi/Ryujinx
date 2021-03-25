namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetDepthModeCommand : IGALCommand
    {
        private DepthMode _mode;

        public SetDepthModeCommand(DepthMode mode)
        {
            _mode = mode;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetDepthMode(_mode);
        }
    }
}
