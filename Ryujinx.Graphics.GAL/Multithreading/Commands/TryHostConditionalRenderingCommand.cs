using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct TryHostConditionalRenderingCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.TryHostConditionalRendering;
        private TableRef<ThreadedCounterEvent> _value;
        private ulong _compare;
        private bool _isEqual;

        public void Set(TableRef<ThreadedCounterEvent> value, ulong compare, bool isEqual)
        {
            _value = value;
            _compare = compare;
            _isEqual = isEqual;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.TryHostConditionalRendering(_value.Get(threaded)?.Base, _compare, _isEqual);
        }
    }
}
