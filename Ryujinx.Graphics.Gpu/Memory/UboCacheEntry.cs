namespace Ryujinx.Graphics.Gpu.Memory
{
    class UboCacheEntry
    {
        public ulong Address;
        public Buffer Buffer;
        public int UnmappedSequence;

        public UboCacheEntry(ulong address, Buffer buffer)
        {
            Address = address;
            Buffer = buffer;
            UnmappedSequence = buffer.UnmappedSequence;
        }
    }
}
