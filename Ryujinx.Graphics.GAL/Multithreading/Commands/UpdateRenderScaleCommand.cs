using Ryujinx.Graphics.Shader;

namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class UpdateRenderScaleCommand : IGALCommand
    {
        private ShaderStage _stage;
        private float[] _scales;
        private int _textureCount;
        private int _imageCount;

        public UpdateRenderScaleCommand(ShaderStage stage, float[] scales, int textureCount, int imageCount)
        {
            _stage = stage;
            _scales = scales;
            _textureCount = textureCount;
            _imageCount = imageCount;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.UpdateRenderScale(_stage, _scales, _textureCount, _imageCount);
        }
    }
}
