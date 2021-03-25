namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class CreateSyncCommand : IGALCommand
    {
        private ulong _id;

        public CreateSyncCommand(ulong id)
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
