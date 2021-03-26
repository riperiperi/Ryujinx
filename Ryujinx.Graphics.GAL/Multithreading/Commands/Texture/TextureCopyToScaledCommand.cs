using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    struct TextureCopyToScaledCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.TextureCopyToScaled;
        private TableRef<ThreadedTexture> _texture;
        private TableRef<ThreadedTexture> _destination;
        private Extents2D _srcRegion;
        private Extents2D _dstRegion;
        private bool _linearFilter;

        public void Set(TableRef<ThreadedTexture> texture, TableRef<ThreadedTexture> destination, Extents2D srcRegion, Extents2D dstRegion, bool linearFilter)
        {
            _texture = texture;
            _destination = destination;
            _srcRegion = srcRegion;
            _dstRegion = dstRegion;
            _linearFilter = linearFilter;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ThreadedTexture source = _texture.Get(threaded);
            source.Base.CopyTo(_destination.Get(threaded).Base, _srcRegion, _dstRegion, _linearFilter);
        }
    }
}
