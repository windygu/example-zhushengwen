namespace SoukeyNetget
{
    partial class frmUpgradeTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpgradeTask));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.IsAutoBackup = new System.Windows.Forms.CheckBox();
            this.listTask = new System.Windows.Forms.ListView();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.stalabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tmenuFindTask = new System.Windows.Forms.ToolStripButton();
            this.tmenuAddTask = new System.Windows.Forms.ToolStripButton();
            this.tmenuResetTask = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tmenuStart = new System.Windows.Forms.ToolStripButton();
            this.tmenuExit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.AccessibleDescription = null;
            this.splitContainer1.AccessibleName = null;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.BackgroundImage = null;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Font = null;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AccessibleDescription = null;
            this.splitContainer1.Panel1.AccessibleName = null;
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.BackgroundImage = null;
            this.splitContainer1.Panel1.Controls.Add(this.IsAutoBackup);
            this.splitContainer1.Panel1.Font = null;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AccessibleDescription = null;
            this.splitContainer1.Panel2.AccessibleName = null;
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.BackgroundImage = null;
            this.splitContainer1.Panel2.Controls.Add(this.listTask);
            this.splitContainer1.Panel2.Font = null;
            // 
            // IsAutoBackup
            // 
            this.IsAutoBackup.AccessibleDescription = null;
            this.IsAutoBackup.AccessibleName = null;
            resources.ApplyResources(this.IsAutoBackup, "IsAutoBackup");
            this.IsAutoBackup.BackgroundImage = null;
            this.IsAutoBackup.Checked = true;
            this.IsAutoBackup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.IsAutoBackup.Font = null;
            this.IsAutoBackup.Name = "IsAutoBackup";
            this.IsAutoBackup.UseVisualStyleBackColor = true;
            // 
            // listTask
            // 
            this.listTask.AccessibleDescription = null;
            this.listTask.AccessibleName = null;
            resources.ApplyResources(this.listTask, "listTask");
            this.listTask.BackgroundImage = null;
            this.listTask.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader6});
            this.listTask.Font = null;
            this.listTask.FullRowSelect = true;
            this.listTask.Name = "listTask";
            this.listTask.SmallImageList = this.imageList1;
            this.listTask.UseCompatibleStateImageBehavior = false;
            this.listTask.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader6
            // 
            resources.ApplyResources(this.columnHeader6, "columnHeader6");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "task");
            this.imageList1.Images.SetKeyName(1, "running");
            this.imageList1.Images.SetKeyName(2, "success");
            this.imageList1.Images.SetKeyName(3, "error");
            // 
            // statusStrip1
            // 
            this.statusStrip1.AccessibleDescription = null;
            this.statusStrip1.AccessibleName = null;
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.BackgroundImage = null;
            this.statusStrip1.Font = null;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stalabel,
            this.ProBar});
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // stalabel
            // 
            this.stalabel.AccessibleDescription = null;
            this.stalabel.AccessibleName = null;
            resources.ApplyResources(this.stalabel, "stalabel");
            this.stalabel.BackgroundImage = null;
            this.stalabel.Name = "stalabel";
            // 
            // ProBar
            // 
            this.ProBar.AccessibleDescription = null;
            this.ProBar.AccessibleName = null;
            resources.ApplyResources(this.ProBar, "ProBar");
            this.ProBar.Name = "ProBar";
            // 
            // toolStrip1
            // 
            this.toolStrip1.AccessibleDescription = null;
            this.toolStrip1.AccessibleName = null;
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.BackgroundImage = null;
            this.toolStrip1.Font = null;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmenuFindTask,
            this.tmenuAddTask,
            this.tmenuResetTask,
            this.toolStripSeparator1,
            this.tmenuStart,
            this.tmenuExit,
            this.toolStripSeparator2});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // tmenuFindTask
            // 
            this.tmenuFindTask.AccessibleDescription = null;
            this.tmenuFindTask.AccessibleName = null;
            resources.ApplyResources(this.tmenuFindTask, "tmenuFindTask");
            this.tmenuFindTask.BackgroundImage = null;
            this.tmenuFindTask.Name = "tmenuFindTask";
            this.tmenuFindTask.Click += new System.EventHandler(this.tmenuFindTask_Click);
            // 
            // tmenuAddTask
            // 
            this.tmenuAddTask.AccessibleDescription = null;
            this.tmenuAddTask.AccessibleName = null;
            resources.ApplyResources(this.tmenuAddTask, "tmenuAddTask");
            this.tmenuAddTask.BackgroundImage = null;
            this.tmenuAddTask.Name = "tmenuAddTask";
            this.tmenuAddTask.Click += new System.EventHandler(this.tmenuAddTask_Click);
            // 
            // tmenuResetTask
            // 
            this.tmenuResetTask.AccessibleDescription = null;
            this.tmenuResetTask.AccessibleName = null;
            resources.ApplyResources(this.tmenuResetTask, "tmenuResetTask");
            this.tmenuResetTask.BackgroundImage = null;
            this.tmenuResetTask.Name = "tmenuResetTask";
            this.tmenuResetTask.Click += new System.EventHandler(this.tmenuResetTask_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AccessibleDescription = null;
            this.toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // tmenuStart
            // 
            this.tmenuStart.AccessibleDescription = null;
            this.tmenuStart.AccessibleName = null;
            resources.ApplyResources(this.tmenuStart, "tmenuStart");
            this.tmenuStart.BackgroundImage = null;
            this.tmenuStart.Name = "tmenuStart";
            this.tmenuStart.Click += new System.EventHandler(this.tmenuStart_Click);
            // 
            // tmenuExit
            // 
            this.tmenuExit.AccessibleDescription = null;
            this.tmenuExit.AccessibleName = null;
            resources.ApplyResources(this.tmenuExit, "tmenuExit");
            this.tmenuExit.BackgroundImage = null;
            this.tmenuExit.Name = "tmenuExit";
            this.tmenuExit.Click += new System.EventHandler(this.tmenuExit_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AccessibleDescription = null;
            this.toolStripSeparator2.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // frmUpgradeTask
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Font = null;
            this.Name = "frmUpgradeTask";
            this.Load += new System.EventHandler(this.frmUpgradeTask_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmUpgradeTask_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listTask;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.CheckBox IsAutoBackup;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel stalabel;
        private System.Windows.Forms.ToolStripProgressBar ProBar;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tmenuStart;
        private System.Windows.Forms.ToolStripButton tmenuExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tmenuFindTask;
        private System.Windows.Forms.ToolStripButton tmenuAddTask;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton tmenuResetTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ImageList imageList1;
    }
}