using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.Memory;
using System;

namespace Ryujinx.HLE.HOS.Kernel.Memory
{
    class KPageTable : KPageTableBase
    {
        private readonly IVirtualMemoryManager _cpuMemory;

        public KPageTable(KernelContext context, IVirtualMemoryManager cpuMemory) : base(context)
        {
            _cpuMemory = cpuMemory;
        }

        protected override void SignalMemoryTracking(ulong va, ulong size, bool write)
        {
            _cpuMemory.SignalMemoryTracking(va, size, write);
        }

        protected override ReadOnlySpan<byte> GetSpan(ulong va, int size)
        {
            return _cpuMemory.GetSpan(va, size);
        }

        protected override void Write(ulong va, ReadOnlySpan<byte> data)
        {
            _cpuMemory.Write(va, data);
        }

        protected override KernelResult MmuUnmap(ulong address, ulong pagesCount)
        {
            return DoMmuOperation(
                address,
                pagesCount,
                0,
                false,
                KMemoryPermission.None,
                MemoryOperation.Unmap);
        }

        protected override KernelResult MmuChangePermission(ulong address, ulong pagesCount, KMemoryPermission permission)
        {
            return DoMmuOperation(
                address,
                pagesCount,
                0,
                false,
                permission,
                MemoryOperation.ChangePermRw);
        }

        protected override KernelResult DoMmuOperation(
            ulong dstVa,
            ulong pagesCount,
            ulong srcPa,
            bool map,
            KMemoryPermission permission,
            MemoryOperation operation)
        {
            if (map != (operation == MemoryOperation.MapPa))
            {
                throw new ArgumentException(nameof(map) + " value is invalid for this operation.");
            }

            ulong size = pagesCount * PageSize;

            KernelResult result;

            switch (operation)
            {
                case MemoryOperation.MapPa:
                    _cpuMemory.Map(dstVa, srcPa - DramMemoryMap.DramBase, size);

                    result = KernelResult.Success;

                    break;

                case MemoryOperation.Allocate:
                    KMemoryRegionManager region = GetMemoryRegionManager();

                    result = region.AllocatePages(pagesCount, AslrDisabled, out KPageList pageList);

                    if (result == KernelResult.Success)
                    {
                        result = MmuMapPages(dstVa, pageList);
                    }

                    break;

                case MemoryOperation.Unmap:
                    _cpuMemory.Unmap(dstVa, size);

                    result = KernelResult.Success;

                    break;

                case MemoryOperation.ChangePermRw: result = KernelResult.Success; break;
                case MemoryOperation.ChangePermsAndAttributes: result = KernelResult.Success; break;

                default: throw new ArgumentException($"Invalid operation \"{operation}\".");
            }

            return result;
        }

        protected override KernelResult DoMmuOperation(
            ulong address,
            ulong pagesCount,
            KPageList pageList,
            KMemoryPermission permission,
            MemoryOperation operation)
        {
            if (operation != MemoryOperation.MapVa)
            {
                throw new ArgumentException($"Invalid memory operation \"{operation}\" specified.");
            }

            return MmuMapPages(address, pageList);
        }

        protected override KernelResult MmuMapPages(ulong address, KPageList pageList)
        {
            foreach (KPageNode pageNode in pageList)
            {
                ulong size = pageNode.PagesCount * PageSize;

                _cpuMemory.Map(address, pageNode.Address - DramMemoryMap.DramBase, size);

                address += size;
            }

            return KernelResult.Success;
        }

        public override ulong ConvertVaToPa(ulong va)
        {
            if (!TryConvertVaToPa(va, out ulong pa))
            {
                throw new ArgumentException($"Invalid virtual address 0x{va:X} specified.");
            }

            return pa;
        }

        public override bool TryConvertVaToPa(ulong va, out ulong pa)
        {
            pa = DramMemoryMap.DramBase + _cpuMemory.GetPhysicalAddress(va);

            return true;
        }
    }
}
