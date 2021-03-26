using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetTextureCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetTexture;
        private int _binding;
        private TableRef<ThreadedTexture> _texture;

        public void Set(int binding, TableRef<ThreadedTexture> texture)
        {
            _binding = binding;
            _texture = texture;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetTexture(_binding, _texture.Get(threaded)?.Base);
        }
    }
}
