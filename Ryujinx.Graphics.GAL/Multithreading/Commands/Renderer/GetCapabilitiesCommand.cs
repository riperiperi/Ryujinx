using Ryujinx.Graphics.GAL.Multithreading.Model;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Renderer
{
    struct GetCapabilitiesCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.GetCapabilities;
        private TableRef<ResultBox<Capabilities>> _result;

        public void Set(TableRef<ResultBox<Capabilities>> result)
        {
            _result = result;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _result.Get(threaded).Result = renderer.GetCapabilities();
        }
    }
}
