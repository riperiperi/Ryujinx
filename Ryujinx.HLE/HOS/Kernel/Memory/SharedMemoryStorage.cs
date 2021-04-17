using Ryujinx.HLE.HOS.Kernel.Process;
using Ryujinx.Memory;
using System;

namespace Ryujinx.HLE.HOS.Kernel.Memory
{
    class SharedMemoryStorage
    {
        private readonly KernelContext _context;
        private readonly KPageList _pageList;
        private readonly ulong _size;

        private IVirtualMemoryManager _borrowerMemory;
        private ulong _borrowerVa;

        public SharedMemoryStorage(KernelContext context, KPageList pageList)
        {
            _context = context;
            _pageList = pageList;
            _size = pageList.GetPagesCount() * KPageTableBase.PageSize;
        }

        public void Borrow(KProcess dstProcess, ulong va)
        {
            ulong currentOffset = 0;

            foreach (KPageNode pageNode in _pageList)
            {
                ulong size = pageNode.PagesCount * KPageTableBase.PageSize;

                dstProcess.CpuMemory.Write(va + currentOffset, _context.Memory.GetSpan(pageNode.Address + currentOffset, (int)size));

                currentOffset += size;
            }

            _borrowerMemory = dstProcess.CpuMemory;
            _borrowerVa = va;
        }

        public void ZeroFill()
        {
            for (ulong offset = 0; offset < _size; offset += sizeof(ulong))
            {
                GetRef<ulong>(offset) = 0;
            }
        }

        public ref T GetRef<T>(ulong offset) where T : unmanaged
        {
            if (_borrowerMemory == null)
            {
                if (_pageList.Nodes.Count == 1)
                {
                    ulong address = _pageList.Nodes.First.Value.Address - DramMemoryMap.DramBase;
                    return ref _context.Memory.GetRef<T>(address + offset);
                }

                throw new NotImplementedException("Non-contiguous shared memory is not yet supported.");
            }
            else
            {
                return ref _borrowerMemory.GetRef<T>(_borrowerVa + offset);
            }
        }

        public KPageList GetPageList()
        {
            return _pageList;
        }
    }
}
