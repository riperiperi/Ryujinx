namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetPrimitiveTopologyCommand : IGALCommand
    {
        private PrimitiveTopology _topology;

        public SetPrimitiveTopologyCommand(PrimitiveTopology topology)
        {
            _topology = topology;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetPrimitiveTopology(_topology);
        }
    }
}
