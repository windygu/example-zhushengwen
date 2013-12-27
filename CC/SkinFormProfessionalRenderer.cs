namespace CCWin
{
    using CCWin.SkinClass;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    public class SkinFormProfessionalRenderer : SkinFormRenderer
    {
        private SkinFormColorTable _colorTable;

        public SkinFormProfessionalRenderer()
        {
        }

        public SkinFormProfessionalRenderer(SkinFormColorTable colortable)
        {
            this._colorTable = colortable;
        }

        private GraphicsPath CreateCloseFlagPath(Rectangle rect)
        {
            PointF centerPoint = new PointF(rect.X + (((float) rect.Width) / 2f), rect.Y + (((float) rect.Height) / 2f));
            GraphicsPath path = new GraphicsPath();
            path.AddLine(centerPoint.X, centerPoint.Y - 2f, centerPoint.X - 2f, centerPoint.Y - 4f);
            path.AddLine((float) (centerPoint.X - 2f), (float) (centerPoint.Y - 4f), (float) (centerPoint.X - 6f), (float) (centerPoint.Y - 4f));
            path.AddLine(centerPoint.X - 6f, centerPoint.Y - 4f, centerPoint.X - 2f, centerPoint.Y);
            path.AddLine(centerPoint.X - 2f, centerPoint.Y, centerPoint.X - 6f, centerPoint.Y + 4f);
            path.AddLine((float) (centerPoint.X - 6f), (float) (centerPoint.Y + 4f), (float) (centerPoint.X - 2f), (float) (centerPoint.Y + 4f));
            path.AddLine(centerPoint.X - 2f, centerPoint.Y + 4f, centerPoint.X, centerPoint.Y + 2f);
            path.AddLine(centerPoint.X, centerPoint.Y + 2f, centerPoint.X + 2f, centerPoint.Y + 4f);
            path.AddLine((float) (centerPoint.X + 2f), (float) (centerPoint.Y + 4f), (float) (centerPoint.X + 6f), (float) (centerPoint.Y + 4f));
            path.AddLine(centerPoint.X + 6f, centerPoint.Y + 4f, centerPoint.X + 2f, centerPoint.Y);
            path.AddLine(centerPoint.X + 2f, centerPoint.Y, centerPoint.X + 6f, centerPoint.Y - 4f);
            path.AddLine((float) (centerPoint.X + 6f), (float) (centerPoint.Y - 4f), (float) (centerPoint.X + 2f), (float) (centerPoint.Y - 4f));
            path.CloseFigure();
            return path;
        }

        private GraphicsPath CreateMaximizeFlafPath(Rectangle rect, bool maximize)
        {
            PointF centerPoint = new PointF(rect.X + (((float) rect.Width) / 2f), rect.Y + (((float) rect.Height) / 2f));
            GraphicsPath path = new GraphicsPath();
            if (maximize)
            {
                path.AddLine((float) (centerPoint.X - 3f), (float) (centerPoint.Y - 3f), (float) (centerPoint.X - 6f), (float) (centerPoint.Y - 3f));
                path.AddLine((float) (centerPoint.X - 6f), (float) (centerPoint.Y - 3f), (float) (centerPoint.X - 6f), (float) (centerPoint.Y + 5f));
                path.AddLine((float) (centerPoint.X - 6f), (float) (centerPoint.Y + 5f), (float) (centerPoint.X + 3f), (float) (centerPoint.Y + 5f));
                path.AddLine((float) (centerPoint.X + 3f), (float) (centerPoint.Y + 5f), (float) (centerPoint.X + 3f), (float) (centerPoint.Y + 1f));
                path.AddLine((float) (centerPoint.X + 3f), (float) (centerPoint.Y + 1f), (float) (centerPoint.X + 6f), (float) (centerPoint.Y + 1f));
                path.AddLine((float) (centerPoint.X + 6f), (float) (centerPoint.Y + 1f), (float) (centerPoint.X + 6f), (float) (centerPoint.Y - 6f));
                path.AddLine((float) (centerPoint.X + 6f), (float) (centerPoint.Y - 6f), (float) (centerPoint.X - 3f), (float) (centerPoint.Y - 6f));
                path.CloseFigure();
                path.AddRectangle(new RectangleF(centerPoint.X - 4f, centerPoint.Y, 5f, 3f));
                path.AddLine((float) (centerPoint.X - 1f), (float) (centerPoint.Y - 4f), (float) (centerPoint.X + 4f), (float) (centerPoint.Y - 4f));
                path.AddLine((float) (centerPoint.X + 4f), (float) (centerPoint.Y - 4f), (float) (centerPoint.X + 4f), (float) (centerPoint.Y - 1f));
                path.AddLine((float) (centerPoint.X + 4f), (float) (centerPoint.Y - 1f), (float) (centerPoint.X + 3f), (float) (centerPoint.Y - 1f));
                path.AddLine((float) (centerPoint.X + 3f), (float) (centerPoint.Y - 1f), (float) (centerPoint.X + 3f), (float) (centerPoint.Y - 3f));
                path.AddLine((float) (centerPoint.X + 3f), (float) (centerPoint.Y - 3f), (float) (centerPoint.X - 1f), (float) (centerPoint.Y - 3f));
                path.CloseFigure();
                return path;
            }
            path.AddRectangle(new RectangleF(centerPoint.X - 6f, centerPoint.Y - 4f, 12f, 8f));
            path.AddRectangle(new RectangleF(centerPoint.X - 3f, centerPoint.Y - 1f, 6f, 3f));
            return path;
        }

        private GraphicsPath CreateMinimizeFlagPath(Rectangle rect)
        {
            PointF centerPoint = new PointF(rect.X + (((float) rect.Width) / 2f), rect.Y + (((float) rect.Height) / 2f));
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(centerPoint.X - 6f, centerPoint.Y + 1f, 12f, 3f));
            return path;
        }

        public override Region CreateRegion(CCSkinMain form)
        {
            Rectangle rect = new Rectangle(Point.Empty, form.Size);
            using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, form.Radius, form.RoundStyle, false))
            {
                return new Region(path);
            }
        }

        private void DrawBorder(Graphics g, Rectangle rect, RoundStyle roundStyle, int radius, CCSkinMain frm)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            rect.Width--;
            rect.Height--;
            using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, radius, roundStyle, false))
            {
                using (Pen pen = new Pen(this.ColorTable.Border))
                {
                    g.DrawPath(pen, path);
                }
            }
            rect.Inflate(-1, -1);
            using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, radius, roundStyle, false))
            {
                using (Pen pen = new Pen(this.ColorTable.InnerBorder))
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        private void DrawCaptionBackground(Graphics g, Rectangle captionRect, bool active)
        {
            Color baseColor = active ? this.ColorTable.CaptionActive : this.ColorTable.CaptionDeactive;
            RenderHelper.RenderBackgroundInternal(g, captionRect, baseColor, this.ColorTable.Border, this.ColorTable.InnerBorder, RoundStyle.None, 0, 0.25f, false, false, LinearGradientMode.Vertical);
        }

        private void DrawCaptionText(Graphics g, Rectangle textRect, string text, Font font, bool Effect, Color EffetBack, int EffectWidth, Color FrmColor)
        {
            if (Effect)
            {
                Size txtsize = TextRenderer.MeasureText(text, font);
                Image imgtext = UpdateForm.ImageLightEffect(text, font, FrmColor, EffetBack, EffectWidth, new Rectangle(0, 0, textRect.Width, txtsize.Height), true);
                g.DrawImage(imgtext, (int) (textRect.X - (EffectWidth / 2)), (int) (textRect.Y - (EffectWidth / 2)));
            }
        }

        private void DrawIcon(Graphics g, Rectangle iconRect, Icon icon)
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawIcon(icon, iconRect);
        }

        public override void InitSkinForm(CCSkinMain form)
        {
            form.BackColor = this.ColorTable.Back;
        }

        protected override void OnRenderSkinFormBorder(SkinFormBorderRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            using (new AntiAliasGraphics(g))
            {
                this.DrawBorder(g, e.ClipRectangle, e.SkinForm.RoundStyle, e.SkinForm.Radius, e.SkinForm);
            }
        }

        protected override void OnRenderSkinFormCaption(SkinFormCaptionRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.ClipRectangle;
            CCSkinMain form = e.SkinForm;
            Rectangle iconRect = form.IconRect;
            Rectangle textRect = Rectangle.Empty;
            bool closeBox = form.ControlBox;
            bool minimizeBox = form.ControlBox && form.MinimizeBox;
            bool maximizeBox = form.ControlBox && form.MaximizeBox;
            bool sysBox = form.ControlBox && form.SysBottomVisibale;
            int textWidthDec = 0;
            if (closeBox)
            {
                textWidthDec += form.CloseBoxSize.Width + form.ControlBoxOffset.X;
            }
            if (maximizeBox)
            {
                textWidthDec += form.MaxSize.Width + form.ControlBoxSpace;
            }
            if (minimizeBox)
            {
                textWidthDec += form.MiniSize.Width + form.ControlBoxSpace;
            }
            if (sysBox)
            {
                textWidthDec += form.SysBottomSize.Width + form.ControlBoxSpace;
            }
            textRect = new Rectangle(iconRect.Right + 3, form.BorderWidth, ((rect.Width - iconRect.Right) - textWidthDec) - 6, rect.Height - form.BorderWidth);
            using (new AntiAliasGraphics(g))
            {
                this.DrawCaptionBackground(g, rect, e.Active);
                if (form.ShowDrawIcon && (form.Icon != null))
                {
                    this.DrawIcon(g, iconRect, form.Icon);
                }
                if (!string.IsNullOrEmpty(form.Text))
                {
                    this.DrawCaptionText(g, textRect, form.Text, form.CaptionFont, form.EffectCaption, form.EffectBack, form.EffectWidth, form.ForeColor);
                }
            }
        }

        protected override void OnRenderSkinFormControlBox(SkinFormControlBoxRenderEventArgs e)
        {
            CCSkinMain form = e.Form;
            Graphics g = e.Graphics;
            Rectangle rect = e.ClipRectangle;
            ControlBoxState state = e.ControlBoxtate;
            bool active = e.Active;
            bool minimizeBox = form.ControlBox && form.MinimizeBox;
            bool maximizeBox = form.ControlBox && form.MaximizeBox;
            switch (e.ControlBoxStyle)
            {
                case ControlBoxStyle.Minimize:
                    this.RenderSkinFormMinimizeBoxInternal(g, rect, state, active, form);
                    return;

                case ControlBoxStyle.Maximize:
                    this.RenderSkinFormMaximizeBoxInternal(g, rect, state, active, minimizeBox, form.WindowState == FormWindowState.Maximized, form);
                    return;

                case ControlBoxStyle.Close:
                    this.RenderSkinFormCloseBoxInternal(g, rect, state, active, minimizeBox, maximizeBox, form);
                    return;

                case ControlBoxStyle.SysBottom:
                    this.RenderSkinFormSysBottomInternal(g, rect, state, active, form);
                    return;
            }
        }

        private void RenderSkinFormCloseBoxInternal(Graphics g, Rectangle rect, ControlBoxState state, bool active, bool minimizeBox, bool maximizeBox, CCSkinMain form)
        {
            Bitmap btm = null;
            Color baseColor = this.ColorTable.ControlBoxActive;
            if (state == ControlBoxState.Pressed)
            {
                btm = (Bitmap) form.CloseDownBack;
                baseColor = this.ColorTable.ControlCloseBoxPressed;
            }
            else if (state == ControlBoxState.Hover)
            {
                btm = (Bitmap) form.CloseMouseBack;
                baseColor = this.ColorTable.ControlCloseBoxHover;
            }
            else
            {
                btm = (Bitmap) form.CloseNormlBack;
                baseColor = active ? this.ColorTable.ControlBoxActive : this.ColorTable.ControlBoxDeactive;
            }
            if (btm != null)
            {
                g.DrawImage(btm, rect);
            }
            else
            {
                RoundStyle roundStyle = (minimizeBox || maximizeBox) ? RoundStyle.BottomRight : RoundStyle.Bottom;
                using (new AntiAliasGraphics(g))
                {
                    RenderHelper.RenderBackgroundInternal(g, rect, baseColor, baseColor, this.ColorTable.ControlBoxInnerBorder, roundStyle, 6, 0.38f, true, false, LinearGradientMode.Vertical);
                    using (Pen pen = new Pen(this.ColorTable.Border))
                    {
                        g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);
                    }
                    using (GraphicsPath path = this.CreateCloseFlagPath(rect))
                    {
                        g.FillPath(Brushes.White, path);
                        using (Pen pen = new Pen(baseColor))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }
        }

        private void RenderSkinFormMaximizeBoxInternal(Graphics g, Rectangle rect, ControlBoxState state, bool active, bool minimizeBox, bool maximize, CCSkinMain form)
        {
            Bitmap btm = null;
            Color baseColor = this.ColorTable.ControlBoxActive;
            if (state == ControlBoxState.Pressed)
            {
                btm = maximize ? ((Bitmap) form.RestoreDownBack) : ((Bitmap) form.MaxDownBack);
                baseColor = this.ColorTable.ControlBoxPressed;
            }
            else if (state == ControlBoxState.Hover)
            {
                btm = maximize ? ((Bitmap) form.RestoreMouseBack) : ((Bitmap) form.MaxMouseBack);
                baseColor = this.ColorTable.ControlBoxHover;
            }
            else
            {
                btm = maximize ? ((Bitmap) form.RestoreNormlBack) : ((Bitmap) form.MaxNormlBack);
                baseColor = active ? this.ColorTable.ControlBoxActive : this.ColorTable.ControlBoxDeactive;
            }
            if (btm != null)
            {
                g.DrawImage(btm, rect);
            }
            else
            {
                RoundStyle roundStyle = minimizeBox ? RoundStyle.None : RoundStyle.BottomLeft;
                using (new AntiAliasGraphics(g))
                {
                    RenderHelper.RenderBackgroundInternal(g, rect, baseColor, baseColor, this.ColorTable.ControlBoxInnerBorder, roundStyle, 6, 0.38f, true, false, LinearGradientMode.Vertical);
                    using (Pen pen = new Pen(this.ColorTable.Border))
                    {
                        g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);
                    }
                    using (GraphicsPath path = this.CreateMaximizeFlafPath(rect, maximize))
                    {
                        g.FillPath(Brushes.White, path);
                        using (Pen pen = new Pen(baseColor))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }
        }

        private void RenderSkinFormMinimizeBoxInternal(Graphics g, Rectangle rect, ControlBoxState state, bool active, CCSkinMain form)
        {
            Bitmap btm = null;
            Color baseColor = this.ColorTable.ControlBoxActive;
            if (state == ControlBoxState.Pressed)
            {
                btm = (Bitmap) form.MiniDownBack;
                baseColor = this.ColorTable.ControlBoxPressed;
            }
            else if (state == ControlBoxState.Hover)
            {
                btm = (Bitmap) form.MiniMouseBack;
                baseColor = this.ColorTable.ControlBoxHover;
            }
            else
            {
                btm = (Bitmap) form.MiniNormlBack;
                baseColor = active ? this.ColorTable.ControlBoxActive : this.ColorTable.ControlBoxDeactive;
            }
            if (btm != null)
            {
                g.DrawImage(btm, rect);
            }
            else
            {
                RoundStyle roundStyle = RoundStyle.BottomLeft;
                using (new AntiAliasGraphics(g))
                {
                    RenderHelper.RenderBackgroundInternal(g, rect, baseColor, baseColor, this.ColorTable.ControlBoxInnerBorder, roundStyle, 6, 0.38f, true, false, LinearGradientMode.Vertical);
                    using (Pen pen = new Pen(this.ColorTable.Border))
                    {
                        g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);
                    }
                    using (GraphicsPath path = this.CreateMinimizeFlagPath(rect))
                    {
                        g.FillPath(Brushes.White, path);
                        using (Pen pen = new Pen(baseColor))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
            }
        }

        private void RenderSkinFormSysBottomInternal(Graphics g, Rectangle rect, ControlBoxState state, bool active, CCSkinMain form)
        {
            Bitmap btm = null;
            Color baseColor = this.ColorTable.ControlBoxActive;
            if (state == ControlBoxState.Pressed)
            {
                btm = (Bitmap) form.SysBottomDown;
                baseColor = this.ColorTable.ControlBoxPressed;
            }
            else if (state == ControlBoxState.Hover)
            {
                btm = (Bitmap) form.SysBottomMouse;
                baseColor = this.ColorTable.ControlBoxHover;
            }
            else
            {
                btm = (Bitmap) form.SysBottomNorml;
                baseColor = active ? this.ColorTable.ControlBoxActive : this.ColorTable.ControlBoxDeactive;
            }
            if (btm != null)
            {
                g.DrawImage(btm, rect);
            }
            else
            {
                RoundStyle roundStyle = RoundStyle.BottomLeft;
                using (new AntiAliasGraphics(g))
                {
                    RenderHelper.RenderBackgroundInternal(g, rect, baseColor, baseColor, this.ColorTable.ControlBoxInnerBorder, roundStyle, 6, 0.38f, true, false, LinearGradientMode.Vertical);
                    using (Pen pen = new Pen(this.ColorTable.Border))
                    {
                        g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Y);
                    }
                }
            }
        }

        public ImageAttributes Trank(Bitmap btm, int Alphas)
        {
            Bitmap box = (Bitmap) btm.Clone();
            Graphics.FromImage(box);
            float Alpha = ((float) Alphas) / 100f;
            float[][] CS_0_0000 = new float[5][];
            float[] CS_0_0001 = new float[5];
            CS_0_0001[0] = 1f;
            CS_0_0000[0] = CS_0_0001;
            float[] CS_0_0002 = new float[5];
            CS_0_0002[1] = 1f;
            CS_0_0000[1] = CS_0_0002;
            float[] CS_0_0003 = new float[5];
            CS_0_0003[2] = 1f;
            CS_0_0000[2] = CS_0_0003;
            float[] CS_0_0004 = new float[5];
            CS_0_0004[3] = Alpha;
            CS_0_0000[3] = CS_0_0004;
            float[] CS_0_0005 = new float[5];
            CS_0_0005[4] = 1f;
            CS_0_0000[4] = CS_0_0005;
            float[][] matrixltems = CS_0_0000;
            ColorMatrix colorMatrix = new ColorMatrix(matrixltems);
            ImageAttributes ImageAtt = new ImageAttributes();
            ImageAtt.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            return ImageAtt;
        }

        public SkinFormColorTable ColorTable
        {
            get
            {
                if (this._colorTable == null)
                {
                    this._colorTable = new SkinFormColorTable();
                }
                return this._colorTable;
            }
        }
    }
}

