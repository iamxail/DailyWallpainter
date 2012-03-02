using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace DailyWallpainter.Helpers
{
    public static class BitmapHelper
    {
        public static void SafeSave(this Bitmap bitmap, string path, string filename)
        {
            SafeSave(bitmap, path, filename, System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public static void SafeSave(this Bitmap bitmap, string path, string filename, System.Drawing.Imaging.ImageFormat format)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            var fullPath = Path.Combine(path, filename);

            bitmap.Save(fullPath, format);
        }

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
    }
}
