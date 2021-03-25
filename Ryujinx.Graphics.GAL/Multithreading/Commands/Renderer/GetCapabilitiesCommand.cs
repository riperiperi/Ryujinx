namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class GetCapabilitiesCommand : IGALCommand
    {
        public Capabilities Result;

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            Result = renderer.GetCapabilities();
        }
    }
}
