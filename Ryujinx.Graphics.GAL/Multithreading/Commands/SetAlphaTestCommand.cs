namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetAlphaTestCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetAlphaTest;
        private bool _enable;
        private float _reference;
        private CompareOp _op;

        public void Set(bool enable, float reference, CompareOp op)
        {
            _enable = enable;
            _reference = reference;
            _op = op;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetAlphaTest(_enable, _reference, _op);
        }
    }
}
