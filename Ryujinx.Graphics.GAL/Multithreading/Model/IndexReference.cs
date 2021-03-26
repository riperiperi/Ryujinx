namespace Ryujinx.Graphics.GAL.Multithreading.Model
{
    struct TableRef<T>
    {
        private int _index;

        public TableRef(ThreadedRenderer renderer, T reference)
        {
            _index = renderer.AddTableRef(reference);
        }

        public T Get(ThreadedRenderer renderer)
        {
            return (T)renderer.RemoveTableRef(_index);
        }
    }
}
