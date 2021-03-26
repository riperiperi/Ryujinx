using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    struct TextureGetDataCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.TextureGetData;
        private TableRef<ThreadedTexture> _texture;
        private TableRef<ResultBox<byte[]>> _result;

        public void Set(TableRef<ThreadedTexture> texture, TableRef<ResultBox<byte[]>> result)
        {
            _texture = texture;
            _result = result;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            byte[] result = _texture.Get(threaded).Base.GetData();

            _result.Get(threaded).Result = result;
        }
    }
}
