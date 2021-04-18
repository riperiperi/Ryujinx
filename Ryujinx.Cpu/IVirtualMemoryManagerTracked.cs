using Ryujinx.Cpu.Tracking;
using Ryujinx.Memory;
using System;

namespace Ryujinx.Cpu
{
    public interface IVirtualMemoryManagerTracked : IVirtualMemoryManager
    {
        void WriteUntracked(ulong va, ReadOnlySpan<byte> data);

        CpuRegionHandle BeginTracking(ulong address, ulong size);
        CpuMultiRegionHandle BeginGranularTracking(ulong address, ulong size, ulong granularity);
        CpuSmartMultiRegionHandle BeginSmartGranularTracking(ulong address, ulong size, ulong granularity);
    }
}
