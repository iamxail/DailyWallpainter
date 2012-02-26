using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace DailyWallpainter.Helpers
{
    public static class BitmapHelper
    {
        public static Bitmap Crop(this Bitmap original, int x, int y, int width, int height)
        {
            try
            {
                Bitmap result = new Bitmap(width, height);

                using (var g = Graphics.FromImage(result))
                {
                    g.DrawImageUnscaledAndClipped(original, new Rectangle(x, y, width, height));
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        public static void DrawImageFitOutside(this Graphics g, Image image, Rectangle destRect)
        {
            //below calculation is different from ResizeToFitOutside
            Rectangle srcRect;
            if ((float)image.Width / (float)image.Height > (float)destRect.Width / (float)destRect.Height)
            {
                int newWidth = (int)((float)destRect.Width / (float)destRect.Height * (float)image.Height);

                srcRect = new Rectangle((image.Width - newWidth) / 2, 0, newWidth, image.Height);
            }
            else
            {
                int newHeight = (int)((float)destRect.Height / (float)destRect.Width * (float)image.Width);

                srcRect = new Rectangle(0, (image.Height - newHeight) / 2, image.Width, newHeight);
            }

            g.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
        }

        public static void SetHighQuality(this Graphics g)
        {
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        public static Bitmap ResizeToFitOutside(this Bitmap target, int width, int height)
        {
            try
            {
                Bitmap result = new Bitmap(width, height);

                using (var g = Graphics.FromImage(result))
                {
                    g.SetHighQuality();

                    Rectangle rect;
                    if ((float)width / (float)height > (float)target.Width / (float)target.Height)
                    {
                        int newHeight = (int)((float)target.Height / (float)target.Width * (float)width);

                        rect = new Rectangle(0, (height - newHeight) / 2, width, newHeight);
                    }
                    else
                    {
                        int newWidth = (int)((float)target.Width / (float)target.Height * (float)height);

                        rect = new Rectangle((width - newWidth) / 2, 0, newWidth, height);
                    }

                    g.DrawImage(target, rect);
                }

                return result;
            }
            catch
            {
                return null;
            }
        }

        private static string GetBitmapExtension(byte[] bitmapData)
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

        public static void SaveBitmap(this byte[] bitmapData, string directory, string filenamePrefix)
        {
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
    }
}
