namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetPrimitiveRestartCommand : IGALCommand
    {
        private bool _enable;
        private int _index;

        public SetPrimitiveRestartCommand(bool enable, int index)
        {
            _enable = enable;
            _index = index;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetPrimitiveRestart(_enable, _index);
        }
    }
}
