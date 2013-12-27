namespace CCWin.SkinControl
{
    using CCWin;
    using CCWin.SkinClass;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class ProfessionalToolStripRendererEx : ToolStripRenderer
    {
        private ToolStripColorTable _colorTable;

        public ProfessionalToolStripRendererEx()
        {
            this.ColorTable = new ToolStripColorTable();
        }

        public ProfessionalToolStripRendererEx(ToolStripColorTable colorTable)
        {
            this.ColorTable = colorTable;
        }

        private void DrawCircle(Graphics g, Rectangle bounds, Color borderColor, Color fillColor)
        {
            using (GraphicsPath circlePath = new GraphicsPath())
            {
                circlePath.AddEllipse(bounds);
                circlePath.CloseFigure();
                using (Pen borderPen = new Pen(borderColor))
                {
                    g.DrawPath(borderPen, circlePath);
                }
                using (Brush backBrush = new SolidBrush(fillColor))
                {
                    g.FillPath(backBrush, circlePath);
                }
            }
        }

        private void DrawDottedGrip(Graphics g, Rectangle bounds, bool vertical, bool largeDot, Color innerColor, Color outerColor)
        {
            bounds.Height -= 3;
            Point position = new Point(bounds.X, bounds.Y);
            Rectangle posRect = new Rectangle(0, 0, 2, 2);
            using (new SmoothingModeGraphics(g))
            {
                int sep;
                IntPtr hdc;
                if (vertical)
                {
                    sep = bounds.Height;
                    position.Y += 8;
                    for (int i = 0; position.Y > 4; i += 4)
                    {
                        position.Y = sep - (2 + i);
                        if (largeDot)
                        {
                            posRect.Location = position;
                            this.DrawCircle(g, posRect, outerColor, innerColor);
                        }
                        else
                        {
                            int innerWin32Corlor = ColorTranslator.ToWin32(innerColor);
                            int outerWin32Corlor = ColorTranslator.ToWin32(outerColor);
                            hdc = g.GetHdc();
                            SetPixel(hdc, position.X, position.Y, innerWin32Corlor);
                            SetPixel(hdc, position.X + 1, position.Y, outerWin32Corlor);
                            SetPixel(hdc, position.X, position.Y + 1, outerWin32Corlor);
                            SetPixel(hdc, position.X + 3, position.Y, innerWin32Corlor);
                            SetPixel(hdc, position.X + 4, position.Y, outerWin32Corlor);
                            SetPixel(hdc, position.X + 3, position.Y + 1, outerWin32Corlor);
                            g.ReleaseHdc(hdc);
                        }
                    }
                }
                else
                {
                    bounds.Inflate(-2, 0);
                    sep = bounds.Width;
                    position.X += 2;
                    for (int i = 1; position.X > 0; i += 4)
                    {
                        position.X = sep - (2 + i);
                        if (largeDot)
                        {
                            posRect.Location = position;
                            this.DrawCircle(g, posRect, outerColor, innerColor);
                        }
                        else
                        {
                            int innerWin32Corlor = ColorTranslator.ToWin32(innerColor);
                            int outerWin32Corlor = ColorTranslator.ToWin32(outerColor);
                            hdc = g.GetHdc();
                            SetPixel(hdc, position.X, position.Y, innerWin32Corlor);
                            SetPixel(hdc, position.X + 1, position.Y, outerWin32Corlor);
                            SetPixel(hdc, position.X, position.Y + 1, outerWin32Corlor);
                            SetPixel(hdc, position.X + 3, position.Y, innerWin32Corlor);
                            SetPixel(hdc, position.X + 4, position.Y, outerWin32Corlor);
                            SetPixel(hdc, position.X + 3, position.Y + 1, outerWin32Corlor);
                            g.ReleaseHdc(hdc);
                        }
                    }
                }
            }
        }

        private void DrawDottedStatusGrip(Graphics g, Rectangle bounds, Color innerColor, Color outerColor)
        {
            Rectangle shape = new Rectangle(0, 0, 2, 2);
            shape.X = bounds.Width - 0x11;
            shape.Y = bounds.Height - 8;
            using (new SmoothingModeGraphics(g))
            {
                this.DrawCircle(g, shape, outerColor, innerColor);
                shape.X = bounds.Width - 12;
                this.DrawCircle(g, shape, outerColor, innerColor);
                shape.X = bounds.Width - 7;
                this.DrawCircle(g, shape, outerColor, innerColor);
                shape.Y = bounds.Height - 13;
                this.DrawCircle(g, shape, outerColor, innerColor);
                shape.Y = bounds.Height - 0x12;
                this.DrawCircle(g, shape, outerColor, innerColor);
                shape.Y = bounds.Height - 13;
                shape.X = bounds.Width - 12;
                this.DrawCircle(g, shape, outerColor, innerColor);
            }
        }

        private void DrawSolidStatusGrip(Graphics g, Rectangle bounds, Color innerColor, Color outerColor)
        {
            using (new SmoothingModeGraphics(g))
            {
                using (Pen innerPen = new Pen(innerColor))
                {
                    using (Pen outerPen = new Pen(outerColor))
                    {
                        g.DrawLine(outerPen, new Point(bounds.Width - 14, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 0x10));
                        g.DrawLine(innerPen, new Point(bounds.Width - 13, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 15));
                        g.DrawLine(outerPen, new Point(bounds.Width - 12, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 14));
                        g.DrawLine(innerPen, new Point(bounds.Width - 11, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 13));
                        g.DrawLine(outerPen, new Point(bounds.Width - 10, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 12));
                        g.DrawLine(innerPen, new Point(bounds.Width - 9, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 11));
                        g.DrawLine(outerPen, new Point(bounds.Width - 8, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 10));
                        g.DrawLine(innerPen, new Point(bounds.Width - 7, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 9));
                        g.DrawLine(outerPen, new Point(bounds.Width - 6, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 8));
                        g.DrawLine(innerPen, new Point(bounds.Width - 5, bounds.Height - 6), new Point(bounds.Width - 4, bounds.Height - 7));
                    }
                }
            }
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (e.Item.Enabled)
            {
                e.ArrowColor = this.ColorTable.Arrow;
            }
            if ((e.Item.Owner is ToolStripDropDown) && (e.Item is ToolStripMenuItem))
            {
                Rectangle rect = e.ArrowRectangle;
                e.ArrowRectangle = rect;
            }
            base.OnRenderArrow(e);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripButton item = e.Item as ToolStripButton;
            Graphics g = e.Graphics;
            if (item == null)
            {
                return;
            }
            LinearGradientMode mode = (toolStrip.Orientation == Orientation.Horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
            SmoothingModeGraphics sg = new SmoothingModeGraphics(g);
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
            if (item.BackgroundImage != null)
            {
                Rectangle clipRect = item.Selected ? item.ContentRectangle : bounds;
                CCWin.SkinControl.ControlPaintEx.DrawBackgroundImage(g, item.BackgroundImage, this.ColorTable.Back, item.BackgroundImageLayout, bounds, clipRect);
            }
            Bitmap btm = null;
            Color color = Color.Red;
            if (item.CheckState == CheckState.Unchecked)
            {
                if (item.Selected)
                {
                     btm = item.Pressed ? ((Bitmap) this.ColorTable.BaseItemDown) : ((Bitmap) this.ColorTable.BaseItemMouse);
                    if (btm != null)
                    {
                        CCWin.ImageDrawRect.DrawRect(g, btm, bounds, Rectangle.FromLTRB(this.ColorTable.BackRectangle.X, this.ColorTable.BackRectangle.Y, this.ColorTable.BackRectangle.Width, this.ColorTable.BackRectangle.Height), 1, 1);
                    }
                    else
                    {
                        color = this.ColorTable.BaseItemHover;
                        if (item.Pressed)
                        {
                            color = this.ColorTable.BaseItemPressed;
                        }
                        RenderHelperStrip.RenderBackgroundInternal(g, bounds, color, this.ColorTable.BaseItemBorder, this.ColorTable.Back, this.ColorTable.BaseItemRadiusStyle, this.ColorTable.BaseItemRadius, this.ColorTable.BaseItemBorderShow, this.ColorTable.BaseItemAnamorphosis, mode);
                    }
                    goto Label_0302;
                }
                if (!(toolStrip is ToolStripOverflow))
                {
                    goto Label_0302;
                }
                using (Brush brush = new SolidBrush(this.ColorTable.ItemHover))
                {
                    g.FillRectangle(brush, bounds);
                    goto Label_0302;
                }
            }
             btm = (Bitmap)this.ColorTable.BaseItemMouse;
             color = ControlPaint.Light(this.ColorTable.ItemHover);
            if (item.Selected)
            {
                color = this.ColorTable.ItemHover;
                btm = (Bitmap) this.ColorTable.BaseItemMouse;
            }
            if (item.Pressed)
            {
                color = this.ColorTable.ItemPressed;
                btm = (Bitmap) this.ColorTable.BaseItemDown;
            }
            if (btm == null)
            {
                RenderHelperStrip.RenderBackgroundInternal(g, bounds, color, this.ColorTable.BaseItemBorder, this.ColorTable.Back, this.ColorTable.BaseItemRadiusStyle, this.ColorTable.BaseItemRadius, this.ColorTable.BaseItemBorderShow, this.ColorTable.BaseItemAnamorphosis, mode);
            }
            else
            {
                CCWin.ImageDrawRect.DrawRect(g, btm, bounds, Rectangle.FromLTRB(this.ColorTable.BackRectangle.X, this.ColorTable.BackRectangle.Y, this.ColorTable.BackRectangle.Width, this.ColorTable.BackRectangle.Height), 1, 1);
            }
        Label_0302:
            sg.Dispose();
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripDropDownItem item = e.Item as ToolStripDropDownItem;
            if (item == null)
            {
                return;
            }
            LinearGradientMode mode = (toolStrip.Orientation == Orientation.Horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
            Graphics g = e.Graphics;
            SmoothingModeGraphics sg = new SmoothingModeGraphics(g);
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
            if (item.Pressed && item.HasDropDownItems)
            {
                if (this.ColorTable.BaseItemDown != null)
                {
                    CCWin.ImageDrawRect.DrawRect(g, (Bitmap) this.ColorTable.BaseItemDown, bounds, Rectangle.FromLTRB(this.ColorTable.BackRectangle.X, this.ColorTable.BackRectangle.Y, this.ColorTable.BackRectangle.Width, this.ColorTable.BackRectangle.Height), 1, 1);
                }
                else
                {
                    RenderHelperStrip.RenderBackgroundInternal(g, bounds, this.ColorTable.BaseItemPressed, this.ColorTable.BaseItemBorder, this.ColorTable.Back, this.ColorTable.BaseItemRadiusStyle, this.ColorTable.BaseItemRadius, this.ColorTable.BaseItemBorderShow, this.ColorTable.BaseItemAnamorphosis, mode);
                }
            }
            else if (item.Selected)
            {
                if (this.ColorTable.BaseItemDown != null)
                {
                    CCWin.ImageDrawRect.DrawRect(g, (Bitmap) this.ColorTable.BaseItemMouse, bounds, Rectangle.FromLTRB(this.ColorTable.BackRectangle.X, this.ColorTable.BackRectangle.Y, this.ColorTable.BackRectangle.Width, this.ColorTable.BackRectangle.Height), 1, 1);
                }
                else
                {
                    RenderHelperStrip.RenderBackgroundInternal(g, bounds, this.ColorTable.BaseItemHover, this.ColorTable.BaseItemBorder, this.ColorTable.Back, this.ColorTable.BaseItemRadiusStyle, this.ColorTable.BaseItemRadius, this.ColorTable.BaseItemBorderShow, this.ColorTable.BaseItemAnamorphosis, mode);
                }
            }
            else
            {
                if (toolStrip is ToolStripOverflow)
                {
                    using (Brush brush = new SolidBrush(this.ColorTable.Back))
                    {
                        g.FillRectangle(brush, bounds);
                        goto Label_0256;
                    }
                }
                base.OnRenderDropDownButtonBackground(e);
            }
        Label_0256:
            sg.Dispose();
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
            if (e.GripStyle == ToolStripGripStyle.Visible)
            {
                Rectangle bounds = e.GripBounds;
                bool vert = e.GripDisplayStyle == ToolStripGripDisplayStyle.Vertical;
                ToolStrip toolStrip = e.ToolStrip;
                Graphics g = e.Graphics;
                if (vert)
                {
                    bounds.X = e.AffectedBounds.X;
                    bounds.Width = e.AffectedBounds.Width;
                    if (toolStrip is MenuStrip)
                    {
                        if (e.AffectedBounds.Height > e.AffectedBounds.Width)
                        {
                            vert = false;
                            bounds.Y = e.AffectedBounds.Y;
                        }
                        else
                        {
                            toolStrip.GripMargin = new Padding(0, 2, 0, 2);
                            bounds.Y = e.AffectedBounds.Y;
                            bounds.Height = e.AffectedBounds.Height;
                        }
                    }
                    else
                    {
                        toolStrip.GripMargin = new Padding(2, 2, 4, 2);
                        bounds.X++;
                        bounds.Width++;
                    }
                }
                else
                {
                    bounds.Y = e.AffectedBounds.Y;
                    bounds.Height = e.AffectedBounds.Height;
                }
                this.DrawDottedGrip(g, bounds, vert, false, this.ColorTable.Back, ControlPaint.Dark(this.ColorTable.Base, 0.3f));
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds = e.AffectedBounds;
            if (toolStrip is ToolStripDropDown)
            {
                bool bRightToLeft = toolStrip.RightToLeft == RightToLeft.Yes;
                Rectangle imageBackRect = bounds;
                Rectangle logoRect = bounds;
                if (bRightToLeft)
                {
                    logoRect.X -= 2;
                    imageBackRect.X = logoRect.X;
                }
                else
                {
                    logoRect.X += 2;
                    imageBackRect.X = logoRect.Right;
                }
                logoRect.Y++;
                logoRect.Height -= 2;
                using (LinearGradientBrush brush = new LinearGradientBrush(logoRect, this.ColorTable.TitleColor, this.ColorTable.Back, 90f))
                {
                    Blend blend = new Blend();
                    float[] CS_0_0000 = new float[3];
                    CS_0_0000[1] = 0.2f;
                    CS_0_0000[2] = 1f;
                    blend.Positions = CS_0_0000;
                    float[] CS_0_0001 = new float[3];
                    CS_0_0001[1] = 0.1f;
                    CS_0_0001[2] = 0.9f;
                    blend.Factors = CS_0_0001;
                    brush.Blend = blend;
                    logoRect.Y++;
                    logoRect.Height -= 2;
                    using (GraphicsPath path = GraphicsPathHelper.CreatePath(logoRect, this.ColorTable.TitleRadius, this.ColorTable.TitleRadiusStyle, false))
                    {
                        using (new SmoothingModeGraphics(g))
                        {
                            if (this.ColorTable.TitleAnamorphosis)
                            {
                                g.FillPath(brush, path);
                            }
                            else
                            {
                                SolidBrush br = new SolidBrush(this.ColorTable.TitleColor);
                                g.FillPath(br, path);
                            }
                            return;
                        }
                    }
                }
            }
            base.OnRenderImageMargin(e);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            if ((toolStrip is ToolStripDropDown) && (e.Item is ToolStripMenuItem))
            {
                Rectangle rect = e.ImageRectangle;
                if (e.Item.RightToLeft == RightToLeft.Yes)
                {
                    rect.X -= 2;
                }
                else
                {
                    rect.X += 2;
                }
                rect.Width = 13;
                rect.Y++;
                rect.Height -= 3;
                using (new SmoothingModeGraphics(g))
                {
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        path.AddRectangle(rect);
                        using (PathGradientBrush brush = new PathGradientBrush(path))
                        {
                            brush.CenterColor = Color.White;
                            brush.SurroundColors = new Color[] { ControlPaint.Light(this.ColorTable.Back) };
                            Blend blend = new Blend();
                            float[] CS_0_0001 = new float[3];
                            CS_0_0001[1] = 0.3f;
                            CS_0_0001[2] = 1f;
                            blend.Positions = CS_0_0001;
                            float[] CS_0_0002 = new float[3];
                            CS_0_0002[1] = 0.5f;
                            CS_0_0002[2] = 1f;
                            blend.Factors = CS_0_0002;
                            brush.Blend = blend;
                            g.FillRectangle(brush, rect);
                        }
                    }
                    using (Pen pen = new Pen(ControlPaint.Light(this.ColorTable.Back)))
                    {
                        g.DrawRectangle(pen, rect);
                    }
                    CCWin.SkinControl.ControlPaintEx.DrawCheckedFlag(g, rect, this.ColorTable.Fore);
                    return;
                }
            }
            base.OnRenderItemCheck(e);
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            if ((toolStrip is ToolStripDropDown) && (e.Item is ToolStripMenuItem))
            {
                ToolStripMenuItem item = (ToolStripMenuItem) e.Item;
                if (item.Checked)
                {
                    return;
                }
                Rectangle rect = e.ImageRectangle;
                if (e.Item.RightToLeft == RightToLeft.Yes)
                {
                    rect.X -= 2;
                }
                else
                {
                    rect.X += 2;
                }
                using (new InterpolationModeGraphics(g))
                {
                    ToolStripItemImageRenderEventArgs ne = new ToolStripItemImageRenderEventArgs(g, e.Item, e.Image, rect);
                    base.OnRenderItemImage(ne);
                    return;
                }
            }
            base.OnRenderItemImage(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripItem item = e.Item;
            if (toolStrip is ToolStripDropDown)
            {
                e.TextColor = item.Selected ? this.ColorTable.HoverFore : this.ColorTable.Fore;
            }
            else
            {
                e.TextColor = item.Selected ? this.ColorTable.BaseHoverFore : this.ColorTable.BaseFore;
            }
            if ((toolStrip is ToolStripDropDown) && (e.Item is ToolStripMenuItem))
            {
                Rectangle rect = e.TextRectangle;
                e.TextRectangle = rect;
            }
            if ((!(toolStrip is ToolStripDropDown) && this.ColorTable.BaseForeAnamorphosis) && !string.IsNullOrEmpty(e.Item.Text))
            {
                Graphics g = e.Graphics;
                Image img = UpdateForm.ImageLightEffect(e.Item.Text, e.Item.Font, e.TextColor, this.ColorTable.BaseForeAnamorphosisColor, this.ColorTable.BaseForeAnamorphosisBorder);
                g.DrawImage(img, (int) (e.TextRectangle.Left - (this.ColorTable.BaseForeAnamorphosisBorder / 2)), (int) (e.TextRectangle.Top - (this.ColorTable.BaseForeAnamorphosisBorder / 2)));
            }
            else
            {
                base.OnRenderItemText(e);
            }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripItem item = e.Item;
            if (item.Enabled)
            {
                Graphics g = e.Graphics;
                Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
                if (toolStrip is MenuStrip)
                {
                    LinearGradientMode mode = (toolStrip.Orientation == Orientation.Horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
                    if (item.Selected)
                    {
                        RenderHelperStrip.RenderBackgroundInternal(g, rect, this.ColorTable.ItemHover, this.ColorTable.ItemBorder, this.ColorTable.Back, this.ColorTable.BaseItemRadiusStyle, this.ColorTable.BaseItemRadius, true, true, mode);
                    }
                    else if (item.Pressed)
                    {
                        RenderHelperStrip.RenderBackgroundInternal(g, rect, this.ColorTable.ItemPressed, this.ColorTable.ItemBorder, this.ColorTable.Back, this.ColorTable.BaseItemRadiusStyle, this.ColorTable.BaseItemRadius, true, true, mode);
                    }
                    else
                    {
                        base.OnRenderMenuItemBackground(e);
                    }
                }
                else if (toolStrip is ToolStripDropDown)
                {
                    rect = new Rectangle(new Point(-2, 0), new Size(e.Item.Size.Width + 5, e.Item.Size.Height + 1));
                    if (item.RightToLeft == RightToLeft.Yes)
                    {
                        rect.X += 4;
                    }
                    else
                    {
                        rect.X += 4;
                    }
                    rect.Width -= 8;
                    rect.Height--;
                    if (item.Selected)
                    {
                        RenderHelperStrip.RenderBackgroundInternal(g, rect, this.ColorTable.ItemHover, this.ColorTable.ItemBorder, this.ColorTable.Back, this.ColorTable.ItemRadiusStyle, this.ColorTable.ItemRadius, this.ColorTable.ItemBorderShow, this.ColorTable.ItemAnamorphosis, LinearGradientMode.Vertical);
                    }
                    else
                    {
                        base.OnRenderMenuItemBackground(e);
                    }
                }
                else
                {
                    base.OnRenderMenuItemBackground(e);
                }
            }
        }

        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripItem item = e.Item;
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            bool rightToLeft = item.RightToLeft == RightToLeft.Yes;
            new SmoothingModeGraphics(g);
            this.RenderOverflowBackground(e, rightToLeft);
            bool bHorizontal = toolStrip.Orientation == Orientation.Horizontal;
            Rectangle empty = Rectangle.Empty;
            if (rightToLeft)
            {
                empty = new Rectangle(0, item.Height - 8, 10, 5);
            }
            else
            {
                empty = new Rectangle(item.Width - 12, item.Height - 8, 10, 5);
            }
            ArrowDirection direction = bHorizontal ? ArrowDirection.Down : ArrowDirection.Right;
            int x = (rightToLeft && bHorizontal) ? -1 : 1;
            empty.Offset(x, 1);
            Color arrowColor = toolStrip.Enabled ? this.ColorTable.Fore : SystemColors.ControlDark;
            using (Brush brush = new SolidBrush(arrowColor))
            {
                RenderHelperStrip.RenderArrowInternal(g, empty, direction, brush);
            }
            if (bHorizontal)
            {
                using (Pen pen = new Pen(arrowColor))
                {
                    g.DrawLine(pen, (int) (empty.Right - 8), (int) (empty.Y - 2), (int) (empty.Right - 2), (int) (empty.Y - 2));
                    g.DrawLine(pen, (int) (empty.Right - 8), (int) (empty.Y - 1), (int) (empty.Right - 2), (int) (empty.Y - 1));
                    return;
                }
            }
            using (Pen pen = new Pen(arrowColor))
            {
                g.DrawLine(pen, empty.X, empty.Y, empty.X, empty.Bottom - 1);
                g.DrawLine(pen, empty.X, empty.Y + 1, empty.X, empty.Bottom);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Rectangle rect = e.Item.ContentRectangle;
            Graphics g = e.Graphics;
            if (toolStrip is ToolStripDropDown)
            {
                if (e.Item.RightToLeft != RightToLeft.Yes)
                {
                    rect.X += 0x1a;
                }
                rect.Width -= 0x1c;
            }
            this.RenderSeparatorLine(g, rect, this.ColorTable.BaseItemSplitter, this.ColorTable.Back, SystemColors.ControlLightLight, e.Vertical);
        }

        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripSplitButton item = e.Item as ToolStripSplitButton;
            if (item != null)
            {
                Graphics g = e.Graphics;
                LinearGradientMode mode = (toolStrip.Orientation == Orientation.Horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
                Rectangle bounds = new Rectangle(Point.Empty, item.Size);
                new SmoothingModeGraphics(g);
                Color arrowColor = toolStrip.Enabled ? this.ColorTable.Fore : SystemColors.ControlDark;
                if (item.BackgroundImage != null)
                {
                    Rectangle clipRect = item.Selected ? item.ContentRectangle : bounds;
                    CCWin.SkinControl.ControlPaintEx.DrawBackgroundImage(g, item.BackgroundImage, this.ColorTable.Back, item.BackgroundImageLayout, bounds, clipRect);
                }
                if (item.ButtonPressed)
                {
                    if (this.ColorTable.BaseItemDown != null)
                    {
                        CCWin.ImageDrawRect.DrawRect(g, (Bitmap) this.ColorTable.BaseItemDown, bounds, Rectangle.FromLTRB(this.ColorTable.BackRectangle.X, this.ColorTable.BackRectangle.Y, this.ColorTable.BackRectangle.Width, this.ColorTable.BackRectangle.Height), 1, 1);
                    }
                    else
                    {
                        Rectangle buttonBounds = item.ButtonBounds;
                        Padding padding = (item.RightToLeft == RightToLeft.Yes) ? new Padding(0, 1, 1, 1) : new Padding(1, 1, 0, 1);
                        buttonBounds = LayoutUtils.DeflateRect(buttonBounds, padding);
                        RenderHelperStrip.RenderBackgroundInternal(g, bounds, this.ColorTable.BaseItemHover, this.ColorTable.BaseItemBorder, this.ColorTable.Back, this.ColorTable.BaseItemRadiusStyle, this.ColorTable.BaseItemRadius, this.ColorTable.BaseItemBorderShow, this.ColorTable.BaseItemAnamorphosis, mode);
                        buttonBounds.Inflate(-1, -1);
                        g.SetClip(buttonBounds);
                        RenderHelperStrip.RenderBackgroundInternal(g, buttonBounds, this.ColorTable.BaseItemPressed, this.ColorTable.BaseItemBorder, this.ColorTable.Back, RoundStyle.Left, false, true, mode);
                        g.ResetClip();
                        using (Pen pen = new Pen(this.ColorTable.BaseItemSplitter))
                        {
                            g.DrawLine(pen, item.SplitterBounds.Left, item.SplitterBounds.Top, item.SplitterBounds.Left, item.SplitterBounds.Bottom);
                        }
                    }
                    base.DrawArrow(new ToolStripArrowRenderEventArgs(g, item, item.DropDownButtonBounds, arrowColor, ArrowDirection.Down));
                }
                else if (item.Pressed || item.DropDownButtonPressed)
                {
                    if (this.ColorTable.BaseItemDown != null)
                    {
                        CCWin.ImageDrawRect.DrawRect(g, (Bitmap) this.ColorTable.BaseItemDown, bounds, Rectangle.FromLTRB(this.ColorTable.BackRectangle.X, this.ColorTable.BackRectangle.Y, this.ColorTable.BackRectangle.Width, this.ColorTable.BackRectangle.Height), 1, 1);
                    }
                    else
                    {
                        RenderHelperStrip.RenderBackgroundInternal(g, bounds, this.ColorTable.BaseItemPressed, this.ColorTable.BaseItemBorder, this.ColorTable.Back, this.ColorTable.BaseItemRadiusStyle, this.ColorTable.BaseItemRadius, this.ColorTable.BaseItemBorderShow, this.ColorTable.BaseItemAnamorphosis, mode);
                    }
                    base.DrawArrow(new ToolStripArrowRenderEventArgs(g, item, item.DropDownButtonBounds, arrowColor, ArrowDirection.Down));
                }
                else if (item.Selected)
                {
                    if (this.ColorTable.BaseItemMouse != null)
                    {
                        CCWin.ImageDrawRect.DrawRect(g, (Bitmap) this.ColorTable.BaseItemMouse, bounds, Rectangle.FromLTRB(this.ColorTable.BackRectangle.X, this.ColorTable.BackRectangle.Y, this.ColorTable.BackRectangle.Width, this.ColorTable.BackRectangle.Height), 1, 1);
                    }
                    else
                    {
                        RenderHelperStrip.RenderBackgroundInternal(g, bounds, this.ColorTable.BaseItemHover, this.ColorTable.BaseItemBorder, this.ColorTable.Back, this.ColorTable.BaseItemRadiusStyle, this.ColorTable.BaseItemRadius, this.ColorTable.BaseItemBorderShow, this.ColorTable.BaseItemAnamorphosis, mode);
                        using (Pen pen = new Pen(this.ColorTable.BaseItemSplitter))
                        {
                            g.DrawLine(pen, item.SplitterBounds.Left, item.SplitterBounds.Top, item.SplitterBounds.Left, item.SplitterBounds.Bottom);
                        }
                    }
                    base.DrawArrow(new ToolStripArrowRenderEventArgs(g, item, item.DropDownButtonBounds, arrowColor, ArrowDirection.Down));
                }
                else
                {
                    base.DrawArrow(new ToolStripArrowRenderEventArgs(g, item, item.DropDownButtonBounds, arrowColor, ArrowDirection.Down));
                }
            }
            else
            {
                base.OnRenderSplitButtonBackground(e);
            }
        }

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            this.DrawSolidStatusGrip(e.Graphics, e.AffectedBounds, this.ColorTable.Back, ControlPaint.Dark(this.ColorTable.Base, 0.3f));
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds = e.AffectedBounds;
            if (toolStrip is ToolStripDropDown)
            {
                RegionHelper.CreateRegion(toolStrip, bounds, this.ColorTable.BackRadius, this.ColorTable.RadiusStyle);
                using (SolidBrush brush = new SolidBrush(this.ColorTable.Back))
                {
                    g.FillRectangle(brush, bounds);
                    return;
                }
            }
            LinearGradientMode mode = (toolStrip.Orientation == Orientation.Horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
            RenderHelperStrip.RenderBackgroundInternal(g, bounds, this.ColorTable.Base, this.ColorTable.ItemBorder, this.ColorTable.Back, this.ColorTable.RadiusStyle, this.ColorTable.BackRadius, 0.35f, false, false, mode);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds = e.AffectedBounds;
            if (toolStrip is ToolStripDropDown)
            {
                if (this.ColorTable.RadiusStyle == RoundStyle.None)
                {
                    bounds.Width--;
                    bounds.Height--;
                }
                using (new SmoothingModeGraphics(g))
                {
                    using (GraphicsPath path = GraphicsPathHelper.CreatePath(bounds, this.ColorTable.BackRadius, this.ColorTable.RadiusStyle, true))
                    {
                        using (Pen pen = new Pen(this.ColorTable.DropDownImageSeparator))
                        {
                            path.Widen(pen);
                            g.DrawPath(pen, path);
                        }
                    }
                }
                if (toolStrip is ToolStripOverflow)
                {
                    return;
                }
                bounds.Inflate(-1, -1);
                using (GraphicsPath innerPath = GraphicsPathHelper.CreatePath(bounds, this.ColorTable.BackRadius, this.ColorTable.RadiusStyle, true))
                {
                    using (Pen pen = new Pen(this.ColorTable.Back))
                    {
                        g.DrawPath(pen, innerPath);
                    }
                    return;
                }
            }
            base.OnRenderToolStripBorder(e);
        }

        public void RenderOverflowBackground(ToolStripItemRenderEventArgs e, bool rightToLeft)
        {
            Color color = Color.Empty;
            Graphics g = e.Graphics;
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripOverflowButton item = e.Item as ToolStripOverflowButton;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
            Rectangle withinBounds = bounds;
            bool bParentIsMenuStrip = !(item.GetCurrentParent() is MenuStrip);
            bool bHorizontal = toolStrip.Orientation == Orientation.Horizontal;
            if (bHorizontal)
            {
                bounds.X += (bounds.Width - 12) + 1;
                bounds.Width = 12;
                if (rightToLeft)
                {
                    bounds = LayoutUtils.RTLTranslate(bounds, withinBounds);
                }
            }
            else
            {
                bounds.Y = (bounds.Height - 12) + 1;
                bounds.Height = 12;
            }
            if (item.Pressed)
            {
                color = this.ColorTable.ItemPressed;
            }
            else if (item.Selected)
            {
                color = this.ColorTable.ItemHover;
            }
            else
            {
                color = this.ColorTable.Base;
            }
            if (bParentIsMenuStrip)
            {
                using (Pen pen = new Pen(this.ColorTable.Base))
                {
                    Point point = new Point(bounds.Left - 1, bounds.Height - 2);
                    Point point2 = new Point(bounds.Left, bounds.Height - 2);
                    if (rightToLeft)
                    {
                        point.X = bounds.Right + 1;
                        point2.X = bounds.Right;
                    }
                    g.DrawLine(pen, point, point2);
                }
            }
            LinearGradientMode mode = bHorizontal ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
            RenderHelperStrip.RenderBackgroundInternal(g, bounds, color, this.ColorTable.ItemBorder, this.ColorTable.Back, RoundStyle.None, 0, 0.35f, false, false, mode);
            if (bParentIsMenuStrip)
            {
                using (Brush brush = new SolidBrush(this.ColorTable.Base))
                {
                    if (bHorizontal)
                    {
                        Point point3 = new Point(bounds.X - 2, 0);
                        Point point4 = new Point(bounds.X - 1, 1);
                        if (rightToLeft)
                        {
                            point3.X = bounds.Right + 1;
                            point4.X = bounds.Right;
                        }
                        g.FillRectangle(brush, point3.X, point3.Y, 1, 1);
                        g.FillRectangle(brush, point4.X, point4.Y, 1, 1);
                    }
                    else
                    {
                        g.FillRectangle(brush, bounds.Width - 3, bounds.Top - 1, 1, 1);
                        g.FillRectangle(brush, bounds.Width - 2, bounds.Top - 2, 1, 1);
                    }
                }
                using (Brush brush = new SolidBrush(this.ColorTable.Base))
                {
                    if (bHorizontal)
                    {
                        Rectangle rect = new Rectangle(bounds.X - 1, 0, 1, 1);
                        if (rightToLeft)
                        {
                            rect.X = bounds.Right;
                        }
                        g.FillRectangle(brush, rect);
                    }
                    else
                    {
                        g.FillRectangle(brush, bounds.X, bounds.Top - 1, 1, 1);
                    }
                }
            }
        }

        public void RenderSeparatorLine(Graphics g, Rectangle rect, Color baseColor, Color backColor, Color shadowColor, bool vertical)
        {
            if (vertical)
            {
                rect.Y += 2;
                rect.Height -= 4;
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, baseColor, backColor, LinearGradientMode.Vertical))
                {
                    using (Pen pen = new Pen(brush))
                    {
                        g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Bottom);
                    }
                    return;
                }
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, baseColor, backColor, 180f))
            {
                Blend blend = new Blend();
                blend.Positions = new float[] { 0f, 0.2f, 0.5f, 0.8f, 1f };
                blend.Factors = new float[] { 1f, 0.3f, 0f, 0.3f, 1f };
                brush.Blend = blend;
                using (Pen pen = new Pen(brush))
                {
                    g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);
                    brush.LinearColors = new Color[] { shadowColor, backColor };
                    pen.Brush = brush;
                    g.DrawLine(pen, rect.X, rect.Y + 1, rect.Right, rect.Y + 1);
                }
            }
        }

        [DllImport("gdi32.dll")]
        private static extern uint SetPixel(IntPtr hdc, int X, int Y, int crColor);

        public ToolStripColorTable ColorTable
        {
            get
            {
                return this._colorTable;
            }
            set
            {
                this._colorTable = value;
            }
        }
    }
}

