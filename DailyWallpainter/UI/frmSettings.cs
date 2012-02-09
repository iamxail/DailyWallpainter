using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace DailyWallpainter.UI
{
    public partial class frmSettings : Form
    {
        private Settings s = Settings.Instance;
        private bool refreshingSource = false;
        private bool initialized;

        public frmSettings()
        {
            initialized = false;

            InitializeComponent();

            if (Environment.OSVersion.Version.Major >= 6)
            {
                this.Font = new Font("맑은 고딕", 9);
            }

            /*int daysToSave = s.DaysToSave;
            if (daysToSave == 0)
            {
                chkUnlimitedSave.Checked = true;
            }
            else
            {
                chkUnlimitedSave.Checked = false;
                txtDaysToSave.Text = daysToSave.ToString();
            }*/

            lnkSaveFolder.Text = s.SaveFolder;

            RefreshSources();
            
            string appBgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Daily Wallpainter\appbg.bmp");
            if (File.Exists(appBgPath))
            {
                try
                {
                    picTitle.BackgroundImage = new Bitmap(appBgPath);
                }
                catch (Exception)
                {
                }
            }

            chkStartup.Checked = s.RunOnStartup;

            txtInterval.Text = s.IntervalInMinute.ToString();

            rdoCheckOnlyStart.Checked = s.IsCheckOnlyWhenStartup;

            initialized = true;
        }

        private delegate void Action();
        public void NotifyNewVersion()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(NotifyNewVersion));
            }
            else
            {
                if (Program.IsNeededToNotifyNewVersion())
                {
                    s.LastestVersionInformed = Program.LatestVersion;

                    lnkDownloadUpdate.Visible = true;

                    if (MessageBox.Show(this, "Daily Wallpainter가 새 " + Program.LatestVersion + " 버전으로 업데이트되었습니다.\r\n\r\n지금 다운로드 페이지를 여시겠습니까?", "Daily Wallpainter", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == System.Windows.Forms.DialogResult.Yes)
                    {
                        lnkDownloadUpdate_LinkClicked(this, null);
                    }
                }
            }
        }

        private void RefreshSources()
        {
            refreshingSource = true;

            lstSources.Items.Clear();

            foreach (var source in s.Sources)
            {
                lstSources.Items.Add(source.Name, source.Enabled);
            }

            refreshingSource = false;
        }

        private void btnAddSource_Click(object sender, EventArgs e)
        {
            using (var editor = new frmEditSource())
            {
                if (editor.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    s.Sources.Add(new Source(editor));
                    RefreshSources();
                }
            }
        }

        private void mitEditSource_Click(object sender, EventArgs e)
        {
            int i = lstSources.SelectedIndex;

            if (i < 0)
            {
                return;
            }

            var target = s.Sources[i];

            using (var editor = new frmEditSource(target))
            {
                if (editor.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    s.Sources.Replace(i, new Source(editor));
                    RefreshSources();
                }
            }
        }

        private void mitRemoveSource_Click(object sender, EventArgs e)
        {
            int i = lstSources.SelectedIndex;

            if (i < 0)
            {
                return;
            }

            var target = s.Sources[i];

            if (MessageBox.Show("이 소스를 정말 삭제하시겠습니까?\r\n\r\n" + target.Name, "Daily Wallpainter", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.Yes)
            {
                s.Sources.RemoveAt(i);
                RefreshSources();
            }
        }

        /*private void chkUnlimitedSave_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUnlimitedSave.Checked)
            {
                txtDaysToSave.Enabled = false;
                txtDaysToSave.Text = "";

                s.DaysToSave = 0;
            }
            else
            {
                txtDaysToSave.Enabled = true;
                txtDaysToSave.Text = "14";

                s.DaysToSave = 14;
            }
        }*/

        private void lnkTwitter_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"http://twitter.com/iamxail");
        }

        private void lnkSaveFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(lnkSaveFolder.Text);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var browse = new FolderBrowserDialog())
            {
                browse.Description = "\r\n다운로드 받은 배경화면을 저장할 폴더를 선택하세요:";
                browse.SelectedPath = lnkSaveFolder.Text;
                browse.ShowNewFolderButton = true;

                if (browse.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = browse.SelectedPath;

                    try
                    {
                        string testPath = Path.Combine(path, "YouCanDeleteThisFileWhenever");

                        File.WriteAllText(testPath, "Temp file created by Daily Wallpainter");
                        File.Delete(testPath);

                        lnkSaveFolder.Text = path;
                        s.SaveFolder = path;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("이 폴더에 접근할 권한이 없습니다. 다른 폴더를 지정해주세요.\r\n\r\n" + path, "Daily Wallpainter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /*private void txtDaysToSave_Leave(object sender, EventArgs e)
        {
            int result;

            if (int.TryParse(txtDaysToSave.Text, out result) == false
                || result <= 0)
            {
                MessageBox.Show("0 보다 큰 숫자만 입력할 수 있습니다.");

                txtDaysToSave.Focus();
                txtDaysToSave.SelectAll();
            }
            else
            {
                s.DaysToSave = result;
            }
        }*/

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstSources_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (refreshingSource == false)
            {
                var target = s.Sources[e.Index];

                var source = new Source(target.Name, target.Url, target.RegExp, target.Replacement, (e.NewValue == CheckState.Checked), target.LastBitmapUrl);

                s.Sources.Replace(e.Index, source);
            }
        }

        private void chkStartup_CheckedChanged(object sender, EventArgs e)
        {
            s.RunOnStartup = chkStartup.Checked;
        }

        private void txtInterval_Leave(object sender, EventArgs e)
        {
            int result;

            if (int.TryParse(txtInterval.Text, out result) == false
                || result <= 0)
            {
                MessageBox.Show("새 배경화면 확인 시간 간격은 0 보다 큰 정수만 입력할 수 있습니다.", "Daily Wallpainter", MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtInterval.Focus();
                txtInterval.SelectAll();
            }
            else
            {
                s.IntervalInMinute = result;
            }
        }

        private void rdoCheckByInterval_CheckedChanged(object sender, EventArgs e)
        {
            txtInterval.Enabled = rdoCheckByInterval.Checked;
        }

        private void rdoCheckOnlyStart_CheckedChanged(object sender, EventArgs e)
        {
            s.IsCheckOnlyWhenStartup = rdoCheckOnlyStart.Checked;

            if (initialized)
            {
                MessageBox.Show("이 설정은 다음 시작할 때부터 적용됩니다.", "Daily Wallpainter", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lnkDownloadUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"https://github.com/iamxail/DailyWallpainter#readme");
            lnkDownloadUpdate.Visible = false;
        }

        private void frmSettings_Shown(object sender, EventArgs e)
        {
            NotifyNewVersion();
        }
    }
}
