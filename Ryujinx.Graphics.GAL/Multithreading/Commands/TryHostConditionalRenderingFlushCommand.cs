using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct TryHostConditionalRenderingFlushCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.TryHostConditionalRenderingFlush;
        private TableRef<ThreadedCounterEvent> _value;
        private TableRef<ThreadedCounterEvent> _compare;
        private bool _isEqual;

        public void Set(TableRef<ThreadedCounterEvent> value, TableRef<ThreadedCounterEvent> compare, bool isEqual)
        {
            _value = value;
            _compare = compare;
            _isEqual = isEqual;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.TryHostConditionalRendering(_value.Get(threaded)?.Base, _compare.Get(threaded)?.Base, _isEqual);
        }
    }
}
