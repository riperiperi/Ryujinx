using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    class TextureCopyToScaledCommand : IGALCommand
    {
        private ThreadedTexture _texture;
        private ThreadedTexture _destination;
        private Extents2D _srcRegion;
        private Extents2D _dstRegion;
        private bool _linearFilter;

        public TextureCopyToScaledCommand(ThreadedTexture texture, ThreadedTexture destination, Extents2D srcRegion, Extents2D dstRegion, bool linearFilter)
        {
            _texture = texture;
            _destination = destination;
            _srcRegion = srcRegion;
            _dstRegion = dstRegion;
            _linearFilter = linearFilter;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _texture.Base.CopyTo(_destination.Base, _srcRegion, _dstRegion, _linearFilter);
        }
    }
}
