﻿namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetScissorCommand : IGALCommand
    {
        private int _index;
        private bool _enable;
        private int _x;
        private int _y;
        private int _width;
        private int _height;

        public SetScissorCommand(int index, bool enable, int x, int y, int width, int height)
        {
            _index = index;
            _enable = enable;
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetScissor(_index, _enable, _x, _y, _width, _height);
        }
    }
}