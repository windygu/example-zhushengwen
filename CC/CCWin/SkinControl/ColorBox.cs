namespace CCWin.SkinControl
{
    using CCWin.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows.Forms;

    internal class ColorBox : Control
    {
        public ColorChangedHandler ColorChanged;
        private IContainer components;
        private Bitmap m_clrImage = Resources.color;
        private Color m_lastColor;
        private Point m_ptCurrent;
        private Rectangle m_rectSelected;
        private Color selectedColor;

        public event ColorChangedHandler colorChanged
        {
            add
            {
                ColorChangedHandler handler2;
                ColorChangedHandler colorChanged = this.ColorChanged;
                do
                {
                    handler2 = colorChanged;
                    ColorChangedHandler handler3 = (ColorChangedHandler) Delegate.Combine(handler2, value);
                    colorChanged = Interlocked.CompareExchange<ColorChangedHandler>(ref this.ColorChanged, handler3, handler2);
                }
                while (colorChanged != handler2);
            }
            remove
            {
                ColorChangedHandler handler2;
                ColorChangedHandler colorChanged = this.ColorChanged;
                do
                {
                    handler2 = colorChanged;
                    ColorChangedHandler handler3 = (ColorChangedHandler) Delegate.Remove(handler2, value);
                    colorChanged = Interlocked.CompareExchange<ColorChangedHandler>(ref this.ColorChanged, handler3, handler2);
                }
                while (colorChanged != handler2);
            }
        }

        public ColorBox()
        {
            this.InitializeComponent();
            this.selectedColor = Color.Red;
            this.m_rectSelected = new Rectangle(-100, -100, 14, 14);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.Name = "ColorBox";
            base.Size = new Size(0xcb, 50);
            base.ResumeLayout(false);
        }

        protected override void OnClick(EventArgs e)
        {
            Color clr = this.m_clrImage.GetPixel(this.m_ptCurrent.X, this.m_ptCurrent.Y);
            if (((clr.ToArgb() != Color.FromArgb(0xff, 0xfe, 0xfe, 0xfe).ToArgb()) && (clr.ToArgb() != Color.FromArgb(0xff, 0x85, 0x8d, 0x97).ToArgb())) && (clr.ToArgb() != Color.FromArgb(0xff, 110, 0x7e, 0x95).ToArgb()))
            {
                if (this.selectedColor != clr)
                {
                    this.selectedColor = clr;
                }
                base.Invalidate();
                this.OnColorChanged(new ColorChangedEventArgs(clr));
            }
            base.OnClick(e);
        }

        protected virtual void OnColorChanged(ColorChangedEventArgs e)
        {
            if (this.ColorChanged != null)
            {
                this.ColorChanged(this, e);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.m_rectSelected.X = this.m_rectSelected.Y = -100;
            base.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.m_ptCurrent = e.Location;
            try
            {
                Color clr = this.m_clrImage.GetPixel(this.m_ptCurrent.X, this.m_ptCurrent.Y);
                if (clr != this.m_lastColor)
                {
                    if (((clr.ToArgb() != Color.FromArgb(0xff, 0xfe, 0xfe, 0xfe).ToArgb()) && (clr.ToArgb() != Color.FromArgb(0xff, 0x85, 0x8d, 0x97).ToArgb())) && ((clr.ToArgb() != Color.FromArgb(0xff, 110, 0x7e, 0x95).ToArgb()) && (e.X > 0x27)))
                    {
                        this.m_rectSelected.Y = (e.Y > 0x11) ? 0x11 : 2;
                        this.m_rectSelected.X = (((e.X - 0x27) / 15) * 15) + 0x26;
                        base.Invalidate();
                    }
                    else
                    {
                        this.m_rectSelected.X = this.m_rectSelected.Y = -100;
                        base.Invalidate();
                    }
                }
                this.m_lastColor = clr;
            }
            finally
            {
                base.OnMouseMove(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(Resources.color, new Rectangle(0, 0, 0xa5, 0x23));
            g.DrawRectangle(Pens.SteelBlue, 0, 0, 0xa4, 0x22);
            SolidBrush sb = new SolidBrush(this.selectedColor);
            g.FillRectangle(sb, 9, 5, 0x18, 0x18);
            g.DrawRectangle(Pens.DarkCyan, this.m_rectSelected);
            base.OnPaint(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, 0xa5, 0x23, specified);
        }

        public Color SelectedColor
        {
            get
            {
                return this.selectedColor;
            }
        }

        public delegate void ColorChangedHandler(object sender, ColorChangedEventArgs e);
    }
}

