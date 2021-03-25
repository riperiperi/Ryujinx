using Ryujinx.Graphics.GAL.Multithreading.Commands.CounterEvent;

namespace Ryujinx.Graphics.GAL.Multithreading.Resources
{
    class ThreadedCounterEvent : ICounterEvent
    {
        private ThreadedRenderer _renderer;
        public ICounterEvent Base;

        public bool Invalid { get; set; }

        public CounterType Type { get; }
        public bool ClearCounter { get; }

        public ThreadedCounterEvent(ThreadedRenderer renderer, CounterType type, bool clearCounter)
        {
            _renderer = renderer;
            Type = type;
            ClearCounter = clearCounter;
        }

        public void Dispose()
        {
            _renderer.QueueCommand(new CounterEventDisposeCommand(this));
        }

        public void Flush()
        {
            var cmd = new CounterEventFlushCommand(this);

            _renderer.InvokeCommand(cmd);
        }
    }
}
