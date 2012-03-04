using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DailyWallpainter.UI
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                this.Font = new Font("맑은 고딕", 9);
            }
        }
    }
}
