using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetImageCommand : IGALCommand
    {
        private int _binding;
        private ThreadedTexture _texture;
        private Format _imageFormat;

        public SetImageCommand(int binding, ThreadedTexture texture, Format imageFormat)
        {
            _binding = binding;
            _texture = texture;
            _imageFormat = imageFormat;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetImage(_binding, _texture?.Base, _imageFormat);
        }
    }
}
