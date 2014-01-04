using CCWin;
using CCWin.SkinControl;
using CCWin.Properties;
namespace CCWin
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
            this.txtId = new CCWin.SkinControl.WaterTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.skinRadioButton1 = new CCWin.SkinControl.SkinRadioButton();
            this.skinRadioButton2 = new CCWin.SkinControl.SkinRadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.skinRadioButton3 = new CCWin.SkinControl.SkinRadioButton();
            this.button1 = new CCWin.SkinControl.SkinButtom();
            this.skinComboBox1 = new CCWin.SkinControl.SkinComboBox();
            this.btnClose = new CCWin.SkinControl.SkinButtom();
            this.SuspendLayout();
            // 
            // txtId
            // 
            this.txtId.BackColor = System.Drawing.SystemColors.Control;
            this.txtId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtId.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.txtId.Location = new System.Drawing.Point(32, 64);
            this.txtId.Margin = new System.Windows.Forms.Padding(0);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(109, 29);
            this.txtId.TabIndex = 111;
            this.txtId.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.txtId.WaterText = "请在此输入身份证号...";
            this.txtId.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.label3.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label3.Location = new System.Drawing.Point(34, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "时间:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.label5.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label5.Location = new System.Drawing.Point(34, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 21);
            this.label5.TabIndex = 2;
            this.label5.Text = "状态:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.label4.ForeColor = System.Drawing.Color.Maroon;
            this.label4.Location = new System.Drawing.Point(32, 194);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 21);
            this.label4.TabIndex = 2;
            this.label4.Text = "2013-12-27 7:25:36";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label6.ForeColor = System.Drawing.Color.Maroon;
            this.label6.Location = new System.Drawing.Point(88, 160);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "等待考试...";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(86, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "2013-8-8 7:25:36";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.label2.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label2.Location = new System.Drawing.Point(354, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "流水号:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label7.ForeColor = System.Drawing.Color.Maroon;
            this.label7.Location = new System.Drawing.Point(417, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "0000";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.label8.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label8.Location = new System.Drawing.Point(368, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 21);
            this.label8.TabIndex = 2;
            this.label8.Text = "分数:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.label9.ForeColor = System.Drawing.Color.Maroon;
            this.label9.Location = new System.Drawing.Point(417, 175);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 17);
            this.label9.TabIndex = 2;
            this.label9.Text = "0000";
            // 
            // skinRadioButton1
            // 
            this.skinRadioButton1.AutoSize = true;
            this.skinRadioButton1.BackColor = System.Drawing.Color.Transparent;
            this.skinRadioButton1.Checked = true;
            this.skinRadioButton1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinRadioButton1.DownBack = null;
            this.skinRadioButton1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinRadioButton1.ForeColor = System.Drawing.Color.Maroon;
            this.skinRadioButton1.Location = new System.Drawing.Point(88, 29);
            this.skinRadioButton1.MouseBack = null;
            this.skinRadioButton1.Name = "skinRadioButton1";
            this.skinRadioButton1.NormlBack = null;
            this.skinRadioButton1.SelectedDownBack = null;
            this.skinRadioButton1.SelectedMouseBack = null;
            this.skinRadioButton1.SelectedNormlBack = null;
            this.skinRadioButton1.Size = new System.Drawing.Size(62, 21);
            this.skinRadioButton1.TabIndex = 112;
            this.skinRadioButton1.TabStop = true;
            this.skinRadioButton1.Text = "科目一";
            this.skinRadioButton1.UseVisualStyleBackColor = false;
            // 
            // skinRadioButton2
            // 
            this.skinRadioButton2.AutoSize = true;
            this.skinRadioButton2.BackColor = System.Drawing.Color.Transparent;
            this.skinRadioButton2.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinRadioButton2.DownBack = null;
            this.skinRadioButton2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinRadioButton2.ForeColor = System.Drawing.Color.Maroon;
            this.skinRadioButton2.Location = new System.Drawing.Point(164, 29);
            this.skinRadioButton2.MouseBack = null;
            this.skinRadioButton2.Name = "skinRadioButton2";
            this.skinRadioButton2.NormlBack = null;
            this.skinRadioButton2.SelectedDownBack = null;
            this.skinRadioButton2.SelectedMouseBack = null;
            this.skinRadioButton2.SelectedNormlBack = null;
            this.skinRadioButton2.Size = new System.Drawing.Size(62, 21);
            this.skinRadioButton2.TabIndex = 112;
            this.skinRadioButton2.Text = "科目三";
            this.skinRadioButton2.UseVisualStyleBackColor = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.label10.ForeColor = System.Drawing.SystemColors.Desktop;
            this.label10.Location = new System.Drawing.Point(34, 29);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 21);
            this.label10.TabIndex = 2;
            this.label10.Text = "科目:";
            // 
            // skinRadioButton3
            // 
            this.skinRadioButton3.AutoEllipsis = true;
            this.skinRadioButton3.AutoSize = true;
            this.skinRadioButton3.BackColor = System.Drawing.Color.Transparent;
            this.skinRadioButton3.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinRadioButton3.DownBack = null;
            this.skinRadioButton3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinRadioButton3.ForeColor = System.Drawing.Color.Maroon;
            this.skinRadioButton3.Location = new System.Drawing.Point(249, 29);
            this.skinRadioButton3.MouseBack = null;
            this.skinRadioButton3.Name = "skinRadioButton3";
            this.skinRadioButton3.NormlBack = null;
            this.skinRadioButton3.SelectedDownBack = null;
            this.skinRadioButton3.SelectedMouseBack = null;
            this.skinRadioButton3.SelectedNormlBack = null;
            this.skinRadioButton3.Size = new System.Drawing.Size(62, 21);
            this.skinRadioButton3.TabIndex = 112;
            this.skinRadioButton3.Text = "看视频";
            this.skinRadioButton3.UseVisualStyleBackColor = false;
            this.skinRadioButton3.Visible = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.button1.DownBack = ((System.Drawing.Image)(resources.GetObject("button1.DownBack")));
            this.button1.DrawType = CCWin.SkinControl.DrawStyle.Img;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(346, 118);
            this.button1.MouseBack = ((System.Drawing.Image)(resources.GetObject("button1.MouseBack")));
            this.button1.Name = "button1";
            this.button1.NormlBack = ((System.Drawing.Image)(resources.GetObject("button1.NormlBack")));
            this.button1.Size = new System.Drawing.Size(109, 40);
            this.button1.TabIndex = 131;
            this.button1.Text = "立即考试";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // skinComboBox1
            // 
            this.skinComboBox1.BackColor = System.Drawing.SystemColors.Control;
            this.skinComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.skinComboBox1.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.skinComboBox1.FormattingEnabled = true;
            this.skinComboBox1.Location = new System.Drawing.Point(145, 64);
            this.skinComboBox1.Name = "skinComboBox1";
            this.skinComboBox1.Size = new System.Drawing.Size(211, 28);
            this.skinComboBox1.TabIndex = 132;
            this.skinComboBox1.WaterText = "请在此输入身份证号...";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnClose.DownBack = ((System.Drawing.Image)(resources.GetObject("btnClose.DownBack")));
            this.btnClose.DrawType = CCWin.SkinControl.DrawStyle.Img;
            this.btnClose.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClose.Location = new System.Drawing.Point(370, 65);
            this.btnClose.MouseBack = ((System.Drawing.Image)(resources.GetObject("btnClose.MouseBack")));
            this.btnClose.Name = "btnClose";
            this.btnClose.NormlBack = ((System.Drawing.Image)(resources.GetObject("btnClose.NormlBack")));
            this.btnClose.Size = new System.Drawing.Size(73, 29);
            this.btnClose.TabIndex = 133;
            this.btnClose.Text = "导入账号";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CanResize = false;
            this.CaptionFont = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold);
            this.ClientSize = new System.Drawing.Size(484, 233);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.skinComboBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.skinRadioButton3);
            this.Controls.Add(this.skinRadioButton2);
            this.Controls.Add(this.skinRadioButton1);
            this.Controls.Add(this.txtId);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.SysBottomToolTip = "设置";
            this.Text = "驾考闯关(科目版)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public WaterTextBox txtId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private SkinRadioButton skinRadioButton1;
        private SkinRadioButton skinRadioButton2;
        private System.Windows.Forms.Label label10;
        private SkinRadioButton skinRadioButton3;
        private SkinButtom button1;
        private SkinComboBox skinComboBox1;
        private SkinButtom btnClose;
    }
}

