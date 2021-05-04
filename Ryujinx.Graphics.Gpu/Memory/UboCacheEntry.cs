namespace Ryujinx.Graphics.Gpu.Memory
{
    class UboCacheEntry
    {
        public ulong Address;
        public ulong EndAddress;
        public Buffer Buffer;
        public int UnmappedSequence;

        public UboCacheEntry(ulong address, Buffer buffer)
        {
            Address = address;
            EndAddress = buffer.EndAddress;
            Buffer = buffer;
            UnmappedSequence = buffer.UnmappedSequence;
        }
    }
}
