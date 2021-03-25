namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetRasterizerDiscardCommand : IGALCommand
    {
        private bool _discard;

        public SetRasterizerDiscardCommand(bool discard)
        {
            _discard = discard;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetRasterizerDiscard(_discard);
        }
    }
}
