using Ryujinx.Graphics.GAL.Multithreading.Commands.Texture;
using System;

namespace Ryujinx.Graphics.GAL.Multithreading.Resources
{
    /// <summary>
    /// Threaded representation of a texture.
    /// </summary>
    class ThreadedTexture : ITexture
    {
        private ThreadedRenderer _renderer;
        private TextureCreateInfo _info;
        public ITexture Base;

        public int Width => _info.Width;

        public int Height => _info.Height;

        public float ScaleFactor { get; }

        public ThreadedTexture(ThreadedRenderer renderer, TextureCreateInfo info, float scale)
        {
            _renderer = renderer;
            _info = info;
            ScaleFactor = scale;
        }

        public void CopyTo(ITexture destination, int firstLayer, int firstLevel)
        {
            _renderer.QueueCommand(new TextureCopyToCommand(this, destination as ThreadedTexture, firstLayer, firstLevel));
        }

        public void CopyTo(ITexture destination, int srcLayer, int dstLayer, int srcLevel, int dstLevel)
        {
            _renderer.QueueCommand(new TextureCopyToSliceCommand(this, destination as ThreadedTexture, srcLayer, dstLayer, srcLevel, dstLevel));
        }

        public void CopyTo(ITexture destination, Extents2D srcRegion, Extents2D dstRegion, bool linearFilter)
        {
            ThreadedTexture dest = destination as ThreadedTexture;

            if (_renderer.IsGpuThread())
            {
                _renderer.QueueCommand(new TextureCopyToScaledCommand(this, dest, srcRegion, dstRegion, linearFilter));
            }
            else
            {
                // Scaled copy can happen on another thread for a res scale flush.
                ThreadedHelpers.SpinUntilNonNull(ref Base);
                ThreadedHelpers.SpinUntilNonNull(ref dest.Base);

                Base.CopyTo(dest.Base, srcRegion, dstRegion, linearFilter);
            }
        }

        public ITexture CreateView(TextureCreateInfo info, int firstLayer, int firstLevel)
        {
            ThreadedTexture newTex = new ThreadedTexture(_renderer, info, ScaleFactor);
            _renderer.QueueCommand(new TextureCreateViewCommand(this, newTex, info, firstLayer, firstLevel));

            return newTex;
        }

        public byte[] GetData()
        {
            if (_renderer.IsGpuThread())
            {
                var cmd = new TextureGetDataCommand(this);
                _renderer.InvokeCommand(cmd);

                return cmd.Result;
            }
            else
            {
                ThreadedHelpers.SpinUntilNonNull(ref Base);

                return Base.GetData();
            }
        }

        public void SetData(ReadOnlySpan<byte> data)
        {
            _renderer.QueueCommand(new TextureSetDataCommand(this, data));
        }

        public void SetData(ReadOnlySpan<byte> data, int layer, int level)
        {
            _renderer.QueueCommand(new TextureSetDataSliceCommand(this, data, layer, level));
        }

        public void SetStorage(BufferRange buffer)
        {
            _renderer.QueueCommand(new TextureSetStorageCommand(this, buffer));
        }

        public void Release()
        {
            _renderer.QueueCommand(new TextureReleaseCommand(this));
        }
    }
}
