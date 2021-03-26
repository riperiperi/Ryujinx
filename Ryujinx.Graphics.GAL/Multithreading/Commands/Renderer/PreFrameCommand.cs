using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct PreFrameCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.PreFrame;

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.PreFrame();
        }
    }
}
