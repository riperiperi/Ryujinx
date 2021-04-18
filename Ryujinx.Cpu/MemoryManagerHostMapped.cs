using ARMeilleure.Memory;
using Ryujinx.Cpu.Tracking;
using Ryujinx.Memory;
using Ryujinx.Memory.Range;
using Ryujinx.Memory.Tracking;
using System;
using System.Collections.Generic;

namespace Ryujinx.Cpu
{
    public class MemoryManagerHostMapped : IMemoryManager, IDisposable, IVirtualMemoryManagerTracked
    {
        public const int PageBits = 12;
        public const int PageSize = 1 << PageBits;
        public const int PageMask = PageSize - 1;

        private readonly InvalidAccessHandler _invalidAccessHandler;

        private readonly MemoryBlock _addressSpace;

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

            _addressSpace = new MemoryBlock(asSize, MemoryAllocationFlags.Reserve);
            _mappings = new RangeList<Mapping>();
            Tracking = new MemoryTracking(this, PageSize);
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
            return _addressSpace.Read<T>(va);
        }

        public T ReadTracked<T>(ulong va) where T : unmanaged
        {
            return Read<T>(va);
        }

        public void Read(ulong va, Span<byte> data)
        {
            _addressSpace.Read(va, data);
        }

        public void Write<T>(ulong va, T value) where T : unmanaged
        {
            _addressSpace.Write(va, value);
        }

        public void Write(ulong offset, ReadOnlySpan<byte> data)
        {
            _addressSpace.Write(offset, data);
        }

        public void WriteUntracked(ulong va, ReadOnlySpan<byte> data)
        {
            Write(va, data);
        }

        public ReadOnlySpan<byte> GetSpan(ulong va, int size, bool tracked = false)
        {
            return _addressSpace.GetSpan(va, size);
        }

        public WritableRegion GetWritableRegion(ulong va, int size)
        {
            return _addressSpace.GetWritableRegion(va, size);
        }

        public ref T GetRef<T>(ulong va) where T : unmanaged
        {
            return ref _addressSpace.GetRef<T>(va);
        }

        public bool IsMapped(ulong va)
        {
            return IsRangeMapped(va, 1UL);
        }

        public bool IsRangeMapped(ulong va, ulong size)
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

        public IEnumerable<HostMemoryRange> GetPhysicalRegions(ulong va, ulong size)
        {
            return new HostMemoryRange[] { new HostMemoryRange(_addressSpace.GetPointer(va, size), size) };
        }

        public void SignalMemoryTracking(ulong va, ulong size, bool write)
        {
        }

        public void TrackingReprotect(ulong va, ulong size, MemoryPermission protection)
        {
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
            var ranges = Array.Empty<Mapping>();

            int count = _mappings.FindOverlapsNonOverlapping(new Mapping(va, size), ref ranges);

            for (int i = 0; i < count; i++)
            {
                ref var range = ref ranges[i];

                if (va > range.Address)
                {
                    va = range.Address;

                    ulong delta = va - range.Address;

                    size += delta;
                }

                if (range.EndAddress > va + size)
                {
                    size += range.EndAddress - (va + size);
                }

                _mappings.Remove(range);
            }

            _mappings.Add(new Mapping(va, size));
        }

        private void RemoveMapping(ulong va, ulong size)
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

        public void Dispose()
        {
            _addressSpace.Dispose();
        }
    }
}
