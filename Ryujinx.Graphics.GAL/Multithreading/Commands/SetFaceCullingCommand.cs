namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetFaceCullingCommand : IGALCommand
    {
        private bool _enable;
        private Face _face;

        public SetFaceCullingCommand(bool enable, Face face)
        {
            _enable = enable;
            _face = face;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetFaceCulling(_enable, _face);
        }
    }
}
