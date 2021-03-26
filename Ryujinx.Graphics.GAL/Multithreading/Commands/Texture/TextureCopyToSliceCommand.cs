using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    struct TextureCopyToSliceCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.TextureCopyToSlice;
        private TableRef<ThreadedTexture> _texture;
        private TableRef<ThreadedTexture> _destination;
        private int _srcLayer;
        private int _dstLayer;
        private int _srcLevel;
        private int _dstLevel;

        public void Set(TableRef<ThreadedTexture> texture, TableRef<ThreadedTexture> destination, int srcLayer, int dstLayer, int srcLevel, int dstLevel)
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
            ThreadedTexture source = _texture.Get(threaded);
            source.Base.CopyTo(_destination.Get(threaded).Base, _srcLayer, _dstLayer, _srcLevel, _dstLevel);
        }
    }
}
