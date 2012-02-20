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
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Location = new System.Drawing.Point(-2, 412);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(489, 100);
            this.panel2.TabIndex = 5;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(375, 10);
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
            this.panel1.Location = new System.Drawing.Point(-5, 411);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(492, 1);
            this.panel1.TabIndex = 6;
            // 
            // chkResolutionLowerLimit
            // 
            this.chkResolutionLowerLimit.AutoSize = true;
            this.chkResolutionLowerLimit.Checked = true;
            this.chkResolutionLowerLimit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkResolutionLowerLimit.Location = new System.Drawing.Point(18, 51);
            this.chkResolutionLowerLimit.Name = "chkResolutionLowerLimit";
            this.chkResolutionLowerLimit.Size = new System.Drawing.Size(220, 16);
            this.chkResolutionLowerLimit.TabIndex = 0;
            this.chkResolutionLowerLimit.Text = "저해상도 이미지 배경화면 설정 금지";
            this.chkResolutionLowerLimit.UseVisualStyleBackColor = true;
            this.chkResolutionLowerLimit.CheckedChanged += new System.EventHandler(this.chkResolutionLowerLimit_CheckedChanged);
            // 
            // txtResolutionLowerWidth
            // 
            this.txtResolutionLowerWidth.Location = new System.Drawing.Point(31, 73);
            this.txtResolutionLowerWidth.Name = "txtResolutionLowerWidth";
            this.txtResolutionLowerWidth.Size = new System.Drawing.Size(42, 21);
            this.txtResolutionLowerWidth.TabIndex = 1;
            this.txtResolutionLowerWidth.Leave += new System.EventHandler(this.txtResolutionLowerWidth_Leave);
            // 
            // lblDescRL1
            // 
            this.lblDescRL1.AutoSize = true;
            this.lblDescRL1.Location = new System.Drawing.Point(74, 78);
            this.lblDescRL1.Name = "lblDescRL1";
            this.lblDescRL1.Size = new System.Drawing.Size(12, 12);
            this.lblDescRL1.TabIndex = 2;
            this.lblDescRL1.Text = "x";
            // 
            // txtResolutionLowerHeight
            // 
            this.txtResolutionLowerHeight.Location = new System.Drawing.Point(86, 73);
            this.txtResolutionLowerHeight.Name = "txtResolutionLowerHeight";
            this.txtResolutionLowerHeight.Size = new System.Drawing.Size(42, 21);
            this.txtResolutionLowerHeight.TabIndex = 3;
            this.txtResolutionLowerHeight.Leave += new System.EventHandler(this.txtResolutionLowerHeight_Leave);
            // 
            // lblDescRL2
            // 
            this.lblDescRL2.AutoSize = true;
            this.lblDescRL2.Location = new System.Drawing.Point(130, 78);
            this.lblDescRL2.Name = "lblDescRL2";
            this.lblDescRL2.Size = new System.Drawing.Size(297, 12);
            this.lblDescRL2.TabIndex = 4;
            this.lblDescRL2.Text = "이하의 너비 또는 높이를 가지는 이미지를 금지합니다.";
            // 
            // frmAddtnlOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(475, 459);
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
    }
}