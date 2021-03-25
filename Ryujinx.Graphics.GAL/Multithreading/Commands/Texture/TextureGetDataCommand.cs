using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    class TextureGetDataCommand : IGALCommand
    {
        private ThreadedTexture _texture;

        public byte[] Result;

        public TextureGetDataCommand(ThreadedTexture texture)
        {
            _texture = texture;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            Result = _texture.Base.GetData();
        }
    }
}
