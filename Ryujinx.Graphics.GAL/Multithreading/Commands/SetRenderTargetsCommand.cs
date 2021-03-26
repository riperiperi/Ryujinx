using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;
using System.Linq;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetRenderTargetsCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetRenderTargets;
        private TableRef<ThreadedTexture[]> _colors;
        private TableRef<ThreadedTexture> _depthStencil;

        public void Set(TableRef<ThreadedTexture[]> colors, TableRef<ThreadedTexture> depthStencil)
        {
            _colors = colors;
            _depthStencil = depthStencil;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetRenderTargets(_colors.Get(threaded).Select(color => color?.Base).ToArray(), _depthStencil.Get(threaded)?.Base);
        }
    }
}
