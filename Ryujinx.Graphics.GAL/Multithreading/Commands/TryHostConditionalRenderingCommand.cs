using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class TryHostConditionalRenderingCommand : IGALCommand
    {
        private ThreadedCounterEvent _value;
        private ulong _compare;
        private bool _isEqual;

        public TryHostConditionalRenderingCommand(ThreadedCounterEvent value, ulong compare, bool isEqual)
        {
            _value = value;
            _compare = compare;
            _isEqual = isEqual;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.TryHostConditionalRendering(_value?.Base, _compare, _isEqual);
        }
    }
}
