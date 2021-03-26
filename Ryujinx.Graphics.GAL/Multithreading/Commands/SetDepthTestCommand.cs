namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetDepthTestCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetDepthTest;
        private DepthTestDescriptor _depthTest;

        public void Set(DepthTestDescriptor depthTest)
        {
            _depthTest = depthTest;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetDepthTest(_depthTest);
        }
    }
}
