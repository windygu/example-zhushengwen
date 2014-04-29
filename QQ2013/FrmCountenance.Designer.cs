namespace CC2013
{
    partial class FrmCountenance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCountenance));
            this.skinButtom1 = new CCWin.SkinControl.SkinButtom();
            this.skinButtom2 = new CCWin.SkinControl.SkinButtom();
            this.SuspendLayout();
            // 
            // skinButtom1
            // 
            this.skinButtom1.BackColor = System.Drawing.Color.Transparent;
            this.skinButtom1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButtom1.DownBack = null;
            this.skinButtom1.Location = new System.Drawing.Point(24, 16);
            this.skinButtom1.MouseBack = null;
            this.skinButtom1.Name = "skinButtom1";
            this.skinButtom1.NormlBack = null;
            this.skinButtom1.Size = new System.Drawing.Size(75, 23);
            this.skinButtom1.TabIndex = 0;
            this.skinButtom1.Text = "skinButtom1";
            this.skinButtom1.UseVisualStyleBackColor = false;
            // 
            // skinButtom2
            // 
            this.skinButtom2.BackColor = System.Drawing.Color.Transparent;
            this.skinButtom2.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButtom2.DownBack = null;
            this.skinButtom2.Location = new System.Drawing.Point(132, 16);
            this.skinButtom2.MouseBack = null;
            this.skinButtom2.Name = "skinButtom2";
            this.skinButtom2.NormlBack = null;
            this.skinButtom2.Size = new System.Drawing.Size(75, 23);
            this.skinButtom2.TabIndex = 1;
            this.skinButtom2.Text = "skinButtom2";
            this.skinButtom2.UseVisualStyleBackColor = false;
            // 
            // FrmCountenance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Back = ((System.Drawing.Image)(resources.GetObject("$this.Back")));
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(187)))), ((int)(((byte)(172)))));
            this.BorderPalace = global::CC2013.Properties.Resources.BackPalace;
            this.CanResize = false;
            this.ClientSize = new System.Drawing.Size(546, 371);
            this.ControlBox = false;
            this.Controls.Add(this.skinButtom2);
            this.Controls.Add(this.skinButtom1);
            this.Mobile = CCWin.MobileStyle.None;
            this.Name = "FrmCountenance";
            this.ShowBorder = false;
            this.ShowDrawIcon = false;
            this.ShowInTaskbar = false;
            this.SkinOpacity = 0.9D;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FrmCountenance_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CCWin.SkinControl.SkinButtom skinButtom1;
        private CCWin.SkinControl.SkinButtom skinButtom2;
    }
}