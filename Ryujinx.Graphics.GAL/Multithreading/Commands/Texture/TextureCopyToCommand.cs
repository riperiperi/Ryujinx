using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    struct TextureCopyToCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.TextureCopyTo;
        private TableRef<ThreadedTexture> _texture;
        private TableRef<ThreadedTexture> _destination;
        private int _firstLayer;
        private int _firstLevel;

        public void Set(TableRef<ThreadedTexture> texture, TableRef<ThreadedTexture> destination, int firstLayer, int firstLevel)
        {
            _texture = texture;
            _destination = destination;
            _firstLayer = firstLayer;
            _firstLevel = firstLevel;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ThreadedTexture source = _texture.Get(threaded);
            source.Base.CopyTo(_destination.Get(threaded).Base, _firstLayer, _firstLevel);
        }
    }
}
