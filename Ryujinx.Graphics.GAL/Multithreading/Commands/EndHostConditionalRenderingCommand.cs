namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class EndHostConditionalRenderingCommand : IGALCommand
    {
        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.EndHostConditionalRendering();
        }
    }
}
