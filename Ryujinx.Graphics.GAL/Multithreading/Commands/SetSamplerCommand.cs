using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetSamplerCommand : IGALCommand
    {
        private int _index;
        private ThreadedSampler _sampler;

        public SetSamplerCommand(int index, ThreadedSampler sampler)
        {
            _index = index;
            _sampler = sampler;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetSampler(_index, _sampler?.Base);
        }
    }
}
