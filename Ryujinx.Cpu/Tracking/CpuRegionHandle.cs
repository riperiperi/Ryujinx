using Ryujinx.Memory.Tracking;
using System;

namespace Ryujinx.Cpu.Tracking
{
    public class CpuRegionHandle : IRegionHandle
    {
        private readonly RegionHandle _impl;

        public bool Dirty => _impl.Dirty;

        public event Action OnDirty;

        public ulong Address => _impl.Address;
        public ulong Size => _impl.Size;
        public ulong EndAddress => _impl.EndAddress;

        internal CpuRegionHandle(RegionHandle impl)
        {
            _impl = impl;

            _impl.OnDirty += DirtyHandler;
        }

        private void DirtyHandler()
        {
            OnDirty?.Invoke();
        }

        public void Dispose() => _impl.Dispose();
        public void RegisterAction(RegionSignal action) => _impl.RegisterAction(action);
        public void Reprotect() => _impl.Reprotect();
    }
}
