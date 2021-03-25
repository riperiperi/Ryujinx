namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class BeginTransformFeedbackCommand : IGALCommand
    {
        private PrimitiveTopology _topology;

        public BeginTransformFeedbackCommand(PrimitiveTopology topology)
        {
            _topology = topology;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.BeginTransformFeedback(_topology);
        }
    }
}
