namespace CCWin
{
    using CCWin.SkinClass;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class SkinMain : Form
    {
        private bool _skinmobile = true;
        private bool _skinshowintaskbar = true;
        private Color _skintrankcolor = Color.Transparent;
        private IContainer components;
        private bool show;
        public SkinForm skin;
        private Image skinback;

        public SkinMain()
        {
            this.InitializeComponent();
            this.SetStyles();
            this.Init();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Init()
        {
            base.ShowInTaskbar = false;
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.Control;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            base.ClientSize = new Size(0x11c, 0x106);
            base.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            base.Name = "SkinMain";
            base.ShowInTaskbar = false;
            this.Text = "SkinMain";
            base.ResumeLayout(false);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.Owner.Close();
            base.OnClosing(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (this.SkinBack != null)
            {
                g.DrawImage(this.TrankBack(), 0, 0, base.Width, base.Height);
            }
            base.OnPaint(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if ((this.SkinBack != null) && this.show)
            {
                UpdateForm.CreateControlRegion(this, this.TrankBack(), 0xff);
                this.skin.Size = base.Size;
            }
            base.OnSizeChanged(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!base.DesignMode)
            {
                if (this.skin != null)
                {
                    this.skin.Visible = base.Visible;
                }
                else
                {
                    UpdateForm.CreateControlRegion(this, this.TrankBack(), 0xff);
                    this.show = true;
                    this.skin = new SkinForm(this);
                    this.skin.Show();
                }
            }
            base.OnVisibleChanged(e);
        }

        private void SetStyles()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.UpdateStyles();
            base.AutoScaleMode = AutoScaleMode.None;
        }

        public Bitmap TrankBack()
        {
            Bitmap bitmap = new Bitmap(this.SkinBack);
            if (this.SkinTrankColor != Color.Transparent)
            {
                bitmap.MakeTransparent(this.SkinTrankColor);
            }
            return new Bitmap(bitmap, base.Size);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Windows.Forms.FormBorderStyle FormBorderStyle
        {
            get
            {
                return base.FormBorderStyle;
            }
            set
            {
                base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
        }

        [Category("Skin"), Description("该窗体的背景图像")]
        public Image SkinBack
        {
            get
            {
                return this.skinback;
            }
            set
            {
                if (this.skinback != value)
                {
                    this.skinback = value;
                    if (((value != null) && this.show) && !base.DesignMode)
                    {
                        UpdateForm.CreateControlRegion(this, this.TrankBack(), 0xff);
                    }
                    base.Invalidate();
                    if (this.skin != null)
                    {
                        this.skin.BackgroundImage = this.TrankBack();
                    }
                }
            }
        }

        [Category("Skin"), Description("窗体是否可以移动"), DefaultValue(typeof(bool), "true")]
        public bool SkinMobile
        {
            get
            {
                return this._skinmobile;
            }
            set
            {
                if (this._skinmobile != value)
                {
                    this._skinmobile = value;
                }
            }
        }

        [Category("Skin"), Description("绘图层是否出现在Windows任务栏中。"), DefaultValue(typeof(bool), "true")]
        public bool SkinShowInTaskbar
        {
            get
            {
                return this._skinshowintaskbar;
            }
            set
            {
                if (this._skinshowintaskbar != value)
                {
                    this._skinshowintaskbar = value;
                }
            }
        }

        [Category("Skin"), Description("背景需要透明的颜色"), DefaultValue(typeof(Color), "Color.Transparent")]
        public Color SkinTrankColor
        {
            get
            {
                return this._skintrankcolor;
            }
            set
            {
                if (this._skintrankcolor != value)
                {
                    this._skintrankcolor = value;
                    base.Invalidate();
                    if (this.skin != null)
                    {
                        this.skin.BackgroundImage = this.TrankBack();
                    }
                }
            }
        }
    }
}

