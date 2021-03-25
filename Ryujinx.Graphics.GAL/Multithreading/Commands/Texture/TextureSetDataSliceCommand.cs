using Ryujinx.Graphics.GAL.Multithreading.Resources;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    class TextureSetDataSliceCommand : IGALCommand
    {
        private ThreadedTexture _texture;
        private byte[] _data;
        private int _layer;
        private int _level;

        public TextureSetDataSliceCommand(ThreadedTexture texture, ReadOnlySpan<byte> data, int layer, int level)
        {
            _texture = texture;
            _data = data.ToArray();
            _layer = layer;
            _level = level;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _texture.Base.SetData(new ReadOnlySpan<byte>(_data), _layer, _level);
        }
    }
}
