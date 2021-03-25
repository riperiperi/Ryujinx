namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class TextureBarrierTiledCommand : IGALCommand
    {
        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.TextureBarrierTiled();
        }
    }
}
