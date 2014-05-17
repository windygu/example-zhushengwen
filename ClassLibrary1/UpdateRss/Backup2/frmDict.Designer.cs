namespace SoukeyNetget
{
    partial class frmDict
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDict));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeDict = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddDictClass = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddDict = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDelDictClass = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelDict = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listDict = new System.Windows.Forms.ListView();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolAddDictClass = new System.Windows.Forms.ToolStripMenuItem();
            this.toolAddDict = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.AccessibleDescription = null;
            this.splitContainer1.AccessibleName = null;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.BackgroundImage = null;
            this.errorProvider1.SetError(this.splitContainer1, resources.GetString("splitContainer1.Error"));
            this.splitContainer1.Font = null;
            this.errorProvider1.SetIconAlignment(this.splitContainer1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("splitContainer1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.splitContainer1, ((int)(resources.GetObject("splitContainer1.IconPadding"))));
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AccessibleDescription = null;
            this.splitContainer1.Panel1.AccessibleName = null;
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.BackgroundImage = null;
            this.splitContainer1.Panel1.Controls.Add(this.treeDict);
            this.errorProvider1.SetError(this.splitContainer1.Panel1, resources.GetString("splitContainer1.Panel1.Error"));
            this.splitContainer1.Panel1.Font = null;
            this.errorProvider1.SetIconAlignment(this.splitContainer1.Panel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("splitContainer1.Panel1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.splitContainer1.Panel1, ((int)(resources.GetObject("splitContainer1.Panel1.IconPadding"))));
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AccessibleDescription = null;
            this.splitContainer1.Panel2.AccessibleName = null;
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.BackgroundImage = null;
            this.splitContainer1.Panel2.Controls.Add(this.listDict);
            this.errorProvider1.SetError(this.splitContainer1.Panel2, resources.GetString("splitContainer1.Panel2.Error"));
            this.splitContainer1.Panel2.Font = null;
            this.errorProvider1.SetIconAlignment(this.splitContainer1.Panel2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("splitContainer1.Panel2.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.splitContainer1.Panel2, ((int)(resources.GetObject("splitContainer1.Panel2.IconPadding"))));
            // 
            // treeDict
            // 
            this.treeDict.AccessibleDescription = null;
            this.treeDict.AccessibleName = null;
            resources.ApplyResources(this.treeDict, "treeDict");
            this.treeDict.BackgroundImage = null;
            this.treeDict.ContextMenuStrip = this.contextMenuStrip1;
            this.errorProvider1.SetError(this.treeDict, resources.GetString("treeDict.Error"));
            this.treeDict.Font = null;
            this.treeDict.HideSelection = false;
            this.errorProvider1.SetIconAlignment(this.treeDict, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("treeDict.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.treeDict, ((int)(resources.GetObject("treeDict.IconPadding"))));
            this.treeDict.ImageList = this.imageList1;
            this.treeDict.Name = "treeDict";
            this.treeDict.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeDict.Nodes")))});
            this.treeDict.ShowRootLines = false;
            this.treeDict.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeDict_AfterLabelEdit);
            this.treeDict.Enter += new System.EventHandler(this.treeDict_Enter);
            this.treeDict.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeDict_AfterSelect);
            this.treeDict.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeDict_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.AccessibleDescription = null;
            this.contextMenuStrip1.AccessibleName = null;
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.BackgroundImage = null;
            this.errorProvider1.SetError(this.contextMenuStrip1, resources.GetString("contextMenuStrip1.Error"));
            this.contextMenuStrip1.Font = null;
            this.errorProvider1.SetIconAlignment(this.contextMenuStrip1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("contextMenuStrip1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.contextMenuStrip1, ((int)(resources.GetObject("contextMenuStrip1.IconPadding"))));
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddDictClass,
            this.menuAddDict,
            this.toolStripSeparator1,
            this.menuDelDictClass,
            this.menuDelDict});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            // 
            // menuAddDictClass
            // 
            this.menuAddDictClass.AccessibleDescription = null;
            this.menuAddDictClass.AccessibleName = null;
            resources.ApplyResources(this.menuAddDictClass, "menuAddDictClass");
            this.menuAddDictClass.BackgroundImage = null;
            this.menuAddDictClass.Name = "menuAddDictClass";
            this.menuAddDictClass.ShortcutKeyDisplayString = null;
            this.menuAddDictClass.Click += new System.EventHandler(this.menuAddDictClass_Click);
            // 
            // menuAddDict
            // 
            this.menuAddDict.AccessibleDescription = null;
            this.menuAddDict.AccessibleName = null;
            resources.ApplyResources(this.menuAddDict, "menuAddDict");
            this.menuAddDict.BackgroundImage = null;
            this.menuAddDict.Name = "menuAddDict";
            this.menuAddDict.ShortcutKeyDisplayString = null;
            this.menuAddDict.Click += new System.EventHandler(this.menuAddDict_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AccessibleDescription = null;
            this.toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // menuDelDictClass
            // 
            this.menuDelDictClass.AccessibleDescription = null;
            this.menuDelDictClass.AccessibleName = null;
            resources.ApplyResources(this.menuDelDictClass, "menuDelDictClass");
            this.menuDelDictClass.BackgroundImage = null;
            this.menuDelDictClass.Name = "menuDelDictClass";
            this.menuDelDictClass.ShortcutKeyDisplayString = null;
            this.menuDelDictClass.Click += new System.EventHandler(this.menuDelDictClass_Click);
            // 
            // menuDelDict
            // 
            this.menuDelDict.AccessibleDescription = null;
            this.menuDelDict.AccessibleName = null;
            resources.ApplyResources(this.menuDelDict, "menuDelDict");
            this.menuDelDict.BackgroundImage = null;
            this.menuDelDict.Name = "menuDelDict";
            this.menuDelDict.ShortcutKeyDisplayString = null;
            this.menuDelDict.Click += new System.EventHandler(this.menuDelDict_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "tree.gif");
            this.imageList1.Images.SetKeyName(1, "Cur.ico");
            // 
            // listDict
            // 
            this.listDict.AccessibleDescription = null;
            this.listDict.AccessibleName = null;
            resources.ApplyResources(this.listDict, "listDict");
            this.listDict.BackgroundImage = null;
            this.listDict.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listDict.ContextMenuStrip = this.contextMenuStrip1;
            this.errorProvider1.SetError(this.listDict, resources.GetString("listDict.Error"));
            this.listDict.Font = null;
            this.listDict.HideSelection = false;
            this.errorProvider1.SetIconAlignment(this.listDict, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("listDict.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.listDict, ((int)(resources.GetObject("listDict.IconPadding"))));
            this.listDict.MultiSelect = false;
            this.listDict.Name = "listDict";
            this.listDict.UseCompatibleStateImageBehavior = false;
            this.listDict.View = System.Windows.Forms.View.List;
            this.listDict.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listDict_AfterLabelEdit);
            this.listDict.Enter += new System.EventHandler(this.listDict_Enter);
            this.listDict.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listDict_KeyDown);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            resources.ApplyResources(this.errorProvider1, "errorProvider1");
            // 
            // toolStrip1
            // 
            this.toolStrip1.AccessibleDescription = null;
            this.toolStrip1.AccessibleName = null;
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.BackgroundImage = null;
            this.errorProvider1.SetError(this.toolStrip1, resources.GetString("toolStrip1.Error"));
            this.toolStrip1.Font = null;
            this.errorProvider1.SetIconAlignment(this.toolStrip1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("toolStrip1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.toolStrip1, ((int)(resources.GetObject("toolStrip1.IconPadding"))));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripSeparator2,
            this.toolStripButton3});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.AccessibleDescription = null;
            this.toolStripButton1.AccessibleName = null;
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            this.toolStripButton1.BackgroundImage = null;
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAddDictClass,
            this.toolAddDict});
            this.toolStripButton1.Name = "toolStripButton1";
            // 
            // toolAddDictClass
            // 
            this.toolAddDictClass.AccessibleDescription = null;
            this.toolAddDictClass.AccessibleName = null;
            resources.ApplyResources(this.toolAddDictClass, "toolAddDictClass");
            this.toolAddDictClass.BackgroundImage = null;
            this.toolAddDictClass.Name = "toolAddDictClass";
            this.toolAddDictClass.ShortcutKeyDisplayString = null;
            this.toolAddDictClass.Click += new System.EventHandler(this.toolAddDictClass_Click);
            // 
            // toolAddDict
            // 
            this.toolAddDict.AccessibleDescription = null;
            this.toolAddDict.AccessibleName = null;
            resources.ApplyResources(this.toolAddDict, "toolAddDict");
            this.toolAddDict.BackgroundImage = null;
            this.toolAddDict.Name = "toolAddDict";
            this.toolAddDict.ShortcutKeyDisplayString = null;
            this.toolAddDict.Click += new System.EventHandler(this.toolAddDict_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.AccessibleDescription = null;
            this.toolStripButton2.AccessibleName = null;
            resources.ApplyResources(this.toolStripButton2, "toolStripButton2");
            this.toolStripButton2.BackgroundImage = null;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AccessibleDescription = null;
            this.toolStripSeparator2.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.AccessibleDescription = null;
            this.toolStripButton3.AccessibleName = null;
            resources.ApplyResources(this.toolStripButton3, "toolStripButton3");
            this.toolStripButton3.BackgroundImage = null;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.AccessibleDescription = null;
            this.statusStrip1.AccessibleName = null;
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.BackgroundImage = null;
            this.errorProvider1.SetError(this.statusStrip1, resources.GetString("statusStrip1.Error"));
            this.statusStrip1.Font = null;
            this.errorProvider1.SetIconAlignment(this.statusStrip1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("statusStrip1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.statusStrip1, ((int)(resources.GetObject("statusStrip1.IconPadding"))));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AccessibleDescription = null;
            this.toolStripStatusLabel1.AccessibleName = null;
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.BackgroundImage = null;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // frmDict
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Font = null;
            this.MinimizeBox = false;
            this.Name = "frmDict";
            this.Load += new System.EventHandler(this.frmDict_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDict_FormClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private global::System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private global::System.Windows.Forms.ToolStripMenuItem menuDelDictClass;
        private global::System.Windows.Forms.ToolStripMenuItem menuDelDict;
        private global::System.Windows.Forms.ToolStripMenuItem menuAddDict;
        private global::System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeDict;
        private System.Windows.Forms.ListView listDict;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem toolAddDictClass;
        private System.Windows.Forms.ToolStripMenuItem toolAddDict;
        private System.Windows.Forms.ToolStripMenuItem menuAddDictClass;
    }
}