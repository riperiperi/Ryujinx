namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetAlphaTestCommand : IGALCommand
    {
        private bool _enable;
        private float _reference;
        private CompareOp _op;

        public SetAlphaTestCommand(bool enable, float reference, CompareOp op)
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
