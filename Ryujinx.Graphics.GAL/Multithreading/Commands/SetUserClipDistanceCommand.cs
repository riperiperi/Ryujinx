namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetUserClipDistanceCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetUserClipDistance;
        private int _index;
        private bool _enableClip;

        public void Set(int index, bool enableClip)
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
