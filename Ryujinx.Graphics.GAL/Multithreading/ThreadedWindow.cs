using Ryujinx.Graphics.GAL.Multithreading.Commands.Window;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading
{
    public class ThreadedWindow : IWindow
    {
        private ThreadedRenderer _renderer;
        private IWindow _impl;

        public ThreadedWindow(ThreadedRenderer renderer, IWindow window)
        {
            _renderer = renderer;
            _impl = window;
        }

        public void Present(ITexture texture, ImageCrop crop)
        {
            // If there's already a frame in the pipeline, wait for it to be presented first.
            // This is a multithread rate limit - we can't be more than one frame behind the command queue.

            _renderer.WaitForFrame();
            _renderer.QueueCommand(new WindowPresentCommand(texture as ThreadedTexture, crop));
        }

        public void SetSize(int width, int height)
        {
            _impl.SetSize(width, height);
        }
    }
}
