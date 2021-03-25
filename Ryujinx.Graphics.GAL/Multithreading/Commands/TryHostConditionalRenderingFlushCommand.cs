using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class TryHostConditionalRenderingFlushCommand : IGALCommand
    {
        private ThreadedCounterEvent _value;
        private ThreadedCounterEvent _compare;
        private bool _isEqual;

        public TryHostConditionalRenderingFlushCommand(ThreadedCounterEvent value, ThreadedCounterEvent compare, bool isEqual)
        {
            _value = value;
            _compare = compare;
            _isEqual = isEqual;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.TryHostConditionalRendering(_value?.Base, _compare?.Base, _isEqual);
        }
    }
}
