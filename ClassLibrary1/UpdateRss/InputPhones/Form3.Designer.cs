namespace InputPhones
{
    partial class Form3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.label10 = new System.Windows.Forms.Label();
            this.skinComboBox2 = new CCWin.SkinControl.SkinComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.skinPanel1 = new CCWin.SkinControl.SkinPanel();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.skinButtom1 = new CCWin.SkinControl.SkinButtom();
            this.skinButtom2 = new CCWin.SkinControl.SkinButtom();
            this.skinComboBox1 = new CCWin.SkinControl.SkinComboBox();
            this.skinPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.label10.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label10.Location = new System.Drawing.Point(18, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 21);
            this.label10.TabIndex = 4;
            this.label10.Text = "类型:";
            // 
            // skinComboBox2
            // 
            this.skinComboBox2.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.skinComboBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.skinComboBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.skinComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.skinComboBox2.FormattingEnabled = true;
            this.skinComboBox2.Location = new System.Drawing.Point(210, 33);
            this.skinComboBox2.Name = "skinComboBox2";
            this.skinComboBox2.Size = new System.Drawing.Size(631, 24);
            this.skinComboBox2.TabIndex = 1;
            this.skinComboBox2.WaterText = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label1.Location = new System.Drawing.Point(162, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "标题:";
            // 
            // skinPanel1
            // 
            this.skinPanel1.BackColor = System.Drawing.Color.Transparent;
            this.skinPanel1.Controls.Add(this.webBrowser1);
            this.skinPanel1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel1.DownBack = null;
            this.skinPanel1.Location = new System.Drawing.Point(20, 67);
            this.skinPanel1.MouseBack = null;
            this.skinPanel1.Name = "skinPanel1";
            this.skinPanel1.NormlBack = null;
            this.skinPanel1.Size = new System.Drawing.Size(821, 392);
            this.skinPanel1.TabIndex = 5;
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(821, 392);
            this.webBrowser1.TabIndex = 0;
            // 
            // skinButtom1
            // 
            this.skinButtom1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.skinButtom1.BackColor = System.Drawing.Color.Transparent;
            this.skinButtom1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButtom1.DownBack = ((System.Drawing.Image)(resources.GetObject("skinButtom1.DownBack")));
            this.skinButtom1.DrawType = CCWin.SkinControl.DrawStyle.Img;
            this.skinButtom1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButtom1.Location = new System.Drawing.Point(331, 466);
            this.skinButtom1.MouseBack = ((System.Drawing.Image)(resources.GetObject("skinButtom1.MouseBack")));
            this.skinButtom1.Name = "skinButtom1";
            this.skinButtom1.NormlBack = ((System.Drawing.Image)(resources.GetObject("skinButtom1.NormlBack")));
            this.skinButtom1.Size = new System.Drawing.Size(81, 24);
            this.skinButtom1.TabIndex = 130;
            this.skinButtom1.Text = "上一条(&P)";
            this.skinButtom1.UseVisualStyleBackColor = false;
            this.skinButtom1.Click += new System.EventHandler(this.skinButtom1_Click);
            // 
            // skinButtom2
            // 
            this.skinButtom2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.skinButtom2.BackColor = System.Drawing.Color.Transparent;
            this.skinButtom2.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButtom2.DownBack = ((System.Drawing.Image)(resources.GetObject("skinButtom2.DownBack")));
            this.skinButtom2.DrawType = CCWin.SkinControl.DrawStyle.Img;
            this.skinButtom2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinButtom2.Location = new System.Drawing.Point(418, 465);
            this.skinButtom2.MouseBack = ((System.Drawing.Image)(resources.GetObject("skinButtom2.MouseBack")));
            this.skinButtom2.Name = "skinButtom2";
            this.skinButtom2.NormlBack = ((System.Drawing.Image)(resources.GetObject("skinButtom2.NormlBack")));
            this.skinButtom2.Size = new System.Drawing.Size(81, 24);
            this.skinButtom2.TabIndex = 130;
            this.skinButtom2.Text = "下一条(&N)";
            this.skinButtom2.UseVisualStyleBackColor = false;
            this.skinButtom2.Click += new System.EventHandler(this.skinButtom2_Click);
            // 
            // skinComboBox1
            // 
            this.skinComboBox1.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.skinComboBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.skinComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.skinComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.skinComboBox1.FormattingEnabled = true;
            this.skinComboBox1.Location = new System.Drawing.Point(64, 33);
            this.skinComboBox1.Name = "skinComboBox1";
            this.skinComboBox1.Size = new System.Drawing.Size(89, 24);
            this.skinComboBox1.TabIndex = 1;
            this.skinComboBox1.WaterText = "";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(862, 496);
            this.Controls.Add(this.skinButtom2);
            this.Controls.Add(this.skinButtom1);
            this.Controls.Add(this.skinPanel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.skinComboBox1);
            this.Controls.Add(this.skinComboBox2);
            this.Controls.Add(this.label10);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Form3";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "查看详情";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.skinPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label10;
        private CCWin.SkinControl.SkinComboBox skinComboBox2;
        private System.Windows.Forms.Label label1;
        private CCWin.SkinControl.SkinPanel skinPanel1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private CCWin.SkinControl.SkinButtom skinButtom1;
        private CCWin.SkinControl.SkinButtom skinButtom2;
        private CCWin.SkinControl.SkinComboBox skinComboBox1;

    }
}