namespace WebServiceTestForm
{
    partial class UserControl1
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.p_type = new System.Windows.Forms.Label();
            this.p_Name = new System.Windows.Forms.Label();
            this.p_value = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // p_type
            // 
            this.p_type.AutoSize = true;
            this.p_type.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.p_type.ForeColor = System.Drawing.Color.SteelBlue;
            this.p_type.Location = new System.Drawing.Point(1, -3);
            this.p_type.Name = "p_type";
            this.p_type.Size = new System.Drawing.Size(36, 17);
            this.p_type.TabIndex = 2;
            this.p_type.Text = "FUN:";
            // 
            // p_Name
            // 
            this.p_Name.AutoSize = true;
            this.p_Name.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.p_Name.ForeColor = System.Drawing.Color.DarkSlateGray;
            this.p_Name.Location = new System.Drawing.Point(48, -3);
            this.p_Name.Name = "p_Name";
            this.p_Name.Size = new System.Drawing.Size(36, 17);
            this.p_Name.TabIndex = 8;
            this.p_Name.Text = "FUN:";
            // 
            // p_value
            // 
            this.p_value.AcceptsReturn = true;
            this.p_value.Font = new System.Drawing.Font("宋体", 11F);
            this.p_value.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.p_value.Location = new System.Drawing.Point(3, 14);
            this.p_value.Name = "p_value";
            this.p_value.Size = new System.Drawing.Size(103, 24);
            this.p_value.TabIndex = 9;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.p_value);
            this.Controls.Add(this.p_Name);
            this.Controls.Add(this.p_type);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(111, 41);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label p_type;
        public System.Windows.Forms.Label p_Name;
        public System.Windows.Forms.TextBox p_value;
    }
}
