using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DailyWallpainter
{
    public class Settings
    {
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

        private const string appName = @"Daily Wallpainter";
        private readonly string appKey = @"Software\xail\" + appName;
        private const string startupKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private readonly string DefaultSaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), appName);
        private readonly int DefaultIntervalInMinute = 360;

        protected SourcesCollection sources;
        protected bool initial;

        public Settings()
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
            if (settingsVersion == "1.0.0.0")
            {
                Sources.ForceInitialize();
                Set("IntervalInMinute", "360");
            }
            else
            {
                //do nothing
            }

            RunOnStartup = true;
        }

        public SourcesCollection Sources
        {
            get
            {
                if (sources == null)
                {
                    sources = new SourcesCollection(Get("Sources"));
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
                if (Get(startupKey, appName).ToString().ToLower() == Application.ExecutablePath.ToLower())
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
                    Set(startupKey, appName, Application.ExecutablePath);
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

        protected RegistryKey GetKey()
        {
            return GetKey(appKey);
        }

        protected RegistryKey GetKey(string keyPath)
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

        public string Get(string name)
        {
            return Get(appKey, name);
        }

        public string Get(string keyPath, string name)
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

        public void Set(string name, string value)
        {
            Set(appKey, name, value);
        }

        public void Set(string keyPath, string name, string value)
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
}
