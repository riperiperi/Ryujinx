using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class CreateTextureCommand : IGALCommand
    {
        private ThreadedTexture _texture;
        private TextureCreateInfo _info;
        private float _scale;

        public CreateTextureCommand(ThreadedTexture texture, TextureCreateInfo info, float scale)
        {
            _texture = texture;
            _info = info;
            _scale = scale;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _texture.Base = renderer.CreateTexture(_info, _scale);
        }
    }
}
