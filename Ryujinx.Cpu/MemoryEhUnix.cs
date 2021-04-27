using Mono.Unix.Native;
using Ryujinx.Memory;
using Ryujinx.Memory.Tracking;
using System;
using System.Runtime.InteropServices;

namespace Ryujinx.Cpu
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct SigSet
    {
        fixed long sa_mask[16];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct SigAction
    {
        public IntPtr sa_handler;
        public SigSet sa_mask;
        public int sa_flags;
        public IntPtr sa_restorer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SigInfo
    {
        public int si_signo;
        public int si_errno;
        public int si_code;
        public int pad;
        public nuint si_addr;
    }

    class MemoryEhUnix : MemoryEhBase
    {
        private const int SA_SIGINFO = 0x00000004;

        private unsafe delegate void UnixExceptionHandler(int sig, SigInfo* info, void* ucontext);

        [DllImport("libc", SetLastError = true)]
        private static extern IntPtr sigaction(int signum, ref SigAction sigAction, out SigAction oldAction);

        [DllImport("libc", SetLastError = true)]
        private static extern IntPtr sigemptyset(ref SigSet set);

        private SigAction _old;

        private static SigAction RegisterExceptionHandler(UnixExceptionHandler action)
        {
            SigAction sig = new SigAction
            {
                sa_handler = Marshal.GetFunctionPointerForDelegate(action),
                sa_flags = SA_SIGINFO
            };

            sigemptyset(ref sig.sa_mask);

            sigaction((int)Signum.SIGSEGV, ref sig, out SigAction old);

            return old;
        }

        private UnixExceptionHandler _exceptionHandler;

        public unsafe MemoryEhUnix(MemoryBlock addressSpace, MemoryTracking tracking) : base(addressSpace, tracking)
        {
            _exceptionHandler = ExceptionHandler;

            _old = RegisterExceptionHandler(_exceptionHandler);
        }

        private unsafe void ExceptionHandler(int sig, SigInfo* info, void* ucontext)
        {
            nuint address = info->si_addr;
            long error = info->si_code;

            if (!HandleInRange(address, (error & 2) == 2))
            {
                return; // TODO: call old handler
            }
        }

        public override void Dispose()
        {
            sigaction((int)Signum.SIGSEGV, ref _old, out SigAction old);
        }
    }
}