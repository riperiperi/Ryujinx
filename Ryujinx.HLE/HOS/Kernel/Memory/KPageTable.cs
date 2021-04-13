using Ryujinx.HLE.HOS.Kernel.Common;
using Ryujinx.Memory;
using System;
using System.Diagnostics;

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

        protected override KernelResult Remap(ulong src, ulong dst, ulong size, KMemoryPermission oldSrcPermission, KMemoryPermission newDstPermission)
        {
            ulong pagesCount = size / PageSize;

            KPageList pageList = new KPageList();

            AddVaRangeToPageList(pageList, src, pagesCount);

            KernelResult result = MmuChangePermission(src, pagesCount, KMemoryPermission.None);

            if (result != KernelResult.Success)
            {
                return result;
            }

            result = MapPages(dst, pageList, newDstPermission);

            if (result != KernelResult.Success)
            {
                KernelResult restorePermissionResult = MmuChangePermission(src, pagesCount, oldSrcPermission);
                Debug.Assert(restorePermissionResult == KernelResult.Success);
            }

            return result;
        }

        protected override KernelResult Unremap(ulong dst, ulong src, ulong size, KMemoryPermission oldDstPermission, KMemoryPermission newSrcPermission)
        {
            KPageList srcPageList = new KPageList();
            KPageList dstPageList = new KPageList();

            ulong pagesCount = size / PageSize;

            AddVaRangeToPageList(srcPageList, src, pagesCount);
            AddVaRangeToPageList(dstPageList, dst, pagesCount);

            if (!dstPageList.IsEqual(srcPageList))
            {
                return KernelResult.InvalidMemRange;
            }

            KernelResult result = MmuUnmap(dst, pagesCount);

            if (result != KernelResult.Success)
            {
                return result;
            }

            result = MmuChangePermission(src, pagesCount, newSrcPermission);

            if (result != KernelResult.Success)
            {
                KernelResult mapPagesResult = MapPages(dst, dstPageList, oldDstPermission);
                Debug.Assert(mapPagesResult == KernelResult.Success);
            }

            return result;
        }

        protected override KernelResult MapPages(ulong address, KPageList pageList, KMemoryPermission permission)
        {
            ulong currAddr = address;

            KernelResult result = KernelResult.Success;

            foreach (KPageNode pageNode in pageList)
            {
                result = DoMmuOperation(
                    currAddr,
                    pageNode.PagesCount,
                    pageNode.Address,
                    true,
                    permission,
                    MemoryOperation.MapPa);

                if (result != KernelResult.Success)
                {
                    ulong pagesCount = (address - currAddr) / PageSize;

                    result = MmuUnmap(address, pagesCount);

                    break;
                }

                currAddr += pageNode.PagesCount * PageSize;
            }

            return result;
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

        protected override void AddVaRangeToPageList(KPageList pageList, ulong start, ulong pagesCount)
        {
            ulong address = start;

            while (address < start + pagesCount * PageSize)
            {
                if (!TryConvertVaToPa(address, out ulong pa))
                {
                    throw new InvalidOperationException("Unexpected failure translating virtual address.");
                }

                pageList.AddRange(pa, 1);

                address += PageSize;
            }
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
