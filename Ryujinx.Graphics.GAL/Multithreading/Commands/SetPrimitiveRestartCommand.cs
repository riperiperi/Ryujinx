namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetPrimitiveRestartCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetPrimitiveRestart;
        private bool _enable;
        private int _index;

        public void Set(bool enable, int index)
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
