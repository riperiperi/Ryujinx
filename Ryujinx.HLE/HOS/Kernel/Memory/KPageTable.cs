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

        protected override KernelResult MapHeap(ulong va, ulong size, KPageList pageList, KMemoryPermission permission)
        {
            return DoMmuOperation(
                va,
                size / PageSize,
                pageList,
                KMemoryPermission.ReadAndWrite,
                MemoryOperation.MapVa);
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
                KernelResult reprotectResult = MmuChangePermission(src, pagesCount, oldSrcPermission);
                Debug.Assert(reprotectResult == KernelResult.Success);
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
                KernelResult mapResult = MapPages(dst, dstPageList, oldDstPermission);
                Debug.Assert(mapResult == KernelResult.Success);
            }

            return result;
        }

        protected override KernelResult MapPages(ulong address, KPageList pageList, KMemoryPermission permission)
        {
            using var scopedPageList = new KScopedPageList(Context.MemoryManager, pageList);

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

                    KernelResult unmapResult = MmuUnmap(address, pagesCount);
                    Debug.Assert(unmapResult == KernelResult.Success);

                    break;
                }

                currAddr += pageNode.PagesCount * PageSize;
            }

            if (result != KernelResult.Success)
            {
                return result;
            }

            scopedPageList.SignalSuccess();

            return KernelResult.Success;
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

                    if (DramMemoryMap.IsHeapPhysicalAddress(srcPa))
                    {
                        Context.MemoryManager.IncrementPagesReferenceCount(srcPa, pagesCount);
                    }

                    result = KernelResult.Success;

                    break;

                case MemoryOperation.Allocate:
                    KMemoryRegionManager region = GetMemoryRegionManager();

                    result = region.AllocatePages(pagesCount, AslrDisabled, out KPageList pageList);

                    if (result == KernelResult.Success)
                    {
                        result = MapPages(dstVa, pageList, permission);
                    }

                    break;

                case MemoryOperation.Unmap:
                    KPageList pagesToClose = new KPageList();

                    AddVaRangeToPageList(pagesToClose, dstVa, size / PageSize);

                    _cpuMemory.Unmap(dstVa, size);

                    // TODO: Get all physical regions.
                    if (DramMemoryMap.IsHeapPhysicalAddress(DramMemoryMap.DramBase + _cpuMemory.GetPhysicalAddress(dstVa)))
                    {
                        pagesToClose.DecrementPagesReferenceCount(Context.MemoryManager);
                    }

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

            return MapPages(address, pageList, permission);
        }

        protected override void AddVaRangeToPageList(KPageList pageList, ulong start, ulong pagesCount)
        {
            ulong address = start;

            while (address < start + pagesCount * PageSize)
            {
                pageList.AddRange(DramMemoryMap.DramBase + _cpuMemory.GetPhysicalAddress(address), 1);

                address += PageSize;
            }
        }
    }
}
