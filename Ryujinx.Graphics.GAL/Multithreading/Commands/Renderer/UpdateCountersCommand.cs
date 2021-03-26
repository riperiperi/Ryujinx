namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct UpdateCountersCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.UpdateCounters;

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.UpdateCounters();
        }
    }
}
