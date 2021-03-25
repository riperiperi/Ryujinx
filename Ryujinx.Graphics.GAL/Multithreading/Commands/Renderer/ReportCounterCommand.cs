using Ryujinx.Graphics.GAL.Multithreading.Resources;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class ReportCounterCommand : IGALCommand
    {
        private ThreadedCounterEvent _event;
        private CounterType _type;
        private EventHandler<ulong> _resultHandler;

        public ReportCounterCommand(ThreadedCounterEvent evt, CounterType type, EventHandler<ulong> resultHandler)
        {
            _event = evt;
            _type = type;
            _resultHandler = resultHandler;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _event.Base = renderer.ReportCounter(_type, _resultHandler);
        }
    }
}
