using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace test
{
	/// <summary>
	/// Form1
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private DotNetSkin.SkinControls.SkinImage skinImage2;
		private DotNetSkin.SkinControls.SkinButton skinButton3;
		private DotNetSkin.SkinControls.SkinButton skinButton1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.GroupBox groupBox1;
		private DotNetSkin.SkinControls.SkinRadioButton skinRadioButton1;
		private DotNetSkin.SkinControls.SkinRadioButton skinRadioButton2;
		private DotNetSkin.SkinControls.SkinRadioButton skinRadioButton3;
		private System.Windows.Forms.GroupBox groupBox2;
		private DotNetSkin.SkinControls.SkinCheckBox skinCheckBox1;
		private DotNetSkin.SkinControls.SkinCheckBox skinCheckBox2;
		private DotNetSkin.SkinControls.SkinCheckBox skinCheckBox3;
		private System.ComponentModel.IContainer components;

		public Form1()
		{
			InitializeComponent();

		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.skinImage2 = new DotNetSkin.SkinControls.SkinImage();
			this.skinButton3 = new DotNetSkin.SkinControls.SkinButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.skinButton1 = new DotNetSkin.SkinControls.SkinButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.skinRadioButton3 = new DotNetSkin.SkinControls.SkinRadioButton();
			this.skinRadioButton2 = new DotNetSkin.SkinControls.SkinRadioButton();
			this.skinRadioButton1 = new DotNetSkin.SkinControls.SkinRadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.skinCheckBox3 = new DotNetSkin.SkinControls.SkinCheckBox();
			this.skinCheckBox2 = new DotNetSkin.SkinControls.SkinCheckBox();
			this.skinCheckBox1 = new DotNetSkin.SkinControls.SkinCheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// skinImage2
			// 
			this.skinImage2.Scheme = DotNetSkin.SkinControls.Schemes.MacOs;
			// 
			// skinButton3
			// 
			this.skinButton3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.skinButton3.ImageIndex = 0;
			this.skinButton3.ImageList = this.imageList1;
			this.skinButton3.Location = new System.Drawing.Point(200, 120);
			this.skinButton3.Name = "skinButton3";
			this.skinButton3.Size = new System.Drawing.Size(88, 40);
			this.skinButton3.TabIndex = 0;
			this.skinButton3.Text = "skinButton3";
			this.skinButton3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// skinButton1
			// 
			this.skinButton1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.skinButton1.ImageIndex = 2;
			this.skinButton1.ImageList = this.imageList1;
			this.skinButton1.Location = new System.Drawing.Point(96, 120);
			this.skinButton1.Name = "skinButton1";
			this.skinButton1.Size = new System.Drawing.Size(88, 40);
			this.skinButton1.TabIndex = 1;
			this.skinButton1.Text = "skinButton1";
			this.skinButton1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.skinRadioButton3);
			this.groupBox1.Controls.Add(this.skinRadioButton2);
			this.groupBox1.Controls.Add(this.skinRadioButton1);
			this.groupBox1.Location = new System.Drawing.Point(24, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(168, 88);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Schemes";
			// 
			// skinRadioButton3
			// 
			this.skinRadioButton3.BackColor = System.Drawing.Color.Transparent;
			this.skinRadioButton3.Location = new System.Drawing.Point(16, 64);
			this.skinRadioButton3.Name = "skinRadioButton3";
			this.skinRadioButton3.Size = new System.Drawing.Size(128, 16);
			this.skinRadioButton3.TabIndex = 2;
			this.skinRadioButton3.Text = "Plex Style";
			this.skinRadioButton3.CheckedChanged += new System.EventHandler(this.skinRadioButton3_CheckedChanged);
			// 
			// skinRadioButton2
			// 
			this.skinRadioButton2.BackColor = System.Drawing.Color.Transparent;
			this.skinRadioButton2.Location = new System.Drawing.Point(16, 40);
			this.skinRadioButton2.Name = "skinRadioButton2";
			this.skinRadioButton2.Size = new System.Drawing.Size(120, 16);
			this.skinRadioButton2.TabIndex = 1;
			this.skinRadioButton2.Text = "Xp Style";
			this.skinRadioButton2.CheckedChanged += new System.EventHandler(this.skinRadioButton2_CheckedChanged);
			// 
			// skinRadioButton1
			// 
			this.skinRadioButton1.BackColor = System.Drawing.Color.Transparent;
			this.skinRadioButton1.Checked = true;
			this.skinRadioButton1.Location = new System.Drawing.Point(16, 16);
			this.skinRadioButton1.Name = "skinRadioButton1";
			this.skinRadioButton1.Size = new System.Drawing.Size(136, 16);
			this.skinRadioButton1.TabIndex = 0;
			this.skinRadioButton1.TabStop = true;
			this.skinRadioButton1.Text = "MacOs Style";
			this.skinRadioButton1.CheckedChanged += new System.EventHandler(this.skinRadioButton1_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.skinCheckBox3);
			this.groupBox2.Controls.Add(this.skinCheckBox2);
			this.groupBox2.Controls.Add(this.skinCheckBox1);
			this.groupBox2.Location = new System.Drawing.Point(208, 16);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(168, 88);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "CheckBox";
			// 
			// skinCheckBox3
			// 
			this.skinCheckBox3.BackColor = System.Drawing.Color.Transparent;
			this.skinCheckBox3.Checked = true;
			this.skinCheckBox3.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.skinCheckBox3.Location = new System.Drawing.Point(16, 64);
			this.skinCheckBox3.Name = "skinCheckBox3";
			this.skinCheckBox3.Size = new System.Drawing.Size(96, 16);
			this.skinCheckBox3.TabIndex = 2;
			this.skinCheckBox3.Text = "skinCheckBox3";
			this.skinCheckBox3.ThreeState = true;
			// 
			// skinCheckBox2
			// 
			this.skinCheckBox2.BackColor = System.Drawing.Color.Transparent;
			this.skinCheckBox2.Location = new System.Drawing.Point(16, 40);
			this.skinCheckBox2.Name = "skinCheckBox2";
			this.skinCheckBox2.Size = new System.Drawing.Size(136, 16);
			this.skinCheckBox2.TabIndex = 1;
			this.skinCheckBox2.Text = "skinCheckBox2";
			// 
			// skinCheckBox1
			// 
			this.skinCheckBox1.BackColor = System.Drawing.Color.Transparent;
			this.skinCheckBox1.Checked = true;
			this.skinCheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.skinCheckBox1.Location = new System.Drawing.Point(16, 16);
			this.skinCheckBox1.Name = "skinCheckBox1";
			this.skinCheckBox1.Size = new System.Drawing.Size(136, 16);
			this.skinCheckBox1.TabIndex = 0;
			this.skinCheckBox1.Text = "skinCheckBox1";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 173);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.skinButton1);
			this.Controls.Add(this.skinButton3);
			this.Name = "Form1";
			this.Text = "Form1";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void skinRadioButton1_CheckedChanged(object sender, System.EventArgs e)
		{
			skinImage2.Scheme = DotNetSkin.SkinControls.Schemes.MacOs;
			updatecontrols();
		}

		private void skinRadioButton2_CheckedChanged(object sender, System.EventArgs e)
		{
			skinImage2.Scheme = DotNetSkin.SkinControls.Schemes.Xp;
			updatecontrols();
		}

		private void skinRadioButton3_CheckedChanged(object sender, System.EventArgs e)
		{
			skinImage2.Scheme = DotNetSkin.SkinControls.Schemes.Plex;
			updatecontrols();
		}

		private void updatecontrols()
		{
			foreach(Control c in this.Controls)
			{
				c.Invalidate();
			}
		}
	}
}
