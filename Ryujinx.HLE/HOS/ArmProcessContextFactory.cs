using Ryujinx.Cpu;
using Ryujinx.HLE.HOS.Kernel.Process;

namespace Ryujinx.HLE.HOS
{
    class ArmProcessContextFactory : IProcessContextFactory
    {
        public IProcessContext Create(ulong addressSpaceSize, InvalidAccessHandler invalidAccessHandler)
        {
            return new ArmProcessContext<MemoryManagerHostMapped>(new MemoryManagerHostMapped(addressSpaceSize, invalidAccessHandler));
            // return new ArmProcessContext<MemoryManager>(new MemoryManager(addressSpaceSize, invalidAccessHandler));
        }
    }
}
