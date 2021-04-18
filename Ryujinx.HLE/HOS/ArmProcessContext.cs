using ARMeilleure.Memory;
using ARMeilleure.State;
using Ryujinx.Cpu;
using Ryujinx.HLE.HOS.Kernel.Process;
using Ryujinx.Memory;
using System;

namespace Ryujinx.HLE.HOS
{
    class ArmProcessContext<T> : IProcessContext where T : IVirtualMemoryManager, IMemoryManager
    {
        private readonly T _memoryManager;
        private readonly CpuContext _cpuContext;

        public IVirtualMemoryManager AddressSpace => _memoryManager;

        public ArmProcessContext(T memoryManager)
        {
            _memoryManager = memoryManager;
            _cpuContext = new CpuContext(memoryManager);
        }

        public void Execute(ExecutionContext context, ulong codeAddress)
        {
            _cpuContext.Execute(context, codeAddress);
        }

        public void Dispose()
        {
            if (_memoryManager is IDisposable disposableMm)
            {
                disposableMm.Dispose();
            }
        }
    }
}
