namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    class DrawCommand : IGALCommand
    {
        private int _vertexCount;
        private int _instanceCount;
        private int _firstVertex;
        private int _firstInstance;

        public DrawCommand(int vertexCount, int instanceCount, int firstVertex, int firstInstance)
        {
            _vertexCount = vertexCount;
            _instanceCount = instanceCount;
            _firstVertex = firstVertex;
            _firstInstance = firstInstance;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.Draw(_vertexCount, _instanceCount, _firstVertex, _firstInstance);
        }
    }
}
