namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetBlendStateCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetBlendState;
        private int _index;
        private BlendDescriptor _blend;

        public void Set(int index, BlendDescriptor blend)
        {
            _index = index;
            _blend = blend;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetBlendState(_index, _blend);
        }
    }
}
