﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DailyWallpainter.UI
{
    public partial class frmAddtnlOptions : BaseForm
    {
        private Settings s = Settings.Instance;
        private bool initialized;

        public frmAddtnlOptions() : base()
        {
            initialized = false;

            InitializeComponent();

            var RLowerLimit = s.ResolutionLowerLimit;
            chkResolutionLowerLimit.Checked = RLowerLimit.Enabled;
            txtResolutionLowerWidth.Text = RLowerLimit.Width.ToString();
            txtResolutionLowerHeight.Text = RLowerLimit.Height.ToString();

            if (s.IsStretchForMultiScreen)
            {
                rdoMultiScreenStrectch.Checked = true;
            }
            else
            {
                rdoMultiScreenEach.Checked = true;
            }
            chkMultiScreenStretchCheckRatio.Checked = s.IsCheckRatioWhenStretch;
            chkEachScreenEachSource.Checked = s.IsEachScreenEachSource;
            chkNotStretch.Checked = s.IsNotStretch;

            chkSilentUpdate.Checked = s.IsSilentUpdate;

            initialized = true;
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

        private void rdoMultiScreenStrectch_CheckedChanged(object sender, EventArgs e)
        {
            chkMultiScreenStretchCheckRatio.Enabled = rdoMultiScreenStrectch.Checked;
            chkEachScreenEachSource.Enabled = rdoMultiScreenEach.Checked || chkMultiScreenStretchCheckRatio.Checked;
            chkNotStretch.Enabled = chkEachScreenEachSource.Enabled;

            if (initialized)
            {
                s.IsStretchForMultiScreen = rdoMultiScreenStrectch.Checked;
            }
        }

        private void chkMultiScreenStretchCheckRatio_CheckedChanged(object sender, EventArgs e)
        {
            chkEachScreenEachSource.Enabled = rdoMultiScreenEach.Checked || chkMultiScreenStretchCheckRatio.Checked;
            chkNotStretch.Enabled = chkEachScreenEachSource.Enabled;

            if (initialized)
            {
                s.IsCheckRatioWhenStretch = chkMultiScreenStretchCheckRatio.Checked;
            }
        }

        private void chkSilentUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                s.IsSilentUpdate = chkSilentUpdate.Checked;
            }
        }

        private void chkEachScreenEachSource_CheckedChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                s.IsEachScreenEachSource = chkEachScreenEachSource.Checked;
            }
        }

        private void chkNotStretch_CheckedChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                s.IsNotStretch = chkNotStretch.Checked;
            }
        }
    }
}
