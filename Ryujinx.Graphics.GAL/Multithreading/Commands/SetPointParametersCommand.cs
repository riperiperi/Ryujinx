namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetPointParametersCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetPointParameters;
        private float _size;
        private bool _isProgramPointSize;
        private bool _enablePointSprite;
        private Origin _origin;

        public void Set(float size, bool isProgramPointSize, bool enablePointSprite, Origin origin)
        {
            _size = size;
            _isProgramPointSize = isProgramPointSize;
            _enablePointSprite = enablePointSprite;
            _origin = origin;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetPointParameters(_size, _isProgramPointSize, _enablePointSprite, _origin);
        }
    }
}
