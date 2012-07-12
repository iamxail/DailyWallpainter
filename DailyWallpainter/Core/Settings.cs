using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using DailyWallpainter.Helpers;

namespace DailyWallpainter
{
    public class Settings
    {
        private static readonly string appName = Program.Name;
        private static readonly string appKey = @"Software\xail\" + appName;
        private static readonly string startupKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

        static readonly Settings instance = new Settings();

        static Settings()
        {
            //do nothing
        }

        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        private readonly string DefaultSaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), appName);
        private readonly int DefaultIntervalInMinute = 360;

        protected SourceCollection sources;
        protected bool initial;

        private Settings()
        {
            string settingsVersion = Get("");
            string programVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (settingsVersion != programVersion)
            {
                initial = true;
                Set("", programVersion);

                SetInitialSettings(settingsVersion);
            }
            else
            {
                initial = false;
            }
        }

        private void SetInitialSettings(string settingsVersion)
        {
            switch (settingsVersion)
            {
                case "":
                    //initially installed
                    break;

                case "1.0.0.0":
                    Sources.Initialize(true);
                    Set("IntervalInMinute", "360");
                    goto case "1.1.0.0";

                case "1.1.0.0":
                    if (Get(startupKey, appName).ToLower() == Application.ExecutablePath.ToLower())
                    {
                        RunOnStartup = true;
                    }
                    goto case "1.3.0.0";

                //1.2 is not released

                case "1.3.0.0":
                    //do nothing
                    goto case "1.4.0.0";

                case "1.4.0.0":
                    //do nothing
                    goto case "1.5.0.0";

                case "1.5.0.0":
                    FileUnblocker.Unblock(Application.ExecutablePath);
                    goto case "1.6.0.0";

                case "1.6.0.0":
                    var nasaSource = Sources[2];
                    if (nasaSource.Url == @"http://apod.nasa.gov/apod/"
                        && nasaSource.RegExp == "<a href=\"image/(.*?)\">")
                    {
                        Sources.Replace(2, new Source("NASA - Astronomy Picture of the Day",
                                                      @"http://apod.nasa.gov/apod/",
                                                      "<a href=\"image/(.*?)\"",
                                                      "http://apod.nasa.gov/apod/image/$1"
                                                      , nasaSource.Enabled, nasaSource.LastBitmapUrl));
                    }
                    break;

                default: //maybe settings of higher version is detected
                    //do nothing
                    break;
            }

            RunOnStartup = true;
        }

        public SourceCollection Sources
        {
            get
            {
                if (sources == null)
                {
                    sources = new SourceCollection(Get("Sources"));
                }

                return sources;
            }
        }

        public bool InitialStart
        {
            get
            {
                return initial;
            }
        }

        public bool RunOnStartup
        {
            get
            {
                string startupValue = Get(startupKey, appName).ToLower();
                string exePath = Application.ExecutablePath.ToLower();

                if (startupValue == "\"" + exePath + "\" /winstart")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (value)
                {
                    Set(startupKey, appName, "\"" + Application.ExecutablePath + "\" /winstart");
                }
                else
                {
                    try
                    {
                        using (var key = GetKey(startupKey))
                        {
                            key.DeleteValue(appName);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public string SaveFolder
        {
            get
            {
                string folder = Get("SaveFolder");
                if (folder != "")
                {
                    return folder;
                }
                else
                {
                    folder = DefaultSaveFolder;

                    SaveFolder = folder;

                    return folder;
                }
            }
            set
            {
                if (Directory.Exists(value) == false)
                {
                    Directory.CreateDirectory(value);
                }

                Set("SaveFolder", value);
            }
        }

        public int IntervalInMinute
        {
            get
            {
                string interval = Get("IntervalInMinute");
                int parsed;

                if (interval == ""
                    || int.TryParse(interval, out parsed) == false
                    || parsed <= 0)
                {
                    return DefaultIntervalInMinute;
                }
                else
                {
                    return parsed;
                }
            }
            set
            {
                Set("IntervalInMinute", value.ToString());
            }
        }

        public bool IsCheckOnlyWhenStartup
        {
            get
            {
                return GetBoolean("IsCheckOnlyWhenStartup", false);
            }
            set
            {
                Set("IsCheckOnlyWhenStartup", value.ToString());
            }
        }

        public string LastestVersionInformed
        {
            get
            {
                return Get("LastestVersionInformed");
            }
            set
            {
                Set("LastestVersionInformed", value);
            }
        }

        public SizeWithState ResolutionLowerLimit
        {
            get
            {
                try
                {
                    return SizeWithState.FromString(Get("ResolutionLowerLimit"));
                }
                catch
                {
                }

                return new SizeWithState(true, 700, 500);
            }
            set
            {
                Set("ResolutionLowerLimit", value.ToString());
            }
        }

        public bool IsStretchForMultiScreen
        {
            get
            {
                return GetBoolean("IsStretchForMultiScreen", true);
            }
            set
            {
                Set("IsStretchForMultiScreen", value.ToString());
            }
        }

        public bool IsCheckRatioWhenStretch
        {
            get
            {
                return GetBoolean("IsCheckRatioWhenStretch", true);
            }
            set
            {
                Set("IsCheckRatioWhenStretch", value.ToString());
            }
        }

        public bool IsSilentUpdate
        {
            get
            {
                return GetBoolean("IsSilentUpdate", true);
            }
            set
            {
                Set("IsSilentUpdate", value.ToString());
            }
        }

        public bool IsEachScreenEachSource
        {
            get
            {
                return GetBoolean("IsEachScreenEachSource", true);
            }
            set
            {
                Set("IsEachScreenEachSource", value.ToString());
            }
        }

        public string ScreensRects
        {
            get
            {
                return Get("ScreensRects");
            }
            set
            {
                Set("ScreensRects", value.ToString());
            }
        }

        public int LastUpdatedScreen
        {
            get
            {
                var rstr = Get("LastUpdatedScreen");
                int r;

                if (rstr == ""
                    || int.TryParse(rstr, out r) == false)
                {
                    return 0;
                }
                else
                {
                    return r;
                }
            }
            set
            {
                Set("LastUpdatedScreen", value.ToString());
            }
        }

        public bool IsNotStretch
        {
            get
            {
                return GetBoolean("IsNotStretch", false);
            }
            set
            {
                Set("IsNotStretch", value.ToString());
            }
        }

        /*public int DaysToSave
        {
            get
            {
                string days = Get("DaysToSave");
                if (days == "")
                {
                    return 14;
                }
                else
                {
                    return int.Parse(days);
                }
            }
            set
            {
                Set("DaysToSave", value.ToString());
            }
        }*/

        private static RegistryKey GetKey()
        {
            return GetKey(appKey);
        }

        private static RegistryKey GetKey(string keyPath)
        {
            RegistryKey key;
            try
            {
                key = Registry.CurrentUser.OpenSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);

                if (key == null)
                {
                    key = Registry.CurrentUser.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
                }
            }
            catch (Exception)
            {
                key = Registry.CurrentUser.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            return key;
        }

        public static string Get(string name)
        {
            return Get(appKey, name);
        }

        public static string Get(string keyPath, string name)
        {
            string result = "";

            try
            {
                using (var key = GetKey(keyPath))
                {
                    result = key.GetValue(name).ToString();
                }
            }
            catch
            {
            }

            return result;
        }

        public static bool GetBoolean(string name, bool defaultValue)
        {
            string value = Get(name);
            bool parsed;

            if (value == ""
                || bool.TryParse(value, out parsed) == false)
            {
                return defaultValue;
            }
            else
            {
                return parsed;
            }
        }

        public static void Set(string name, string value)
        {
            Set(appKey, name, value);
        }

        public static void Set(string keyPath, string name, string value)
        {
            try
            {
                using (var key = GetKey(keyPath))
                {
                    key.SetValue(name, value);
                }
            }
            catch
            {
            }
        }
    }

    public class SizeWithState
    {
        public bool Enabled { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public SizeWithState(bool enabled, int width, int height)
        {
            Enabled = enabled;
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return Enabled.ToString() + "," + Width.ToString() + "x" + Height.ToString();
        }

        public static SizeWithState FromString(string source)
        {
            var enabledAndSize = source.Split(',');
            if (enabledAndSize.Length != 2)
            {
                throw new ArgumentException();
            }

            bool enabled;
            if (bool.TryParse(enabledAndSize[0], out enabled) == false)
            {
                throw new ArgumentException();
            }

            var widthAndHeight = enabledAndSize[1].Split('x');
            if (widthAndHeight.Length != 2)
            {
                throw new ArgumentException();
            }

            int width;
            int height;

            if (int.TryParse(widthAndHeight[0], out width) == false
                || int.TryParse(widthAndHeight[1], out height) == false)
            {
                throw new ArgumentException();
            }

            return new SizeWithState(enabled, width, height);
        }
    }
}
