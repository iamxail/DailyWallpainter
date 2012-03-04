using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using DailyWallpainter.Updater;

namespace DailyWallpainter.UI
{
    public partial class frmSettings : BaseForm
    {
        private Settings s = Settings.Instance;
        private bool refreshingSource = false;
        private bool initialized;
        private IUpdater updaterDelayed;

        public frmSettings() : base()
        {
            initialized = false;

            InitializeComponent();

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
            
            string appBgPath = Path.Combine(Program.AppData, @"appbg.bmp");
            if (File.Exists(appBgPath))
            {
                try
                {
                    var ms = new MemoryStream(File.ReadAllBytes(appBgPath));
                    picTitle.BackgroundImage = new Bitmap(ms);
                }
                catch
                {
                }
            }

            chkStartup.Checked = s.RunOnStartup;

            txtInterval.Text = s.IntervalInMinute.ToString();

            rdoCheckOnlyStart.Checked = s.IsCheckOnlyWhenStartup;

            initialized = true;
        }

        private delegate void NotifyNewVersionDelegate(IUpdater updater);
        public void NotifyNewVersion(IUpdater updater)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new NotifyNewVersionDelegate(NotifyNewVersion), new object[] { updater });
            }
            else
            {
                if (Program.Context.IsNeededToNotifyNewVersion)
                {
                    s.LastestVersionInformed = Program.Context.LatestVersion;

                    lnkDownloadUpdate.Visible = true;

                    if (MessageBox.Show(this, Program.Name + "가 새 " + Program.Context.LatestVersion + " 버전으로 업데이트되었습니다.\r\n\r\n지금 업데이트 하시겠습니까?", Program.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == System.Windows.Forms.DialogResult.Yes)
                    {
                        try
                        {
                            WorkingUI.SetParent(this);
                            updater.UpdateAsync(false);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        updaterDelayed = updater;
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

            if (MessageBox.Show("이 소스를 정말 삭제하시겠습니까?\r\n\r\n" + target.Name, Program.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
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

                        File.WriteAllText(testPath, "Temp file created by " + Program.Name);
                        File.Delete(testPath);

                        lnkSaveFolder.Text = path;
                        s.SaveFolder = path;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("이 폴더에 접근할 권한이 없습니다. 다른 폴더를 지정해주세요.\r\n\r\n" + path, Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("새 배경화면 확인 시간 간격은 0 보다 큰 정수만 입력할 수 있습니다.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                MessageBox.Show("이 설정은 다음 시작할 때부터 적용됩니다.", Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void lnkDownloadUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (updaterDelayed == null)
            {
                lnkDownloadUpdate.Visible = false;
                return;
            }

            switch (MessageBox.Show(this, "업데이트를 하는 동안 새 버전의 변경 사항을 확인하시겠습니까?\r\n\r\n취소를 누르면 업데이트를 중단합니다.", Program.Name, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
            {
                case System.Windows.Forms.DialogResult.Yes:
                    Process.Start(@"https://github.com/iamxail/DailyWallpainter#readme");
                    break;

                case System.Windows.Forms.DialogResult.No:
                    break;

                case System.Windows.Forms.DialogResult.Cancel:
                    return;
            }

            lnkDownloadUpdate.Visible = false;

            try
            {
                WorkingUI.SetParent(this);
                updaterDelayed.UpdateAsync(true);
            }
            catch
            {
            }
        }

        private void btnAddtnlOptions_Click(object sender, EventArgs e)
        {
            using (var addopts = new frmAddtnlOptions())
            {
                addopts.ShowDialog(this);
            }
        }

        private void frmSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WorkingUI.Parent == this)
            {
                WorkingUI.DeleteParent();
            }
        }
    }
}
