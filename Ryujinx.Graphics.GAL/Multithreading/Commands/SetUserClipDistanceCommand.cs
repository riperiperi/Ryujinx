namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetUserClipDistanceCommand : IGALCommand
    {
        private int _index;
        private bool _enableClip;

        public SetUserClipDistanceCommand(int index, bool enableClip)
        {
            _index = index;
            _enableClip = enableClip;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetUserClipDistance(_index, _enableClip);
        }
    }
}
