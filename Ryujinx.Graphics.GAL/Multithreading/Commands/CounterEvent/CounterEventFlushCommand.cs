using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.CounterEvent
{
    class CounterEventFlushCommand : IGALCommand
    {
        private ThreadedCounterEvent _event;

        public CounterEventFlushCommand(ThreadedCounterEvent evt)
        {
            _event = evt;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _event.Base.Flush();
        }
    }
}
