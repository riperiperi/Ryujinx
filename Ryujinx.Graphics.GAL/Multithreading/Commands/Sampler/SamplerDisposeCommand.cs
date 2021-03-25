using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Sampler
{
    class SamplerDisposeCommand : IGALCommand
    {
        private ThreadedSampler _sampler;

        public SamplerDisposeCommand(ThreadedSampler sampler)
        {
            _sampler = sampler;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _sampler.Base.Dispose();
        }
    }
}
