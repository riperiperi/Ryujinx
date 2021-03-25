namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class EndTransformFeedbackCommand : IGALCommand
    {
        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.EndTransformFeedback();
        }
    }
}
