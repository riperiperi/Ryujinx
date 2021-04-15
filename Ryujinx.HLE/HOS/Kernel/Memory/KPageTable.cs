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
            return DoMmuOperation(dstVa, pagesCount, srcPa, true, permission, MemoryOperation.Map);
        }

        /// <inheritdoc/>
        protected override KernelResult MapPages(ulong address, KPageList pageList, bool mustAlias, KMemoryPermission permission)
        {
            return DoMmuOperation(address, pageList.GetPagesCount(), pageList, permission, MemoryOperation.MapList);
        }

        /// <inheritdoc/>
        protected override KernelResult Unmap(ulong address, ulong pagesCount)
        {
            return DoMmuOperation(address, pagesCount, 0, false, KMemoryPermission.None, MemoryOperation.Unmap);
        }

        /// <inheritdoc/>
        protected override KernelResult Reprotect(ulong address, ulong pagesCount, KMemoryPermission permission)
        {
            return DoMmuOperation(address, pagesCount, 0, false, permission, MemoryOperation.Reprotect);
        }

        /// <inheritdoc/>
        protected override KernelResult ReprotectWithAttributes(ulong address, ulong pagesCount, KMemoryPermission permission)
        {
            return DoMmuOperation(address, pagesCount, 0, false, permission, MemoryOperation.ReprotectWithAttributes);
        }

        private KernelResult DoMmuOperation(
            ulong dstVa,
            ulong pagesCount,
            ulong srcPa,
            bool map,
            KMemoryPermission permission,
            MemoryOperation operation)
        {
            if (map != (operation == MemoryOperation.Map))
            {
                throw new ArgumentException(nameof(map) + " value is invalid for this operation.");
            }

            ulong size = pagesCount * PageSize;

            KernelResult result;

            switch (operation)
            {
                case MemoryOperation.Map:
                    _cpuMemory.Map(dstVa, (nuint)((ulong)Context.Memory.Pointer + (srcPa - DramMemoryMap.DramBase)), size);

                    if (DramMemoryMap.IsHeapPhysicalAddress(srcPa))
                    {
                        Context.MemoryManager.IncrementPagesReferenceCount(srcPa, pagesCount);
                    }

                    result = KernelResult.Success;

                    break;

                case MemoryOperation.Unmap:
                    KPageList pagesToClose = new KPageList();

                    AddVaRangeToPageList(pagesToClose, dstVa, size / PageSize);

                    bool decRef = DramMemoryMap.IsHeapPhysicalAddress(DramMemoryMap.DramBase + GetDramAddressFromHostAddress(_cpuMemory.GetPhysicalRegions(dstVa, PageSize)[0].hostAddress));

                    _cpuMemory.Unmap(dstVa, size);

                    // TODO: Get all physical regions.
                    if (decRef)
                    {
                        pagesToClose.DecrementPagesReferenceCount(Context.MemoryManager);
                    }

                    result = KernelResult.Success;

                    break;

                case MemoryOperation.Reprotect: result = KernelResult.Success; break;
                case MemoryOperation.ReprotectWithAttributes: result = KernelResult.Success; break;

                default: throw new ArgumentException($"Invalid operation \"{operation}\".");
            }

            return result;
        }

        private KernelResult DoMmuOperation(
            ulong address,
            ulong pagesCount,
            KPageList pageList,
            KMemoryPermission permission,
            MemoryOperation operation)
        {
            if (operation != MemoryOperation.MapList)
            {
                throw new ArgumentException($"Invalid memory operation \"{operation}\" specified.");
            }

            return MapPagesImpl(address, pageList, permission);
        }

        private KernelResult MapPagesImpl(ulong address, KPageList pageList, KMemoryPermission permission)
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
                    MemoryOperation.Map);

                if (result != KernelResult.Success)
                {
                    ulong pagesCount = (address - currAddr) / PageSize;

                    KernelResult unmapResult = Unmap(address, pagesCount);
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
