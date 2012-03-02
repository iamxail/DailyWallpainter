using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DailyWallpainter.UI;
using DailyWallpainter.Helpers;
using DailyWallpainter.Updater;
using System.Drawing;
using System.IO;
using System.Diagnostics;

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
        private IUpdater updater;

        public MainApplicationContext()
        {
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            syncCntx = new WindowsFormsSynchronizationContext();

            var installer = new Installer(this);
            if (installer.CheckNeedInstall())
            {
                installer.Install();

                return;
            }
            installer = null;

            stopTimer = new ManualResetEvent(false);
            passTimer = new ManualResetEvent(true);

            lastWorking = s.IsCheckOnlyWhenStartup;

            if (Program.ArgumentExists("/setstartup")
                || s.InitialStart)
            {
                s.RunOnStartup = true;
            }

            if (IsNeededToShowSettings())
            {
                ShowSettings();
            }

            updater = new GitHubUpdater("iamxail", Program.SafeName, Program.ExeName);
            updater.CheckCompleted += new CheckCompletedEventHandler(updateChecker_CheckCompleted);
            updater.CheckAsync();

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
            bool StartedByWindows = Program.ArgumentExists("/winstart");

            return s.InitialStart
                || StartedByWindows == false;
        }

        private void updateChecker_CheckCompleted(object sender, CheckCompletedEventArgs e)
        {
            try
            {
                if (IsNeededToNotifyNewVersion)
                {
                    if (IsfrmSettingAvailable)
                    {
                        set.NotifyNewVersion(updater);
                    }
                    else
                    {
                        if (s.IsSilentUpdate)
                        {
                            updater.Update(true);
                        }
                        else
                        {
                            ShowSettings();
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                if (IsfrmSettingAvailable == false)
                {
                    passTimer.Reset();
                }
            }
        }

        public string LatestVersion { get { return updater.LatestVersion; } }

        public bool IsNeededToNotifyNewVersion
        {
            get
            {
                return updater.IsNewVersionAvailable
                    && updater.LatestVersion != s.LastestVersionInformed;
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
                        set.Shown += new EventHandler(frmSettings_Shown);
                        set.FormClosed += new FormClosedEventHandler(frmSettings_FormClosed);

                        set.Show();
                        set.Activate();
                    }));
                }
                else
                {
                    set = new frmSettings();
                    set.Shown += new EventHandler(frmSettings_Shown);
                    set.FormClosed += new FormClosedEventHandler(frmSettings_FormClosed);

                    set.Show();
                    set.Activate();
                }
            }
        }

        public bool InvokeRequired
        {
            get
            {
                return !Application.MessageLoop;
            }
        }

        //from http://nosuchblogger.com/post/60/applicationcontext-and-the-ui-thread
        public void BeginInvoke(Delegate callback, object[] args)
        {
            syncCntx.Post(state => callback.DynamicInvoke(state as object[]), args);
        }

        public void BeginInvoke(Delegate callback)
        {
            BeginInvoke(callback, null);
        }

        private void frmSettings_Shown(object sender, EventArgs e)
        {
            if (IsNeededToNotifyNewVersion)
            {
                set.NotifyNewVersion(updater);
            }
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

        public TrayIcon TrayIcon
        {
            get
            {
                return tray;
            }
        }

        private void tmrDownload_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (passTimer.WaitOne(0))
            {
                return;
            }

            try
            {
                tmrDownload.Stop();
                if (tmrDownload.Interval != s.IntervalInMinute * 60000)
                {
                    tmrDownload.Interval = s.IntervalInMinute * 60000;
                }

                var sources = s.Sources.GetEnabledSources();

                foreach (var source in sources)
                {
                    try
                    {
                        using (var bmpDownload = source.GetBitmap())
                        {
                            if (bmpDownload != null)
                            {
                                bmpDownload.Save();

                                using (var wallpaper = new Wallpaper(bmpDownload))
                                {
                                    wallpaper.SetToDesktop();
                                }

                                var appBg = bmpDownload.Bitmap.Crop(0, 0, 100, 165);
                                appBg.SafeSave(Program.AppData, @"appbg.bmp");

                                break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
            finally
            {
                s.Sources.Save();

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
