namespace Ryujinx.Graphics.GAL.Multithreading.Commands
{
    struct DrawIndexedCommand : IGALCommand
    {
        public CommandType CommandType => CommandType.DrawIndexed;
        private int _indexCount;
        private int _instanceCount;
        private int _firstIndex;
        private int _firstVertex;
        private int _firstInstance;

        public void Set(int indexCount, int instanceCount, int firstIndex, int firstVertex, int firstInstance)
        {
            _indexCount = indexCount;
            _instanceCount = instanceCount;
            _firstIndex = firstIndex;
            _firstVertex = firstVertex;
            _firstInstance = firstInstance;
        }

        public void Run(ThreadedRenderer threaded, IRenderer renderer)
        {
            renderer.Pipeline.DrawIndexed(_indexCount, _instanceCount, _firstIndex, _firstVertex, _firstInstance);
        }
    }
}
