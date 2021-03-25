namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetBlendStateCommand : IGALCommand
    {
        private int _index;
        private BlendDescriptor _blend;

        public SetBlendStateCommand(int index, BlendDescriptor blend)
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
