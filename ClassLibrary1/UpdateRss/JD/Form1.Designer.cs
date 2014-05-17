using CCWin;
using CCWin.SkinControl;
namespace InputPhones
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.skinButtom1 = new CCWin.SkinControl.SkinButtom();
            this.skinButtom2 = new CCWin.SkinControl.SkinButtom();
            this.webBrowser2 = new System.Windows.Forms.WebBrowser();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(6, 286);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(319, 281);
            this.webBrowser1.TabIndex = 111;
            this.webBrowser1.Url = new System.Uri("http://www.jikesms.com/user/login.htm", System.UriKind.Absolute);
            this.webBrowser1.Visible = false;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            this.webBrowser1.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser2_Navigated);
            // 
            // skinButtom1
            // 
            this.skinButtom1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.skinButtom1.BackColor = System.Drawing.Color.Transparent;
            this.skinButtom1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButtom1.DownBack = ((System.Drawing.Image)(resources.GetObject("skinButtom1.DownBack")));
            this.skinButtom1.DrawType = CCWin.SkinControl.DrawStyle.Img;
            this.skinButtom1.Enabled = false;
            this.skinButtom1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButtom1.Location = new System.Drawing.Point(514, 47);
            this.skinButtom1.MouseBack = ((System.Drawing.Image)(resources.GetObject("skinButtom1.MouseBack")));
            this.skinButtom1.Name = "skinButtom1";
            this.skinButtom1.NormlBack = ((System.Drawing.Image)(resources.GetObject("skinButtom1.NormlBack")));
            this.skinButtom1.Size = new System.Drawing.Size(110, 38);
            this.skinButtom1.TabIndex = 110;
            this.skinButtom1.Text = "登录";
            this.skinButtom1.UseVisualStyleBackColor = false;
            this.skinButtom1.Click += new System.EventHandler(this.skinButtom1_Click_1);
            // 
            // skinButtom2
            // 
            this.skinButtom2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.skinButtom2.BackColor = System.Drawing.Color.Transparent;
            this.skinButtom2.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButtom2.DownBack = ((System.Drawing.Image)(resources.GetObject("skinButtom2.DownBack")));
            this.skinButtom2.DrawType = CCWin.SkinControl.DrawStyle.Img;
            this.skinButtom2.Enabled = false;
            this.skinButtom2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButtom2.Location = new System.Drawing.Point(514, 109);
            this.skinButtom2.MouseBack = ((System.Drawing.Image)(resources.GetObject("skinButtom2.MouseBack")));
            this.skinButtom2.Name = "skinButtom2";
            this.skinButtom2.NormlBack = ((System.Drawing.Image)(resources.GetObject("skinButtom2.NormlBack")));
            this.skinButtom2.Size = new System.Drawing.Size(110, 38);
            this.skinButtom2.TabIndex = 110;
            this.skinButtom2.Text = "填写手机号";
            this.skinButtom2.UseVisualStyleBackColor = false;
            this.skinButtom2.Click += new System.EventHandler(this.skinButtom2_Click);
            // 
            // webBrowser2
            // 
            this.webBrowser2.Location = new System.Drawing.Point(-269, -48);
            this.webBrowser2.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser2.Name = "webBrowser2";
            this.webBrowser2.Size = new System.Drawing.Size(733, 1043);
            this.webBrowser2.TabIndex = 112;
            this.webBrowser2.Url = new System.Uri("http://safe.jd.com/findPwd/index.action", System.UriKind.Absolute);
            this.webBrowser2.WebBrowserShortcutsEnabled = false;
            this.webBrowser2.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser2_DocumentCompleted);
            this.webBrowser2.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser2_Navigated);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 3000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.webBrowser2);
            this.panel1.Location = new System.Drawing.Point(20, 45);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(476, 329);
            this.panel1.TabIndex = 113;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 391);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.skinButtom2);
            this.Controls.Add(this.skinButtom1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.SysBottomDown = global::InputPhones.Properties.Resources.btn_set_press;
            this.SysBottomMouse = global::InputPhones.Properties.Resources.btn_set_hover;
            this.SysBottomNorml = global::InputPhones.Properties.Resources.btn_set_normal;
            this.SysBottomToolTip = "设置";
            this.SysBottomVisibale = true;
            this.Text = "帐号密码找回换绑";
            this.SysBottomClick += new CCWin.CCSkinMain.SysBottomEventHandler(this.Form1_SysBottomClick);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private SkinButtom skinButtom1;
        private SkinButtom skinButtom2;
        private System.Windows.Forms.WebBrowser webBrowser2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Panel panel1;
    }
}

