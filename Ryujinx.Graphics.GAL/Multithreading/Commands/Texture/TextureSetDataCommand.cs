using Ryujinx.Graphics.GAL.Multithreading.Resources;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands.Texture
{
    class TextureSetDataCommand : IGALCommand
    {
        private ThreadedTexture _texture;
        private byte[] _data;

        public TextureSetDataCommand(ThreadedTexture texture, ReadOnlySpan<byte> data)
        {
            _texture = texture;
            _data = data.ToArray();
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            _texture.Base.SetData(new ReadOnlySpan<byte>(_data));
        }
    }
}
