using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DailyWallpainter.UI
{
    public class TrayIcon : IDisposable
    {
        private static System.Windows.Forms.NotifyIcon ntfTray;
        private static System.Windows.Forms.ContextMenuStrip mnuTray;
        private static System.Windows.Forms.ToolStripMenuItem mitShowSettings;
        private static System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private static System.Windows.Forms.ToolStripMenuItem mitExit;

        public TrayIcon()
        {
            mnuTray = new System.Windows.Forms.ContextMenuStrip();
            mitShowSettings = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            mitExit = new System.Windows.Forms.ToolStripMenuItem();
            ntfTray = new System.Windows.Forms.NotifyIcon();

            // 
            // mnuTray
            // 
            mnuTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                mitShowSettings,
                toolStripMenuItem1,
                mitExit});
            mnuTray.Name = "mnuTray";
            mnuTray.Size = new System.Drawing.Size(153, 76);
            // 
            // mitShowSettings
            // 
            mitShowSettings.Name = "mitShowSettings";
            mitShowSettings.Size = new System.Drawing.Size(152, 22);
            mitShowSettings.Text = "설정(&S)";
            mitShowSettings.Click += new EventHandler(mitShowSettings_Click);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // mitExit
            // 
            mitExit.Name = "mitExit";
            mitExit.Size = new System.Drawing.Size(152, 22);
            mitExit.Text = "종료(&X)";
            mitExit.Click += new EventHandler(mitExit_Click);
            // 
            // ntfTray
            // 
            ntfTray.ContextMenuStrip = mnuTray;
            ntfTray.Icon = Properties.Resources.Paper;
            ntfTray.Text = "Daily Wallpainter";
            ntfTray.Visible = true;
            ntfTray.MouseClick += new MouseEventHandler(ntfTray_MouseClick);
        }

        private static void mitShowSettings_Click(object sender, EventArgs e)
        {
            Program.Context.ShowSettings();
        }

        private static void mitExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private static void ntfTray_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Program.Context.ShowSettings();
            }
        }

        /*~TrayIcon() // no unmanaged resources
        {
            Dispose(false);
        }*/

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ntfTray != null)
                {
                    var tray = ntfTray;
                    ntfTray = null;

                    tray.Visible = false;
                    tray.Dispose();
                }

                mnuTray.Dispose();
                mitShowSettings.Dispose();
                toolStripMenuItem1.Dispose();
                mitExit.Dispose();
            }
        }
    }
}
