namespace CCWin.SkinControl
{
    using CCWin.SkinClass;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(RadioButton))]
    public class SkinRadioButton : RadioButton
    {
        private Color _baseColor = Color.FromArgb(0x33, 0xa1, 0xe0);
        private CCWin.SkinClass.ControlState _controlState;
        private int defaultradiobuttonwidth = 12;
        private Image downback;
        private static readonly ContentAlignment LeftAligbment = (ContentAlignment.BottomLeft | ContentAlignment.MiddleLeft | ContentAlignment.TopLeft);
        private bool lighteffect = true;
        private Color lighteffectback = Color.White;
        private int lighteffectWidth = 4;
        private Image mouseback;
        private Image normlback;
        private static readonly ContentAlignment RightAlignment = (ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight);
        private Image selectedmouseback;
        private Image selectedownback;
        private Image selectenormlback;

        public SkinRadioButton()
        {
            this.Init();
            this.BackColor = Color.Transparent;
            this.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
        }

        private void CalculateRect(out Rectangle radioButtonRect, out Rectangle textRect)
        {
            radioButtonRect = new Rectangle(0, 0, this.DefaultRadioButtonWidth, this.DefaultRadioButtonWidth);
            textRect = Rectangle.Empty;
            bool bCheckAlignLeft = (LeftAligbment & base.CheckAlign) != ((ContentAlignment) 0);
            bool bCheckAlignRight = (RightAlignment & base.CheckAlign) != ((ContentAlignment) 0);
            bool bRightToLeft = this.RightToLeft == RightToLeft.Yes;
            if ((!bCheckAlignLeft || bRightToLeft) && (!bCheckAlignRight || !bRightToLeft))
            {
                if ((!bCheckAlignRight || bRightToLeft) && (!bCheckAlignLeft || !bRightToLeft))
                {
                    switch (base.CheckAlign)
                    {
                        case ContentAlignment.TopCenter:
                            radioButtonRect.Y = 2;
                            textRect.Y = radioButtonRect.Bottom + 2;
                            textRect.Height = (base.Height - this.DefaultRadioButtonWidth) - 6;
                            break;

                        case ContentAlignment.MiddleCenter:
                            radioButtonRect.Y = (base.Height - this.DefaultRadioButtonWidth) / 2;
                            textRect.Y = 0;
                            textRect.Height = base.Height;
                            break;

                        case ContentAlignment.BottomCenter:
                            radioButtonRect.Y = (base.Height - this.DefaultRadioButtonWidth) - 2;
                            textRect.Y = 0;
                            textRect.Height = (base.Height - this.DefaultRadioButtonWidth) - 6;
                            break;
                    }
                    radioButtonRect.X = (base.Width - this.DefaultRadioButtonWidth) / 2;
                    textRect.X = 2;
                    textRect.Width = base.Width - 4;
                    return;
                }
                switch (base.CheckAlign)
                {
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopRight:
                        radioButtonRect.Y = 2;
                        goto Label_017F;

                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        radioButtonRect.Y = (base.Height - this.DefaultRadioButtonWidth) / 2;
                        goto Label_017F;

                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        radioButtonRect.Y = (base.Height - this.DefaultRadioButtonWidth) - 2;
                        goto Label_017F;
                }
            }
            else
            {
                switch (base.CheckAlign)
                {
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopRight:
                        radioButtonRect.Y = 2;
                        break;

                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        radioButtonRect.Y = (base.Height - this.DefaultRadioButtonWidth) / 2;
                        break;

                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        radioButtonRect.Y = (base.Height - this.DefaultRadioButtonWidth) - 2;
                        break;
                }
                radioButtonRect.X = 1;
                textRect = new Rectangle(radioButtonRect.Right + 2, 0, (base.Width - radioButtonRect.Right) - 4, base.Height);
                return;
            }
        Label_017F:
            radioButtonRect.X = (base.Width - this.DefaultRadioButtonWidth) - 1;
            textRect = new Rectangle(2, 0, (base.Width - this.DefaultRadioButtonWidth) - 6, base.Height);
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
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.UpdateStyles();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
            {
                this.ControlState = CCWin.SkinClass.ControlState.Pressed;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.ControlState = CCWin.SkinClass.ControlState.Hover;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.ControlState = CCWin.SkinClass.ControlState.Normal;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
            {
                if (base.ClientRectangle.Contains(e.Location))
                {
                    this.ControlState = CCWin.SkinClass.ControlState.Hover;
                }
                else
                {
                    this.ControlState = CCWin.SkinClass.ControlState.Normal;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle radioButtonrect;
            Rectangle textRect;
            Color borderColor;
            Color innerBorderColor;
            Color checkColor;
            Color textColor;
            base.OnPaint(e);
            base.OnPaintBackground(e);
            Graphics g = e.Graphics;
            this.CalculateRect(out radioButtonrect, out textRect);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Bitmap btm = null;
            bool hover = false;
            if (base.Enabled)
            {
                switch (this.ControlState)
                {
                    case CCWin.SkinClass.ControlState.Hover:
                        borderColor = this._baseColor;
                        innerBorderColor = this._baseColor;
                        checkColor = this.GetColor(this._baseColor, 0, 0x23, 0x18, 9);
                        btm = base.Checked ? ((Bitmap) this.SelectedMouseBack) : ((Bitmap) this.MouseBack);
                        hover = true;
                        goto Label_016A;

                    case CCWin.SkinClass.ControlState.Pressed:
                        borderColor = this._baseColor;
                        innerBorderColor = this.GetColor(this._baseColor, 0, -13, -8, -3);
                        checkColor = this.GetColor(this._baseColor, 0, -35, -24, -9);
                        btm = base.Checked ? ((Bitmap) this.SelectedDownBack) : ((Bitmap) this.DownBack);
                        hover = true;
                        goto Label_016A;
                }
                borderColor = this._baseColor;
                innerBorderColor = Color.Empty;
                checkColor = this._baseColor;
                btm = base.Checked ? ((Bitmap) this.SelectedNormlBack) : ((Bitmap) this.NormlBack);
            }
            else
            {
                borderColor = SystemColors.ControlDark;
                innerBorderColor = SystemColors.ControlDark;
                checkColor = SystemColors.ControlDark;
                btm = base.Checked ? ((Bitmap) this.SelectedNormlBack) : ((Bitmap) this.NormlBack);
            }
        Label_016A:
            if (btm == null)
            {
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(brush, radioButtonrect);
                }
                if (hover)
                {
                    using (Pen pen = new Pen(innerBorderColor, 2f))
                    {
                        g.DrawEllipse(pen, radioButtonrect);
                    }
                }
                if (base.Checked)
                {
                    radioButtonrect.Inflate(-2, -2);
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddEllipse(radioButtonrect);
                        using (PathGradientBrush brush = new PathGradientBrush(path))
                        {
                            brush.CenterColor = checkColor;
                            brush.SurroundColors = new Color[] { Color.White };
                            Blend blend = new Blend();
                            float[] CS_0_0002 = new float[3];
                            CS_0_0002[1] = 0.4f;
                            CS_0_0002[2] = 1f;
                            blend.Positions = CS_0_0002;
                            float[] CS_0_0003 = new float[3];
                            CS_0_0003[1] = 0.4f;
                            CS_0_0003[2] = 1f;
                            blend.Factors = CS_0_0003;
                            brush.Blend = blend;
                            g.FillEllipse(brush, radioButtonrect);
                        }
                    }
                    radioButtonrect.Inflate(2, 2);
                }
                using (Pen pen = new Pen(borderColor))
                {
                    g.DrawEllipse(pen, radioButtonrect);
                    goto Label_02C2;
                }
            }
            g.DrawImage(btm, radioButtonrect);
        Label_02C2:
            textColor = base.Enabled ? this.ForeColor : SystemColors.GrayText;
            if (this.LightEffect)
            {
                Image imgText = UpdateForm.ImageLightEffect(this.Text, this.Font, textColor, this.LightEffectBack, this.LightEffectWidth);
                g.DrawImage(imgText, textRect);
            }
            else
            {
                TextRenderer.DrawText(g, this.Text, this.Font, textRect, textColor, GetTextFormatFlags(this.TextAlign, this.RightToLeft == RightToLeft.Yes));
            }
        }

        [Category("Skin"), DefaultValue(typeof(Color), "51, 161, 224"), Description("非图片绘制时RadioButton色调")]
        public Color BaseColor
        {
            get
            {
                return this._baseColor;
            }
            set
            {
                if (this._baseColor != value)
                {
                    this._baseColor = value;
                    base.Invalidate();
                }
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

        [Description("选择框大小"), Category("Skin"), DefaultValue(12)]
        public int DefaultRadioButtonWidth
        {
            get
            {
                return this.defaultradiobuttonwidth;
            }
            set
            {
                if (this.defaultradiobuttonwidth != value)
                {
                    this.defaultradiobuttonwidth = value;
                    base.Invalidate();
                }
            }
        }

        [Description("点击时图像"), Category("MouseDown")]
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

        [Description("是否绘制发光字体"), DefaultValue(typeof(bool), "true"), Category("Skin")]
        public bool LightEffect
        {
            get
            {
                return this.lighteffect;
            }
            set
            {
                if (this.lighteffect != value)
                {
                    this.lighteffect = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Skin"), DefaultValue(typeof(Color), "White"), Description("发光字体背景色")]
        public Color LightEffectBack
        {
            get
            {
                return this.lighteffectback;
            }
            set
            {
                if (this.lighteffectback != value)
                {
                    this.lighteffectback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("光圈大小"), DefaultValue(typeof(int), "4"), Category("Skin")]
        public int LightEffectWidth
        {
            get
            {
                return this.lighteffectWidth;
            }
            set
            {
                if (this.lighteffectWidth != value)
                {
                    this.lighteffectWidth = value;
                    base.Invalidate();
                }
            }
        }

        [Description("悬浮时图像"), Category("MouseEnter")]
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

        [Description("初始时图像"), Category("MouseNorml")]
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

        [Category("MouseDown"), Description("选中点击时图像")]
        public Image SelectedDownBack
        {
            get
            {
                return this.selectedownback;
            }
            set
            {
                if (this.selectedownback != value)
                {
                    this.selectedownback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("选中悬浮时图像"), Category("MouseEnter")]
        public Image SelectedMouseBack
        {
            get
            {
                return this.selectedmouseback;
            }
            set
            {
                if (this.selectedmouseback != value)
                {
                    this.selectedmouseback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("选中初始时图像"), Category("MouseNorml")]
        public Image SelectedNormlBack
        {
            get
            {
                return this.selectenormlback;
            }
            set
            {
                if (this.selectenormlback != value)
                {
                    this.selectenormlback = value;
                    base.Invalidate();
                }
            }
        }
    }
}

