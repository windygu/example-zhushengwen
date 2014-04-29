namespace CCWin

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmInformation));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.skinLabel1 = new CCWin.SkinControl.SkinLabel();
            this.skinLabel2 = new CCWin.SkinControl.SkinLabel();
            this.button1 = new CCWin.SkinControl.SkinButtom();
            this.skinLabel3 = new CCWin.SkinControl.SkinLabel();
            this.skinPanel1 = new CCWin.SkinControl.SkinPanel();
            this.skinGifBox1 = new CCWin.SkinControl.SkinGifBox();
            this.skinLabel4 = new CCWin.SkinControl.SkinLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.skinPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(80, 236);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(603, 301);
            this.webBrowser1.TabIndex = 128;
            this.webBrowser1.Url = new System.Uri("http://zqjp.iabe.cn/public/Default.aspx", System.UriKind.Absolute);
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // skinLabel1
            // 
            this.skinLabel1.AutoSize = true;
            this.skinLabel1.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel1.BorderColor = System.Drawing.Color.White;
            this.skinLabel1.Font = new System.Drawing.Font("宋体", 12F);
            this.skinLabel1.ForeColor = System.Drawing.Color.Maroon;
            this.skinLabel1.Location = new System.Drawing.Point(24, 38);
            this.skinLabel1.Name = "skinLabel1";
            this.skinLabel1.Size = new System.Drawing.Size(88, 16);
            this.skinLabel1.TabIndex = 129;
            this.skinLabel1.Text = "视频标题：";
            // 
            // skinLabel2
            // 
            this.skinLabel2.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel2.BorderColor = System.Drawing.Color.White;
            this.skinLabel2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.skinLabel2.Location = new System.Drawing.Point(112, 35);
            this.skinLabel2.Name = "skinLabel2";
            this.skinLabel2.Size = new System.Drawing.Size(323, 21);
            this.skinLabel2.TabIndex = 129;
            this.skinLabel2.Text = "标题";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.button1.DownBack = ((System.Drawing.Image)(resources.GetObject("button1.DownBack")));
            this.button1.DrawType = CCWin.SkinControl.DrawStyle.Img;
            this.button1.Enabled = false;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(716, 482);
            this.button1.MouseBack = ((System.Drawing.Image)(resources.GetObject("button1.MouseBack")));
            this.button1.Name = "button1";
            this.button1.NormlBack = ((System.Drawing.Image)(resources.GetObject("button1.NormlBack")));
            this.button1.Size = new System.Drawing.Size(109, 40);
            this.button1.TabIndex = 130;
            this.button1.Text = "立即观看";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // skinLabel3
            // 
            this.skinLabel3.AutoSize = true;
            this.skinLabel3.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel3.BorderColor = System.Drawing.Color.White;
            this.skinLabel3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel3.ForeColor = System.Drawing.Color.RoyalBlue;
            this.skinLabel3.Location = new System.Drawing.Point(37, 12);
            this.skinLabel3.Name = "skinLabel3";
            this.skinLabel3.Size = new System.Drawing.Size(134, 21);
            this.skinLabel3.TabIndex = 129;
            this.skinLabel3.Text = "视频正在加载中...";
            // 
            // skinPanel1
            // 
            this.skinPanel1.BackColor = System.Drawing.Color.Transparent;
            this.skinPanel1.Controls.Add(this.skinGifBox1);
            this.skinPanel1.Controls.Add(this.skinLabel3);
            this.skinPanel1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel1.DownBack = null;
            this.skinPanel1.Location = new System.Drawing.Point(27, 75);
            this.skinPanel1.MouseBack = null;
            this.skinPanel1.Name = "skinPanel1";
            this.skinPanel1.NormlBack = null;
            this.skinPanel1.Size = new System.Drawing.Size(200, 44);
            this.skinPanel1.TabIndex = 132;
            // 
            // skinGifBox1
            // 
            this.skinGifBox1.BorderColor = System.Drawing.Color.Transparent;
            this.skinGifBox1.Image = ((System.Drawing.Image)(resources.GetObject("skinGifBox1.Image")));
            this.skinGifBox1.Location = new System.Drawing.Point(15, 16);
            this.skinGifBox1.Name = "skinGifBox1";
            this.skinGifBox1.Size = new System.Drawing.Size(16, 16);
            this.skinGifBox1.TabIndex = 0;
            this.skinGifBox1.Text = "skinGifBox1";
            // 
            // skinLabel4
            // 
            this.skinLabel4.AutoSize = true;
            this.skinLabel4.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel4.BorderColor = System.Drawing.Color.White;
            this.skinLabel4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel4.ForeColor = System.Drawing.Color.Magenta;
            this.skinLabel4.Location = new System.Drawing.Point(29, 136);
            this.skinLabel4.Name = "skinLabel4";
            this.skinLabel4.Size = new System.Drawing.Size(312, 16);
            this.skinLabel4.TabIndex = 133;
            this.skinLabel4.Text = "如未安装快播播放器，请安装快播播放器！";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
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
            this.ClientSize = new System.Drawing.Size(855, 578);
            this.CloseBoxSize = new System.Drawing.Size(39, 20);
            this.ControlBoxOffset = new System.Drawing.Point(0, -1);
            this.Controls.Add(this.skinLabel4);
            this.Controls.Add(this.skinPanel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.skinLabel2);
            this.Controls.Add(this.skinLabel1);
            this.Controls.Add(this.webBrowser1);
            this.DropBack = false;
            this.EffectWidth = 2;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaxSize = new System.Drawing.Size(28, 20);
            this.MinimizeBox = false;
            this.MiniSize = new System.Drawing.Size(28, 20);
            this.Name = "FrmInformation";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "自动看视频";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmInformation_Load);
            this.skinPanel1.ResumeLayout(false);
            this.skinPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timShow;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private SkinControl.SkinLabel skinLabel1;
        private SkinControl.SkinLabel skinLabel2;
        private SkinControl.SkinButtom button1;
        private SkinControl.SkinLabel skinLabel3;
        private SkinControl.SkinPanel skinPanel1;
        private SkinControl.SkinGifBox skinGifBox1;
        private SkinControl.SkinLabel skinLabel4;
        private System.Windows.Forms.Timer timer1;
    }
}