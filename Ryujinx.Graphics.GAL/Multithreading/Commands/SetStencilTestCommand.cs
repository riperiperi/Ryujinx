namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct SetStencilTestCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.SetStencilTest;
        private StencilTestDescriptor _stencilTest;

        public void Set(StencilTestDescriptor stencilTest)
        {
            _stencilTest = stencilTest;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetStencilTest(_stencilTest);
        }
    }
}
