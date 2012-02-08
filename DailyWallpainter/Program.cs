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

namespace DailyWallpainter
{
    static class Program
    {
        private static System.Timers.Timer tmrDownload;
        private static ManualResetEvent stopTimer;
        private static ManualResetEvent passTimer;
        private static frmSettings set;
        private static Settings s = Settings.Instance;

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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            stopTimer = new ManualResetEvent(false);
            passTimer = new ManualResetEvent(false);

            if (s.InitialStart)
            {
                ShowSettings();
            }

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

        public static void ShowSettings()
        {
            passTimer.Set();

            if (set == null || set.IsDisposed)
            {
                set = new frmSettings();
                set.FormClosed += new FormClosedEventHandler(frmSettings_FormClosed);
            }

            set.Show();
            set.Activate();
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
                if (stopTimer.WaitOne(0) == false)
                {
                    tmrDownload.Start();
                }

                if (desktop != null)
                {
                    desktop.Dispose();
                }

                if (appBg != null)
                {
                    appBg.Dispose();
                }
            }
        }
    }
}
