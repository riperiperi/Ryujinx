namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct TextureBarrierCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.TextureBarrier;

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.TextureBarrier();
        }
    }
}
