namespace ARMeilleure.Memory
{
    /// <summary>
    /// Indicates the type of a memory manager and the method it uses for memory mapping
    /// and address translation. This controls the code generated for memory accesses on the JIT.
    /// </summary>
    public enum MemoryManagerType
    {
        /// <summary>
        /// Complete software MMU implementation, the read/write methods are always called,
        /// without any attempt to perform faster memory access.
        /// </summary>
        SoftwareMmu,

        /// <summary>
        /// High level implementation using a software flat page table for address translation,
        /// used to speed up address translation if possible without calling the read/write methods.
        /// </summary>
        SoftwarePageTable,

        /// <summary>
        /// High level implementation with mappings managed by the host OS, effectively using hardware
        /// page tables. No address translation is performed in software and the memory is just accessed directly.
        /// </summary>
        HostMapped
    }
}
