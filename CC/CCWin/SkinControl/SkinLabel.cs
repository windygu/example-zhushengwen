namespace CCWin.SkinControl
{
    using CCWin.SkinClass;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(Label))]
    public class SkinLabel : Label
    {
        private CCWin.SkinControl.ArtTextStyle _artTextStyle = CCWin.SkinControl.ArtTextStyle.Border;
        private Color _borderColor = Color.White;
        private int _borderSize = 1;

        public SkinLabel()
        {
            this.SetStyles();
            this.Font = new Font("微软雅黑", 9f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
        }

        private PointF CalculateRenderTextStartPoint(Graphics g)
        {
            PointF point = PointF.Empty;
            SizeF textSize = g.MeasureString(base.Text, base.Font, PointF.Empty, StringFormat.GenericTypographic);
            if (this.AutoSize)
            {
                point.X = base.Padding.Left;
                point.Y = base.Padding.Top;
                return point;
            }
            ContentAlignment align = base.TextAlign;
            switch (align)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft:
                    point.X = base.Padding.Left;
                    break;

                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                    point.X = (base.Width - textSize.Width) / 2f;
                    break;

                default:
                    point.X = base.Width - (base.Padding.Right + textSize.Width);
                    break;
            }
            switch (align)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    point.Y = base.Padding.Top;
                    return point;

                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    point.Y = (base.Height - textSize.Height) / 2f;
                    return point;
            }
            point.Y = base.Height - (base.Padding.Bottom + textSize.Height);
            return point;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.ArtTextStyle == CCWin.SkinControl.ArtTextStyle.None)
            {
                base.OnPaint(e);
            }
            else if (base.Text.Length != 0)
            {
                this.RenderText(e.Graphics);
            }
        }

        private void RenderAnamorphosisText(Graphics g, PointF point)
        {
            using (new SolidBrush(base.ForeColor))
            {
                Rectangle rc = new Rectangle(new Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)), base.ClientRectangle.Size);
                Image img = UpdateForm.ImageLightEffect(this.Text, base.Font, this.ForeColor, this.BorderColor, this.BorderSize, rc, !this.AutoSize);
                g.DrawImage(img, (float) (point.X - (this.BorderSize / 2)), (float) (point.Y - (this.BorderSize / 2)));
            }
        }

        private void RenderBordText(Graphics g, PointF point)
        {
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.Trimming = this.AutoSize ? StringTrimming.None : StringTrimming.EllipsisWord;
            Rectangle rc = new Rectangle(new Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)), base.ClientRectangle.Size);
            using (Brush brush = new SolidBrush(this._borderColor))
            {
                for (int i = 1; i <= this._borderSize; i++)
                {
                    g.DrawString(this.Text, base.Font, brush, new Rectangle(new Point(Convert.ToInt32((float) (point.X - i)), Convert.ToInt32(point.Y)), base.ClientRectangle.Size), sf);
                    g.DrawString(this.Text, base.Font, brush, new Rectangle(new Point(Convert.ToInt32(point.X), Convert.ToInt32((float) (point.Y - i))), base.ClientRectangle.Size), sf);
                    g.DrawString(this.Text, base.Font, brush, new Rectangle(new Point(Convert.ToInt32((float) (point.X + i)), Convert.ToInt32(point.Y)), base.ClientRectangle.Size), sf);
                    g.DrawString(this.Text, base.Font, brush, new Rectangle(new Point(Convert.ToInt32(point.X), Convert.ToInt32((float) (point.Y + i))), base.ClientRectangle.Size), sf);
                }
            }
            using (Brush brush = new SolidBrush(base.ForeColor))
            {
                g.DrawString(this.Text, base.Font, brush, rc, sf);
            }
        }

        private void RenderFormeText(Graphics g, PointF point)
        {
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.Trimming = this.AutoSize ? StringTrimming.None : StringTrimming.EllipsisWord;
            Rectangle rc = new Rectangle(new Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)), base.ClientRectangle.Size);
            using (Brush brush = new SolidBrush(this._borderColor))
            {
                for (int i = 1; i <= this._borderSize; i++)
                {
                    g.DrawString(this.Text, base.Font, brush, new Rectangle(new Point(Convert.ToInt32((float) (point.X - i)), Convert.ToInt32((float) (point.Y + i))), base.ClientRectangle.Size), sf);
                }
            }
            using (Brush brush = new SolidBrush(base.ForeColor))
            {
                g.DrawString(this.Text, this.Font, brush, rc, sf);
            }
        }

        private void RenderRelievoText(Graphics g, PointF point)
        {
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.Trimming = this.AutoSize ? StringTrimming.None : StringTrimming.EllipsisWord;
            Rectangle rc = new Rectangle(new Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y)), base.ClientRectangle.Size);
            using (Brush brush = new SolidBrush(this._borderColor))
            {
                for (int i = 1; i <= this._borderSize; i++)
                {
                    g.DrawString(this.Text, base.Font, brush, new Rectangle(new Point(Convert.ToInt32((float) (point.X + i)), Convert.ToInt32(point.Y)), base.ClientRectangle.Size), sf);
                    g.DrawString(this.Text, base.Font, brush, new Rectangle(new Point(Convert.ToInt32(point.X), Convert.ToInt32((float) (point.Y + i))), base.ClientRectangle.Size), sf);
                }
            }
            using (Brush brush = new SolidBrush(base.ForeColor))
            {
                g.DrawString(this.Text, base.Font, brush, rc, sf);
            }
        }

        private void RenderText(Graphics g)
        {
            using (new CCWin.SkinControl.TextRenderingHintGraphics(g))
            {
                PointF point = this.CalculateRenderTextStartPoint(g);
                switch (this._artTextStyle)
                {
                    case CCWin.SkinControl.ArtTextStyle.Border:
                        this.RenderBordText(g, point);
                        return;

                    case CCWin.SkinControl.ArtTextStyle.Relievo:
                        this.RenderRelievoText(g, point);
                        return;

                    case CCWin.SkinControl.ArtTextStyle.Forme:
                        this.RenderFormeText(g, point);
                        return;

                    case CCWin.SkinControl.ArtTextStyle.Anamorphosis:
                        this.RenderAnamorphosisText(g, point);
                        return;
                }
            }
        }

        public string SetStrLeng(string txt, Font font, int width)
        {
            for (Size sizef = TextRenderer.MeasureText(txt, font); (sizef.Width > width) && (txt.Length != 0); sizef = TextRenderer.MeasureText(txt, font))
            {
                txt = txt.Substring(0, txt.Length - 1);
            }
            return txt;
        }

        private void SetStyles()
        {
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.ResizeRedraw = true;
            this.BackColor = Color.Transparent;
            base.UpdateStyles();
        }

        [Browsable(true), Description("字体样式（None:正常样式,Border:边框样式,Relievo:浮雕样式,Forme:印版样式,Anamorphosis:渐变样式）"), Category("Skin"), DefaultValue(typeof(CCWin.SkinControl.ArtTextStyle), "1")]
        public CCWin.SkinControl.ArtTextStyle ArtTextStyle
        {
            get
            {
                return this._artTextStyle;
            }
            set
            {
                if (this._artTextStyle != value)
                {
                    this._artTextStyle = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Color), "80, 0, 0, 0"), Browsable(true), Description("样式效果颜色"), Category("Skin")]
        public Color BorderColor
        {
            get
            {
                return this._borderColor;
            }
            set
            {
                if (this._borderColor != value)
                {
                    this._borderColor = value;
                    base.Invalidate();
                }
            }
        }

        [Description("样式效果宽度"), Browsable(true), Category("Skin"), DefaultValue(1)]
        public int BorderSize
        {
            get
            {
                return this._borderSize;
            }
            set
            {
                if (this._borderSize != value)
                {
                    this._borderSize = value;
                    base.Invalidate();
                }
            }
        }
    }
}

