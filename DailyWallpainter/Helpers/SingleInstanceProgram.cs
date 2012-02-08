using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DailyWallpainter.Helpers
{
    public static class SingleInstanceProgram
    {
        private static Mutex mutex = new Mutex(true, Application.ProductName + "MutexForSingleInstance");

        public static bool IsSingleInstaced()
        {
            return mutex.WaitOne(TimeSpan.Zero, true);
        }

        public static void Release()
        {
            mutex.ReleaseMutex();
        }
    }
}
