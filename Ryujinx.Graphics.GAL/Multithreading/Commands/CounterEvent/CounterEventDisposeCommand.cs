using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.CounterEvent
{
    class CounterEventDisposeCommand : IGALCommand
    {
        private ThreadedCounterEvent _event;

        public CounterEventDisposeCommand(ThreadedCounterEvent evt)
        {
            _event = evt;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _event.Base.Dispose();
        }
    }
}
