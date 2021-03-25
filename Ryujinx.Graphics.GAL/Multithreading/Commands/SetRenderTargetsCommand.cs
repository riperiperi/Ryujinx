using Ryujinx.Graphics.GAL.Multithreading.Resources;
using System.Linq;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetRenderTargetsCommand : IGALCommand
    {
        private ThreadedTexture[] _colors;
        private ThreadedTexture _depthStencil;

        public SetRenderTargetsCommand(ITexture[] colors, ITexture depthStencil)
        {
            _colors = colors.Select(color => color as ThreadedTexture).ToArray();
            _depthStencil = depthStencil as ThreadedTexture;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetRenderTargets(_colors.Select(color => color?.Base).ToArray(), _depthStencil?.Base);
        }
    }
}
