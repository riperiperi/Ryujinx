using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    class TextureSetStorageCommand : IGALCommand
    {
        private ThreadedTexture _texture;
        private BufferRange _storage;

        public TextureSetStorageCommand(ThreadedTexture texture, BufferRange storage)
        {
            _texture = texture;
            _storage = storage;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _texture.Base.SetStorage(threaded.Buffers.MapBufferRange(_storage));
        }
    }
}
