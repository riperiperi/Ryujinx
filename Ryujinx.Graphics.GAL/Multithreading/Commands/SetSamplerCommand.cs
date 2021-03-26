using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetSamplerCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetSampler;
        private int _index;
        private TableRef<ThreadedSampler> _sampler;

        public void Set(int index, TableRef<ThreadedSampler> sampler)
        {
            _index = index;
            _sampler = sampler;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetSampler(_index, _sampler.Get(threaded)?.Base);
        }
    }
}
