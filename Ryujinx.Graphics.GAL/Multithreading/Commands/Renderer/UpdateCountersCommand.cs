namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class UpdateCountersCommand : IGALCommand
    {
        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.UpdateCounters();
        }
    }
}
