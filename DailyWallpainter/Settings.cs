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

        public Settings()
        {
            string settingsVersion = Get("");
            string programVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (settingsVersion != programVersion)
            {
                initial = true;
                Set("", programVersion);

                if (settingsVersion == "1.0.0.0")
                {
                    Set("Sources", "");
                    Set("IntervalInMinute", "360");
                }
            }
            else
            {
                initial = false;
            }

            sources = new SourcesCollection(Get("Sources"));
        }

        protected SourcesCollection sources;
        protected bool initial;

        public SourcesCollection Sources
        {
            get
            {
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
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        if (key.GetValue("Daily Wallpainter", "").ToString().ToLower() == Application.ExecutablePath.ToLower())
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                }

                return false;
            }
            set
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                    {
                        key.SetValue("Daily Wallpainter", Application.ExecutablePath);
                    }
                }
                catch (Exception)
                {
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
                    folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Daily Wallpainter");

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
                if (interval == "")
                {
                    return 360;
                }
                else
                {
                    return int.Parse(interval);
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
            RegistryKey key;
            try
            {
                key = Registry.CurrentUser.OpenSubKey(@"Software\xail\Daily Wallpainter", RegistryKeyPermissionCheck.ReadWriteSubTree);

                if (key == null)
                {
                    key = Registry.CurrentUser.CreateSubKey(@"Software\xail\Daily Wallpainter", RegistryKeyPermissionCheck.ReadWriteSubTree);
                }
            }
            catch (Exception)
            {
                key = Registry.CurrentUser.CreateSubKey(@"Software\xail\Daily Wallpainter", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            return key;
        }

        public string Get(string name)
        {
            RegistryKey key = null;
            string result = "";

            try
            {
                key = GetKey();
                result = key.GetValue(name).ToString();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }

            return result;
        }

        public void Set(string name, string value)
        {
            RegistryKey key = null;

            try
            {
                key = GetKey();
                key.SetValue(name, value);
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
            }
        }
    }
}
