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
        private static ManualResetEvent runningTimer;
        private static TrayIcon tray;
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
            runningTimer = new ManualResetEvent(false);
            tmrDownload = new System.Timers.Timer();
            tray = new TrayIcon();

            // 
            // tmrDownload
            // 
            tmrDownload.Interval = 5000;
            tmrDownload.Elapsed += new System.Timers.ElapsedEventHandler(tmrDownload_Elapsed);

            if (s.InitialStart)
            {
                s.RunOnStartup = true;

                s.Sources.Add(new Source("National Geographic - Photo of the Day",
                    @"http://photography.nationalgeographic.com/photography/photo-of-the-day/",
                    "class=\"primary_photo\"(?>\\r\\n|[\\r\\n]|.)*?<div class=\"download_link\"><a href=\"(.*?)\"|title=\"Go to the previous Photo of the Day\">(?>\\r\\n|[\\r\\n]|.)*?<img src=\"(.*?)\"",
                    "$1$2"));

                s.Sources.Add(new Source("National Geographic - Photo of the Day (High Quality Only, Not Daily)",
                    @"http://photography.nationalgeographic.com/photography/photo-of-the-day/",
                    "<div class=\"download_link\"><a href=\"(.*?)\"",
                    "$1",
                    false, ""));

                s.Sources.Add(new Source("NASA - Astronomy Picture of the Day",
                    @"http://apod.nasa.gov/apod/",
                    "<a href=\"image/(.*?)\">",
                    "http://apod.nasa.gov/apod/image/$1"));

                ShowSettings();
            }
            else
            {
                tmrDownload.Start();
            }

            Application.Run();

            tray.Dispose();
            tmrDownload.Stop();
            stopTimer.Set();
        }

        public static void ShowSettings()
        {
            tmrDownload.Stop();

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

            if (runningTimer.WaitOne(0) == false)
            {
                tmrDownload.Start();
            }
        }

        private static void SaveBitmap(string prefix, byte[] data)
        {
            string safeBitmapFilename = prefix + " at " + string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                safeBitmapFilename.Replace(c, '_');
            }

            string ext = ".unknown";
            if (data[0] == 0xFF && data[1] == 0xD8)
            {
                ext = ".jpg";
            }
            else if (data[0] == 137 && data[1] == 80 && data[2] == 78 && data[3] == 71)
            {
                ext = ".png";
            }
            else
            {
                string header = Encoding.ASCII.GetString(data, 0, 3);
                if (header == "GIF")
                {
                    ext = ".gif";
                }
                else if (header.Substring(0, 2) == "BM")
                {
                    ext = ".bmp";
                }
            }

            string path = Path.Combine(s.SaveFolder, safeBitmapFilename);
            string suffix = "";
            int i = 1;
            while (File.Exists(path + suffix + ext))
            {
                i++;
                suffix = " (" + i.ToString() + ")";
            }

            File.WriteAllBytes(path + suffix + ext, data);
        }

        private static void tmrDownload_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Bitmap desktop = null;
            Bitmap appBg = null;

            if (runningTimer.WaitOne(0))
            {
                return;
            }

            try
            {
                tmrDownload.Stop();
                runningTimer.Set();
                if (tmrDownload.Interval != s.IntervalInMinute * 60000)
                {
                    tmrDownload.Interval = s.IntervalInMinute * 60000;
                }

                var sources = s.Sources.GetEnabledSources();

                foreach (var source in sources)
                {
                    var data = source.GetBitmap();
                    if (data != null)
                    {
                        if (Directory.Exists(s.SaveFolder) == false)
                        {
                            Directory.CreateDirectory(s.SaveFolder);
                        }

                        SaveBitmap(source.Name, data);

                        desktop = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
                        appBg = new Bitmap(100, 165);

                        using (var g = Graphics.FromImage(desktop))
                        using (var ga = Graphics.FromImage(appBg))
                        using (var ms = new MemoryStream(data))
                        using (var b = new Bitmap(ms))
                        {
                            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                            Rectangle rect;
                            if ((float)desktop.Width / (float)desktop.Height > (float)b.Width / (float)b.Height)
                            {
                                int newHeight = (int)((float)b.Height / (float)b.Width * (float)desktop.Width);

                                rect = new Rectangle(0, (desktop.Height - newHeight) / 2, desktop.Width, newHeight);
                            }
                            else
                            {
                                int newWidth = (int)((float)b.Width / (float)b.Height * (float)desktop.Height);

                                rect = new Rectangle((desktop.Width - newWidth) / 2, 0, newWidth, desktop.Height);
                            }

                            g.DrawImage(b, rect);
                            ga.DrawImageUnscaledAndClipped(b, new Rectangle(0, 0, 100, 165));
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
                runningTimer.Reset();

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
