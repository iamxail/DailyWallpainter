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
                DrawBitmapToDesktop(bitmaps[0]);

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

                        DrawBitmapToDesktop(bitmaps[i], nextUpdateScreen);

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

                        DrawBitmapToDesktop(bitmaps[i], nextUpdateScreen);

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

        private void DrawBitmapToDesktop(SourceBitmap bitmap)
        {
            gDesktop.DrawImageFitOutside(bitmaps[0].Bitmap, allScreen.AdjustedVirtualDesktop);

            bitmap.Save();
            bitmap.Apply();
        }

        private void DrawBitmapToDesktop(SourceBitmap bitmap, int screenNumber)
        {
            Rectangle dest = allScreen.AllScreens[screenNumber].AdjustedBounds;
            int bitmapWidth = bitmap.Bitmap.Width;
            int bitmapHeight = bitmap.Bitmap.Height;

            if (s.IsNotStretch
                && (bitmapWidth < dest.Width || bitmapHeight < dest.Height))
            {
                int srcX;
                int srcWidth;
                int srcY;
                int srcHeight;
                if (bitmapWidth <= dest.Width)
                {
                    srcX = 0;
                    srcWidth = bitmapWidth;
                }
                else //bitmapWidth > bounds.Width
                {
                    srcX = (bitmapWidth - dest.Width) / 2;
                    srcWidth = dest.Width;
                }
                if (bitmapHeight <= dest.Height)
                {
                    srcY = 0;
                    srcHeight = bitmapHeight;
                }
                else //bitmapHeight > bounds.Height
                {
                    srcY = (bitmapHeight - dest.Height) / 2;
                    srcHeight = dest.Height;
                }
                Rectangle src = new Rectangle(srcX, srcY, srcWidth, srcHeight);

                //get edge color
                Color bgColor;
                try
                {
                    const int pickingPointNum = 5;
                    int R = 0;
                    int G = 0;
                    int B = 0;
                    for (int x = 0; x < pickingPointNum; x++)
                    {
                        var c = bitmap.Bitmap.GetPixel(((srcWidth - 1) / pickingPointNum) * x + srcX, srcY);
                        R += c.R;
                        G += c.G;
                        B += c.B;

                        c = bitmap.Bitmap.GetPixel(((srcWidth - 1) / pickingPointNum) * x + srcX, srcY + srcHeight - 1);
                        R += c.R;
                        G += c.G;
                        B += c.B;
                    }
                    for (int y = 1; y < pickingPointNum - 1; y++)
                    {
                        var c = bitmap.Bitmap.GetPixel(srcX, ((srcHeight - 1) / pickingPointNum) * y + srcY);
                        R += c.R;
                        G += c.G;
                        B += c.B;

                        c = bitmap.Bitmap.GetPixel(srcX + srcWidth - 1, ((srcHeight - 1) / pickingPointNum) * y + srcY);
                        R += c.R;
                        G += c.G;
                        B += c.B;
                    }
                    R /= pickingPointNum * 4 - 4; //2n + 2*(n - 2)
                    G /= pickingPointNum * 4 - 4;
                    B /= pickingPointNum * 4 - 4;
                
                    bgColor = Color.FromArgb(R, G, B);
                }
                catch
                {
                    bgColor = Color.Black;
                }

                using (var brush = new SolidBrush(bgColor))
                {
                    gDesktop.FillRectangle(brush, src);
                }

                gDesktop.DrawImage(bitmap.Bitmap, dest, src, GraphicsUnit.Pixel);
            }
            else
            {
                gDesktop.DrawImageFitOutside(bitmap.Bitmap, dest);
            }

            bitmap.Save();
            bitmap.Apply();
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
