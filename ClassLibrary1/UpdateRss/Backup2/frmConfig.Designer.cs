namespace SoukeyNetget
{
    partial class frmConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfig));
            this.treeMenu = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.comUILanguage = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLogPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.IsAutoSystemLog = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.raExit = new System.Windows.Forms.RadioButton();
            this.raMin = new System.Windows.Forms.RadioButton();
            this.IsSave = new System.Windows.Forms.TextBox();
            this.cmdApply = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeMenu
            // 
            this.treeMenu.AccessibleDescription = null;
            this.treeMenu.AccessibleName = null;
            resources.ApplyResources(this.treeMenu, "treeMenu");
            this.treeMenu.BackgroundImage = null;
            this.treeMenu.Font = null;
            this.treeMenu.HideSelection = false;
            this.treeMenu.ImageList = this.imageList1;
            this.treeMenu.Name = "treeMenu";
            this.treeMenu.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeMenu.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeMenu.Nodes1")))});
            this.treeMenu.ShowRootLines = false;
            this.treeMenu.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMenu_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "a01");
            this.imageList1.Images.SetKeyName(1, "a02");
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.comUILanguage);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtLogPath);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.IsAutoSystemLog);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
            // 
            // label4
            // 
            this.label4.AccessibleDescription = null;
            this.label4.AccessibleName = null;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Font = null;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label4.Name = "label4";
            // 
            // comUILanguage
            // 
            this.comUILanguage.AccessibleDescription = null;
            this.comUILanguage.AccessibleName = null;
            resources.ApplyResources(this.comUILanguage, "comUILanguage");
            this.comUILanguage.BackgroundImage = null;
            this.comUILanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comUILanguage.Font = null;
            this.comUILanguage.FormattingEnabled = true;
            this.comUILanguage.Name = "comUILanguage";
            this.comUILanguage.SelectedIndexChanged += new System.EventHandler(this.comUILanguage_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label2.Name = "label2";
            // 
            // txtLogPath
            // 
            this.txtLogPath.AccessibleDescription = null;
            this.txtLogPath.AccessibleName = null;
            resources.ApplyResources(this.txtLogPath, "txtLogPath");
            this.txtLogPath.BackColor = System.Drawing.Color.White;
            this.txtLogPath.BackgroundImage = null;
            this.txtLogPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLogPath.Font = null;
            this.txtLogPath.Name = "txtLogPath";
            this.txtLogPath.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Name = "label1";
            // 
            // IsAutoSystemLog
            // 
            this.IsAutoSystemLog.AccessibleDescription = null;
            this.IsAutoSystemLog.AccessibleName = null;
            resources.ApplyResources(this.IsAutoSystemLog, "IsAutoSystemLog");
            this.IsAutoSystemLog.BackgroundImage = null;
            this.IsAutoSystemLog.Font = null;
            this.IsAutoSystemLog.Name = "IsAutoSystemLog";
            this.IsAutoSystemLog.UseVisualStyleBackColor = true;
            this.IsAutoSystemLog.CheckedChanged += new System.EventHandler(this.IsAutoSystemLog_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.checkBox1);
            this.panel2.Controls.Add(this.raExit);
            this.panel2.Controls.Add(this.raMin);
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // checkBox1
            // 
            this.checkBox1.AccessibleDescription = null;
            this.checkBox1.AccessibleName = null;
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.BackgroundImage = null;
            this.checkBox1.Font = null;
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // raExit
            // 
            this.raExit.AccessibleDescription = null;
            this.raExit.AccessibleName = null;
            resources.ApplyResources(this.raExit, "raExit");
            this.raExit.BackgroundImage = null;
            this.raExit.Font = null;
            this.raExit.Name = "raExit";
            this.raExit.UseVisualStyleBackColor = true;
            this.raExit.CheckedChanged += new System.EventHandler(this.raExit_CheckedChanged);
            // 
            // raMin
            // 
            this.raMin.AccessibleDescription = null;
            this.raMin.AccessibleName = null;
            resources.ApplyResources(this.raMin, "raMin");
            this.raMin.BackgroundImage = null;
            this.raMin.Checked = true;
            this.raMin.Font = null;
            this.raMin.Name = "raMin";
            this.raMin.TabStop = true;
            this.raMin.UseVisualStyleBackColor = true;
            this.raMin.CheckedChanged += new System.EventHandler(this.raMin_CheckedChanged);
            // 
            // IsSave
            // 
            this.IsSave.AccessibleDescription = null;
            this.IsSave.AccessibleName = null;
            resources.ApplyResources(this.IsSave, "IsSave");
            this.IsSave.BackgroundImage = null;
            this.IsSave.Font = null;
            this.IsSave.Name = "IsSave";
            this.IsSave.TextChanged += new System.EventHandler(this.IsSave_TextChanged);
            // 
            // cmdApply
            // 
            this.cmdApply.AccessibleDescription = null;
            this.cmdApply.AccessibleName = null;
            resources.ApplyResources(this.cmdApply, "cmdApply");
            this.cmdApply.BackgroundImage = null;
            this.cmdApply.Font = null;
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.AccessibleDescription = null;
            this.cmdCancel.AccessibleName = null;
            resources.ApplyResources(this.cmdCancel, "cmdCancel");
            this.cmdCancel.BackgroundImage = null;
            this.cmdCancel.Font = null;
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.AccessibleDescription = null;
            this.cmdOK.AccessibleName = null;
            resources.ApplyResources(this.cmdOK, "cmdOK");
            this.cmdOK.BackgroundImage = null;
            this.cmdOK.Font = null;
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // frmConfig
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.IsSave);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.treeMenu);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfig";
            this.Load += new System.EventHandler(this.frmConfig_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmConfig_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeMenu;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox IsAutoSystemLog;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.RadioButton raExit;
        private System.Windows.Forms.RadioButton raMin;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLogPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox IsSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comUILanguage;
        private System.Windows.Forms.Label label4;
    }
}