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
        public Bitmap Bitmap { get { return desktop; } }
        public bool CanAddAnotherBitmap { get; protected set; }

        protected Bitmap desktop;
        protected Settings s;
        protected MultiScreenInfo allScreen;
        protected Graphics gDesktop;
        protected readonly bool needFullRedraw;
        protected List<SourceBitmap> bitmaps = new List<SourceBitmap>();

        public Wallpaper()
        {
            s = Settings.Instance;
            allScreen = MultiScreenInfo.Instance;
            CanAddAnotherBitmap = true;

            try
            {
                try
                {
                    if (allScreen.IsChanged == false)
                    {
                        var currentPath = GetCurrentPath();
                        var expectedPath = SafeFilename.Convert(s.SaveFolder, "Current Wallpaper.bmp", true);

                        if (currentPath == expectedPath)
                        {
                            desktop = new Bitmap(currentPath);

                            //not needed?
                            if (desktop.Size != allScreen.VirtualDesktop.Size)
                            {
                                desktop.Dispose();
                                desktop = null;
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
                finally
                {
                    if (desktop == null)
                    {
                        desktop = new Bitmap(allScreen.VirtualDesktop.Width, allScreen.VirtualDesktop.Height);

                        needFullRedraw = true;
                    }
                    else
                    {
                        needFullRedraw = false;
                    }
                }

                gDesktop = Graphics.FromImage(desktop);
                gDesktop.SetHighQuality();
            }
            catch
            {
                if (gDesktop != null)
                {
                    gDesktop.Dispose();
                    gDesktop = null;
                }

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
                if (bitmaps != null)
                {
                    var tBitmaps = bitmaps;
                    bitmaps = null;
                    tBitmaps.Clear();
                }

                if (gDesktop != null)
                {
                    var tGDesktop = gDesktop;
                    gDesktop = null;
                    tGDesktop.Dispose();
                }

                if (desktop != null)
                {
                    var tdesktop = desktop;
                    desktop = null;
                    tdesktop.Dispose();
                }

                s = null;
            }
        }

        public void AddBitmap(SourceBitmap bitmap)
        {
            if (gDesktop == null
                || CanAddAnotherBitmap == false)
            {
                throw new InvalidOperationException();
            }

            if (bitmap.CheckResolutionLowerLimit())
            {
                if (bitmap.CheckStretchForMultiScreen()
                    || s.IsEachScreenEachSource == false)
                {
                    bitmaps.Clear();
                    bitmaps.Add(bitmap);

                    CanAddAnotherBitmap = false;
                }
                else
                {
                    if (bitmaps.Count < allScreen.AllScreens.Length)
                    {
                        bitmaps.Add(bitmap);
                    }

                    if (bitmaps.Count >= allScreen.AllScreens.Length)
                    {
                        CanAddAnotherBitmap = false;
                    }
                }
            }
        }

        public void SetToDesktop()
        {
            SetToDesktop(s.SaveFolder, "Current Wallpaper.bmp");
        }

        public void SetToDesktop(string path, string filename)
        {
            if (gDesktop == null)
            {
                throw new InvalidOperationException();
            }

            int bitmapsCount = bitmaps.Count;
            if (bitmapsCount == 1
                && bitmaps[0].CheckStretchForMultiScreen())
            {
                gDesktop.DrawImageFitOutside(bitmaps[0].Bitmap, allScreen.AdjustedVirtualDesktop);

                bitmaps[0].Save();
                bitmaps[0].Apply();

                s.LastUpdatedScreen = 0;
            }
            else if (bitmapsCount > 0)
            {
                int i = 0;
                int nextUpdateScreen;

                if (needFullRedraw
                    || s.IsEachScreenEachSource == false)
                {
                    nextUpdateScreen = 0;
                    while (nextUpdateScreen < allScreen.AllScreens.Length)
                    {
                        if (i >= bitmapsCount)
                        {
                            i = 0;
                        }

                        SourceBitmap bitmap = bitmaps[i];
                        gDesktop.DrawImageFitOutside(bitmap.Bitmap, allScreen.AllScreens[nextUpdateScreen].AdjustedBounds);
                        bitmap.Save();
                        bitmap.Apply();

                        i++;
                        nextUpdateScreen++;
                    }
                }
                else
                {
                    nextUpdateScreen = s.LastUpdatedScreen + 1;
                    while (i < bitmapsCount)
                    {
                        if (nextUpdateScreen >= allScreen.AllScreens.Length)
                        {
                            nextUpdateScreen = 0;
                        }

                        SourceBitmap bitmap = bitmaps[i];
                        gDesktop.DrawImageFitOutside(bitmap.Bitmap, allScreen.AllScreens[nextUpdateScreen].AdjustedBounds);
                        bitmap.Save();
                        bitmap.Apply();

                        i++;
                        nextUpdateScreen++;
                    }
                }

                s.LastUpdatedScreen = nextUpdateScreen - 1;
            }

            bitmaps.Clear();

            gDesktop.Dispose();
            gDesktop = null;

            if (bitmapsCount > 0)
            {
                string fullPath = desktop.SafeSave(path, filename, true);
                Wallpaper.Change(fullPath);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        private static void Change(string path)
        {
            using (var key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true))
            {
                //tiled (for multiscreen)
                key.SetValue(@"WallpaperStyle", "1");
                key.SetValue(@"TileWallpaper", "1");
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        private static string GetCurrentPath()
        {
            string path;
            using (var key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false))
            {
                path = key.GetValue("Wallpaper").ToString();
            }

            return path;
        }
    }
}
