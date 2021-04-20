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
    public class MemoryManagerHostMapped : IMemoryManager, IDisposable, IVirtualMemoryManagerTracked
    {
        public const int PageBits = 12;
        public const int PageSize = 1 << PageBits;
        public const int PageMask = PageSize - 1;

        private readonly InvalidAccessHandler _invalidAccessHandler;

        private readonly MemoryBlock _addressSpace;
        private readonly MemoryBlock _addressSpaceMirror;

        private struct Mapping : IRange
        {
            public ulong Address { get; }
            public ulong Size { get; }
            public ulong EndAddress => Address + Size;

            public Mapping(ulong va, ulong size)
            {
                Address = va;
                Size = size;
            }

            public bool OverlapsWith(ulong address, ulong size)
            {
                return Address < address + size && address < EndAddress;
            }
        }

        private readonly RangeList<Mapping> _mappings;
        private readonly MemoryEh _memoryEh;

        private ulong[] _pageTable;

        public int AddressSpaceBits { get; }

        public IntPtr PageTablePointer => _addressSpace.Pointer;

        public MemoryManagerType Type => MemoryManagerType.HostMapped;

        public MemoryTracking Tracking { get; }

        public MemoryManagerHostMapped(ulong addressSpaceSize, InvalidAccessHandler invalidAccessHandler = null)
        {
            _invalidAccessHandler = invalidAccessHandler;

            ulong asSize = PageSize;
            int asBits = PageBits;

            while (asSize < addressSpaceSize)
            {
                asSize <<= 1;
                asBits++;
            }

            AddressSpaceBits = asBits;

            _pageTable = new ulong[1 << (AddressSpaceBits - (PageBits + 5))];
            _addressSpace = new MemoryBlock(asSize, MemoryAllocationFlags.Reserve | MemoryAllocationFlags.Mirrorable);
            _addressSpaceMirror = _addressSpace.CreateMirror();
            _mappings = new RangeList<Mapping>();
            Tracking = new MemoryTracking(this, PageSize);
            _memoryEh = new MemoryEh(_addressSpace, Tracking);
        }

        public void Map(ulong va, nuint hostAddress, ulong size)
        {
            _addressSpace.Commit(va, size);
            AddMapping(va, size);
        }

        public void Unmap(ulong va, ulong size)
        {
            RemoveMapping(va, size);
            _addressSpace.Decommit(va, size);
        }

        public T Read<T>(ulong va) where T : unmanaged
        {
            return _addressSpaceMirror.Read<T>(va);
        }

        public T ReadTracked<T>(ulong va) where T : unmanaged
        {
            SignalMemoryTracking(va, (ulong)Unsafe.SizeOf<T>(), false);
            return Read<T>(va);
        }

        public void Read(ulong va, Span<byte> data)
        {
            _addressSpaceMirror.Read(va, data);
        }

        public void Write<T>(ulong va, T value) where T : unmanaged
        {
            _addressSpaceMirror.Write(va, value);
        }

        public void Write(ulong va, ReadOnlySpan<byte> data)
        {
            SignalMemoryTracking(va, (ulong)data.Length, write: true);
            _addressSpaceMirror.Write(va, data);
        }

        public void WriteUntracked(ulong va, ReadOnlySpan<byte> data)
        {
            _addressSpaceMirror.Write(va, data);
        }

        public ReadOnlySpan<byte> GetSpan(ulong va, int size, bool tracked = false)
        {
            if (tracked)
            {
                SignalMemoryTracking(va, (ulong)size, write: false);
            }

            return _addressSpaceMirror.GetSpan(va, size);
        }

        public WritableRegion GetWritableRegion(ulong va, int size)
        {
            return _addressSpaceMirror.GetWritableRegion(va, size);
        }

        public ref T GetRef<T>(ulong va) where T : unmanaged
        {
            SignalMemoryTracking(va, (ulong)Unsafe.SizeOf<T>(), true);

            return ref _addressSpaceMirror.GetRef<T>(va);
        }

        public bool IsMapped(ulong va)
        {
            return IsRangeMapped(va, 1UL);
        }

        public bool IsRangeMapped(ulong va, ulong size)
        {
            lock (_mappings)
            {
                var ranges = Array.Empty<Mapping>();

                int count = _mappings.FindOverlapsNonOverlapping(new Mapping(va, size), ref ranges);

                ulong mappedSize = 0;

                for (int i = 0; i < count; i++)
                {
                    ref var range = ref ranges[i];

                    ulong vaInRange = Math.Max(range.Address, va);
                    ulong endVaInRange = Math.Min(range.EndAddress, va + size);

                    mappedSize += endVaInRange - vaInRange;
                }

                return mappedSize == size;
            }
        }

        public IEnumerable<HostMemoryRange> GetPhysicalRegions(ulong va, ulong size)
        {
            return new HostMemoryRange[] { new HostMemoryRange(_addressSpaceMirror.GetPointer(va, size), size) };
        }

        public void SignalMemoryTracking(ulong va, ulong size, bool write)
        {
            // Software table, used for managed memory tracking.

            int pages = GetPagesCount(va, (uint)size, out _);
            ulong pageStart = va >> PageBits;

            for (int page = 0; page < pages; page++)
            {
                int bit = (int)((pageStart & 31) << 1);

                ulong tag = (write ? 3UL : 1UL) << bit;

                int pageIndex = (int)(pageStart >> 5);
                ref ulong pageRef = ref _pageTable[pageIndex];

                ulong pte = Volatile.Read(ref pageRef);

                if ((pte & tag) != 0)
                {
                    Tracking.VirtualMemoryEvent(va, size, write);
                    break;
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

        public void TrackingReprotect(ulong va, ulong size, MemoryPermission protection)
        {
            // Protection is inverted on software pages, since the default value is 0.
            protection = (~protection) & MemoryPermission.ReadAndWrite;

            int pages = GetPagesCount(va, (uint)size, out va);
            ulong pageStart = va >> PageBits;

            // Software table, used for managed memory tracking.

            for (int page = 0; page < pages; page++)
            {
                int bit = (int)((pageStart & 31) << 1);

                ulong invTagMask = ~(3UL << bit);

                ulong tag = protection switch
                {
                    MemoryPermission.None => 0UL,
                    MemoryPermission.Write => 2UL << bit,
                    _ => 3UL << bit
                };

                int pageIndex = (int)(pageStart >> 5);
                ref ulong pageRef = ref _pageTable[pageIndex];

                ulong pte;

                do
                {
                    pte = Volatile.Read(ref pageRef);
                }
                while (Interlocked.CompareExchange(ref pageRef, (pte & invTagMask) | tag, pte) != pte);

                pageStart++;
            }

            protection = protection switch
            {
                MemoryPermission.None => MemoryPermission.ReadAndWrite,
                MemoryPermission.Write => MemoryPermission.Read,
                _ => MemoryPermission.None
            };

            _addressSpace.Reprotect(va, size, protection);
        }

        public CpuRegionHandle BeginTracking(ulong address, ulong size)
        {
            return new CpuRegionHandle(Tracking.BeginTracking(address, size));
        }

        public CpuMultiRegionHandle BeginGranularTracking(ulong address, ulong size, ulong granularity)
        {
            return new CpuMultiRegionHandle(Tracking.BeginGranularTracking(address, size, granularity));
        }

        public CpuSmartMultiRegionHandle BeginSmartGranularTracking(ulong address, ulong size, ulong granularity)
        {
            return new CpuSmartMultiRegionHandle(Tracking.BeginSmartGranularTracking(address, size, granularity));
        }

        private void AddMapping(ulong va, ulong size)
        {
            lock (_mappings)
            {
                ulong endAddress = va + size;

                var ranges = Array.Empty<Mapping>();

                int count = _mappings.FindOverlapsNonOverlapping(new Mapping(va, size), ref ranges);

                for (int i = 0; i < count; i++)
                {
                    ref var range = ref ranges[i];

                    if (va > range.Address)
                    {
                        va = range.Address;
                    }

                    if (endAddress < range.EndAddress)
                    {
                        endAddress = range.EndAddress;
                    }

                    _mappings.Remove(range);
                }

                _mappings.Add(new Mapping(va, endAddress - va));
            }
        }

        private void RemoveMapping(ulong va, ulong size)
        {
            lock (_mappings)
            {
                var ranges = Array.Empty<Mapping>();

                int count = _mappings.FindOverlapsNonOverlapping(new Mapping(va, size), ref ranges);

                for (int i = 0; i < count; i++)
                {
                    ref var range = ref ranges[i];

                    _mappings.Remove(range);

                    if (range.Address < va)
                    {
                        _mappings.Add(new Mapping(range.Address, va - range.Address));
                    }

                    if (range.EndAddress > va + size)
                    {
                        ulong delta = range.EndAddress - (va + size);

                        _mappings.Add(new Mapping(range.EndAddress - delta, delta));
                    }
                }
            }
        }

        public void Dispose()
        {
            _addressSpace.Dispose();
            _addressSpaceMirror.Dispose();
            _memoryEh.Dispose();
        }
    }
}
