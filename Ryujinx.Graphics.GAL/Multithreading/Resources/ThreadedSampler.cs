using Ryujinx.Graphics.GAL.Multithreading.Commands.Sampler;

namespace Ryujinx.Graphics.GAL.Multithreading.Resources
{
    class ThreadedSampler : ISampler
    {
        private ThreadedRenderer _renderer;
        public ISampler Base;

        public ThreadedSampler(ThreadedRenderer renderer)
        {
            _renderer = renderer;
        }

        public void Dispose()
        {
            _renderer.QueueCommand(new SamplerDisposeCommand(this));
        }
    }
}
