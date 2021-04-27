using Ryujinx.Memory;
using Ryujinx.Memory.Tracking;
using System;

namespace Ryujinx.Cpu
{
    abstract class MemoryEhBase : IDisposable
    {
        private const int PageSize = 0x1000;
        private const int PageMask = PageSize - 1;

        private readonly MemoryBlock _addressSpace;
        private readonly MemoryTracking _tracking;

        public MemoryEhBase(MemoryBlock addressSpace, MemoryTracking tracking)
        {
            _addressSpace = addressSpace;
            _tracking = tracking;
        }

        protected bool HandleInRange(nuint faultAddress, bool isWrite)
        {
            if (faultAddress < (nuint)(ulong)_addressSpace.Pointer || faultAddress >= (nuint)(ulong)_addressSpace.Pointer + _addressSpace.Size)
            {
                return false;
            }

            ulong offset = ((ulong)faultAddress - (ulong)_addressSpace.Pointer) & ~(ulong)PageMask;

            _tracking.VirtualMemoryEvent(offset, PageSize, isWrite);

            return true;
        }

        public abstract void Dispose();
    }
}