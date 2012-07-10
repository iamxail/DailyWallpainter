using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DailyWallpainter.Helpers
{
    public class MultiScreenInfo
    {
        private static readonly MultiScreenInfo instance = new MultiScreenInfo();

        static MultiScreenInfo()
        {
            //do nothing
        }

        public static MultiScreenInfo Instance
        {
            get
            {
                return instance;
            }
        }

        public bool IsChanged { get; protected set; }

        protected Rectangle allScreen;
        protected Rectangle allScreenOffsetted;
        protected Point allScreenOffset;
        protected SingleScreenInfo[] scrs;

        private MultiScreenInfo()
        {
            allScreen = Rectangle.Empty;
            foreach (var scr in Screen.AllScreens)
            {
                allScreen = Rectangle.Union(allScreen, scr.Bounds);
            }

            allScreenOffset = new Point(-allScreen.Left, -allScreen.Top);

            allScreenOffsetted = allScreen;
            allScreenOffsetted.Offset(allScreenOffset);

            List<SingleScreenInfo> lstScrs = new List<SingleScreenInfo>();
            foreach (var scr in Screen.AllScreens)
            {
                lstScrs.Add(new SingleScreenInfo(scr, allScreenOffset));
            }
            scrs = lstScrs.ToArray();

            var toStr = this.ToString();
            var s = Settings.Instance;
            if (toStr != s.ScreensRects)
            {
                s.ScreensRects = toStr;

                IsChanged = true;
            }
            else
            {
                IsChanged = false;
            }
        }

        public bool IsPreferredToStretch(Size imageSize)
        {
            Size smallImageSize;
            if (imageSize.Width > imageSize.Height)
            {
                smallImageSize = new Size(100, (int)((float)imageSize.Height / (float)imageSize.Width * 100));
            }
            else
            {
                smallImageSize = new Size((int)((float)imageSize.Width / (float)imageSize.Height * 100), 100);
            }

            int stretchCropArea = CalculateCropArea(smallImageSize, allScreen.Size);
            int totalEachCropArea = 0;
            foreach (var scr in scrs)
            {
                totalEachCropArea += CalculateCropArea(smallImageSize, scr.Bounds.Size);
            }

            return stretchCropArea <= (totalEachCropArea / scrs.Length);
        }

        protected int CalculateCropArea(Size source, Size dest)
        {
            if ((float)source.Width / (float)source.Height > (float)dest.Width / (float)dest.Height)
            {
                int newWidth = (int)((float)dest.Width / (float)dest.Height * (float)source.Height);

                return (source.Width - newWidth) * source.Height;
            }
            else
            {
                int newHeight = (int)((float)dest.Height / (float)dest.Width * (float)source.Width);

                return source.Width * (source.Height - newHeight);
            }
        }

        public Rectangle VirtualDesktop
        {
            get
            {
                return allScreen;
            }
        }

        public Rectangle AdjustedVirtualDesktop
        {
            get
            {
                return allScreenOffsetted;
            }
        }

        public SingleScreenInfo[] AllScreens
        {
            get
            {
                return scrs;
            }
        }

        public override string ToString()
        {
            string result = "";
            foreach (var scr in scrs)
            {
                result += scr.ToString() + "/";
            }

            return result.Substring(0, result.Length - 1);
        }
    }

    public class SingleScreenInfo
    {
        protected Rectangle bounds;
        protected Rectangle offsettedBounds;

        internal SingleScreenInfo(Screen sourceScr, Point offset)
        {
            bounds = sourceScr.Bounds;

            offsettedBounds = bounds;
            offsettedBounds.Offset(offset);
        }

        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        public Rectangle AdjustedBounds
        {
            get
            {
                return offsettedBounds;
            }
        }

        public override string ToString()
        {
            return bounds.ToString();
        }
    }
}
