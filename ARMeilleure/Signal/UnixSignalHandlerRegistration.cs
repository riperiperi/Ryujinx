using Mono.Unix.Native;
using System;
using System.Runtime.InteropServices;

namespace ARMeilleure.Signal
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

    static class UnixSignalHandlerRegistration
    {
        private const int SA_SIGINFO = 0x00000004;

        [DllImport("libc", SetLastError = true)]
        private static extern IntPtr sigaction(int signum, ref SigAction sigAction, out SigAction oldAction);

        [DllImport("libc", SetLastError = true)]
        private static extern IntPtr sigemptyset(ref SigSet set);

        public static SigAction RegisterExceptionHandler(IntPtr action)
        {
            SigAction sig = new SigAction
            {
                sa_handler = action,
                sa_flags = SA_SIGINFO
            };

            sigemptyset(ref sig.sa_mask);

            sigaction((int)Signum.SIGSEGV, ref sig, out SigAction old);

            return old;
        }
    }
}
