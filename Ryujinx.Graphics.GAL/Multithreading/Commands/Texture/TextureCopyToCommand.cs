using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    class TextureCopyToCommand : IGALCommand
    {
        private ThreadedTexture _texture;
        private ThreadedTexture _destination;
        private int _firstLayer;
        private int _firstLevel;

        public TextureCopyToCommand(ThreadedTexture texture, ThreadedTexture destination, int firstLayer, int firstLevel)
        {
            _texture = texture;
            _destination = destination;
            _firstLayer = firstLayer;
            _firstLevel = firstLevel;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _texture.Base.CopyTo(_destination.Base, _firstLayer, _firstLevel);
        }
    }
}
