namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct EndHostConditionalRenderingCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.EndHostConditionalRendering;

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.EndHostConditionalRendering();
        }
    }
}
