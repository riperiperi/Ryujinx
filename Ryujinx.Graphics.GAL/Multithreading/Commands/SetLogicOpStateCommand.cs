namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetLogicOpStateCommand : IGALCommand
    {
        private bool _enable;
        private LogicalOp _op;

        public SetLogicOpStateCommand(bool enable, LogicalOp op)
        {
            _enable = enable;
            _op = op;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetLogicOpState(_enable, _op);
        }
    }
}
