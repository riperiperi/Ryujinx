using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetImageCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetImage;
        private int _binding;
        private TableRef<ThreadedTexture> _texture;
        private Format _imageFormat;

        public void Set(int binding, TableRef<ThreadedTexture> texture, Format imageFormat)
        {
            _binding = binding;
            _texture = texture;
            _imageFormat = imageFormat;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetImage(_binding, _texture.Get(threaded)?.Base, _imageFormat);
        }
    }
}
