using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    class CreateSamplerCommand : IGALCommand
    {
        private ThreadedSampler _sampler;
        private SamplerCreateInfo _info;

        public CreateSamplerCommand(ThreadedSampler sampler, SamplerCreateInfo info)
        {
            _sampler = sampler;
            _info = info;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _sampler.Base = renderer.CreateSampler(_info);
        }
    }
}
