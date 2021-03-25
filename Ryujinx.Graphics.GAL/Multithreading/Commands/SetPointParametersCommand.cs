namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetPointParametersCommand : IGALCommand
    {
        private float _size;
        private bool _isProgramPointSize;
        private bool _enablePointSprite;
        private Origin _origin;

        public SetPointParametersCommand(float size, bool isProgramPointSize, bool enablePointSprite, Origin origin)
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
