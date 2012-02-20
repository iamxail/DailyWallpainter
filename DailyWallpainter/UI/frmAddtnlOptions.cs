using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DailyWallpainter.UI
{
    public partial class frmAddtnlOptions : Form
    {
        private Settings s = Settings.Instance;
        private bool initialized;

        public frmAddtnlOptions()
        {
            initialized = false;

            InitializeComponent();

            var RLowerLimit = s.ResolutionLowerLimit;
            chkResolutionLowerLimit.Checked = RLowerLimit.Enabled;
            txtResolutionLowerWidth.Text = RLowerLimit.Width.ToString();
            txtResolutionLowerHeight.Text = RLowerLimit.Height.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkResolutionLowerLimit_CheckedChanged(object sender, EventArgs e)
        {
            var chked = chkResolutionLowerLimit.Checked;

            txtResolutionLowerWidth.Enabled = chked;
            txtResolutionLowerHeight.Enabled = chked;
            lblDescRL1.Enabled = chked;
            lblDescRL2.Enabled = chked;

            if (initialized)
            {
                s.ResolutionLowerLimit = new SizeWithState(chked, int.Parse(txtResolutionLowerWidth.Text), int.Parse(txtResolutionLowerHeight.Text));
            }
        }

        private void txtResolutionLowerWidth_Leave(object sender, EventArgs e)
        {
            int result;

            if (int.TryParse(txtResolutionLowerWidth.Text, out result) == false
                || result <= 0)
            {
                MessageBox.Show("너비는 0 보다 큰 정수만 입력할 수 있습니다.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtResolutionLowerWidth.Focus();
                txtResolutionLowerWidth.SelectAll();
            }
            else
            {
                s.ResolutionLowerLimit = new SizeWithState(chkResolutionLowerLimit.Checked, result, int.Parse(txtResolutionLowerHeight.Text));
            }
        }

        private void txtResolutionLowerHeight_Leave(object sender, EventArgs e)
        {
            int result;

            if (int.TryParse(txtResolutionLowerHeight.Text, out result) == false
                || result <= 0)
            {
                MessageBox.Show("높이는 0 보다 큰 정수만 입력할 수 있습니다.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtResolutionLowerHeight.Focus();
                txtResolutionLowerHeight.SelectAll();
            }
            else
            {
                s.ResolutionLowerLimit = new SizeWithState(chkResolutionLowerLimit.Checked, int.Parse(txtResolutionLowerWidth.Text), result);
            }
        }
    }
}
