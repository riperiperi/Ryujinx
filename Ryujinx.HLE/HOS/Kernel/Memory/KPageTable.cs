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

        /// <inheritdoc/>
        protected override KernelResult MapMemory(ulong src, ulong dst, ulong pagesCount, KMemoryPermission oldSrcPermission, KMemoryPermission newDstPermission)
        {
            KPageList pageList = new KPageList();

            AddVaRangeToPageList(pageList, src, pagesCount);

            KernelResult result = Reprotect(src, pagesCount, KMemoryPermission.None);

            if (result != KernelResult.Success)
            {
                return result;
            }

            result = MapPages(dst, pageList, true, newDstPermission);

            if (result != KernelResult.Success)
            {
                KernelResult reprotectResult = Reprotect(src, pagesCount, oldSrcPermission);
                Debug.Assert(reprotectResult == KernelResult.Success);
            }

            return result;
        }

        /// <inheritdoc/>
        protected override KernelResult UnmapMemory(ulong dst, ulong src, ulong pagesCount, KMemoryPermission oldDstPermission, KMemoryPermission newSrcPermission)
        {
            KPageList srcPageList = new KPageList();
            KPageList dstPageList = new KPageList();

            AddVaRangeToPageList(srcPageList, src, pagesCount);
            AddVaRangeToPageList(dstPageList, dst, pagesCount);

            if (!dstPageList.IsEqual(srcPageList))
            {
                return KernelResult.InvalidMemRange;
            }

            KernelResult result = Unmap(dst, pagesCount);

            if (result != KernelResult.Success)
            {
                return result;
            }

            result = Reprotect(src, pagesCount, newSrcPermission);

            if (result != KernelResult.Success)
            {
                KernelResult mapResult = MapPages(dst, dstPageList, true, oldDstPermission);
                Debug.Assert(mapResult == KernelResult.Success);
            }

            return result;
        }

        /// <inheritdoc/>
        protected override KernelResult MapPages(ulong dstVa, ulong pagesCount, ulong srcPa, bool mustAlias, KMemoryPermission permission)
        {
            _cpuMemory.Map(dstVa, (nuint)((ulong)Context.Memory.Pointer + (srcPa - DramMemoryMap.DramBase)), pagesCount * PageSize);

            if (DramMemoryMap.IsHeapPhysicalAddress(srcPa))
            {
                Context.MemoryManager.IncrementPagesReferenceCount(srcPa, pagesCount);
            }

            return KernelResult.Success;
        }

        /// <inheritdoc/>
        protected override KernelResult MapPages(ulong address, KPageList pageList, bool mustAlias, KMemoryPermission permission)
        {
            using var scopedPageList = new KScopedPageList(Context.MemoryManager, pageList);

            ulong currAddr = address;

            foreach (KPageNode pageNode in pageList)
            {
                ulong size = pageNode.PagesCount * PageSize;

                _cpuMemory.Map(currAddr, (nuint)((ulong)Context.Memory.Pointer + (pageNode.Address - DramMemoryMap.DramBase)), size);

                currAddr += size;
            }

            scopedPageList.SignalSuccess();

            return KernelResult.Success;
        }

        /// <inheritdoc/>
        protected override KernelResult Unmap(ulong address, ulong pagesCount)
        {
            KPageList pagesToClose = new KPageList();

            AddVaRangeToPageList(pagesToClose, address, pagesCount);

            bool decRef = DramMemoryMap.IsHeapPhysicalAddress(DramMemoryMap.DramBase + GetDramAddressFromHostAddress(_cpuMemory.GetPhysicalRegions(address, PageSize)[0].hostAddress));

            _cpuMemory.Unmap(address, pagesCount * PageSize);

            // TODO: Get all physical regions.
            if (decRef)
            {
                pagesToClose.DecrementPagesReferenceCount(Context.MemoryManager);
            }

            return KernelResult.Success;
        }

        /// <inheritdoc/>
        protected override KernelResult Reprotect(ulong address, ulong pagesCount, KMemoryPermission permission)
        {
            // TODO.
            return KernelResult.Success;
        }

        /// <inheritdoc/>
        protected override KernelResult ReprotectWithAttributes(ulong address, ulong pagesCount, KMemoryPermission permission)
        {
            // TODO.
            return KernelResult.Success;
        }

        protected override void AddVaRangeToPageList(KPageList pageList, ulong start, ulong pagesCount)
        {
            ulong address = start;

            while (address < start + pagesCount * PageSize)
            {
                pageList.AddRange(DramMemoryMap.DramBase + GetDramAddressFromHostAddress(_cpuMemory.GetPhysicalRegions(address, PageSize)[0].hostAddress), 1);

                address += PageSize;
            }
        }

        private ulong GetDramAddressFromHostAddress(nuint hostAddress)
        {
            return hostAddress - (ulong)Context.Memory.Pointer;
        }
    }
}
