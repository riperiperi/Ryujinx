using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Window
{
    class WindowPresentCommand : IGALCommand
    {
        private ThreadedTexture _texture;
        private ImageCrop _crop;

        public WindowPresentCommand(ThreadedTexture texture, ImageCrop crop)
        {
            _texture = texture;
            _crop = crop;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            threaded.SignalFrame();
            renderer.Window.Present(_texture?.Base, _crop);
        }
    }
}
