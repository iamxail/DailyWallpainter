using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace DailyWallpainter
{
    public partial class frmEditSource : Form
    {
        private delegate void Action();

        public frmEditSource()
        {
            InitializeComponent();
        }

        public frmEditSource(Source target)
            : this()
        {
            txtName.Text = target.Name;
            txtUrl.Text = target.Url;
            txtRegExp.Text = target.RegExp;
            txtReplacement.Text = target.Replacement;
        }

        public string SourceName
        {
            get
            {
                return txtName.Text;
            }
        }

        public string Url
        {
            get
            {
                return txtUrl.Text;
            }
        }

        public string RegExp
        {
            get
            {
                return txtRegExp.Text;
            }
        }

        public string Replacement
        {
            get
            {
                return txtReplacement.Text;
            }
        }

        private WebClient client;
        private void btnTest_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            lblTestInfo.Text = "대상 URL에서 HTML 원본을 다운받는 중...";

            try
            {
                var uri = new Uri(txtUrl.Text);

                client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
                client.DownloadStringAsync(uri);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Daily Wallpainter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Enabled = true;
                txtUrl.Focus();
                lblTestInfo.Text = "실패";

                return;
            }
        }

        private void InvokeIfRequired(Action func)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(func);
            }
            else
            {
                func();
            }
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    throw e.Error;
                }

                string html = e.Result;
                string regexpStr = txtRegExp.Text;
                string replacement = txtReplacement.Text;
                
                InvokeIfRequired(() =>
                    {
                        lblTestInfo.Text = "HTML 원본에 대해 정규표현식을 확인하는 중...";
                    });

                Regex regexp = new Regex(regexpStr);

                var match = regexp.Match(html);
                InvokeIfRequired(() =>
                    {
                        txtHtml.Text = html;
                        txtRegExpResult.Text = "";

                        cmbGroups.Items.Clear();
                        for (int i = 1; i < match.Groups.Count; i++)
                        {
                            cmbGroups.Items.Add("그룹 $" + i.ToString() + ": \"" + match.Groups[i].Value + "\"");
                        }
                        if (cmbGroups.Items.Count == 0)
                        {
                            cmbGroups.Items.Add("(정규표현식에 맞는 그룹이 없습니다.)");
                        }
                        cmbGroups.SelectedIndex = 0;
                    });

                if (match.Success)
                {
                    string result = match.Result(replacement);
                    InvokeIfRequired(() =>
                        {
                            txtRegExpResult.Text = result;
                        });

                    var uri = new Uri(result);

                    client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataCompleted);
                    client.DownloadDataAsync(uri);

                    InvokeIfRequired(() =>
                    {
                        lblTestInfo.Text = "미리보기 이미지를 다운받는 중...";
                    });
                }
                else
                {
                    throw new Exception("정규식에 해당하는 문자열을 찾을 수 없습니다.");
                }
            }
            catch (Exception ex)
            {
                client.Dispose();
                client = null;

                InvokeIfRequired(() =>
                    {
                        MessageBox.Show(ex.Message, "Daily Wallpainter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.Enabled = true;
                        txtRegExp.Focus();
                        lblTestInfo.Text = "실패";
                    });

                return;
            }
        }

        private void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    throw e.Error;
                }

                var data = e.Result;
                var bitmapStream = new MemoryStream(data);
                Bitmap bitmap = new Bitmap(bitmapStream);

                InvokeIfRequired(() =>
                    {
                        if (picImage.Image != null)
                        {
                            var oldBitmap = picImage.Image;
                            picImage.Image = bitmap;
                            oldBitmap.Dispose();
                        }
                        else
                        {
                            picImage.Image = bitmap;
                        }

                        lblResolution.Text = bitmap.Width.ToString() + " x " + bitmap.Height.ToString();
                    });

                client.Dispose();
                client = null;

                InvokeIfRequired(() =>
                {
                    this.Enabled = true;
                    txtHtml.Focus();
                    lblTestInfo.Text = "완료";
                });
            }
            catch (Exception ex)
            {
                client.Dispose();
                client = null;

                InvokeIfRequired(() =>
                {
                    MessageBox.Show(ex.Message, "Daily Wallpainter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Enabled = true;
                    txtRegExp.Focus();
                    lblTestInfo.Text = "실패";
                });

                return;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string error = "";
            Control target = null;

            if (txtName.Text == "")
            {
                error += "\r\n소스 이름이 입력되지 않았습니다.";

                target = txtName;
            }

            if (txtUrl.Text == "")
            {
                error += "\r\n대상 URL이 입력되지 않았습니다.";

                if (target == null)
                {
                    target = txtUrl;
                }
            }

            if (txtRegExp.Text == "")
            {
                error += "\r\n정규표현식이 입력되지 않았습니다.";

                if (target == null)
                {
                    target = txtRegExp;
                }
            }
            else
            {
                try
                {
                    var test = new Regex(txtRegExp.Text);
                }
                catch (Exception ex)
                {
                    error += "\r\n" + ex.Message;

                    if (target == null)
                    {
                        target = txtRegExp;
                    }
                }
            }

            if (txtReplacement.Text == "")
            {
                error += "\r\n바꾸기 패턴이 입력되지 않았습니다.";

                if (target == null)
                {
                    target = txtReplacement;
                }
            }

            if (error == "")
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("다음 문제가 발생하였습니다:\r\n" + error, "Daily Wallpainter", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (target != null)
                {
                    target.Focus();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
