using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DailyWallpainter.UI;
using System.IO;
using System.Diagnostics;
using DailyWallpainter.Helpers;

namespace DailyWallpainter.Updater
{
    public class Installer
    {
        protected MainApplicationContext cntx;

        public Installer(MainApplicationContext context)
        {
            cntx = context;
        }

        public bool CheckNeedInstall()
        {
#if DEBUG
            return false;
#else
            return !Application.ExecutablePath.ToLower().StartsWith(Program.AppData.ToLower());
#endif
        }

        public void InstallAsync()
        {
            var work = new WorkingUI(cntx);

            IUpdater updater = new GitHubUpdater("iamxail", Program.SafeName);
            updater.CheckCompleted += new CheckCompletedEventHandler(updater_CheckCompleted);
            updater.CheckAsync(new object[] { updater, work } );
        }

        private void updater_CheckCompleted(object sender, CheckCompletedEventArgs e)
        {
            var states = e.UserState as object[];
            var updater = states[0] as IUpdater;
            var work = states[1] as WorkingUI;

            if (updater.IsNewVersionAvailable)
            {
                updater.UpdateCompleted += new UpdateCompletedEventHandler(updater_UpdateCompleted);
                updater.UpdateAsync(false, work);
            }
            else
            {
                InstallInternal(work);
            }
        }

        private void updater_UpdateCompleted(object sender, UpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                InstallInternal(e.UserState as WorkingUI);
            }
        }

        private void InstallInternal(WorkingUI work)
        {
            try
            {
                string installedExePath = Path.Combine(Program.AppData, Program.SafeName + "_" + Program.Version + ".exe");

                try
                {
                    if (Directory.Exists(Program.AppData) == false)
                    {
                        Directory.CreateDirectory(Program.AppData);
                    }

                    File.Copy(Application.ExecutablePath, installedExePath, true);
                    FileUnblocker.Unblock(installedExePath);
                }
                catch
                {
                }

                string argsStr = string.Empty;

                var args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    argsStr = string.Join(" ", args, 1, args.Length - 1);
                }

                argsStr += " /setstartup";

                Process.Start(installedExePath, argsStr + " /forcestart");
            }
            catch (Exception ex)
            {
                work.MessageBoxShow("Daily Wallpainter를 설치하는 중에 문제가 발견되었습니다.\r\n\r\n" + ex.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                cntx.BeginInvoke(new MethodInvoker(() =>
                {
                    Application.Exit();
                }));
            }
        }
    }
}
