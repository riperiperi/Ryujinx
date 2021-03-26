using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Window
{
    struct WindowPresentCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.WindowPresent;
        private TableRef<ThreadedTexture> _texture;
        private ImageCrop _crop;

        public void Set(TableRef<ThreadedTexture> texture, ImageCrop crop)
        {
            _texture = texture;
            _crop = crop;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            threaded.SignalFrame();
            renderer.Window.Present(_texture.Get(threaded)?.Base, _crop);
        }
    }
}
