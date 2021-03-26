namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct BarrierCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.Barrier;

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.Barrier();
        }
    }
}
