namespace SoukeyNetget
{
    partial class frmAddPlanTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddPlanTask));
            this.raSoukeyTask = new System.Windows.Forms.RadioButton();
            this.raOtherTask = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listTask = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.comTaskClass = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comTableName = new System.Windows.Forms.ComboBox();
            this.button12 = new System.Windows.Forms.Button();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.raMySqlTask = new System.Windows.Forms.RadioButton();
            this.raMSSQLTask = new System.Windows.Forms.RadioButton();
            this.raAccessTask = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtPara = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.raDataTask = new System.Windows.Forms.RadioButton();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // raSoukeyTask
            // 
            this.raSoukeyTask.AccessibleDescription = null;
            this.raSoukeyTask.AccessibleName = null;
            resources.ApplyResources(this.raSoukeyTask, "raSoukeyTask");
            this.raSoukeyTask.BackgroundImage = null;
            this.raSoukeyTask.Checked = true;
            this.raSoukeyTask.Font = null;
            this.raSoukeyTask.Name = "raSoukeyTask";
            this.raSoukeyTask.TabStop = true;
            this.raSoukeyTask.UseVisualStyleBackColor = true;
            this.raSoukeyTask.CheckedChanged += new System.EventHandler(this.raSoukeyTask_CheckedChanged);
            // 
            // raOtherTask
            // 
            this.raOtherTask.AccessibleDescription = null;
            this.raOtherTask.AccessibleName = null;
            resources.ApplyResources(this.raOtherTask, "raOtherTask");
            this.raOtherTask.BackgroundImage = null;
            this.raOtherTask.Font = null;
            this.raOtherTask.Name = "raOtherTask";
            this.raOtherTask.UseVisualStyleBackColor = true;
            this.raOtherTask.CheckedChanged += new System.EventHandler(this.raOtherTask_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.AccessibleDescription = null;
            this.panel1.AccessibleName = null;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BackgroundImage = null;
            this.panel1.Controls.Add(this.listTask);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comTaskClass);
            this.panel1.Font = null;
            this.panel1.Name = "panel1";
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
            this.listTask.Font = null;
            this.listTask.FullRowSelect = true;
            this.listTask.MultiSelect = false;
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
            // label1
            // 
            this.label1.AccessibleDescription = null;
            this.label1.AccessibleName = null;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Font = null;
            this.label1.Name = "label1";
            // 
            // comTaskClass
            // 
            this.comTaskClass.AccessibleDescription = null;
            this.comTaskClass.AccessibleName = null;
            resources.ApplyResources(this.comTaskClass, "comTaskClass");
            this.comTaskClass.BackgroundImage = null;
            this.comTaskClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comTaskClass.Font = null;
            this.comTaskClass.FormattingEnabled = true;
            this.comTaskClass.Name = "comTaskClass";
            this.comTaskClass.SelectedIndexChanged += new System.EventHandler(this.comTaskClass_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = null;
            this.groupBox1.AccessibleName = null;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.BackgroundImage = null;
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Font = null;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.AccessibleDescription = null;
            this.panel3.AccessibleName = null;
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.BackgroundImage = null;
            this.panel3.Controls.Add(this.comTableName);
            this.panel3.Controls.Add(this.button12);
            this.panel3.Controls.Add(this.txtDataSource);
            this.panel3.Controls.Add(this.raMySqlTask);
            this.panel3.Controls.Add(this.raMSSQLTask);
            this.panel3.Controls.Add(this.raAccessTask);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Font = null;
            this.panel3.Name = "panel3";
            // 
            // comTableName
            // 
            this.comTableName.AccessibleDescription = null;
            this.comTableName.AccessibleName = null;
            resources.ApplyResources(this.comTableName, "comTableName");
            this.comTableName.BackgroundImage = null;
            this.comTableName.Font = null;
            this.comTableName.FormattingEnabled = true;
            this.comTableName.Name = "comTableName";
            this.comTableName.DropDown += new System.EventHandler(this.comTableName_DropDown);
            // 
            // button12
            // 
            this.button12.AccessibleDescription = null;
            this.button12.AccessibleName = null;
            resources.ApplyResources(this.button12, "button12");
            this.button12.BackgroundImage = null;
            this.button12.Font = null;
            this.button12.Name = "button12";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // txtDataSource
            // 
            this.txtDataSource.AccessibleDescription = null;
            this.txtDataSource.AccessibleName = null;
            resources.ApplyResources(this.txtDataSource, "txtDataSource");
            this.txtDataSource.BackgroundImage = null;
            this.txtDataSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDataSource.Font = null;
            this.txtDataSource.Name = "txtDataSource";
            // 
            // raMySqlTask
            // 
            this.raMySqlTask.AccessibleDescription = null;
            this.raMySqlTask.AccessibleName = null;
            resources.ApplyResources(this.raMySqlTask, "raMySqlTask");
            this.raMySqlTask.BackgroundImage = null;
            this.raMySqlTask.Font = null;
            this.raMySqlTask.Name = "raMySqlTask";
            this.raMySqlTask.UseVisualStyleBackColor = true;
            // 
            // raMSSQLTask
            // 
            this.raMSSQLTask.AccessibleDescription = null;
            this.raMSSQLTask.AccessibleName = null;
            resources.ApplyResources(this.raMSSQLTask, "raMSSQLTask");
            this.raMSSQLTask.BackgroundImage = null;
            this.raMSSQLTask.Font = null;
            this.raMSSQLTask.Name = "raMSSQLTask";
            this.raMSSQLTask.UseVisualStyleBackColor = true;
            // 
            // raAccessTask
            // 
            this.raAccessTask.AccessibleDescription = null;
            this.raAccessTask.AccessibleName = null;
            resources.ApplyResources(this.raAccessTask, "raAccessTask");
            this.raAccessTask.BackgroundImage = null;
            this.raAccessTask.Checked = true;
            this.raAccessTask.Font = null;
            this.raAccessTask.Name = "raAccessTask";
            this.raAccessTask.TabStop = true;
            this.raAccessTask.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AccessibleDescription = null;
            this.label8.AccessibleName = null;
            resources.ApplyResources(this.label8, "label8");
            this.label8.Font = null;
            this.label8.Name = "label8";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // panel2
            // 
            this.panel2.AccessibleDescription = null;
            this.panel2.AccessibleName = null;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.BackgroundImage = null;
            this.panel2.Controls.Add(this.txtPara);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.txtFileName);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Font = null;
            this.panel2.Name = "panel2";
            // 
            // txtPara
            // 
            this.txtPara.AccessibleDescription = null;
            this.txtPara.AccessibleName = null;
            resources.ApplyResources(this.txtPara, "txtPara");
            this.txtPara.BackgroundImage = null;
            this.txtPara.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPara.Font = null;
            this.txtPara.Name = "txtPara";
            // 
            // label3
            // 
            this.label3.AccessibleDescription = null;
            this.label3.AccessibleName = null;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Font = null;
            this.label3.Name = "label3";
            // 
            // button1
            // 
            this.button1.AccessibleDescription = null;
            this.button1.AccessibleName = null;
            resources.ApplyResources(this.button1, "button1");
            this.button1.BackgroundImage = null;
            this.button1.Font = null;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.AccessibleDescription = null;
            this.txtFileName.AccessibleName = null;
            resources.ApplyResources(this.txtFileName, "txtFileName");
            this.txtFileName.BackgroundImage = null;
            this.txtFileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFileName.Font = null;
            this.txtFileName.Name = "txtFileName";
            // 
            // label2
            // 
            this.label2.AccessibleDescription = null;
            this.label2.AccessibleName = null;
            resources.ApplyResources(this.label2, "label2");
            this.label2.Font = null;
            this.label2.Name = "label2";
            // 
            // groupBox2
            // 
            this.groupBox2.AccessibleDescription = null;
            this.groupBox2.AccessibleName = null;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.BackgroundImage = null;
            this.groupBox2.Font = null;
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // raDataTask
            // 
            this.raDataTask.AccessibleDescription = null;
            this.raDataTask.AccessibleName = null;
            resources.ApplyResources(this.raDataTask, "raDataTask");
            this.raDataTask.BackgroundImage = null;
            this.raDataTask.Font = null;
            this.raDataTask.Name = "raDataTask";
            this.raDataTask.TabStop = true;
            this.raDataTask.UseVisualStyleBackColor = true;
            this.raDataTask.CheckedChanged += new System.EventHandler(this.raDataTask_CheckedChanged);
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
            // frmAddPlanTask
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.raDataTask);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.raOtherTask);
            this.Controls.Add(this.raSoukeyTask);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddPlanTask";
            this.Load += new System.EventHandler(this.frmAddPlanTask_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmAddPlanTask_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton raSoukeyTask;
        private System.Windows.Forms.RadioButton raOtherTask;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView listTask;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comTaskClass;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPara;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RadioButton raDataTask;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox comTableName;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.RadioButton raMySqlTask;
        private System.Windows.Forms.RadioButton raMSSQLTask;
        private System.Windows.Forms.RadioButton raAccessTask;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
    }
}