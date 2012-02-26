namespace DailyWallpainter.UI
{
    partial class frmAddtnlOptions
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkResolutionLowerLimit = new System.Windows.Forms.CheckBox();
            this.txtResolutionLowerWidth = new System.Windows.Forms.TextBox();
            this.lblDescRL1 = new System.Windows.Forms.Label();
            this.txtResolutionLowerHeight = new System.Windows.Forms.TextBox();
            this.lblDescRL2 = new System.Windows.Forms.Label();
            this.lblMSDesc = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rdoMultiScreenStrectch = new System.Windows.Forms.RadioButton();
            this.rdoMultiScreenEach = new System.Windows.Forms.RadioButton();
            this.chkMultiScreenStretchCheckRatio = new System.Windows.Forms.CheckBox();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Location = new System.Drawing.Point(-2, 233);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(543, 100);
            this.panel2.TabIndex = 7;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(429, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(82, 24);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "닫기(&C)";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(-5, 232);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(546, 1);
            this.panel1.TabIndex = 6;
            // 
            // chkResolutionLowerLimit
            // 
            this.chkResolutionLowerLimit.AutoSize = true;
            this.chkResolutionLowerLimit.Checked = true;
            this.chkResolutionLowerLimit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkResolutionLowerLimit.Location = new System.Drawing.Point(18, 47);
            this.chkResolutionLowerLimit.Name = "chkResolutionLowerLimit";
            this.chkResolutionLowerLimit.Size = new System.Drawing.Size(220, 16);
            this.chkResolutionLowerLimit.TabIndex = 0;
            this.chkResolutionLowerLimit.Text = "저해상도 이미지 배경화면 설정 금지";
            this.chkResolutionLowerLimit.UseVisualStyleBackColor = true;
            this.chkResolutionLowerLimit.CheckedChanged += new System.EventHandler(this.chkResolutionLowerLimit_CheckedChanged);
            // 
            // txtResolutionLowerWidth
            // 
            this.txtResolutionLowerWidth.Location = new System.Drawing.Point(31, 69);
            this.txtResolutionLowerWidth.Name = "txtResolutionLowerWidth";
            this.txtResolutionLowerWidth.Size = new System.Drawing.Size(42, 21);
            this.txtResolutionLowerWidth.TabIndex = 1;
            this.txtResolutionLowerWidth.Leave += new System.EventHandler(this.txtResolutionLowerWidth_Leave);
            // 
            // lblDescRL1
            // 
            this.lblDescRL1.AutoSize = true;
            this.lblDescRL1.Location = new System.Drawing.Point(74, 74);
            this.lblDescRL1.Name = "lblDescRL1";
            this.lblDescRL1.Size = new System.Drawing.Size(12, 12);
            this.lblDescRL1.TabIndex = 2;
            this.lblDescRL1.Text = "x";
            // 
            // txtResolutionLowerHeight
            // 
            this.txtResolutionLowerHeight.Location = new System.Drawing.Point(86, 69);
            this.txtResolutionLowerHeight.Name = "txtResolutionLowerHeight";
            this.txtResolutionLowerHeight.Size = new System.Drawing.Size(42, 21);
            this.txtResolutionLowerHeight.TabIndex = 3;
            this.txtResolutionLowerHeight.Leave += new System.EventHandler(this.txtResolutionLowerHeight_Leave);
            // 
            // lblDescRL2
            // 
            this.lblDescRL2.AutoSize = true;
            this.lblDescRL2.Location = new System.Drawing.Point(130, 74);
            this.lblDescRL2.Name = "lblDescRL2";
            this.lblDescRL2.Size = new System.Drawing.Size(297, 12);
            this.lblDescRL2.TabIndex = 4;
            this.lblDescRL2.Text = "이하의 너비 또는 높이를 가지는 이미지를 금지합니다.";
            // 
            // lblMSDesc
            // 
            this.lblMSDesc.AutoSize = true;
            this.lblMSDesc.Location = new System.Drawing.Point(18, 0);
            this.lblMSDesc.Name = "lblMSDesc";
            this.lblMSDesc.Size = new System.Drawing.Size(93, 12);
            this.lblMSDesc.TabIndex = 0;
            this.lblMSDesc.Text = "멀티스크린 옵션";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.chkMultiScreenStretchCheckRatio);
            this.panel3.Controls.Add(this.rdoMultiScreenEach);
            this.panel3.Controls.Add(this.lblMSDesc);
            this.panel3.Controls.Add(this.rdoMultiScreenStrectch);
            this.panel3.Location = new System.Drawing.Point(-2, 124);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(543, 100);
            this.panel3.TabIndex = 5;
            // 
            // rdoMultiScreenStrectch
            // 
            this.rdoMultiScreenStrectch.AutoSize = true;
            this.rdoMultiScreenStrectch.Checked = true;
            this.rdoMultiScreenStrectch.Location = new System.Drawing.Point(33, 21);
            this.rdoMultiScreenStrectch.Name = "rdoMultiScreenStrectch";
            this.rdoMultiScreenStrectch.Size = new System.Drawing.Size(271, 16);
            this.rdoMultiScreenStrectch.TabIndex = 1;
            this.rdoMultiScreenStrectch.TabStop = true;
            this.rdoMultiScreenStrectch.Text = "모든 스크린에 배경화면이 걸치도록 늘립니다.";
            this.rdoMultiScreenStrectch.UseVisualStyleBackColor = true;
            this.rdoMultiScreenStrectch.CheckedChanged += new System.EventHandler(this.rdoMultiScreenStrectch_CheckedChanged);
            // 
            // rdoMultiScreenEach
            // 
            this.rdoMultiScreenEach.AutoSize = true;
            this.rdoMultiScreenEach.Location = new System.Drawing.Point(33, 67);
            this.rdoMultiScreenEach.Name = "rdoMultiScreenEach";
            this.rdoMultiScreenEach.Size = new System.Drawing.Size(275, 16);
            this.rdoMultiScreenEach.TabIndex = 3;
            this.rdoMultiScreenEach.TabStop = true;
            this.rdoMultiScreenEach.Text = "각 스크린에 배경화면이 각각 나오도록 합니다.";
            this.rdoMultiScreenEach.UseVisualStyleBackColor = true;
            // 
            // chkMultiScreenStretchCheckRatio
            // 
            this.chkMultiScreenStretchCheckRatio.AutoSize = true;
            this.chkMultiScreenStretchCheckRatio.Checked = true;
            this.chkMultiScreenStretchCheckRatio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMultiScreenStretchCheckRatio.Location = new System.Drawing.Point(46, 42);
            this.chkMultiScreenStretchCheckRatio.Name = "chkMultiScreenStretchCheckRatio";
            this.chkMultiScreenStretchCheckRatio.Size = new System.Drawing.Size(386, 16);
            this.chkMultiScreenStretchCheckRatio.TabIndex = 2;
            this.chkMultiScreenStretchCheckRatio.Text = "가로/세로 비율이 스크린 구성과 비슷한 배경화면만 걸치게 합니다.";
            this.chkMultiScreenStretchCheckRatio.UseVisualStyleBackColor = true;
            this.chkMultiScreenStretchCheckRatio.CheckedChanged += new System.EventHandler(this.chkMultiScreenStretchCheckRatio_CheckedChanged);
            // 
            // frmAddtnlOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(529, 280);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.lblDescRL2);
            this.Controls.Add(this.txtResolutionLowerHeight);
            this.Controls.Add(this.lblDescRL1);
            this.Controls.Add(this.txtResolutionLowerWidth);
            this.Controls.Add(this.chkResolutionLowerLimit);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddtnlOptions";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkResolutionLowerLimit;
        private System.Windows.Forms.TextBox txtResolutionLowerWidth;
        private System.Windows.Forms.Label lblDescRL1;
        private System.Windows.Forms.TextBox txtResolutionLowerHeight;
        private System.Windows.Forms.Label lblDescRL2;
        private System.Windows.Forms.Label lblMSDesc;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox chkMultiScreenStretchCheckRatio;
        private System.Windows.Forms.RadioButton rdoMultiScreenEach;
        private System.Windows.Forms.RadioButton rdoMultiScreenStrectch;
    }
}