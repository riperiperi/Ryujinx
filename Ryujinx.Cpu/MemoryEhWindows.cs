using Ryujinx.Memory;
using Ryujinx.Memory.Tracking;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Ryujinx.Cpu
{
    unsafe class MemoryEhWindows : MemoryEhBase
    {
        private const int PageSize = 0x1000;
        private const int PageMask = PageSize - 1;

        private const int EXCEPTION_CONTINUE_SEARCH = 0;
        private const int EXCEPTION_CONTINUE_EXECUTION = -1;

        private const uint EXCEPTION_ACCESS_VIOLATION = 0xc0000005;

        private unsafe struct ExceptionRecord
        {
            public uint ExceptionCode;
            public uint ExceptionFlags;
            public ExceptionRecord* ExceptionRecord_;
            public nuint ExceptionAddress;
            public uint NumberParameters;
            public nuint ExceptionInformation0;
            public nuint ExceptionInformation1;
        }

        private unsafe struct ExceptionPointers
        {
            public ExceptionRecord* ExceptionRecord;
            public nuint ContextRecord; // This struct is platform dependent, we don't need it.
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private unsafe delegate int VectoredExceptionHandler(ExceptionPointers* exceptionInfo);

        [DllImport("kernel32.dll")]
        private static extern IntPtr AddVectoredExceptionHandler(uint first, VectoredExceptionHandler handler);

        [DllImport("kernel32.dll")]
        private static extern ulong RemoveVectoredExceptionHandler(IntPtr handle);

        private readonly MemoryBlock _addressSpace;
        private readonly MemoryTracking _tracking;
        private readonly VectoredExceptionHandler _exceptionHandler;
        private IntPtr _handle;

        public MemoryEhWindows(MemoryBlock addressSpace, MemoryTracking tracking) : base(addressSpace, tracking)
        {
            _exceptionHandler = ExceptionHandler;

            _handle = AddVectoredExceptionHandler(1, _exceptionHandler);
        }

        private int ExceptionHandler(ExceptionPointers* exceptionInfo)
        {
            if (exceptionInfo->ExceptionRecord->ExceptionCode != EXCEPTION_ACCESS_VIOLATION)
            {
                return EXCEPTION_CONTINUE_SEARCH;
            }

            nuint faultAddress = exceptionInfo->ExceptionRecord->ExceptionInformation1;

            bool isWrite = exceptionInfo->ExceptionRecord->ExceptionInformation0 != 0;

            if (!HandleInRange(faultAddress, isWrite))
            {
                return EXCEPTION_CONTINUE_SEARCH;
            }

            return EXCEPTION_CONTINUE_EXECUTION;
        }

        public override void Dispose()
        {
            IntPtr handle = Interlocked.Exchange(ref _handle, IntPtr.Zero);

            if (handle != IntPtr.Zero)
            {
                RemoveVectoredExceptionHandler(handle);
            }
        }
    }
}