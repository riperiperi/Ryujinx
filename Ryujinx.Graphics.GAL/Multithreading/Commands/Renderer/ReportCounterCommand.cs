using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct ReportCounterCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.ReportCounter;
        private TableRef<ThreadedCounterEvent> _event;
        private CounterType _type;
        private TableRef<EventHandler<ulong>> _resultHandler;

        public void Set(TableRef<ThreadedCounterEvent> evt, CounterType type, TableRef<EventHandler<ulong>> resultHandler)
        {
            _event = evt;
            _type = type;
            _resultHandler = resultHandler;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ThreadedCounterEvent evt = _event.Get(threaded);

            evt.Base = renderer.ReportCounter(_type, _resultHandler.Get(threaded));
        }
    }
}
