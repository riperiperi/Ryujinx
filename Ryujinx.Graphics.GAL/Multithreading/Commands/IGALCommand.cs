namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    interface IGALCommand
    {
        void Run(ThreadedRenderer threaded, IRenderer renderer);
    }
}
