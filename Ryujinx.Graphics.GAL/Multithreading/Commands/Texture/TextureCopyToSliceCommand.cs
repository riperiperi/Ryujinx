using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    class TextureCopyToSliceCommand : IGALCommand
    {
        private ThreadedTexture _texture;
        private ThreadedTexture _destination;
        private int _srcLayer;
        private int _dstLayer;
        private int _srcLevel;
        private int _dstLevel;

        public TextureCopyToSliceCommand(ThreadedTexture texture, ThreadedTexture destination, int srcLayer, int dstLayer, int srcLevel, int dstLevel)
        {
            _texture = texture;
            _destination = destination;
            _srcLayer = srcLayer;
            _dstLayer = dstLayer;
            _srcLevel = srcLevel;
            _dstLevel = dstLevel;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _texture.Base.CopyTo(_destination.Base, _srcLayer, _dstLayer, _srcLevel, _dstLevel);
        }
    }
}
