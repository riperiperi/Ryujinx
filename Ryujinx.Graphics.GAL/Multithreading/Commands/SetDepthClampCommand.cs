namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetDepthClampCommand : IGALCommand
    {
        private bool _clamp;

        public SetDepthClampCommand(bool clamp)
        {
            _clamp = clamp;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetDepthClamp(_clamp);
        }
    }
}
