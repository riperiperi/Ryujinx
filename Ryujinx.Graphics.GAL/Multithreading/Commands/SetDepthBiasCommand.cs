namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class SetDepthBiasCommand : IGALCommand
    {
        private PolygonModeMask _enables;
        private float _factor;
        private float _units;
        private float _clamp;

        public SetDepthBiasCommand(PolygonModeMask enables, float factor, float units, float clamp)
        {
            _enables = enables;
            _factor = factor;
            _units = units;
            _clamp = clamp;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.SetDepthBias(_enables, _factor, _units, _clamp);
        }
    }
}
