namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class ClearRenderTargetColorCommand : IGALCommand
    {
        private int _index;
        private uint _componentMask;
        private ColorF _color;

        public ClearRenderTargetColorCommand(int index, uint componentMask, ColorF color)
        {
            _index = index;
            _componentMask = componentMask;
            _color = color;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.ClearRenderTargetColor(_index, _componentMask, _color);
        }
    }
}
