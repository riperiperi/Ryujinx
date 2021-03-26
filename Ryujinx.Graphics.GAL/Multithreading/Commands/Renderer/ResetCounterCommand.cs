namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct ResetCounterCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.ResetCounter;
        private CounterType _type;

        public void Set(CounterType type)
        {
            _type = type;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.ResetCounter(_type);
        }
    }
}
