using System;
using System.Runtime.InteropServices;

namespace Ryujinx.Memory
{
    static class MemoryManagementWindows
    {
        private static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

        [Flags]
        private enum AllocationType : uint
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        private enum MemoryProtection : uint
        {
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [Flags]
        private enum FileMapProtection : uint
        {
            PageReadonly = 0x02,
            PageReadWrite = 0x04,
            PageWriteCopy = 0x08,
            PageExecuteRead = 0x20,
            PageExecuteReadWrite = 0x40,
            SectionCommit = 0x8000000,
            SectionImage = 0x1000000,
            SectionNoCache = 0x10000000,
            SectionReserve = 0x4000000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAlloc(
            IntPtr lpAddress,
            IntPtr dwSize,
            AllocationType flAllocationType,
            MemoryProtection flProtect);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(
            IntPtr lpAddress,
            IntPtr dwSize,
            MemoryProtection flNewProtect,
            out MemoryProtection lpflOldProtect);

        [DllImport("kernel32.dll")]
        private static extern bool VirtualFree(IntPtr lpAddress, IntPtr dwSize, AllocationType dwFreeType);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CreateFileMapping(
            IntPtr hFile,
            IntPtr lpFileMappingAttributes,
            FileMapProtection flProtect,
            uint dwMaximumSizeHigh,
            uint dwMaximumSizeLow,
            [MarshalAs(UnmanagedType.LPWStr)] string lpName);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern IntPtr MapViewOfFile(
            IntPtr hFileMappingObject,
            uint dwDesiredAccess,
            uint dwFileOffsetHigh,
            uint dwFileOffsetLow,
            IntPtr dwNumberOfBytesToMap);

        [DllImport("kernel32.dll")]
        private static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        public static IntPtr Allocate(IntPtr size)
        {
            return AllocateInternal(size, AllocationType.Reserve | AllocationType.Commit);
        }

        public static IntPtr Reserve(IntPtr size)
        {
            return AllocateInternal(size, AllocationType.Reserve);
        }

        private static IntPtr AllocateInternal(IntPtr size, AllocationType flags = 0)
        {
            IntPtr ptr = VirtualAlloc(IntPtr.Zero, size, flags, MemoryProtection.ReadWrite);

            if (ptr == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }

            return ptr;
        }

        public static bool Commit(IntPtr location, IntPtr size)
        {
            return VirtualAlloc(location, size, AllocationType.Commit, MemoryProtection.ReadWrite) != IntPtr.Zero;
        }

        public static bool Decommit(IntPtr location, IntPtr size)
        {
            return VirtualFree(location, size, AllocationType.Decommit);
        }

        public static bool Reprotect(IntPtr address, IntPtr size, MemoryPermission permission)
        {
            return VirtualProtect(address, size, GetProtection(permission), out _);
        }

        private static MemoryProtection GetProtection(MemoryPermission permission)
        {
            return permission switch
            {
                MemoryPermission.None => MemoryProtection.NoAccess,
                MemoryPermission.Read => MemoryProtection.ReadOnly,
                MemoryPermission.ReadAndWrite => MemoryProtection.ReadWrite,
                MemoryPermission.ReadAndExecute => MemoryProtection.ExecuteRead,
                MemoryPermission.ReadWriteExecute => MemoryProtection.ExecuteReadWrite,
                MemoryPermission.Execute => MemoryProtection.Execute,
                _ => throw new MemoryProtectionException(permission)
            };
        }

        public static bool Free(IntPtr address)
        {
            return VirtualFree(address, IntPtr.Zero, AllocationType.Release);
        }

        public static IntPtr CreateSharedMemory(IntPtr size, bool reserve)
        {
            var prot = reserve ? FileMapProtection.SectionReserve : FileMapProtection.SectionCommit;

            IntPtr handle = CreateFileMapping(
                InvalidHandleValue,
                IntPtr.Zero,
                FileMapProtection.PageReadWrite | prot,
                (uint)(size.ToInt64() >> 32),
                (uint)size.ToInt64(),
                null);

            if (handle == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }

            return handle;
        }

        public static void DestroySharedMemory(IntPtr handle)
        {
            if (!CloseHandle(handle))
            {
                throw new ArgumentException("Invalid handle.", nameof(handle));
            }
        }

        public static IntPtr MapSharedMemory(IntPtr handle)
        {
            IntPtr ptr = MapViewOfFile(handle, 4 | 2, 0, 0, IntPtr.Zero);

            if (ptr == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }

            return ptr;
        }

        public static void UnmapSharedMemory(IntPtr address)
        {
            if (!UnmapViewOfFile(address))
            {
                throw new ArgumentException("Invalid address.", nameof(address));
            }
        }
    }
}