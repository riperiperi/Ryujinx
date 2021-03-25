namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class BarrierCommand : IGALCommand
    {
        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.Barrier();
        }
    }
}
