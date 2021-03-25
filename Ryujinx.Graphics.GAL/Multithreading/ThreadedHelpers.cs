using System.Threading;

namespace Ryujinx.Graphics.GAL.Multithreading
{
    static class ThreadedHelpers
    {
        public static void SpinUntilNonNull<T>(ref T obj) where T : class
        {
            while (obj == null)
            {
                Thread.SpinWait(5);
            }
        }
    }
}
