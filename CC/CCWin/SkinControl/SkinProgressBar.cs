namespace CCWin.SkinControl
{
    using CCWin.SkinClass;
    using CCWin.Win32;
    using CCWin.Win32.Struct;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(ProgressBar))]
    public class SkinProgressBar : ProgressBar
    {
        private Color _border = Color.FromArgb(0x9e, 0x9e, 0x9e);
        private bool _bPainting;
        private BufferedGraphics _bufferedGraphics;
        private BufferedGraphicsContext _context = BufferedGraphicsManager.Current;
        private string _formatString = "{0:0.0%}";
        private Color _innerBorder = Color.FromArgb(200, 250, 250, 250);
        private Timer _timer;
        private Color _trackBack = Color.FromArgb(0xb9, 0xb9, 0xb9);
        private Color _trackFore = Color.FromArgb(15, 0xb5, 0x2b);
        private int _trackX = -100;
        private Image back;
        private Image barBack;
        private BackStyle barBackStyle;
        private bool barGlass = true;
        private System.Drawing.Size barMinusSize = new System.Drawing.Size(1, 1);
        private int barradius = 2;
        private RoundStyle barradiusStyle = RoundStyle.All;
        private bool glass = true;
        private const int Internal = 10;
        private const int MarqueeWidth = 100;
        private int radius = 2;
        private RoundStyle radiusStyle = RoundStyle.All;
        private bool txtShow = true;

        public SkinProgressBar()
        {
            this._context.MaximumBuffer = new System.Drawing.Size(base.Width + 1, base.Height + 1);
            this._bufferedGraphics = this._context.Allocate(base.CreateGraphics(), new Rectangle(System.Drawing.Point.Empty, base.Size));
            this.ForeColor = Color.Red;
            base.ResizeRedraw = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this._timer != null)
                {
                    this._timer.Dispose();
                    this._timer = null;
                }
                if (this._bufferedGraphics != null)
                {
                    this._bufferedGraphics.Dispose();
                    this._bufferedGraphics = null;
                }
                if (this._context != null)
                {
                    this._context = null;
                }
            }
        }

        private void DrawProgressBar(IntPtr hWnd)
        {
            Graphics g = this._bufferedGraphics.Graphics;
            g.Clear(Color.Transparent);
            Rectangle rect = new Rectangle(System.Drawing.Point.Empty, base.Size);
            bool bBlock = (this.Style != ProgressBarStyle.Marquee) || base.DesignMode;
            float basePosition = bBlock ? 0.3f : 0.45f;
            SmoothingModeGraphics sg = new SmoothingModeGraphics(g);
            if (this.Back != null)
            {
                Bitmap btm = new Bitmap(this.Back, base.Size);
                UpdateForm.CreateControlRegion(this, btm, 200);
                g.DrawImage(this.Back, rect);
            }
            else
            {
                RenderHelper.RenderBackgroundInternal(g, rect, this.TrackBack, this.Border, this.InnerBorder, this.RadiusStyle, this.Radius, basePosition, true, this.Glass, LinearGradientMode.Vertical);
            }
            Rectangle trackRect = rect;
            trackRect.Inflate(-this.BarMinusSize.Width, -this.BarMinusSize.Height);
            if (!bBlock)
            {
                GraphicsState state = g.Save();
                g.SetClip(trackRect);
                trackRect.X = this._trackX;
                trackRect.Width = 100;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(trackRect);
                    g.SetClip(path, CombineMode.Intersect);
                }
                RenderHelper.RenderBackgroundInternal(g, trackRect, this.TrackFore, this.Border, this.InnerBorder, RoundStyle.None, 8, basePosition, false, this.BarGlass, LinearGradientMode.Vertical);
                using (LinearGradientBrush brush = new LinearGradientBrush(trackRect, this.InnerBorder, Color.Transparent, 0f))
                {
                    Blend blend = new Blend();
                    float[] CS_0_0002 = new float[3];
                    CS_0_0002[1] = 1f;
                    blend.Factors = CS_0_0002;
                    float[] CS_0_0003 = new float[3];
                    CS_0_0003[1] = 0.5f;
                    CS_0_0003[2] = 1f;
                    blend.Positions = CS_0_0003;
                    brush.Blend = blend;
                    g.FillRectangle(brush, trackRect);
                }
                g.Restore(state);
                goto Label_02F1;
            }
            trackRect.Width = (int) ((((double) base.Value) / ((double) (base.Maximum - base.Minimum))) * trackRect.Width);
            if (this.BarBack != null)
            {
                if (this.BarBackStyle == BackStyle.Tile)
                {
                    using (TextureBrush Txbrus = new TextureBrush(this.BarBack))
                    {
                        Txbrus.WrapMode = WrapMode.Tile;
                        g.FillRectangle(Txbrus, trackRect);
                        goto Label_019B;
                    }
                }
                Bitmap btm = new Bitmap(this.BarBack, base.Size);
                g.DrawImageUnscaledAndClipped(btm, trackRect);
            }
            else
            {
                RenderHelper.RenderBackgroundInternal(g, trackRect, this.TrackFore, this.Border, this.InnerBorder, this.BarRadiusStyle, this.BarRadius, basePosition, false, this.BarGlass, LinearGradientMode.Vertical);
            }
        Label_019B:
            if (!string.IsNullOrEmpty(this._formatString) && this.TxtShow)
            {
                TextRenderer.DrawText(g, string.Format(this._formatString, ((double) base.Value) / ((double) (base.Maximum - base.Minimum))), base.Font, rect, base.ForeColor, TextFormatFlags.WordEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }
        Label_02F1:
            sg.Dispose();
            IntPtr hDC = CCWin.Win32.NativeMethods.GetDC(hWnd);
            this._bufferedGraphics.Render(hDC);
            CCWin.Win32.NativeMethods.ReleaseDC(hWnd, hDC);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.SetRegion();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.SetRegion();
            this._context.MaximumBuffer = new System.Drawing.Size(base.Width + 1, base.Height + 1);
            if (this._bufferedGraphics != null)
            {
                this._bufferedGraphics.Dispose();
                this._bufferedGraphics = null;
            }
            this._bufferedGraphics = this._context.Allocate(base.CreateGraphics(), new Rectangle(System.Drawing.Point.Empty, base.Size));
        }

        private void SetRegion()
        {
            RegionHelper.CreateRegion(this, new Rectangle(System.Drawing.Point.Empty, base.Size), this.Radius, this.RadiusStyle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 15)
            {
                if (!this._bPainting)
                {
                    this._bPainting = true;
                    CCWin.Win32.Struct.PAINTSTRUCT ps = new CCWin.Win32.Struct.PAINTSTRUCT();
                    CCWin.Win32.NativeMethods.BeginPaint(m.HWnd, ref ps);
                    try
                    {
                        this.DrawProgressBar(m.HWnd);
                    }
                    catch
                    {
                    }
                    CCWin.Win32.NativeMethods.ValidateRect(m.HWnd, ref ps.rcPaint);
                    CCWin.Win32.NativeMethods.EndPaint(m.HWnd, ref ps);
                    this._bPainting = false;
                    m.Result = Result.TRUE;
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        [Description("控件背景图片"), Category("Skin")]
        public Image Back
        {
            get
            {
                return this.back;
            }
            set
            {
                if (this.back != value)
                {
                    this.back = value;
                    base.Invalidate();
                }
            }
        }

        [Description("进度条图片"), Category("Bar")]
        public Image BarBack
        {
            get
            {
                return this.barBack;
            }
            set
            {
                if (this.barBack != value)
                {
                    this.barBack = value;
                    base.Invalidate();
                }
            }
        }

        [Description("进度条的图像绘制模式"), DefaultValue(typeof(BackStyle), "0"), Category("Bar")]
        public BackStyle BarBackStyle
        {
            get
            {
                return this.barBackStyle;
            }
            set
            {
                if (this.barBackStyle != value)
                {
                    this.barBackStyle = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Bar"), Description("滚动条是否启用颜色渐变"), DefaultValue(typeof(bool), "true")]
        public bool BarGlass
        {
            get
            {
                return this.barGlass;
            }
            set
            {
                if (this.barGlass != value)
                {
                    this.barGlass = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(System.Drawing.Size), "1,1"), Description("自减宽高。"), Category("Bar")]
        public System.Drawing.Size BarMinusSize
        {
            get
            {
                return this.barMinusSize;
            }
            set
            {
                if (this.barMinusSize != value)
                {
                    this.barMinusSize = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Bar"), DefaultValue(typeof(int), "2"), Description("进度条圆角大小")]
        public int BarRadius
        {
            get
            {
                return this.barradius;
            }
            set
            {
                if (this.barradius != value)
                {
                    this.barradius = (value < 1) ? 1 : value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(RoundStyle), "1"), Description("进度条圆角样式"), Category("Bar")]
        public RoundStyle BarRadiusStyle
        {
            get
            {
                return this.barradiusStyle;
            }
            set
            {
                if (this.barradiusStyle != value)
                {
                    this.barradiusStyle = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Color), "158, 158, 158"), Category("Skin")]
        public Color Border
        {
            get
            {
                return this._border;
            }
            set
            {
                if (this._border != value)
                {
                    this._border = value;
                    base.Invalidate();
                }
            }
        }

        [Browsable(true)]
        public override System.Drawing.Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        [Category("Skin"), DefaultValue("{0:0.0%}")]
        public string FormatString
        {
            get
            {
                return this._formatString;
            }
            set
            {
                if (this._formatString != value)
                {
                    this._formatString = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(bool), "true"), Category("Skin"), Description("控件是否启用颜色渐变")]
        public bool Glass
        {
            get
            {
                return this.glass;
            }
            set
            {
                if (this.glass != value)
                {
                    this.glass = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Skin"), DefaultValue(typeof(Color), "200, 250, 250, 250")]
        public Color InnerBorder
        {
            get
            {
                return this._innerBorder;
            }
            set
            {
                if (this._innerBorder != value)
                {
                    this._innerBorder = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Skin"), Description("控件圆角大小"), DefaultValue(typeof(int), "2")]
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
                    this.radius = (value < 1) ? 1 : value;
                    this.SetRegion();
                    base.Invalidate();
                }
            }
        }

        [Description("控件圆角样式"), Category("Skin"), DefaultValue(typeof(RoundStyle), "1")]
        public RoundStyle RadiusStyle
        {
            get
            {
                return this.radiusStyle;
            }
            set
            {
                if (this.radiusStyle != value)
                {
                    this.radiusStyle = value;
                    this.SetRegion();
                    base.Invalidate();
                }
            }
        }

        public ProgressBarStyle Style
        {
            get
            {
                return base.Style;
            }
            set
            {
                if (base.Style != value)
                {
                    base.Style = value;
                    if (value == ProgressBarStyle.Marquee)
                    {
                        if (this._timer != null)
                        {
                            this._timer.Dispose();
                        }
                        this._timer = new Timer();
                        this._timer.Interval = 10;
                        this._timer.Tick += delegate (object sender, EventArgs e) {
                            this._trackX += (int) Math.Ceiling((double) (((float) base.Width) / ((float) base.MarqueeAnimationSpeed)));
                            if (this._trackX > base.Width)
                            {
                                this._trackX = -100;
                            }
                            base.Invalidate();
                        };
                        if (!base.DesignMode)
                        {
                            this._timer.Start();
                        }
                    }
                    else if (this._timer != null)
                    {
                        this._timer.Dispose();
                        this._timer = null;
                    }
                }
            }
        }

        [Category("Skin"), DefaultValue(typeof(Color), "185, 185, 185")]
        public Color TrackBack
        {
            get
            {
                return this._trackBack;
            }
            set
            {
                if (this._trackBack != value)
                {
                    this._trackBack = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Bar"), DefaultValue(typeof(Color), "15, 181, 43")]
        public Color TrackFore
        {
            get
            {
                return this._trackFore;
            }
            set
            {
                if (this._trackFore != value)
                {
                    this._trackFore = value;
                    base.Invalidate();
                }
            }
        }

        [Description("是否显示进度百分比"), Category("Skin"), DefaultValue(typeof(bool), "true")]
        public bool TxtShow
        {
            get
            {
                return this.txtShow;
            }
            set
            {
                if (this.txtShow != value)
                {
                    this.txtShow = value;
                    base.Invalidate();
                }
            }
        }
    }
}

