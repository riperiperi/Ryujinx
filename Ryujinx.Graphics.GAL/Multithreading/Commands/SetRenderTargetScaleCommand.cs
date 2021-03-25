namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetRenderTargetScaleCommand : IGALCommand
    {
        private float _scale;

        public SetRenderTargetScaleCommand(float scale)
        {
            _scale = scale;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetRenderTargetScale(_scale);
        }
    }
}
