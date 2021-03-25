using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    class TextureCreateViewCommand : IGALCommand
    {
        private ThreadedTexture _texture;
        private ThreadedTexture _destination;
        private TextureCreateInfo _info;
        private int _firstLayer;
        private int _firstLevel;

        public TextureCreateViewCommand(ThreadedTexture texture, ThreadedTexture destination, TextureCreateInfo info, int firstLayer, int firstLevel)
        {
            _texture = texture;
            _destination = destination;
            _info = info;
            _firstLayer = firstLayer;
            _firstLevel = firstLevel;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _destination.Base = _texture.Base.CreateView(_info, _firstLayer, _firstLevel);
        }
    }
}
