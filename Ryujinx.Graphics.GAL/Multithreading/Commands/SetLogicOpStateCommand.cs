namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetLogicOpStateCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetLogicOpState;
        private bool _enable;
        private LogicalOp _op;

        public void Set(bool enable, LogicalOp op)
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
