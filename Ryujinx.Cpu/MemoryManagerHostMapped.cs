using ARMeilleure.Memory;
using Ryujinx.Cpu.Tracking;
using Ryujinx.Memory;
using Ryujinx.Memory.Range;
using Ryujinx.Memory.Tracking;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Ryujinx.Cpu
{
    /// <summary>
    /// Represents a CPU memory manager which maps guest virtual memory directly onto a host virtual region.
    /// </summary>
    public class MemoryManagerHostMapped : MemoryManagerBase, IMemoryManager, IVirtualMemoryManagerTracked
    {
        public const int PageBits = 12;
        public const int PageSize = 1 << PageBits;
        public const int PageMask = PageSize - 1;

        public const int PageToPteShift = 5; // 32 pages (2 bits each) in one ulong page table entry.

        private readonly InvalidAccessHandler _invalidAccessHandler;

        private readonly MemoryBlock _addressSpace;
        private readonly MemoryBlock _addressSpaceMirror;
        private readonly ulong _addressSpaceSize;

        private enum HostMappedPtBits : ulong
        {
            Unmapped = 0,
            Mapped,
            WriteTracked,
            ReadWriteTracked
        }

        private readonly IDisposable _memoryEh;

        private ulong[] _pageTable;

        public int AddressSpaceBits { get; }

        public IntPtr PageTablePointer => _addressSpace.Pointer;

        public MemoryManagerType Type => MemoryManagerType.HostMapped;

        public MemoryTracking Tracking { get; }

        /// <summary>
        /// Creates a new instance of the host mapped memory manager.
        /// </summary>
        /// <param name="addressSpaceSize">Size of the address space</param>
        /// <param name="invalidAccessHandler">Optional function to handle invalid memory accesses</param>
        public MemoryManagerHostMapped(ulong addressSpaceSize, InvalidAccessHandler invalidAccessHandler = null)
        {
            _invalidAccessHandler = invalidAccessHandler;
            _addressSpaceSize = addressSpaceSize;

            ulong asSize = PageSize;
            int asBits = PageBits;

            while (asSize < addressSpaceSize)
            {
                asSize <<= 1;
                asBits++;
            }

            AddressSpaceBits = asBits;

            _pageTable = new ulong[1 << (AddressSpaceBits - (PageBits + PageToPteShift))];
            _addressSpace = new MemoryBlock(asSize, MemoryAllocationFlags.Reserve | MemoryAllocationFlags.Mirrorable);
            _addressSpaceMirror = _addressSpace.CreateMirror();
            Tracking = new MemoryTracking(this, PageSize);
            _memoryEh = new MemoryEhMeilleure(_addressSpace, Tracking);
        }

        /// <summary>
        /// Checks if the combination of virtual address and size is part of the addressable space.
        /// </summary>
        /// <param name="va">Virtual address of the range</param>
        /// <param name="size">Size of the range in bytes</param>
        /// <returns>True if the combination of virtual address and size is part of the addressable space</returns>
        private bool ValidateAddressAndSize(ulong va, ulong size)
        {
            ulong endVa = va + size;
            return endVa >= va && endVa >= size && endVa <= _addressSpaceSize;
        }

        /// <summary>
        /// Ensures the combination of virtual address and size is part of the addressable space.
        /// </summary>
        /// <param name="va">Virtual address of the range</param>
        /// <param name="size">Size of the range in bytes</param>
        /// <exception cref="InvalidMemoryRegionException">Throw when the memory region specified outside the addressable space</exception>
        private void AssertValidAddressAndSize(ulong va, ulong size)
        {
            if (!ValidateAddressAndSize(va, size))
            {
                throw new InvalidMemoryRegionException($"va=0x{va:X16}, size=0x{size:X16}");
            }
        }

        /// <summary>
        /// Ensures the combination of virtual address and size is part of the addressable space and fully mapped.
        /// </summary>
        /// <param name="va">Virtual address of the range</param>
        /// <param name="size">Size of the range in bytes</param>
        private void AssertMapped(ulong va, ulong size)
        {
            if (!ValidateAddressAndSize(va, size) || !IsRangeMappedImpl(va, size))
            {
                throw new InvalidMemoryRegionException($"Not mapped: va=0x{va:X16}, size=0x{size:X16}");
            }
        }

        /// <summary>
        /// Maps a virtual memory range into a physical memory range.
        /// </summary>
        /// <remarks>
        /// Addresses and size must be page aligned.
        /// </remarks>
        /// <param name="va">Virtual memory address</param>
        /// <param name="hostAddress">Pointer where the region should be mapped to</param>
        /// <param name="size">Size to be mapped</param>
        public void Map(ulong va, nuint hostAddress, ulong size)
        {
            AssertValidAddressAndSize(va, size);

            _addressSpace.Commit(va, size);
            AddMapping(va, size);

            Tracking.Map(va, size);
        }

        /// <summary>
        /// Unmaps a previously mapped range of virtual memory.
        /// </summary>
        /// <param name="va">Virtual address of the range to be unmapped</param>
        /// <param name="size">Size of the range to be unmapped</param>
        public void Unmap(ulong va, ulong size)
        {
            AssertValidAddressAndSize(va, size);

            Tracking.Unmap(va, size);

            RemoveMapping(va, size);
            _addressSpace.Decommit(va, size);
        }

        /// <summary>
        /// Reads data from CPU mapped memory.
        /// </summary>
        /// <typeparam name="T">Type of the data being read</typeparam>
        /// <param name="va">Virtual address of the data in memory</param>
        /// <returns>The data</returns>
        /// <exception cref="InvalidMemoryRegionException">Throw for unhandled invalid or unmapped memory accesses</exception>
        public T Read<T>(ulong va) where T : unmanaged
        {
            AssertMapped(va, (ulong)Unsafe.SizeOf<T>());

            return _addressSpaceMirror.Read<T>(va);
        }

        /// <summary>
        /// Reads data from CPU mapped memory, with read tracking
        /// </summary>
        /// <typeparam name="T">Type of the data being read</typeparam>
        /// <param name="va">Virtual address of the data in memory</param>
        /// <returns>The data</returns>
        public T ReadTracked<T>(ulong va) where T : unmanaged
        {
            SignalMemoryTracking(va, (ulong)Unsafe.SizeOf<T>(), false);

            return Read<T>(va);
        }

        /// <summary>
        /// Reads data from CPU mapped memory.
        /// </summary>
        /// <param name="va">Virtual address of the data in memory</param>
        /// <param name="data">Span to store the data being read into</param>
        /// <exception cref="InvalidMemoryRegionException">Throw for unhandled invalid or unmapped memory accesses</exception>
        public void Read(ulong va, Span<byte> data)
        {
            AssertMapped(va, (ulong)data.Length);

            _addressSpaceMirror.Read(va, data);
        }

        /// <summary>
        /// Writes data to CPU mapped memory.
        /// </summary>
        /// <typeparam name="T">Type of the data being written</typeparam>
        /// <param name="va">Virtual address to write the data into</param>
        /// <param name="value">Data to be written</param>
        /// <exception cref="InvalidMemoryRegionException">Throw for unhandled invalid or unmapped memory accesses</exception>
        public void Write<T>(ulong va, T value) where T : unmanaged
        {
            SignalMemoryTracking(va, (ulong)Unsafe.SizeOf<T>(), write: true);

            _addressSpaceMirror.Write(va, value);
        }

        /// <summary>
        /// Writes data to CPU mapped memory, with write tracking.
        /// </summary>
        /// <param name="va">Virtual address to write the data into</param>
        /// <param name="data">Data to be written</param>
        /// <exception cref="InvalidMemoryRegionException">Throw for unhandled invalid or unmapped memory accesses</exception>
        public void Write(ulong va, ReadOnlySpan<byte> data)
        {
            SignalMemoryTracking(va, (ulong)data.Length, write: true);

            _addressSpaceMirror.Write(va, data);
        }

        /// <summary>
        /// Writes data to CPU mapped memory, without write tracking.
        /// </summary>
        /// <param name="va">Virtual address to write the data into</param>
        /// <param name="data">Data to be written</param>
        public void WriteUntracked(ulong va, ReadOnlySpan<byte> data)
        {
            AssertMapped(va, (ulong)data.Length);

            _addressSpaceMirror.Write(va, data);
        }

        /// <summary>
        /// Gets a read-only span of data from CPU mapped memory.
        /// </summary>
        /// <param name="va">Virtual address of the data</param>
        /// <param name="size">Size of the data</param>
        /// <param name="tracked">True if read tracking is triggered on the span</param>
        /// <returns>A read-only span of the data</returns>
        /// <exception cref="InvalidMemoryRegionException">Throw for unhandled invalid or unmapped memory accesses</exception>
        public ReadOnlySpan<byte> GetSpan(ulong va, int size, bool tracked = false)
        {
            if (tracked)
            {
                SignalMemoryTracking(va, (ulong)size, write: false);
            }
            else
            {
                AssertMapped(va, (ulong)size);
            }

            return _addressSpaceMirror.GetSpan(va, size);
        }

        /// <summary>
        /// Gets a region of memory that can be written to.
        /// </summary>
        /// <param name="va">Virtual address of the data</param>
        /// <param name="size">Size of the data</param>
        /// <returns>A writable region of memory containing the data</returns>
        /// <exception cref="InvalidMemoryRegionException">Throw for unhandled invalid or unmapped memory accesses</exception>
        public WritableRegion GetWritableRegion(ulong va, int size)
        {
            AssertMapped(va, (ulong)size);

            return _addressSpaceMirror.GetWritableRegion(va, size);
        }

        public ref T GetRef<T>(ulong va) where T : unmanaged
        {
            SignalMemoryTracking(va, (ulong)Unsafe.SizeOf<T>(), true);

            return ref _addressSpaceMirror.GetRef<T>(va);
        }

        /// <summary>
        /// Checks if the page at a given CPU virtual address is mapped.
        /// </summary>
        /// <param name="va">Virtual address to check</param>
        /// <returns>True if the address is mapped, false otherwise</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMapped(ulong va)
        {
            ulong page = va >> PageBits;

            int bit = (int)((page & 31) << 1);

            int pageIndex = (int)(page >> PageToPteShift);
            ref ulong pageRef = ref _pageTable[pageIndex];

            ulong pte = Volatile.Read(ref pageRef);

            return ((pte >> bit) & 3) != 0;
        }

        /// <summary>
        /// Checks if a memory range is mapped.
        /// </summary>
        /// <param name="va">Virtual address of the range</param>
        /// <param name="size">Size of the range in bytes</param>
        /// <returns>True if the entire range is mapped, false otherwise</returns>
        public bool IsRangeMapped(ulong va, ulong size)
        {
            AssertValidAddressAndSize(va, size);

            return IsRangeMappedImpl(va, size);
        }

        private bool IsRangeMappedImpl(ulong va, ulong size)
        {
            int pages = GetPagesCount(va, (uint)size, out _);
            ulong pageStart = va >> PageBits;

            for (int page = 0; page < pages; page++)
            {
                int bit = (int)((pageStart & 31) << 1);

                int pageIndex = (int)(pageStart >> PageToPteShift);
                ref ulong pageRef = ref _pageTable[pageIndex];

                ulong pte = Volatile.Read(ref pageRef);

                if (((pte >> bit) & 3) == 0)
                {
                    return false;
                }

                pageStart++;
            }

            return true;
        }

        /// <summary>
        /// Gets the physical regions that make up the given virtual address region.
        /// If any part of the virtual region is unmapped, null is returned.
        /// </summary>
        /// <param name="va">Virtual address of the range</param>
        /// <param name="size">Size of the range</param>
        /// <returns>Array of physical regions</returns>
        public IEnumerable<HostMemoryRange> GetPhysicalRegions(ulong va, ulong size)
        {
            AssertMapped(va, size);

            return new HostMemoryRange[] { new HostMemoryRange(_addressSpaceMirror.GetPointer(va, size), size) };
        }

        /// <summary>
        /// Alerts the memory tracking that a given region has been read from or written to.
        /// This should be called before read/write is performed.
        /// This function also validates that the given range is both valid and mapped, and will throw if it is not.
        /// </summary>
        /// <param name="va">Virtual address of the region</param>
        /// <param name="size">Size of the region</param>
        public void SignalMemoryTracking(ulong va, ulong size, bool write)
        {
            AssertValidAddressAndSize(va, size);

            // Software table, used for managed memory tracking.

            int pages = GetPagesCount(va, (uint)size, out _);
            ulong pageStart = va >> PageBits;

            ulong tag = (ulong)(write ? HostMappedPtBits.WriteTracked : HostMappedPtBits.ReadWriteTracked);

            for (int page = 0; page < pages; page++)
            {
                int bit = (int)((pageStart & 31) << 1);

                int pageIndex = (int)(pageStart >> PageToPteShift);
                ref ulong pageRef = ref _pageTable[pageIndex];

                ulong pte = Volatile.Read(ref pageRef);
                ulong state = ((pte >> bit) & 3);

                if (state >= tag)
                {
                    Tracking.VirtualMemoryEvent(va, size, write);
                    break;
                }
                else if (state == 0)
                {
                    ThrowInvalidMemoryRegionException($"Not mapped: va=0x{va:X16}, size=0x{size:X16}");
                }

                pageStart++;
            }
        }

        /// <summary>
        /// Computes the number of pages in a virtual address range.
        /// </summary>
        /// <param name="va">Virtual address of the range</param>
        /// <param name="size">Size of the range</param>
        /// <param name="startVa">The virtual address of the beginning of the first page</param>
        /// <remarks>This function does not differentiate between allocated and unallocated pages.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetPagesCount(ulong va, uint size, out ulong startVa)
        {
            // WARNING: Always check if ulong does not overflow during the operations.
            startVa = va & ~(ulong)PageMask;
            ulong vaSpan = (va - startVa + size + PageMask) & ~(ulong)PageMask;

            return (int)(vaSpan / PageSize);
        }

        /// <summary>
        /// Reprotect a region of virtual memory for tracking. Sets software protection bits.
        /// </summary>
        /// <param name="va">Virtual address base</param>
        /// <param name="size">Size of the region to protect</param>
        /// <param name="protection">Memory protection to set</param>
        public void TrackingReprotect(ulong va, ulong size, MemoryPermission protection)
        {
            // Protection is inverted on software pages, since the default value is 0.
            protection = (~protection) & MemoryPermission.ReadAndWrite;

            int pages = GetPagesCount(va, (uint)size, out va);
            ulong pageStart = va >> PageBits;
            ulong protTag = protection switch
            {
                MemoryPermission.None => (ulong)HostMappedPtBits.Mapped,
                MemoryPermission.Write => (ulong)HostMappedPtBits.WriteTracked,
                _ => (ulong)HostMappedPtBits.ReadWriteTracked,
            };

            // Software table, used for managed memory tracking.

            for (int page = 0; page < pages; page++)
            {
                int bit = (int)((pageStart & 31) << 1);

                ulong tagMask = 3UL << bit;
                ulong invTagMask = ~tagMask;

                ulong tag = protTag << bit;

                int pageIndex = (int)(pageStart >> PageToPteShift);
                ref ulong pageRef = ref _pageTable[pageIndex];

                ulong pte;

                do
                {
                    pte = Volatile.Read(ref pageRef);
                }
                while ((pte & tagMask) != 0 && Interlocked.CompareExchange(ref pageRef, (pte & invTagMask) | tag, pte) != pte);

                pageStart++;
            }

            protection = protection switch
            {
                MemoryPermission.None => MemoryPermission.ReadAndWrite,
                MemoryPermission.Write => MemoryPermission.Read,
                _ => MemoryPermission.None
            };

            _addressSpace.Reprotect(va, size, protection, false);
        }

        /// <summary>
        /// Obtains a memory tracking handle for the given virtual region. This should be disposed when finished with.
        /// </summary>
        /// <param name="address">CPU virtual address of the region</param>
        /// <param name="size">Size of the region</param>
        /// <returns>The memory tracking handle</returns>
        public CpuRegionHandle BeginTracking(ulong address, ulong size)
        {
            return new CpuRegionHandle(Tracking.BeginTracking(address, size));
        }

        /// <summary>
        /// Obtains a memory tracking handle for the given virtual region, with a specified granularity. This should be disposed when finished with.
        /// </summary>
        /// <param name="address">CPU virtual address of the region</param>
        /// <param name="size">Size of the region</param>
        /// <param name="granularity">Desired granularity of write tracking</param>
        /// <returns>The memory tracking handle</returns>
        public CpuMultiRegionHandle BeginGranularTracking(ulong address, ulong size, ulong granularity)
        {
            return new CpuMultiRegionHandle(Tracking.BeginGranularTracking(address, size, granularity));
        }

        /// <summary>
        /// Obtains a smart memory tracking handle for the given virtual region, with a specified granularity. This should be disposed when finished with.
        /// </summary>
        /// <param name="address">CPU virtual address of the region</param>
        /// <param name="size">Size of the region</param>
        /// <param name="granularity">Desired granularity of write tracking</param>
        /// <returns>The memory tracking handle</returns>
        public CpuSmartMultiRegionHandle BeginSmartGranularTracking(ulong address, ulong size, ulong granularity)
        {
            return new CpuSmartMultiRegionHandle(Tracking.BeginSmartGranularTracking(address, size, granularity));
        }

        private void AddMapping(ulong va, ulong size)
        {
            int pages = GetPagesCount(va, (uint)size, out va);
            ulong pageStart = va >> PageBits;

            for (int page = 0; page < pages; page++)
            {
                int bit = (int)((pageStart & 31) << 1);

                ulong tagMask = 3UL << bit;
                ulong invTagMask = ~tagMask;

                ulong tag = (ulong)HostMappedPtBits.Mapped << bit;

                int pageIndex = (int)(pageStart >> PageToPteShift);
                ref ulong pageRef = ref _pageTable[pageIndex];

                ulong pte;

                do
                {
                    pte = Volatile.Read(ref pageRef);
                }
                while ((pte & tagMask) == 0 && Interlocked.CompareExchange(ref pageRef, (pte & invTagMask) | tag, pte) != pte);

                pageStart++;
            }
        }

        private void RemoveMapping(ulong va, ulong size)
        {
            int pages = GetPagesCount(va, (uint)size, out va);
            ulong pageStart = va >> PageBits;

            for (int page = 0; page < pages; page++)
            {
                int bit = (int)((pageStart & 31) << 1);

                ulong invTagMask = ~(3UL << bit);

                int pageIndex = (int)(pageStart >> PageToPteShift);
                ref ulong pageRef = ref _pageTable[pageIndex];

                ulong pte;

                do
                {
                    pte = Volatile.Read(ref pageRef);
                }
                while (Interlocked.CompareExchange(ref pageRef, pte & invTagMask, pte) != pte);

                pageStart++;
            }
        }

        /// <summary>
        /// Disposes of resources used by the memory manager.
        /// </summary>
        protected override void Destroy()
        {
            _addressSpaceMirror.Dispose();
            _addressSpace.Dispose();
            _memoryEh.Dispose();
        }

        private void ThrowInvalidMemoryRegionException(string message) => throw new InvalidMemoryRegionException(message);
    }
}
