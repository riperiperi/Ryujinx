using Ryujinx.Graphics.GAL.Multithreading.Model;
using Ryujinx.Graphics.GAL.Multithreading.Resources;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    struct TextureSetDataSliceCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.TextureSetDataSlice;
        private TableRef<ThreadedTexture> _texture;
        private TableRef<byte[]> _data;
        private int _layer;
        private int _level;

        public void Set(TableRef<ThreadedTexture> texture, TableRef<byte[]> data, int layer, int level)
        {
            _texture = texture;
            _data = data;
            _layer = layer;
            _level = level;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            ThreadedTexture texture = _texture.Get(threaded);
            texture.Base.SetData(new ReadOnlySpan<byte>(_data.Get(threaded)), _layer, _level);
        }
    }
}
