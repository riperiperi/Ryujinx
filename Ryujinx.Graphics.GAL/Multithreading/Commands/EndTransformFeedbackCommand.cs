namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct EndTransformFeedbackCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.EndTransformFeedback;

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.EndTransformFeedback();
        }
    }
}
