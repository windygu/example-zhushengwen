namespace InputPhones
{
    partial class FrmInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmInformation));
            this.skinButtom1 = new CCWin.SkinControl.SkinButtom();
            this.skinLabel1 = new CCWin.SkinControl.SkinLabel();
            this.txtId = new CCWin.SkinControl.WaterTextBox();
            this.btnClose = new CCWin.SkinControl.SkinButtom();
            this.skinLabel2 = new CCWin.SkinControl.SkinLabel();
            this.skinButtom2 = new CCWin.SkinControl.SkinButtom();
            this.waterTextBox1 = new CCWin.SkinControl.WaterTextBox();
            this.SuspendLayout();
            // 
            // skinButtom1
            // 
            this.skinButtom1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.skinButtom1.BackColor = System.Drawing.Color.Transparent;
            this.skinButtom1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButtom1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.skinButtom1.DownBack = null;
            this.skinButtom1.DrawType = CCWin.SkinControl.DrawStyle.None;
            this.skinButtom1.Image = ((System.Drawing.Image)(resources.GetObject("skinButtom1.Image")));
            this.skinButtom1.Location = new System.Drawing.Point(23, 38);
            this.skinButtom1.Margin = new System.Windows.Forms.Padding(0);
            this.skinButtom1.MouseBack = null;
            this.skinButtom1.Name = "skinButtom1";
            this.skinButtom1.NormlBack = null;
            this.skinButtom1.Size = new System.Drawing.Size(16, 16);
            this.skinButtom1.TabIndex = 127;
            this.skinButtom1.UseVisualStyleBackColor = false;
            // 
            // skinLabel1
            // 
            this.skinLabel1.ArtTextStyle = CCWin.SkinControl.ArtTextStyle.None;
            this.skinLabel1.AutoSize = true;
            this.skinLabel1.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel1.BorderColor = System.Drawing.Color.White;
            this.skinLabel1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel1.Location = new System.Drawing.Point(44, 38);
            this.skinLabel1.Name = "skinLabel1";
            this.skinLabel1.Size = new System.Drawing.Size(80, 17);
            this.skinLabel1.TabIndex = 0;
            this.skinLabel1.Text = "连接字符串：";
            // 
            // txtId
            // 
            this.txtId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtId.Font = new System.Drawing.Font("微软雅黑", 11.75F);
            this.txtId.Location = new System.Drawing.Point(23, 59);
            this.txtId.Margin = new System.Windows.Forms.Padding(0);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(364, 21);
            this.txtId.TabIndex = 128;
            this.txtId.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.txtId.WaterText = "请在此输入连接字符串...";
            this.txtId.WordWrap = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnClose.DownBack = ((System.Drawing.Image)(resources.GetObject("btnClose.DownBack")));
            this.btnClose.DrawType = CCWin.SkinControl.DrawStyle.Img;
            this.btnClose.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClose.Location = new System.Drawing.Point(317, 136);
            this.btnClose.MouseBack = ((System.Drawing.Image)(resources.GetObject("btnClose.MouseBack")));
            this.btnClose.Name = "btnClose";
            this.btnClose.NormlBack = ((System.Drawing.Image)(resources.GetObject("btnClose.NormlBack")));
            this.btnClose.Size = new System.Drawing.Size(69, 24);
            this.btnClose.TabIndex = 129;
            this.btnClose.Text = "确定(&C)";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // skinLabel2
            // 
            this.skinLabel2.ArtTextStyle = CCWin.SkinControl.ArtTextStyle.None;
            this.skinLabel2.AutoSize = true;
            this.skinLabel2.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel2.BorderColor = System.Drawing.Color.White;
            this.skinLabel2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel2.Location = new System.Drawing.Point(44, 87);
            this.skinLabel2.Name = "skinLabel2";
            this.skinLabel2.Size = new System.Drawing.Size(66, 17);
            this.skinLabel2.TabIndex = 0;
            this.skinLabel2.Text = "RSS地址：";
            // 
            // skinButtom2
            // 
            this.skinButtom2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.skinButtom2.BackColor = System.Drawing.Color.Transparent;
            this.skinButtom2.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButtom2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.skinButtom2.DownBack = null;
            this.skinButtom2.DrawType = CCWin.SkinControl.DrawStyle.None;
            this.skinButtom2.Image = ((System.Drawing.Image)(resources.GetObject("skinButtom2.Image")));
            this.skinButtom2.Location = new System.Drawing.Point(23, 87);
            this.skinButtom2.Margin = new System.Windows.Forms.Padding(0);
            this.skinButtom2.MouseBack = null;
            this.skinButtom2.Name = "skinButtom2";
            this.skinButtom2.NormlBack = null;
            this.skinButtom2.Size = new System.Drawing.Size(16, 16);
            this.skinButtom2.TabIndex = 127;
            this.skinButtom2.UseVisualStyleBackColor = false;
            // 
            // waterTextBox1
            // 
            this.waterTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.waterTextBox1.Font = new System.Drawing.Font("微软雅黑", 11.75F);
            this.waterTextBox1.Location = new System.Drawing.Point(23, 108);
            this.waterTextBox1.Margin = new System.Windows.Forms.Padding(0);
            this.waterTextBox1.Name = "waterTextBox1";
            this.waterTextBox1.Size = new System.Drawing.Size(364, 21);
            this.waterTextBox1.TabIndex = 128;
            this.waterTextBox1.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.waterTextBox1.WaterText = "请在此输入新闻订阅地址...";
            this.waterTextBox1.WordWrap = false;
            // 
            // FrmInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Back = ((System.Drawing.Image)(resources.GetObject("$this.Back")));
            this.BackLayout = false;
            this.BorderPalace = ((System.Drawing.Image)(resources.GetObject("$this.BorderPalace")));
            this.CanResize = false;
            this.CaptionFont = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.ClientSize = new System.Drawing.Size(399, 169);
            this.CloseBoxSize = new System.Drawing.Size(39, 20);
            this.ControlBoxOffset = new System.Drawing.Point(0, -1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.waterTextBox1);
            this.Controls.Add(this.skinButtom2);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.skinLabel2);
            this.Controls.Add(this.skinButtom1);
            this.Controls.Add(this.skinLabel1);
            this.DropBack = false;
            this.EffectWidth = 2;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaxSize = new System.Drawing.Size(28, 20);
            this.MinimizeBox = false;
            this.MiniSize = new System.Drawing.Size(28, 20);
            this.Name = "FrmInformation";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "系统设置";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmInformation_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CCWin.SkinControl.SkinButtom skinButtom1;
        private CCWin.SkinControl.SkinLabel skinLabel1;
        private CCWin.SkinControl.WaterTextBox txtId;
        private CCWin.SkinControl.SkinButtom btnClose;
        private CCWin.SkinControl.SkinLabel skinLabel2;
        private CCWin.SkinControl.SkinButtom skinButtom2;
        private CCWin.SkinControl.WaterTextBox waterTextBox1;
    }
}