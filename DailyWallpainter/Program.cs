using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.Win32;
using System.Threading;
using DailyWallpainter.UI;
using DailyWallpainter.Helpers;
using DailyWallpainter.UpdateChecker;

namespace DailyWallpainter
{
    static class Program
    {
        private static System.Timers.Timer tmrDownload;
        private static ManualResetEvent stopTimer;
        private static ManualResetEvent passTimer;
        private static frmSettings set;
        private static Settings s = Settings.Instance;
        private static bool lastWorking;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (SingleInstanceProgram.IsSingleInstaced() == false)
            {
                return;
            }

            bool StartByWindows = false;
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (StartByWindows == false
                    && arg.ToLower() == "/winstart")
                {
                    StartByWindows = true;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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

            TrayIcon tray = new TrayIcon();

            Application.Run();

            SingleInstanceProgram.Release();
            tray.Dispose();
            tmrDownload.Stop();
            stopTimer.Set();
        }

        private static void updateChecker_CheckCompleted(object sender, CheckCompletedEventArgs e)
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

        private static bool isNewVersionAvailable;
        public static string LatestVersion { get; private set; }

        public static bool IsNeededToNotifyNewVersion()
        {
            return isNewVersionAvailable
                && LatestVersion != s.LastestVersionInformed;
        }

        private static bool IsfrmSettingAvailable()
        {
            return set != null
                && set.IsDisposed == false;
        }

        private delegate void Action();
        public static void ShowSettings()
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
                //todo
                /*
                if (Application.MessageLoop)
                {
                    set = new frmSettings();
                    set.FormClosed += new FormClosedEventHandler(frmSettings_FormClosed);

                    set.Show();
                    set.Activate();
                }
                else
                {
                    syncContx.Post(new SendOrPostCallback((o) =>
                        {
                            set = new frmSettings();
                            set.FormClosed += new FormClosedEventHandler(frmSettings_FormClosed);

                            set.Show();
                            set.Activate();
                        }), null);
                }*/
            }
        }

        private static void frmSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (tmrDownload.Interval >= 60000 && tmrDownload.Interval != s.IntervalInMinute * 60000)
            {
                tmrDownload.Interval = s.IntervalInMinute * 60000;
            }

            passTimer.Reset();
        }

        private static void tmrDownload_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
