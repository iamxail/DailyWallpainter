using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DailyWallpainter.UI;
using DailyWallpainter.Helpers;
using DailyWallpainter.UpdateChecker;
using System.Drawing;
using System.IO;

namespace DailyWallpainter
{
    public class MainApplicationContext : ApplicationContext
    {
        private System.Timers.Timer tmrDownload;
        private ManualResetEvent stopTimer;
        private ManualResetEvent passTimer;
        private frmSettings set;
        private Settings s = Settings.Instance;
        private bool lastWorking;
        private readonly SynchronizationContext syncCntx;
        private TrayIcon tray;

        public MainApplicationContext()
        {
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            syncCntx = new WindowsFormsSynchronizationContext();

            stopTimer = new ManualResetEvent(false);
            passTimer = new ManualResetEvent(true);

            isNewVersionAvailable = false;
            LatestVersion = string.Empty;

            lastWorking = s.IsCheckOnlyWhenStartup;

            if (IsNeededToShowSettings())
            {
                ShowSettings();
            }

            var updateChecker = new GitHubUpdateChecker("iamxail", Program.SafeName, Program.ExeName);
            updateChecker.CheckCompleted += new GitHubUpdateChecker.CheckCompletedEventHandler(updateChecker_CheckCompleted);
            updateChecker.CheckAsync(updateChecker);

            tmrDownload = new System.Timers.Timer();
            tmrDownload.Interval = 5000;
            tmrDownload.Elapsed += new System.Timers.ElapsedEventHandler(tmrDownload_Elapsed);
            tmrDownload.Start();

            tray = new TrayIcon();
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            //http://stackoverflow.com/questions/1067844/issue-with-notifyicon-not-dissappearing-on-winforms-app
            //this event can be raised several times. - it can cause NullReferenceException

            try
            {
                tray.Dispose();
            }
            catch
            {
            }

            try
            {
                tmrDownload.Stop();
            }
            catch
            {
            }

            try
            {
                stopTimer.Set();
            }
            catch
            {
            }
        }

        private bool IsNeededToShowSettings()
        {
            bool StartByWindows = false;
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (StartByWindows == false
                    && arg.ToLower() == "/winstart")
                {
                    StartByWindows = true;
                }
            }

            return s.InitialStart
                || StartByWindows == false;
        }

        private void updateChecker_CheckCompleted(object sender, CheckCompletedEventArgs e)
        {
            var updateChecker = e.UserState as GitHubUpdateChecker;

            LatestVersion = updateChecker.LatestVersion;
            isNewVersionAvailable = updateChecker.IsNewVersionAvailable;

            if (IsNeededToNotifyNewVersion)
            {
                if (IsfrmSettingAvailable)
                {
                    set.NotifyNewVersion();
                }
                else
                {
                    ShowSettings();
                }
            }
            else if (IsfrmSettingAvailable == false)
            {
                passTimer.Reset();
            }
        }

        private bool isNewVersionAvailable;
        public string LatestVersion { get; private set; }

        public bool IsNeededToNotifyNewVersion
        {
            get
            {
                return isNewVersionAvailable
                    && LatestVersion != s.LastestVersionInformed;
            }
        }

        private bool IsfrmSettingAvailable
        {
            get
            {
                return set != null
                    && set.IsDisposed == false;
            }
        }

        public void ShowSettings()
        {
            passTimer.Set();

            if (IsfrmSettingAvailable)
            {
                if (set.InvokeRequired)
                {
                    set.Invoke(new MethodInvoker(() =>
                    {
                        set.Show();
                        set.Activate();
                    }));
                }
                else
                {
                    set.Show();
                    set.Activate();
                }
            }
            else
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(() =>
                    {
                        set = new frmSettings();
                        set.FormClosed += new FormClosedEventHandler(frmSettings_FormClosed);

                        set.Show();
                        set.Activate();
                    }));
                }
                else
                {
                    set = new frmSettings();
                    set.FormClosed += new FormClosedEventHandler(frmSettings_FormClosed);

                    set.Show();
                    set.Activate();
                }
            }
        }

        private bool InvokeRequired
        {
            get
            {
                return !Application.MessageLoop;
            }
        }

        //from http://nosuchblogger.com/post/60/applicationcontext-and-the-ui-thread
        private void BeginInvoke(Delegate callback, object[] args)
        {
            syncCntx.Post(state => callback.DynamicInvoke(state as object[]), args);
        }

        private void BeginInvoke(Delegate callback)
        {
            BeginInvoke(callback, null);
        }

        private void frmSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (tmrDownload.Interval >= 60000
                && tmrDownload.Interval != s.IntervalInMinute * 60000)
            {
                tmrDownload.Interval = s.IntervalInMinute * 60000;
            }

            passTimer.Reset();
        }

        private void tmrDownload_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (passTimer.WaitOne(0))
            {
                return;
            }

            Bitmap desktop = null;
            Bitmap appBg = null;

            try
            {
                tmrDownload.Stop();
                if (tmrDownload.Interval != s.IntervalInMinute * 60000)
                {
                    tmrDownload.Interval = s.IntervalInMinute * 60000;
                }

                var sources = s.Sources.GetEnabledSources();
                var allScreen = new MultiScreenInfo();

                foreach (var source in sources)
                {
                    using (var bmpDownload = source.GetBitmap())
                    {
                        s.Sources.Save();

                        if (bmpDownload != null)
                        {
                            bmpDownload.Save();

                            if (bmpDownload.CheckResolutionLowerLimit())
                            {
                                desktop = new Bitmap(allScreen.VirtualDesktop.Width, allScreen.VirtualDesktop.Height);

                                using (var gDesktop = Graphics.FromImage(desktop))
                                {
                                    gDesktop.SetHighQuality();

                                    if (bmpDownload.CheckStretchForMultiScreen())
                                    {
                                        gDesktop.DrawImageFitOutside(bmpDownload.Bitmap, allScreen.AdjustedVirtualDesktop);
                                    }
                                    else
                                    {
                                        foreach (var scr in allScreen.AllScreens)
                                        {
                                            gDesktop.DrawImageFitOutside(bmpDownload.Bitmap, scr.AdjustedBounds);
                                        }
                                    }
                                }

                                appBg = bmpDownload.Bitmap.Crop(0, 0, 100, 165);

                                break;
                            }
                        }
                    }
                }

                if (desktop != null && appBg != null)
                {
                    string path = Path.Combine(s.SaveFolder, "Current Wallpaper.bmp");
                    desktop.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
                    Wallpaper.Change(path);

                    string appBgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.Name);
                    if (Directory.Exists(appBgPath) == false)
                    {
                        Directory.CreateDirectory(appBgPath);
                    }
                    appBgPath = Path.Combine(appBgPath, @"appbg.bmp");
                    appBg.Save(appBgPath, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            catch
            {
            }
            finally
            {
                if (desktop != null)
                {
                    desktop.Dispose();
                }

                if (appBg != null)
                {
                    appBg.Dispose();
                }

                if (lastWorking)
                {
                    Application.Exit();
                }
                else if (stopTimer.WaitOne(0) == false)
                {
                    tmrDownload.Start();
                }
            }
        }
    }
}
