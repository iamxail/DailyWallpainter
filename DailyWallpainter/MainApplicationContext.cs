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

            bool StartByWindows = false;
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (StartByWindows == false
                    && arg.ToLower() == "/winstart")
                {
                    StartByWindows = true;
                }
            }

            stopTimer = new ManualResetEvent(false);
            passTimer = new ManualResetEvent(true);

            isNewVersionAvailable = false;
            LatestVersion = string.Empty;

            lastWorking = s.IsCheckOnlyWhenStartup;

            if (s.InitialStart
                || StartByWindows == false)
            {
                ShowSettings();
            }

            var updateChecker = new GitHubUpdateChecker("iamxail", "DailyWallpainter", "DailyWallpainter.exe");
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
            SingleInstanceProgram.Release();
            tray.Dispose();
            tmrDownload.Stop();
            stopTimer.Set();
        }

        private void updateChecker_CheckCompleted(object sender, CheckCompletedEventArgs e)
        {
            var updateChecker = e.UserState as GitHubUpdateChecker;

            LatestVersion = updateChecker.LatestVersion;
            isNewVersionAvailable = updateChecker.IsNewVersionAvailable;

            if (IsNeededToNotifyNewVersion())
            {
                if (IsfrmSettingAvailable())
                {
                    set.NotifyNewVersion();
                }
                else
                {
                    ShowSettings();
                }
            }
            else
            {
                passTimer.Reset();
            }
        }

        private bool isNewVersionAvailable;
        public string LatestVersion { get; private set; }

        public bool IsNeededToNotifyNewVersion()
        {
            return isNewVersionAvailable
                && LatestVersion != s.LastestVersionInformed;
        }

        private bool IsfrmSettingAvailable()
        {
            return set != null
                && set.IsDisposed == false;
        }

        private delegate void Action();
        public void ShowSettings()
        {
            passTimer.Set();

            if (IsfrmSettingAvailable())
            {
                if (set.InvokeRequired)
                {
                    set.Invoke(new Action(() =>
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
                if (Application.MessageLoop)
                {
                    set = new frmSettings();
                    set.FormClosed += new FormClosedEventHandler(frmSettings_FormClosed);

                    set.Show();
                    set.Activate();
                }
                else
                {
                    this.BeginInvoke(new Action(() =>
                        {
                            set = new frmSettings();
                            set.FormClosed += new FormClosedEventHandler(frmSettings_FormClosed);

                            set.Show();
                            set.Activate();
                        }));
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
            if (tmrDownload.Interval >= 60000 && tmrDownload.Interval != s.IntervalInMinute * 60000)
            {
                tmrDownload.Interval = s.IntervalInMinute * 60000;
            }

            passTimer.Reset();
        }

        private void tmrDownload_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Bitmap desktop = null;
            Bitmap appBg = null;

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
                    var data = source.GetBitmapBytes();
                    if (data != null)
                    {
                        data.SaveBitmap(s.SaveFolder, source.Name);

                        using (var ms = new MemoryStream(data))
                        using (var bitmap = new Bitmap(ms))
                        {
                            desktop = bitmap.ResizeToFitOutside(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                            appBg = bitmap.Crop(0, 0, 100, 165);
                        }

                        s.Sources.Save();

                        break;
                    }
                }

                if (desktop != null)
                {
                    string path = Path.Combine(s.SaveFolder, "Current Wallpaper.bmp");

                    desktop.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);

                    Wallpaper.Change(path);
                }

                if (appBg != null)
                {
                    string appBgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Daily Wallpainter");
                    if (Directory.Exists(appBgPath) == false)
                    {
                        Directory.CreateDirectory(appBgPath);
                    }
                    appBgPath = Path.Combine(appBgPath, @"appbg.bmp");

                    appBg.Save(appBgPath, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            catch (Exception)
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

