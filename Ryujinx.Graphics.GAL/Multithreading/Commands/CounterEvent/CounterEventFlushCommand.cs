using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.CounterEvent
{
    struct CounterEventFlushCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.CounterEventFlush;
        private TableRef<ThreadedCounterEvent> _event;

        public void Set(TableRef<ThreadedCounterEvent> evt)
        {
            _event = evt;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _event.Get(threaded).Base.Flush();
        }
    }
}
