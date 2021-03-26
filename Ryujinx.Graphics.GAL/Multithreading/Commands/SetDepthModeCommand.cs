namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetDepthModeCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetDepthMode;
        private DepthMode _mode;

        public void Set(DepthMode mode)
        {
            _mode = mode;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetDepthMode(_mode);
        }
    }
}
