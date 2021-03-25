namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class ClearRenderTargetDepthStencilCommand : IGALCommand
    {
        private float _depthValue;
        private bool _depthMask;
        private int _stencilValue;
        private int _stencilMask;

        public ClearRenderTargetDepthStencilCommand(float depthValue, bool depthMask, int stencilValue, int stencilMask)
        {
            _depthValue = depthValue;
            _depthMask = depthMask;
            _stencilValue = stencilValue;
            _stencilMask = stencilMask;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.ClearRenderTargetDepthStencil(_depthValue, _depthMask, _stencilValue, _stencilMask);
        }
    }
}
