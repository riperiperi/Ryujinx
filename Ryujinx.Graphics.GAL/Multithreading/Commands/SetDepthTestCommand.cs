namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetDepthTestCommand : IGALCommand
    {
        private DepthTestDescriptor _depthTest;

        public SetDepthTestCommand(DepthTestDescriptor depthTest)
        {
            _depthTest = depthTest;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetDepthTest(_depthTest);
        }
    }
}
