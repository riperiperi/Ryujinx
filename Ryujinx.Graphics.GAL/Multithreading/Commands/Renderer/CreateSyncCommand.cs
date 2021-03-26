namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct CreateSyncCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.CreateSync;
        private ulong _id;

        public void Set(ulong id)
        {
            _id = id;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.CreateSync(_id);

            threaded.Sync.AssignSync(_id);
        }
    }
}
