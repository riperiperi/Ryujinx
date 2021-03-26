namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetDepthClampCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetDepthClamp;
        private bool _clamp;

        public void Set(bool clamp)
        {
            _clamp = clamp;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetDepthClamp(_clamp);
        }
    }
}
