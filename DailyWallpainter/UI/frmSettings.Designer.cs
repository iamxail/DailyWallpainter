namespace DailyWallpainter.UI
{
    partial class frmSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.lstSources = new System.Windows.Forms.CheckedListBox();
            this.mnuSources = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mitEditSource = new System.Windows.Forms.ToolStripMenuItem();
            this.mitRemoveSource = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.lnkSaveFolder = new System.Windows.Forms.LinkLabel();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnAddSource = new System.Windows.Forms.Button();
            this.lnkTwitter = new System.Windows.Forms.LinkLabel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.picTitle = new System.Windows.Forms.PictureBox();
            this.chkStartup = new System.Windows.Forms.CheckBox();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mnuSources.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTitle)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 200);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "다음 소스에서 배경화면을 가져옵니다(&S).";
            // 
            // lstSources
            // 
            this.lstSources.ContextMenuStrip = this.mnuSources;
            this.lstSources.FormattingEnabled = true;
            this.lstSources.IntegralHeight = false;
            this.lstSources.Location = new System.Drawing.Point(58, 218);
            this.lstSources.Name = "lstSources";
            this.lstSources.Size = new System.Drawing.Size(480, 70);
            this.lstSources.TabIndex = 2;
            this.lstSources.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstSources_ItemCheck);
            // 
            // mnuSources
            // 
            this.mnuSources.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mitEditSource,
            this.mitRemoveSource});
            this.mnuSources.Name = "mnuSources";
            this.mnuSources.Size = new System.Drawing.Size(122, 48);
            // 
            // mitEditSource
            // 
            this.mitEditSource.Name = "mitEditSource";
            this.mitEditSource.Size = new System.Drawing.Size(121, 22);
            this.mitEditSource.Text = "편집(&E)...";
            this.mitEditSource.Click += new System.EventHandler(this.mitEditSource_Click);
            // 
            // mitRemoveSource
            // 
            this.mitRemoveSource.Name = "mitRemoveSource";
            this.mitRemoveSource.Size = new System.Drawing.Size(121, 22);
            this.mitRemoveSource.Text = "제거(&R)";
            this.mitRemoveSource.Click += new System.EventHandler(this.mitRemoveSource_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 316);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(246, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "가져온 배경화면을 이 폴더에 저장합니다(&F).";
            // 
            // lnkSaveFolder
            // 
            this.lnkSaveFolder.AutoSize = true;
            this.lnkSaveFolder.Location = new System.Drawing.Point(56, 338);
            this.lnkSaveFolder.Name = "lnkSaveFolder";
            this.lnkSaveFolder.Size = new System.Drawing.Size(61, 12);
            this.lnkSaveFolder.TabIndex = 5;
            this.lnkSaveFolder.TabStop = true;
            this.lnkSaveFolder.Text = "linkLabel1";
            this.lnkSaveFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSaveFolder_LinkClicked);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(401, 333);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(137, 22);
            this.btnBrowse.TabIndex = 6;
            this.btnBrowse.Text = "다른 폴더에 저장(&B)";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnAddSource
            // 
            this.btnAddSource.Location = new System.Drawing.Point(431, 291);
            this.btnAddSource.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddSource.Name = "btnAddSource";
            this.btnAddSource.Size = new System.Drawing.Size(107, 22);
            this.btnAddSource.TabIndex = 3;
            this.btnAddSource.Text = "새 소스 추가(&A)";
            this.btnAddSource.UseVisualStyleBackColor = true;
            this.btnAddSource.Click += new System.EventHandler(this.btnAddSource_Click);
            // 
            // lnkTwitter
            // 
            this.lnkTwitter.AutoSize = true;
            this.lnkTwitter.Location = new System.Drawing.Point(480, 132);
            this.lnkTwitter.Name = "lnkTwitter";
            this.lnkTwitter.Size = new System.Drawing.Size(58, 12);
            this.lnkTwitter.TabIndex = 11;
            this.lnkTwitter.TabStop = true;
            this.lnkTwitter.Text = "@iamxail";
            this.lnkTwitter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTwitter_LinkClicked);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(456, 11);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(82, 24);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "닫기(&C)";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(0, 410);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(570, 1);
            this.panel1.TabIndex = 10;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Location = new System.Drawing.Point(0, 411);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(570, 100);
            this.panel2.TabIndex = 9;
            // 
            // picTitle
            // 
            this.picTitle.BackgroundImage = global::DailyWallpainter.Properties.Resources.DefaultBackground;
            this.picTitle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picTitle.Image = global::DailyWallpainter.Properties.Resources.Title;
            this.picTitle.Location = new System.Drawing.Point(0, 0);
            this.picTitle.Name = "picTitle";
            this.picTitle.Size = new System.Drawing.Size(370, 165);
            this.picTitle.TabIndex = 0;
            this.picTitle.TabStop = false;
            // 
            // chkStartup
            // 
            this.chkStartup.AutoSize = true;
            this.chkStartup.Location = new System.Drawing.Point(46, 167);
            this.chkStartup.Name = "chkStartup";
            this.chkStartup.Size = new System.Drawing.Size(208, 16);
            this.chkStartup.TabIndex = 0;
            this.chkStartup.Text = "윈도 시작 시 같이 실행합니다(&W).";
            this.chkStartup.UseVisualStyleBackColor = true;
            this.chkStartup.CheckedChanged += new System.EventHandler(this.chkStartup_CheckedChanged);
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(46, 370);
            this.txtInterval.MaxLength = 3;
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(31, 21);
            this.txtInterval.TabIndex = 8;
            this.txtInterval.Leave += new System.EventHandler(this.txtInterval_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(78, 375);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(246, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "분 마다 새 배경화면이 있는지 확인합니다(&I).";
            // 
            // frmSettings
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(559, 456);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtInterval);
            this.Controls.Add(this.chkStartup);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lnkTwitter);
            this.Controls.Add(this.btnAddSource);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lnkSaveFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lstSources);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.mnuSources.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTitle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox lstSources;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel lnkSaveFolder;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnAddSource;
        private System.Windows.Forms.LinkLabel lnkTwitter;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ContextMenuStrip mnuSources;
        private System.Windows.Forms.ToolStripMenuItem mitRemoveSource;
        private System.Windows.Forms.ToolStripMenuItem mitEditSource;
        private System.Windows.Forms.CheckBox chkStartup;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.Label label3;
    }
}

