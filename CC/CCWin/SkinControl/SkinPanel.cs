namespace CCWin.SkinControl
{
    using CCWin;
    using CCWin.SkinClass;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(Panel))]
    public class SkinPanel : Panel
    {
        private CCWin.SkinClass.ControlState _controlState;
        private Rectangle backrectangle = new Rectangle(10, 10, 10, 10);
        private IContainer components;
        private Image downback;
        private Image mouseback;
        private Image normlback;
        private bool palace;
        private int radius;

        public SkinPanel()
        {
            this.Init();
            base.ResizeRedraw = true;
            this.BackColor = Color.Transparent;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void Init()
        {
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.UpdateStyles();
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this._controlState = CCWin.SkinClass.ControlState.Pressed;
                base.Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this._controlState = CCWin.SkinClass.ControlState.Hover;
            base.Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this._controlState = CCWin.SkinClass.ControlState.Normal;
            base.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this._controlState = CCWin.SkinClass.ControlState.Hover;
            base.Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Bitmap btm = null;
            switch (this._controlState)
            {
                case CCWin.SkinClass.ControlState.Hover:
                    btm = (Bitmap) this.MouseBack;
                    break;

                case CCWin.SkinClass.ControlState.Pressed:
                    btm = (Bitmap) this.DownBack;
                    break;

                default:
                    btm = (Bitmap) this.NormlBack;
                    break;
            }
            if (btm != null)
            {
                if (this.Palace)
                {
                    CCWin.ImageDrawRect.DrawRect(g, btm, new Rectangle(base.ClientRectangle.X, base.ClientRectangle.Y, base.ClientRectangle.Width, base.ClientRectangle.Height), Rectangle.FromLTRB(this.BackRectangle.X, this.BackRectangle.Y, this.BackRectangle.Width, this.BackRectangle.Height), 1, 1);
                }
                else
                {
                    this.BackgroundImage = btm;
                }
            }
            UpdateForm.CreateRegion(this, this.radius);
            base.OnPaint(e);
        }

        [DefaultValue(typeof(Rectangle), "10,10,10,10"), Description("九宫绘画区域"), Category("Skin")]
        public Rectangle BackRectangle
        {
            get
            {
                return this.backrectangle;
            }
            set
            {
                if (this.backrectangle != value)
                {
                    this.backrectangle = value;
                }
                base.Invalidate();
            }
        }

        public CCWin.SkinClass.ControlState ControlState
        {
            get
            {
                return this._controlState;
            }
            set
            {
                if (this._controlState != value)
                {
                    this._controlState = value;
                    base.Invalidate();
                }
            }
        }

        [Category("MouseDown"), Description("点击时背景")]
        public Image DownBack
        {
            get
            {
                return this.downback;
            }
            set
            {
                if (this.downback != value)
                {
                    this.downback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("悬浮时背景"), Category("MouseEnter")]
        public Image MouseBack
        {
            get
            {
                return this.mouseback;
            }
            set
            {
                if (this.mouseback != value)
                {
                    this.mouseback = value;
                    base.Invalidate();
                }
            }
        }

        [Category("MouseNorml"), Description("初始时背景")]
        public Image NormlBack
        {
            get
            {
                return this.normlback;
            }
            set
            {
                if (this.normlback != value)
                {
                    this.normlback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("是否开启九宫绘图"), Category("Skin"), DefaultValue(typeof(bool), "false")]
        public bool Palace
        {
            get
            {
                return this.palace;
            }
            set
            {
                if (this.palace != value)
                {
                    this.palace = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(int), "0"), Description("圆角大小"), Category("Skin")]
        public int Radius
        {
            get
            {
                return this.radius;
            }
            set
            {
                if (this.radius != value)
                {
                    this.radius = (value < 0) ? 0 : value;
                    base.Invalidate();
                }
            }
        }
    }
}

