namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class TextureBarrierCommand : IGALCommand
    {
        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.TextureBarrier();
        }
    }
}
