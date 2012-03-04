using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using DailyWallpainter.UI;
using System.Threading;

namespace DailyWallpainter.Updater
{
    public abstract class Updater : IUpdater
    {
        public event UpdateCompletedEventHandler UpdateCompleted = delegate { };

        protected virtual void UpdateAsync(string url, string filename, bool silent, object userState)
        {
            WebClient client = null;
            WorkingUI work = null;

            try
            {
                if (silent == false)
                {
                    work = new WorkingUI();
                }

                string updatedExePath = Path.Combine(Program.AppData, filename);

                client = new WebClient();
                client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(updater_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(url), updatedExePath, new object[] { client, updatedExePath, silent, work, userState });
            }
            catch
            {
                if (client != null)
                {
                    client.Dispose();
                    client = null;
                }

                if (silent == false)
                {
                    work.Dispose();
                }

                throw;
            }
        }

        private void updater_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Exception ex = null;
            var states = e.UserState as object[];
            var client = states[0] as WebClient;
            string updatedExePath = states[1] as string;
            bool silent = (states[2] as bool?).Value;
            var work = states[3] as WorkingUI;
            object userState = states[4];

            try
            {
                try
                {
                    client.Dispose();
                    client = null;
                }
                catch
                {
                }

                if (e.Error != null)
                {
                    throw e.Error;
                }

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
            catch (Exception thrown)
            {
                ex = thrown;
            }
            finally
            {
                try
                {
                    work.Dispose();
                }
                catch
                {
                }

                OnUpdateCompleted(userState, ex);
            }
        }

        protected virtual void OnUpdateCompleted(object userState, Exception ex)
        {
            UpdateCompleted(this, new UpdateCompletedEventArgs(userState, ex));
        }

        /*public void Check()
        {
            ManualResetEvent evt = new ManualResetEvent(false);
            object[] args = new object[] { evt, null };

            CheckCompleted += new CheckCompletedEventHandler(Updater_CheckCompleted);
            CheckAsync(args);

            evt.WaitOne(Timeout.Infinite);
            evt.Close();

            if (args[1] != null)
            {
                throw args[1] as Exception;
            }
        }

        private void Updater_CheckCompleted(object sender, CheckCompletedEventArgs e)
        {
            object[] args = e.UserState as object[];

            args[1] = e.Error;
            (args[0] as ManualResetEvent).Set();
        }*/

        public abstract event CheckCompletedEventHandler CheckCompleted;

        public abstract void CheckAsync();
        public abstract void CheckAsync(object userState);
        public abstract void UpdateAsync(bool silent);
        public abstract void UpdateAsync(bool silent, object userState);
        public abstract bool IsChecked { get; protected set; }
        public abstract bool IsNewVersionAvailable { get; protected set; }
        public abstract string LatestVersion { get; protected set; }
    }
}
