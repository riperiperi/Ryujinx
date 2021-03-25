using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    class TextureReleaseCommand : IGALCommand
    {
        private ThreadedTexture _texture;

        public TextureReleaseCommand(ThreadedTexture texture)
        {
            _texture = texture;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _texture.Base.Release();
        }
    }
}
