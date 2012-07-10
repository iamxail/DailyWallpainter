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
        private Installer installer;

        public MainApplicationContext()
        {
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            syncCntx = new WindowsFormsSynchronizationContext();

            installer = new Installer(this);
            if (installer.CheckNeedInstall())
            {
                installer.InstallAsync();

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

            SingleInstanceProgram.Instance.AnotherProcessLaunched += new EventHandler<EventArgs>(Instance_AnotherProcessLaunched);

            updater = new GitHubUpdater("iamxail", Program.SafeName, Program.ExeName);
#if DEBUG
            //do nothing
#else
            updater.CheckCompleted += new CheckCompletedEventHandler(updater_CheckCompleted);
            updater.UpdateCompleted += new UpdateCompletedEventHandler(updater_UpdateCompleted);
            updater.CheckAsync();
#endif

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
                SingleInstanceProgram.Instance.AnotherProcessLaunched -= Instance_AnotherProcessLaunched;
            }
            catch
            {
            }

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

        private void Instance_AnotherProcessLaunched(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private bool IsNeededToShowSettings()
        {
            bool StartedByWindows = Program.ArgumentExists("/winstart");

            return s.InitialStart
                || StartedByWindows == false;
        }

        private void updater_CheckCompleted(object sender, CheckCompletedEventArgs e)
        {
            bool updating = false;

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
                            try
                            {
                                updater.UpdateAsync(true);
                                updating = true;
                            }
                            catch
                            {
                            }
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
                if (IsfrmSettingAvailable == false
                    && updating == false)
                {
                    passTimer.Reset();
                }
            }
        }

        private void updater_UpdateCompleted(object sender, UpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                using (var work = new WorkingUI())
                {
                    work.MessageBoxShow("업데이트 설치 중에 문제가 발견되었습니다.\r\n\r\n" + e.Error.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public string LatestVersion { get { return updater.LatestVersion; } }

        public bool IsNeededToNotifyNewVersion
        {
            get
            {
#if DEBUG
                return false;
#else
                return updater.IsNewVersionAvailable
                    && updater.LatestVersion != s.LastestVersionInformed;
#endif
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
            if (InvokeRequired)
            {
                syncCntx.Post(state => callback.DynamicInvoke(state as object[]), args);
            }
            else
            {
                callback.DynamicInvoke(args);
            }
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
                Wallpaper wallpaper = null;
                bool isAppBgSaved = false;

                try
                {
                    using (var sourcesEnumerator = sources.GetEnumerator())
                    {
                        if (sourcesEnumerator.MoveNext()) // enumerator created or resetted, need MoveNext() before get Current
                        {
                            bool tryForceGrab = false;

                            while (true)
                            {
                                try
                                {
                                    var source = sourcesEnumerator.Current;
                                    var bmpDownload = source.GetBitmap(tryForceGrab);
                                    if (bmpDownload != null)
                                    {
                                        if (wallpaper == null)
                                        {
                                            wallpaper = new Wallpaper();
                                        }

                                        wallpaper.AddBitmap(bmpDownload);

                                        if (isAppBgSaved == false)
                                        {
                                            try
                                            {
                                                var appBg = bmpDownload.Bitmap.Crop(0, 0, 100, 165);
                                                appBg.SafeSave(Program.AppData, @"appbg.bmp", true);
                                            }
                                            catch
                                            {
                                            }

                                            isAppBgSaved = true;
                                        }
                                    }
                                }
                                catch
                                {
                                }

                                try
                                {
                                    if (wallpaper != null
                                        && wallpaper.CanAddAnotherBitmap == false)
                                    {
                                        break;
                                    }
                                    else if (sourcesEnumerator.MoveNext() == false)
                                    {
                                        if (tryForceGrab == false
                                            && MultiScreenInfo.Instance.IsChanged)
                                        {
                                            tryForceGrab = true;

                                            sourcesEnumerator.Reset();
                                            if (sourcesEnumerator.MoveNext() == false) //need MoveNext() after Reset()
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                catch
                                {
                                    break;
                                }
                            }

                            if (wallpaper != null)
                            {
                                wallpaper.SetToDesktop();
                            }
                        }
                    }
                }
                finally
                {
                    if (wallpaper != null)
                    {
                        wallpaper.Dispose();
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
