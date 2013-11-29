namespace JrIntercepter
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.lvSessions = new System.Windows.Forms.ListView();
            this.pLeftTop = new System.Windows.Forms.Panel();
            this.btnClearSessions = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.pLeftTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 516);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(475, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.Black;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(41, 17);
            this.toolStripStatusLabel1.Text = "状态栏";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(736, 472);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 32);
            this.button1.TabIndex = 3;
            this.button1.Text = "开始";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lvSessions
            // 
            this.lvSessions.FullRowSelect = true;
            this.lvSessions.Location = new System.Drawing.Point(42, 43);
            this.lvSessions.Name = "lvSessions";
            this.lvSessions.Size = new System.Drawing.Size(208, 220);
            this.lvSessions.TabIndex = 0;
            this.lvSessions.UseCompatibleStateImageBehavior = false;
            this.lvSessions.View = System.Windows.Forms.View.Details;
            this.lvSessions.SelectedIndexChanged += new System.EventHandler(this.lvSessions_SelectedIndexChanged);
            // 
            // pLeftTop
            // 
            this.pLeftTop.Controls.Add(this.btnClearSessions);
            this.pLeftTop.Location = new System.Drawing.Point(42, 12);
            this.pLeftTop.Name = "pLeftTop";
            this.pLeftTop.Size = new System.Drawing.Size(200, 25);
            this.pLeftTop.TabIndex = 4;
            // 
            // btnClearSessions
            // 
            this.btnClearSessions.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnClearSessions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearSessions.Location = new System.Drawing.Point(0, 0);
            this.btnClearSessions.Name = "btnClearSessions";
            this.btnClearSessions.Size = new System.Drawing.Size(25, 25);
            this.btnClearSessions.TabIndex = 0;
            this.btnClearSessions.Tag = "";
            this.btnClearSessions.Text = "C";
            this.btnClearSessions.UseVisualStyleBackColor = true;
            this.btnClearSessions.Click += new System.EventHandler(this.btnClearSessions_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 538);
            this.Controls.Add(this.pLeftTop);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lvSessions);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "主界面";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pLeftTop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel pLeftTop;
        private System.Windows.Forms.Button btnClearSessions;
        private System.Windows.Forms.ListView lvSessions;




    }
}

