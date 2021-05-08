namespace Ryujinx.Graphics.Gpu.Memory
{
    /// <summary>
    /// A cached entry for easily locating a uniform buffer update's target buffer.
    /// </summary>
    class UboCacheEntry
    {
        /// <summary>
        /// The CPU VA of the uniform buffer destination.
        /// </summary>
        public ulong Address;

        /// <summary>
        /// The end GPU VA of the associated buffer, used to check if new data can fit.
        /// </summary>
        public ulong EndGpuAddress;

        /// <summary>
        /// The buffer associated with this cache entry.
        /// </summary>
        public Buffer Buffer;

        /// <summary>
        /// The UnmappedSequence of the buffer at the time of creation.
        /// If this differs from the value currently in the buffer, then this cache entry is outdated.
        /// </summary>
        public int UnmappedSequence;

        /// <summary>
        /// Create a new cache entry.
        /// </summary>
        /// <param name="address">The CPU VA of the uniform buffer destination</param>
        /// <param name="gpuVa">The GPU VA of the uniform buffer destination</param>
        /// <param name="buffer">The buffer object containing the target uniform buffer</param>
        public UboCacheEntry(ulong address, ulong gpuVa, Buffer buffer)
        {
            Address = address;
            EndGpuAddress = gpuVa + (buffer.EndAddress - address);
            Buffer = buffer;
            UnmappedSequence = buffer.UnmappedSequence;
        }
    }
}
