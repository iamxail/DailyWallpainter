//http://channel9.msdn.com/coding4fun/articles/Setting-Wallpaper

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using DailyWallpainter.Helpers;
using System.Drawing;
using System.IO;

namespace DailyWallpainter
{
    public class Wallpaper : IDisposable
    {
        protected Bitmap desktop;
        protected Settings s;

        public Wallpaper(SourceBitmap origin)
        {
            s = Settings.Instance;
            var allScreen = new MultiScreenInfo();
            desktop = null;

            try
            {
                if (origin.CheckResolutionLowerLimit())
                {
                    desktop = new Bitmap(allScreen.VirtualDesktop.Width, allScreen.VirtualDesktop.Height);
                    using (var gDesktop = Graphics.FromImage(desktop))
                    {
                        gDesktop.SetHighQuality();

                        if (origin.CheckStretchForMultiScreen())
                        {
                            gDesktop.DrawImageFitOutside(origin.Bitmap, allScreen.AdjustedVirtualDesktop);
                        }
                        else
                        {
                            foreach (var scr in allScreen.AllScreens)
                            {
                                gDesktop.DrawImageFitOutside(origin.Bitmap, scr.AdjustedBounds);
                            }
                        }
                    }
                }
            }
            catch
            {
                if (desktop != null)
                {
                    desktop.Dispose();
                    desktop = null;
                }
            }

            if (desktop == null)
            {
                throw new ArgumentException();
            }
        }


        /*~Wallpaper() // no unmanaged resources
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
                if (desktop != null)
                {
                    var tdesktop = desktop;
                    desktop = null;
                    tdesktop.Dispose();
                }

                s = null;
            }
        }

        public void SetToDesktop()
        {
            SetToDesktop(Path.Combine(s.SaveFolder, "Current Wallpaper.bmp"));
        }

        public void SetToDesktop(string path)
        {
            desktop.Save(path, System.Drawing.Imaging.ImageFormat.Bmp);
            Wallpaper.Change(path);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        private static void Change(string path)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            //tiled (for multiscreen)
            key.SetValue(@"WallpaperStyle", "1");
            key.SetValue(@"TileWallpaper", "1");

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
