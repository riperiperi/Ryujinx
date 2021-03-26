namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct ClearRenderTargetColorCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.ClearRenderTargetColor;
        private int _index;
        private uint _componentMask;
        private ColorF _color;

        public void Set(int index, uint componentMask, ColorF color)
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
