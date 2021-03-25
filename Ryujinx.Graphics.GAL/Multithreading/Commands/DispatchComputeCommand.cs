namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class DispatchComputeCommand : IGALCommand
    {
        private int _groupsX;
        private int _groupsY;
        private int _groupsZ;

        public DispatchComputeCommand(int groupsX, int groupsY, int groupsZ)
        {
            _groupsX = groupsX;
            _groupsY = groupsY;
            _groupsZ = groupsZ;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.DispatchCompute(_groupsX, _groupsY, _groupsZ);
        }
    }
}
