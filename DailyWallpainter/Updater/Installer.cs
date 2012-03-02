using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DailyWallpainter.UI;
using System.IO;
using System.Diagnostics;

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
            return !Application.ExecutablePath.ToLower().StartsWith(Program.AppData.ToLower());
        }

        public void Install()
        {
            frmWorking work = null;

            try
            {
                cntx.BeginInvoke(new MethodInvoker(() =>
                {
                    work = new frmWorking();
                    work.Show();
                    work.Activate();
                }));

                string installedExePath = Path.Combine(Program.AppData, Program.SafeName + "_" + Program.Version + ".exe");

                File.Copy(Application.ExecutablePath, installedExePath, true);

                string argsStr = string.Empty;

                var args = Environment.GetCommandLineArgs();
                if (args.Length > 1)
                {
                    argsStr = string.Join(" ", args, 1, args.Length - 1);
                }

                if (Settings.Instance.RunOnStartup)
                {
                    argsStr += " /setstartup";
                }

                Process.Start(installedExePath, argsStr + " /forcestart");

                cntx.BeginInvoke(new MethodInvoker(() =>
                {
                    Application.Exit();
                }));
            }
            catch
            {
                if (work != null)
                {
                    work.Dispose();
                    work = null;
                }
            }
        }
    }
}
