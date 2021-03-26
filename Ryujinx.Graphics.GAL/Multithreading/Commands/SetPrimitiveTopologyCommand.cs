namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetPrimitiveTopologyCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetPrimitiveTopology;
        private PrimitiveTopology _topology;

        public void Set(PrimitiveTopology topology)
        {
            _topology = topology;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetPrimitiveTopology(_topology);
        }
    }
}
