namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct BeginTransformFeedbackCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.BeginTransformFeedback;
        private PrimitiveTopology _topology;

        public void Set(PrimitiveTopology topology)
        {
            _topology = topology;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.BeginTransformFeedback(_topology);
        }
    }
}
