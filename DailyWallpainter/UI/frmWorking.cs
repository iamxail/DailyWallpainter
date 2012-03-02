using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DailyWallpainter.UI
{
    public partial class frmWorking : BaseForm
    {
        public frmWorking()
        {
            InitializeComponent();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                pictureBox1.Visible = false;
                label1.Left = progressBar1.Left;
            }
            else
            {
                this.ClientSize = new Size(this.ClientSize.Width, this.ClientSize.Height - 16);
                progressBar1.Visible = false;
            }
        }
    }
}
