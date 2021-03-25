using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class ActionCommand : IGALCommand
    {
        private Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _action();
        }
    }
}
