using Ryujinx.Common.Configuration;
using Ryujinx.Cpu;
using Ryujinx.HLE.HOS.Kernel;
using Ryujinx.HLE.HOS.Kernel.Process;
using System;

namespace Ryujinx.HLE.HOS
{
    class ArmProcessContextFactory : IProcessContextFactory
    {
        public IProcessContext Create(KernelContext context, ulong addressSpaceSize, InvalidAccessHandler invalidAccessHandler)
        {
            MemoryManagerMode mode = context.Device.Configuration.MemoryManagerMode;

            switch (mode)
            {
                case MemoryManagerMode.SoftwarePageTable:
                    return new ArmProcessContext<MemoryManager>(new MemoryManager(addressSpaceSize, invalidAccessHandler));

                case MemoryManagerMode.HostMapped:
                case MemoryManagerMode.HostMappedUnsafe:
                    ARMeilleure.Optimizations.UnsafeHostMappedMemory = mode == MemoryManagerMode.HostMappedUnsafe;
                    return new ArmProcessContext<MemoryManagerHostMapped>(new MemoryManagerHostMapped(addressSpaceSize, invalidAccessHandler));

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
