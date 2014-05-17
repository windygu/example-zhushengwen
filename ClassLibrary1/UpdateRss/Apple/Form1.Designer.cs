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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.skinButtom1 = new CCWin.SkinControl.SkinButtom();
            this.skinButtom3 = new CCWin.SkinControl.SkinButtom();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
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
            this.skinButtom1.Location = new System.Drawing.Point(823, 176);
            this.skinButtom1.MouseBack = ((System.Drawing.Image)(resources.GetObject("skinButtom1.MouseBack")));
            this.skinButtom1.Name = "skinButtom1";
            this.skinButtom1.NormlBack = ((System.Drawing.Image)(resources.GetObject("skinButtom1.NormlBack")));
            this.skinButtom1.Size = new System.Drawing.Size(110, 38);
            this.skinButtom1.TabIndex = 110;
            this.skinButtom1.Text = "登录";
            this.skinButtom1.UseVisualStyleBackColor = false;
            this.skinButtom1.Click += new System.EventHandler(this.skinButtom1_Click_1);
            // 
            // skinButtom3
            // 
            this.skinButtom3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.skinButtom3.BackColor = System.Drawing.Color.Transparent;
            this.skinButtom3.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButtom3.DownBack = ((System.Drawing.Image)(resources.GetObject("skinButtom3.DownBack")));
            this.skinButtom3.DrawType = CCWin.SkinControl.DrawStyle.Img;
            this.skinButtom3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButtom3.Location = new System.Drawing.Point(822, 237);
            this.skinButtom3.MouseBack = ((System.Drawing.Image)(resources.GetObject("skinButtom3.MouseBack")));
            this.skinButtom3.Name = "skinButtom3";
            this.skinButtom3.NormlBack = ((System.Drawing.Image)(resources.GetObject("skinButtom3.NormlBack")));
            this.skinButtom3.Size = new System.Drawing.Size(110, 38);
            this.skinButtom3.TabIndex = 110;
            this.skinButtom3.Text = "重来";
            this.skinButtom3.UseVisualStyleBackColor = false;
            this.skinButtom3.Click += new System.EventHandler(this.skinButtom3_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Location = new System.Drawing.Point(9, 41);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(807, 615);
            this.webBrowser1.TabIndex = 112;
            this.webBrowser1.Url = new System.Uri(resources.GetString("webBrowser1.Url"), System.UriKind.Absolute);
            this.webBrowser1.WebBrowserShortcutsEnabled = false;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser2_DocumentCompleted);
            this.webBrowser1.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser2_Navigated);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 674);
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.skinButtom3);
            this.Controls.Add(this.skinButtom1);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.SysBottomDown = global::InputPhones.Properties.Resources.btn_set_press;
            this.SysBottomMouse = global::InputPhones.Properties.Resources.btn_set_hover;
            this.SysBottomNorml = global::InputPhones.Properties.Resources.btn_set_normal;
            this.SysBottomToolTip = "设置";
            this.SysBottomVisibale = true;
            this.Text = "iphone服务预约";
            this.SysBottomClick += new CCWin.CCSkinMain.SysBottomEventHandler(this.Form1_SysBottomClick);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private SkinButtom skinButtom1;
        private SkinButtom skinButtom3;
        private System.Windows.Forms.WebBrowser webBrowser1;
    }
}

