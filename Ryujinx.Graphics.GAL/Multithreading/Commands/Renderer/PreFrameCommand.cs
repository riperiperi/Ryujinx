using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class PreFrameCommand : IGALCommand
    {
        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.PreFrame();
        }
    }
}
