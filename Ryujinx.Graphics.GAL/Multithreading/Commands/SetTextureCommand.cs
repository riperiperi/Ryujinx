using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetTextureCommand : IGALCommand
    {
        private int _binding;
        private ThreadedTexture _texture;

        public SetTextureCommand(int binding, ThreadedTexture texture)
        {
            _binding = binding;
            _texture = texture;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetTexture(_binding, _texture?.Base);
        }
    }
}
