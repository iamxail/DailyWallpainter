using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DailyWallpainter.UI
{
    public class WorkingUI : IDisposable
    {
        private MainApplicationContext cntx;

        public WorkingUI()
            : this(Program.Context)
        {
            //do nothing
        }

        public WorkingUI(MainApplicationContext context)
        {
            cntx = context;

            UI(true, cntx);
        }

        /*~WorkingUI() // no unmanaged resources
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
                UI(false, cntx);
            }
        }

        public DialogResult MessageBoxShow(string text, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(workingUI, text, Program.Name, buttons, icon);
        }

        private static Form parent;
        public static void SetParent(Form modalParent)
        {
            parent = modalParent;
        }

        public static void DeleteParent()
        {
            parent = null;
        }

        public static Form Parent
        {
            get
            {
                return parent;
            }
        }

        private static frmWorking workingUI;
        private static int workingUICounter = 0;
        private static bool workingUIShown = false;
        private static void UI(bool show, MainApplicationContext cntx)
        {
            if (show)
            {
                workingUICounter++;
            }
            else
            {
                workingUICounter--;
            }

            if (workingUIShown == false && workingUICounter > 0)
            {
                workingUIShown = true;

                cntx.BeginInvoke(new MethodInvoker(() =>
                {
                    try
                    {
                        cntx.TrayIcon.Visible = false;
                    }
                    catch
                    {
                    }

                    workingUI = new frmWorking();
                    if (parent != null
                        && parent.IsDisposed == false
                        && parent.Visible)
                    {
                        try
                        {
                            parent.BeginInvoke(new MethodInvoker(() =>
                                {
                                    workingUI.ShowDialog(parent);
                                }));
                        }
                        catch
                        {
                            workingUI.Show();
                        }
                    }
                    else
                    {
                        workingUI.Show();
                    }
                    workingUI.Activate();
                }));
            }
            else if (workingUIShown && workingUICounter <= 0)
            {
                workingUIShown = false;
                workingUICounter = 0;

                cntx.BeginInvoke(new MethodInvoker(() =>
                {
                    try
                    {
                        cntx.TrayIcon.Visible = true;
                    }
                    catch
                    {
                    }

                    try
                    {
                        workingUI.Close();
                    }
                    catch
                    {
                    }
                }));
            }
        }
    }
}
