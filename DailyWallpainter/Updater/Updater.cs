using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using DailyWallpainter.UI;

namespace DailyWallpainter.Updater
{
    public abstract class Updater : IUpdater
    {
        protected virtual void Update(string url, string filename, bool silent)
        {
            frmWorking work = null;
            if (silent == false)
            {
                Program.Context.BeginInvoke(new MethodInvoker(() =>
                    {
                        Program.Context.TrayIcon.Visible = false;

                        work = new frmWorking();
                        work.Show();
                        work.Activate();
                    }));
            }

            string updatedExePath = Path.Combine(Program.AppData, filename);

            WebClient client = null;

            try
            {
                client = new WebClient();
                client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(updater_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(url), updatedExePath, new object[] { client, updatedExePath, silent });
            }
            catch
            {
                if (client != null)
                {
                    client.Dispose();
                    client = null;
                }

                throw;
            }
        }

        private void updater_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var states = e.UserState as object[];
            var client = states[0] as WebClient;
            string updatedExePath = states[1] as string;
            bool silent = (states[2] as bool?).Value;

            client.Dispose();
            client = null;

            string args = "/forcestart";

            if (Settings.Instance.RunOnStartup)
            {
                args += " /setstartup";
            }

            if (silent)
            {
                args += " /winstart";
            }

            Process.Start(updatedExePath, args);

            Application.Exit();
        }

        public abstract event CheckCompletedEventHandler CheckCompleted;

        public abstract void CheckAsync();
        public abstract void CheckAsync(object userState);
        public abstract void Update(bool silent);
        public abstract bool IsChecked { get; protected set; }
        public abstract bool IsNewVersionAvailable { get; protected set; }
        public abstract string LatestVersion { get; protected set; }
    }
}
