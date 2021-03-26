using Ryujinx.Graphics.GAL.Multithreading.Commands.CounterEvent;
using Ryujinx.Graphics.GAL.Multithreading.Model;

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

        private TableRef<T> Ref<T>(T reference)
        {
            return new TableRef<T>(_renderer, reference);
        }


        public void Dispose()
        {
            _renderer.New<CounterEventDisposeCommand>().Set(Ref(this));
            _renderer.QueueCommand();
        }

        public void Flush()
        {
            _renderer.New<CounterEventFlushCommand>().Set(Ref(this));
            _renderer.InvokeCommand();
        }
    }
}
