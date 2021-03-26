namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct TextureBarrierTiledCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.TextureBarrierTiled;

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.TextureBarrierTiled();
        }
    }
}
