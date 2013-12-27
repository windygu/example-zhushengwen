namespace CCWin.SkinControl
{
    using CCWin;
    using CCWin.SkinClass;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(Button))]
    public class SkinButtom : Button
    {
        private Color _baseColor = Color.FromArgb(0x33, 0xa1, 0xe0);
        private CCWin.SkinClass.ControlState _controlState;
        private int _imageWidth = 0x12;
        private CCWin.SkinClass.RoundStyle _roundStyle;
        private Rectangle backrectangle = new Rectangle(10, 10, 10, 10);
        private IContainer components;
        private bool create;
        private Image downback;
        private DrawStyle drawType = DrawStyle.Draw;
        private Image mouseback;
        private Image normlback;
        private bool palace;
        private int radius = 8;
        private CCWin.SkinClass.ControlState states;

        public SkinButtom()
        {
            this.Init();
            base.ResizeRedraw = true;
            this.BackColor = Color.Transparent;
        }

        private void CalculateRect(out Rectangle imageRect, out Rectangle textRect)
        {
            imageRect = Rectangle.Empty;
            textRect = Rectangle.Empty;
            if (base.Image == null)
            {
                textRect = new Rectangle(2, 0, base.Width - 4, base.Height);
            }
            else
            {
                switch (base.TextImageRelation)
                {
                    case TextImageRelation.Overlay:
                        imageRect = new Rectangle(2, (base.Height - this.ImageWidth) / 2, this.ImageWidth, this.ImageWidth);
                        textRect = new Rectangle(2, 0, base.Width - 4, base.Height);
                        break;

                    case TextImageRelation.ImageAboveText:
                        imageRect = new Rectangle((base.Width - this.ImageWidth) / 2, 2, this.ImageWidth, this.ImageWidth);
                        textRect = new Rectangle(2, imageRect.Bottom, base.Width, (base.Height - imageRect.Bottom) - 2);
                        break;

                    case TextImageRelation.TextAboveImage:
                        imageRect = new Rectangle((base.Width - this.ImageWidth) / 2, (base.Height - this.ImageWidth) - 2, this.ImageWidth, this.ImageWidth);
                        textRect = new Rectangle(0, 2, base.Width, (base.Height - imageRect.Y) - 2);
                        break;

                    case TextImageRelation.ImageBeforeText:
                        imageRect = new Rectangle(2, (base.Height - this.ImageWidth) / 2, this.ImageWidth, this.ImageWidth);
                        textRect = new Rectangle(imageRect.Right + 2, 0, (base.Width - imageRect.Right) - 4, base.Height);
                        break;

                    case TextImageRelation.TextBeforeImage:
                        imageRect = new Rectangle((base.Width - this.ImageWidth) - 2, (base.Height - this.ImageWidth) / 2, this.ImageWidth, this.ImageWidth);
                        textRect = new Rectangle(2, 0, imageRect.X - 2, base.Height);
                        break;
                }
                if (this.RightToLeft == RightToLeft.Yes)
                {
                    imageRect.X = base.Width - imageRect.Right;
                    textRect.X = base.Width - textRect.Right;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DrawGlass(Graphics g, RectangleF glassRect, int alphaCenter, int alphaSurround)
        {
            this.DrawGlass(g, glassRect, Color.White, alphaCenter, alphaSurround);
        }

        private void DrawGlass(Graphics g, RectangleF glassRect, Color glassColor, int alphaCenter, int alphaSurround)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(glassRect);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(alphaCenter, glassColor);
                    brush.SurroundColors = new Color[] { Color.FromArgb(alphaSurround, glassColor) };
                    brush.CenterPoint = new PointF(glassRect.X + (glassRect.Width / 2f), glassRect.Y + (glassRect.Height / 2f));
                    g.FillPath(brush, path);
                }
            }
        }

        private Color GetColor(Color colorBase, int a, int r, int g, int b)
        {
            int a0 = colorBase.A;
            int r0 = colorBase.R;
            int g0 = colorBase.G;
            int b0 = colorBase.B;
            if ((a + a0) > 0xff)
            {
                a = 0xff;
            }
            else
            {
                a = Math.Max(a + a0, 0);
            }
            if ((r + r0) > 0xff)
            {
                r = 0xff;
            }
            else
            {
                r = Math.Max(r + r0, 0);
            }
            if ((g + g0) > 0xff)
            {
                g = 0xff;
            }
            else
            {
                g = Math.Max(g + g0, 0);
            }
            if ((b + b0) > 0xff)
            {
                b = 0xff;
            }
            else
            {
                b = Math.Max(b + b0, 0);
            }
            return Color.FromArgb(a, r, g, b);
        }

        public static TextFormatFlags GetTextFormatFlags(ContentAlignment alignment, bool rightToleft)
        {
            TextFormatFlags flags = TextFormatFlags.SingleLine | TextFormatFlags.WordBreak;
            if (rightToleft)
            {
                flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }
            ContentAlignment CS_0_0000 = alignment;
            if (CS_0_0000 <= ContentAlignment.MiddleCenter)
            {
                switch (CS_0_0000)
                {
                    case ContentAlignment.TopLeft:
                        return flags;

                    case ContentAlignment.TopCenter:
                        return (flags | TextFormatFlags.HorizontalCenter);

                    case (ContentAlignment.TopCenter | ContentAlignment.TopLeft):
                        return flags;

                    case ContentAlignment.TopRight:
                        return (flags | TextFormatFlags.Right);

                    case ContentAlignment.MiddleLeft:
                        return (flags | TextFormatFlags.VerticalCenter);

                    case ContentAlignment.MiddleCenter:
                        return (flags | (TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter));
                }
                return flags;
            }
            if (CS_0_0000 <= ContentAlignment.BottomLeft)
            {
                switch (CS_0_0000)
                {
                    case ContentAlignment.MiddleRight:
                        return (flags | (TextFormatFlags.VerticalCenter | TextFormatFlags.Right));

                    case ContentAlignment.BottomLeft:
                        return (flags | TextFormatFlags.Bottom);
                }
                return flags;
            }
            if (CS_0_0000 != ContentAlignment.BottomCenter)
            {
                if (CS_0_0000 != ContentAlignment.BottomRight)
                {
                    return flags;
                }
                return (flags | (TextFormatFlags.Bottom | TextFormatFlags.Right));
            }
            return (flags | (TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter));
        }

        public void Init()
        {
            base.SetStyle(ControlStyles.DoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.SetStyle(ControlStyles.StandardDoubleClick, false);
            base.SetStyle(ControlStyles.Selectable, true);
            base.UpdateStyles();
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this._controlState = CCWin.SkinClass.ControlState.Pressed;
                base.Invalidate();
                base.OnMouseDown(e);
            }
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
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this._controlState = CCWin.SkinClass.ControlState.Hover;
                base.Invalidate();
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle imageRect;
            Rectangle textRect;
            Color baseColor;
            Color borderColor;
            base.OnPaint(e);
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;
            Rectangle rc = base.ClientRectangle;
            this.CalculateRect(out imageRect, out textRect);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color innerBorderColor = Color.FromArgb(200, 0xff, 0xff, 0xff);
            Bitmap btm = null;
            int Tx = 0;
            switch (this._controlState)
            {
                case CCWin.SkinClass.ControlState.Hover:
                    btm = (Bitmap) this.MouseBack;
                    baseColor = this.GetColor(this._baseColor, 0, -13, -8, -3);
                    borderColor = this._baseColor;
                    break;

                case CCWin.SkinClass.ControlState.Pressed:
                    btm = (Bitmap) this.DownBack;
                    baseColor = this.GetColor(this._baseColor, 0, -35, -24, -9);
                    borderColor = this._baseColor;
                    Tx = 1;
                    break;

                default:
                    btm = (Bitmap) this.NormlBack;
                    baseColor = this._baseColor;
                    borderColor = this._baseColor;
                    break;
            }
            if (!base.Enabled)
            {
                baseColor = SystemColors.ControlDark;
                borderColor = SystemColors.ControlDark;
            }
            if ((btm != null) && (this.DrawType == DrawStyle.Img))
            {
                UpdateForm.CreateRegion(this, rc, this.Radius, this.RoundStyle);
                if (this.Create && (this._controlState != this.states))
                {
                    UpdateForm.CreateControlRegion(this, btm, 1);
                }
                if (this.Palace)
                {
                    CCWin.ImageDrawRect.DrawRect(g, btm, new Rectangle(rc.X, rc.Y, rc.Width, rc.Height), Rectangle.FromLTRB(this.BackRectangle.X, this.BackRectangle.Y, this.BackRectangle.Width, this.BackRectangle.Height), 1, 1);
                }
                else
                {
                    g.DrawImage(btm, 0, 0, base.Width, base.Height);
                }
            }
            else if (this.DrawType == DrawStyle.Draw)
            {
                this.RenderBackgroundInternal(g, rc, baseColor, borderColor, innerBorderColor, this.RoundStyle, this.Radius, 0.35f, true, true, LinearGradientMode.Vertical);
            }
            Image img = null;
            Size imgs = Size.Empty;
            if (base.Image != null)
            {
                if (string.IsNullOrEmpty(this.Text))
                {
                    img = base.Image;
                    imgs = new Size(img.Width, img.Height);
                    rc.Inflate(-4, -4);
                    if ((imgs.Width * imgs.Height) != 0)
                    {
                        Rectangle imgr = rc;
                        imgr = CCWin.ImageDrawRect.HAlignWithin(imgs, imgr, base.ImageAlign);
                        imgr = CCWin.ImageDrawRect.VAlignWithin(imgs, imgr, base.ImageAlign);
                        if (!base.Enabled)
                        {
                            ControlPaint.DrawImageDisabled(g, img, imgr.Left, imgr.Top, this.BackColor);
                        }
                        else
                        {
                            g.DrawImage(img, imgr.Left + Tx, imgr.Top + Tx, img.Width, img.Height);
                        }
                    }
                }
                else
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                    g.DrawImage(base.Image, imageRect, -Tx, -Tx, base.Image.Width, base.Image.Height, GraphicsUnit.Pixel);
                }
            }
            else if ((base.ImageList != null) && (base.ImageIndex != -1))
            {
                img = base.ImageList.Images[base.ImageIndex];
            }
            Color txtColor = base.Enabled ? this.ForeColor : SystemColors.ControlDark;
            TextRenderer.DrawText(g, this.Text, this.Font, textRect, txtColor, GetTextFormatFlags(this.TextAlign, this.RightToLeft == RightToLeft.Yes));
            this.states = this._controlState;
        }

        public void RenderBackgroundInternal(Graphics g, Rectangle rect, Color baseColor, Color borderColor, Color innerBorderColor, CCWin.SkinClass.RoundStyle style, int roundWidth, float basePosition, bool drawBorder, bool drawGlass, LinearGradientMode mode)
        {
            if (drawBorder)
            {
                rect.Width--;
                rect.Height--;
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Transparent, Color.Transparent, mode))
            {
                Color[] colors = new Color[] { this.GetColor(baseColor, 0, 0x23, 0x18, 9), this.GetColor(baseColor, 0, 13, 8, 3), baseColor, this.GetColor(baseColor, 0, 0x44, 0x45, 0x36) };
                ColorBlend blend = new ColorBlend();
                float[] CS_0_0000 = new float[4];
                CS_0_0000[1] = basePosition;
                CS_0_0000[2] = basePosition + 0.05f;
                CS_0_0000[3] = 1f;
                blend.Positions = CS_0_0000;
                blend.Colors = colors;
                brush.InterpolationColors = blend;
                if (style != CCWin.SkinClass.RoundStyle.None)
                {
                    using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                    {
                        g.FillPath(brush, path);
                    }
                    if (baseColor.A > 80)
                    {
                        Rectangle rectTop = rect;
                        if (mode == LinearGradientMode.Vertical)
                        {
                            rectTop.Height = (int) (rectTop.Height * basePosition);
                        }
                        else
                        {
                            rectTop.Width = (int) (rect.Width * basePosition);
                        }
                        using (GraphicsPath pathTop = GraphicsPathHelper.CreatePath(rectTop, roundWidth, CCWin.SkinClass.RoundStyle.Top, false))
                        {
                            using (SolidBrush brushAlpha = new SolidBrush(Color.FromArgb(80, 0xff, 0xff, 0xff)))
                            {
                                g.FillPath(brushAlpha, pathTop);
                            }
                        }
                    }
                    if (drawGlass)
                    {
                        RectangleF glassRect = rect;
                        if (mode == LinearGradientMode.Vertical)
                        {
                            glassRect.Y = rect.Y + (rect.Height * basePosition);
                            glassRect.Height = (rect.Height - (rect.Height * basePosition)) * 2f;
                        }
                        else
                        {
                            glassRect.X = rect.X + (rect.Width * basePosition);
                            glassRect.Width = (rect.Width - (rect.Width * basePosition)) * 2f;
                        }
                        this.DrawGlass(g, glassRect, 170, 0);
                    }
                    if (!drawBorder)
                    {
                        return;
                    }
                    using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                    {
                        using (Pen pen = new Pen(borderColor))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    rect.Inflate(-1, -1);
                    using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                    {
                        using (Pen pen = new Pen(innerBorderColor))
                        {
                            g.DrawPath(pen, path);
                        }
                        return;
                    }
                }
                g.FillRectangle(brush, rect);
                if (baseColor.A > 80)
                {
                    Rectangle rectTop = rect;
                    if (mode == LinearGradientMode.Vertical)
                    {
                        rectTop.Height = (int) (rectTop.Height * basePosition);
                    }
                    else
                    {
                        rectTop.Width = (int) (rect.Width * basePosition);
                    }
                    using (SolidBrush brushAlpha = new SolidBrush(Color.FromArgb(80, 0xff, 0xff, 0xff)))
                    {
                        g.FillRectangle(brushAlpha, rectTop);
                    }
                }
                if (drawGlass)
                {
                    RectangleF glassRect = rect;
                    if (mode == LinearGradientMode.Vertical)
                    {
                        glassRect.Y = rect.Y + (rect.Height * basePosition);
                        glassRect.Height = (rect.Height - (rect.Height * basePosition)) * 2f;
                    }
                    else
                    {
                        glassRect.X = rect.X + (rect.Width * basePosition);
                        glassRect.Width = (rect.Width - (rect.Width * basePosition)) * 2f;
                    }
                    this.DrawGlass(g, glassRect, 200, 0);
                }
                if (drawBorder)
                {
                    using (Pen pen = new Pen(borderColor))
                    {
                        g.DrawRectangle(pen, rect);
                    }
                    rect.Inflate(-1, -1);
                    using (Pen pen = new Pen(innerBorderColor))
                    {
                        g.DrawRectangle(pen, rect);
                    }
                }
            }
        }

        [Category("Skin"), DefaultValue(typeof(Rectangle), "10,10,10,10"), Description("九宫绘画区域")]
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

        [Category("Skin"), Description("非图片绘制时Bottom色调"), DefaultValue(typeof(Color), "51, 161, 224")]
        public Color BaseColor
        {
            get
            {
                return this._baseColor;
            }
            set
            {
                this._baseColor = value;
                base.Invalidate();
            }
        }

        [Description("控件状态")]
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

        [DefaultValue(typeof(bool), "false"), Description("是否开启:根据所绘图限制控件范围"), Category("Skin")]
        public bool Create
        {
            get
            {
                return this.create;
            }
            set
            {
                if (this.create != value)
                {
                    this.create = value;
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

        [DefaultValue(typeof(DrawStyle), "2"), Category("Skin"), Description("按钮的绘画模式")]
        public DrawStyle DrawType
        {
            get
            {
                return this.drawType;
            }
            set
            {
                if (this.drawType != value)
                {
                    this.drawType = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Skin"), DefaultValue(0x12), Description("设置或获取图像的大小")]
        public int ImageWidth
        {
            get
            {
                return this._imageWidth;
            }
            set
            {
                if (value != this._imageWidth)
                {
                    this._imageWidth = (value < 12) ? 12 : value;
                    base.Invalidate();
                }
            }
        }

        [Category("MouseEnter"), Description("悬浮时背景")]
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

        [DefaultValue(typeof(bool), "false"), Category("Skin"), Description("是否开启九宫绘图")]
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

        [Description("圆角大小"), DefaultValue(typeof(int), "8"), Category("Skin")]
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
                    this.radius = (value < 4) ? 4 : value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(CCWin.SkinClass.RoundStyle), "0"), Category("Skin"), Description("设置或获取按钮圆角的样式")]
        public CCWin.SkinClass.RoundStyle RoundStyle
        {
            get
            {
                return this._roundStyle;
            }
            set
            {
                if (this._roundStyle != value)
                {
                    this._roundStyle = value;
                    base.Invalidate();
                }
            }
        }
    }
}

