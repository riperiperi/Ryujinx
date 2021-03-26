namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    interface IGALCommand
    {
        CommandType CommandType { get; }
        void Run(ThreadedRenderer threaded, IRenderer renderer);
    }
}
