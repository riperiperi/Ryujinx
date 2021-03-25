namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class ResetCounterCommand : IGALCommand
    {
        private CounterType _type;

        public ResetCounterCommand(CounterType type)
        {
            _type = type;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.ResetCounter(_type);
        }
    }
}
