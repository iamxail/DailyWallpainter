using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using DailyWallpainter.Helpers;

namespace DailyWallpainter
{
    public class SourceBitmap : IDisposable
    {
        protected byte[] bitmapData;
        protected MemoryStream ms;
        protected Bitmap bitmap;
        protected Source parent;
        protected Settings s;

        internal SourceBitmap(Source from, byte[] bitmapBytes)
        {
            try
            {
                bitmapData = bitmapBytes;
                ms = new MemoryStream(bitmapData);
                bitmap = new Bitmap(ms);
                s = Settings.Instance;
                parent = from;
            }
            catch
            {
                Dispose(true);

                throw;
            }
        }

        /*~SourceBitmap() // no unmanaged resources
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
                if (bitmap != null)
                {
                    var tb = bitmap;
                    bitmap = null;
                    tb.Dispose();
                }

                if (ms != null)
                {
                    var tms = ms;
                    ms = null;
                    tms.Dispose();
                }

                bitmapData = null;
                parent = null;
                s = null;
            }
        }

        private string GetBitmapExtension(byte[] bitmapData)
        {
            string ext = ".unknown";
            if (bitmapData[0] == 0xFF && bitmapData[1] == 0xD8)
            {
                ext = ".jpg";
            }
            else if (bitmapData[0] == 137 && bitmapData[1] == 80 && bitmapData[2] == 78 && bitmapData[3] == 71)
            {
                ext = ".png";
            }
            else
            {
                string header = Encoding.ASCII.GetString(bitmapData, 0, 3);
                if (header == "GIF")
                {
                    ext = ".gif";
                }
                else if (header.Substring(0, 2) == "BM")
                {
                    ext = ".bmp";
                }
            }

            return ext;
        }

        public void Save()
        {
            Save(s.SaveFolder);
        }

        public void Save(string directory)
        {
            string filenamePrefix = parent.Name;

            string safeBitmapFilename = filenamePrefix + " at " + string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                safeBitmapFilename.Replace(c, '_');
            }

            string ext = GetBitmapExtension(bitmapData);

            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            string path = Path.Combine(directory, safeBitmapFilename);

            string suffix = "";
            int i = 1;
            while (File.Exists(path + suffix + ext))
            {
                i++;
                suffix = " (" + i.ToString() + ")";
            }

            File.WriteAllBytes(path + suffix + ext, bitmapData);
        }

        public bool CheckResolutionLowerLimit()
        {
            var limit = s.ResolutionLowerLimit;

            return limit.Enabled == false
                || (bitmap.Width > limit.Width && bitmap.Height > limit.Height);
        }

        public bool CheckStretchForMultiScreen()
        {
            var allScreen = new MultiScreenInfo();

            return s.IsStretchForMultiScreen
                && (s.IsCheckRatioWhenStretch == false || allScreen.IsPreferredToStretch(bitmap.Size));
        }

        public Bitmap Bitmap
        {
            get
            {
                return bitmap;
            }
        }
    }
}
