using Ryujinx.Graphics.GAL.Multithreading.Model;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct ActionCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.Action;
        private TableRef<Action> _action;

        public void Set(TableRef<Action> action)
        {
            _action = action;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _action.Get(threaded)();
        }
    }
}
