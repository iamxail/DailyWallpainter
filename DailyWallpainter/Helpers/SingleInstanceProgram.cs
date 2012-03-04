//http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c/229567

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DailyWallpainter.Helpers
{
    public static class SingleInstanceProgram
    {
        private static Mutex mutex = new Mutex(false, Application.ProductName + "MutexForSingleInstance");

        public static bool IsSingleInstaced()
        {
            bool hasHandle;

            try
            {
                hasHandle = mutex.WaitOne(TimeSpan.Zero, false);
            }
            catch (AbandonedMutexException)
            {
                hasHandle = true;
            }

            return hasHandle;
        }

        public static void Release()
        {
            try
            {
                mutex.ReleaseMutex();
            }
            catch
            {
            }
        }
    }
}
