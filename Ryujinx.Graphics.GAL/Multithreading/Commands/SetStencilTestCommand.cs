namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetStencilTestCommand : IGALCommand
    {
        private StencilTestDescriptor _stencilTest;

        public SetStencilTestCommand(StencilTestDescriptor stencilTest)
        {
            _stencilTest = stencilTest;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetStencilTest(_stencilTest);
        }
    }
}
