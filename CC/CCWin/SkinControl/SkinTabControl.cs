namespace CCWin.SkinControl
{
    using CCWin.Properties;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(TabControl))]
    public class SkinTabControl : TabControl
    {
        private Color _backColor = Color.Transparent;
        private Color _baseColor = Color.White;
        private Color _borderColor = Color.White;
        private Rectangle _btnArrowRect = Rectangle.Empty;
        private bool _isFocus;
        private Color _pageColor = Color.White;
        private Image _titleBackground = Resources.main_tab_highlighttwo;
        private Image Icon;

        public SkinTabControl()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.SizeMode = TabSizeMode.Fixed;
            base.ItemSize = new Size(70, 0x24);
            base.UpdateStyles();
        }

        private void contextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this._isFocus = false;
            base.Invalidate(this._btnArrowRect);
        }

        private void DrawBackground(Graphics g)
        {
            int width = base.ClientRectangle.Width;
            int height = base.ClientRectangle.Height;
            int num3 = this.DisplayRectangle.Height;
            Color backColor = base.Enabled ? this._backColor : SystemColors.Control;
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, base.ClientRectangle);
            }
        }

        private void DrawImage(Graphics g, Image image, Rectangle rect)
        {
            g.DrawImage(image, new Rectangle(rect.X, rect.Y, 5, rect.Height), 0, 0, 5, image.Height, GraphicsUnit.Pixel);
            g.DrawImage(image, new Rectangle(rect.X + 5, rect.Y, rect.Width - 10, rect.Height), 5, 0, image.Width - 10, image.Height, GraphicsUnit.Pixel);
            g.DrawImage(image, new Rectangle((rect.X + rect.Width) - 5, rect.Y, 5, rect.Height), image.Width - 5, 0, 5, image.Height, GraphicsUnit.Pixel);
        }

        private void DrawTabPages(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(this._pageColor))
            {
                int x = 2;
                int y = base.ItemSize.Height;
                int width = base.Width - 2;
                int height = base.Height - base.ItemSize.Height;
                g.FillRectangle(brush, x, y, width, height);
                g.DrawRectangle(new Pen(this._borderColor), x, y, width - 1, height - 1);
            }
            Rectangle tabRect = Rectangle.Empty;
            Point cursorPoint = base.PointToClient(Control.MousePosition);
            for (int i = 0; i < base.TabCount; i++)
            {
                TabPage page = base.TabPages[i];
                tabRect = base.GetTabRect(i);
                Color yellow = Color.Yellow;
                this.Icon = ((base.TabPages[i].ImageIndex != -1) && (base.ImageList != null)) ? base.ImageList.Images[base.TabPages[i].ImageIndex] : null;
                Image baseTabHeaderImage = null;
                Image btnArrowImage = null;
                if (base.SelectedIndex == i)
                {
                    baseTabHeaderImage = Resources.tab_dots_down;
                    Point contextMenuLocation = base.PointToScreen(new Point(this._btnArrowRect.Left, (this._btnArrowRect.Top + this._btnArrowRect.Height) + 5));
                    ContextMenuStrip contextMenuStrip = base.TabPages[i].ContextMenuStrip;
                    if (contextMenuStrip != null)
                    {
                        contextMenuStrip.Closed -= new ToolStripDropDownClosedEventHandler(this.contextMenuStrip_Closed);
                        contextMenuStrip.Closed += new ToolStripDropDownClosedEventHandler(this.contextMenuStrip_Closed);
                        if ((contextMenuLocation.X + contextMenuStrip.Width) > (Screen.PrimaryScreen.WorkingArea.Width - 20))
                        {
                            contextMenuLocation.X = (Screen.PrimaryScreen.WorkingArea.Width - contextMenuStrip.Width) - 50;
                        }
                        if (tabRect.Contains(cursorPoint))
                        {
                            if (this._isFocus)
                            {
                                btnArrowImage = Resources.tab_dots_down;
                                contextMenuStrip.Show(contextMenuLocation);
                            }
                            else
                            {
                                btnArrowImage = Resources.tab_dots_normal;
                            }
                            this._btnArrowRect = new Rectangle((tabRect.X + tabRect.Width) - btnArrowImage.Width, tabRect.Y, btnArrowImage.Width, btnArrowImage.Height);
                        }
                        else if (this._isFocus)
                        {
                            btnArrowImage = Resources.tab_dots_down;
                            contextMenuStrip.Show(contextMenuLocation);
                        }
                    }
                }
                else if (tabRect.Contains(cursorPoint))
                {
                    baseTabHeaderImage = Resources.tab_dots_mouseover;
                }
                if (baseTabHeaderImage != null)
                {
                    if (base.SelectedIndex == i)
                    {
                        if (base.SelectedIndex == (base.TabCount - 1))
                        {
                            tabRect.Inflate(2, 0);
                        }
                        else
                        {
                            tabRect.Inflate(1, 0);
                        }
                    }
                    this.DrawImage(g, baseTabHeaderImage, tabRect);
                    if (btnArrowImage != null)
                    {
                        g.DrawImage(btnArrowImage, this._btnArrowRect);
                    }
                }
                if (this.Icon != null)
                {
                    g.DrawImage(this.Icon, (int) (tabRect.X + ((tabRect.Width - this.Icon.Width) / 2)), (int) (tabRect.Y + ((tabRect.Height - this.Icon.Height) / 2)));
                }
                TextRenderer.DrawText(g, page.Text, page.Font, tabRect, page.ForeColor);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if ((!base.DesignMode && (e.Button == MouseButtons.Left)) && this._btnArrowRect.Contains(e.Location))
            {
                this._isFocus = true;
                base.Invalidate(this._btnArrowRect);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            this.DrawBackground(g);
            this.DrawTabPages(g);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 0x7b)
            {
                base.WndProc(ref m);
            }
        }

        [Browsable(true), DefaultValue(typeof(Color), "Transparent"), Category("Skin")]
        public override Color BackColor
        {
            get
            {
                return this._backColor;
            }
            set
            {
                this._backColor = value;
                base.Invalidate(true);
            }
        }

        [Category("Skin"), DefaultValue(typeof(Color), "102, 180, 228")]
        public Color BaseColor
        {
            get
            {
                return this._baseColor;
            }
            set
            {
                this._baseColor = value;
                base.Invalidate(true);
            }
        }

        [DefaultValue(typeof(Color), "102, 180, 228"), Category("Skin")]
        public Color BorderColor
        {
            get
            {
                return this._borderColor;
            }
            set
            {
                this._borderColor = value;
                base.Invalidate(true);
            }
        }

        [Category("Skin"), Description("所有TabPage的背景颜色")]
        public Color PageColor
        {
            get
            {
                return this._pageColor;
            }
            set
            {
                this._pageColor = value;
                if (base.TabPages.Count > 0)
                {
                    for (int i = 0; i < base.TabPages.Count; i++)
                    {
                        base.TabPages[i].BackColor = this._pageColor;
                    }
                }
                base.Invalidate(true);
            }
        }
    }
}

