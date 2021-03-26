namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetRenderTargetScaleCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetRenderTargetScale;
        private float _scale;

        public void Set(float scale)
        {
            _scale = scale;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetRenderTargetScale(_scale);
        }
    }
}
