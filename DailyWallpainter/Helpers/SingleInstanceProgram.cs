//http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c/229567

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DailyWallpainter.Helpers
{
    public class SingleInstanceProgram : IDisposable
    {
        static readonly SingleInstanceProgram instance = new SingleInstanceProgram();

        static SingleInstanceProgram()
        {
            //do nothing
        }

        public static SingleInstanceProgram Instance
        {
            get
            {
                return instance;
            }
        }

        private Mutex mutex;
        private EventWaitHandle detector;
        private ManualResetEvent released;

        public bool IsSingleInstanced { get; protected set; }

        public event EventHandler<EventArgs> AnotherProcessLaunched = delegate { };

        private SingleInstanceProgram()
        {
            mutex = new Mutex(false, Application.ProductName + "MutexForSingleInstance");
            try
            {
                IsSingleInstanced = mutex.WaitOne(TimeSpan.Zero, false);
            }
            catch (AbandonedMutexException)
            {
                IsSingleInstanced = true;
            }

            if (IsSingleInstanced)
            {
                detector = new EventWaitHandle(false, EventResetMode.ManualReset, Application.ProductName + "EventToDetectAnotherProcessLaunched");

                released = new ManualResetEvent(false);

                Thread waiter = new Thread(new ThreadStart(WaitingAnotherProcessLaunch));
                waiter.Start();
            }
            else
            {
                detector = EventWaitHandle.OpenExisting(Application.ProductName + "EventToDetectAnotherProcessLaunched");
                detector.Set();
            }
        }

        /*~SingleInstanceProgram()
        {
            Dispose(false);
        }*/

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Release();
            }
        }

        public void Release()
        {
            try
            {
                if (mutex != null)
                {
                    var tMutex = mutex;
                    mutex = null;

                    if (IsSingleInstanced)
                    {
                        tMutex.ReleaseMutex();
                    }
                }
            }
            catch
            {
            }

            try
            {
                if (released != null)
                {
                    released.Set();
                }
            }
            catch
            {
            }

            try
            {
                AnotherProcessLaunched = delegate { };
            }
            catch
            {
            }
        }

        private void WaitingAnotherProcessLaunch()
        {
            while (true)
            {
                switch (WaitHandle.WaitAny(new WaitHandle[] { released, detector }, Timeout.Infinite))
                {
                    case 0:
                        return;

                    case 1:
                        detector.Reset();
                        AnotherProcessLaunched(this, new EventArgs());
                        break;
                }
            }
        }
    }
}
