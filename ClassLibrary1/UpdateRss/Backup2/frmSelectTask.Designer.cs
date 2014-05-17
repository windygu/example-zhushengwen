namespace SoukeyNetget
{
    partial class frmSelectTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectTask));
            this.listTask = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comTaskClass = new System.Windows.Forms.ComboBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // listTask
            // 
            this.listTask.AccessibleDescription = null;
            this.listTask.AccessibleName = null;
            resources.ApplyResources(this.listTask, "listTask");
            this.listTask.BackgroundImage = null;
            this.listTask.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.errorProvider1.SetError(this.listTask, resources.GetString("listTask.Error"));
            this.listTask.Font = null;
            this.listTask.FullRowSelect = true;
            this.errorProvider1.SetIconAlignment(this.listTask, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("listTask.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.listTask, ((int)(resources.GetObject("listTask.IconPadding"))));
            this.listTask.Name = "listTask";
            this.listTask.UseCompatibleStateImageBehavior = false;
            this.listTask.View = System.Windows.Forms.View.Details;
            this.listTask.DoubleClick += new System.EventHandler(this.listTask_DoubleClick);
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
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.errorProvider1.SetError(this.groupBox1, resources.GetString("groupBox1.Error"));
            this.groupBox1.Font = null;
            this.errorProvider1.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding"))));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.errorProvider1.SetError(this.label1, resources.GetString("label1.Error"));
            this.label1.Font = null;
            this.errorProvider1.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            // 
            // comTaskClass
            // 
            this.comTaskClass.AccessibleDescription = null;
            this.comTaskClass.AccessibleName = null;
            resources.ApplyResources(this.comTaskClass, "comTaskClass");
            this.comTaskClass.BackgroundImage = null;
            this.comTaskClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider1.SetError(this.comTaskClass, resources.GetString("comTaskClass.Error"));
            this.comTaskClass.Font = null;
            this.comTaskClass.FormattingEnabled = true;
            this.errorProvider1.SetIconAlignment(this.comTaskClass, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("comTaskClass.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.comTaskClass, ((int)(resources.GetObject("comTaskClass.IconPadding"))));
            this.comTaskClass.Name = "comTaskClass";
            this.comTaskClass.SelectedIndexChanged += new System.EventHandler(this.comTaskClass_SelectedIndexChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            resources.ApplyResources(this.errorProvider1, "errorProvider1");
            // 
            // cmdCancel
            // 
            this.cmdCancel.AccessibleDescription = null;
            this.cmdCancel.AccessibleName = null;
            resources.ApplyResources(this.cmdCancel, "cmdCancel");
            this.cmdCancel.BackgroundImage = null;
            this.errorProvider1.SetError(this.cmdCancel, resources.GetString("cmdCancel.Error"));
            this.cmdCancel.Font = null;
            this.errorProvider1.SetIconAlignment(this.cmdCancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cmdCancel.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.cmdCancel, ((int)(resources.GetObject("cmdCancel.IconPadding"))));
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
            this.errorProvider1.SetError(this.cmdOK, resources.GetString("cmdOK.Error"));
            this.cmdOK.Font = null;
            this.errorProvider1.SetIconAlignment(this.cmdOK, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cmdOK.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.cmdOK, ((int)(resources.GetObject("cmdOK.IconPadding"))));
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // frmSelectTask
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.listTask);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comTaskClass);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectTask";
            this.Load += new System.EventHandler(this.frmSelectTask_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSelectTask_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listTask;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comTaskClass;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}