namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetRasterizerDiscardCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetRasterizerDiscard;
        private bool _discard;

        public void Set(bool discard)
        {
            _discard = discard;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetRasterizerDiscard(_discard);
        }
    }
}
