namespace CCWin.SkinControl
{
    using CCWin.SkinClass;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    internal class RenderHelperStrip
    {
        internal static Color GetColor(Color colorBase, int a, int r, int g, int b)
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
                a = Math.Max(0, a + a0);
            }
            if ((r + r0) > 0xff)
            {
                r = 0xff;
            }
            else
            {
                r = Math.Max(0, r + r0);
            }
            if ((g + g0) > 0xff)
            {
                g = 0xff;
            }
            else
            {
                g = Math.Max(0, g + g0);
            }
            if ((b + b0) > 0xff)
            {
                b = 0xff;
            }
            else
            {
                b = Math.Max(0, b + b0);
            }
            return Color.FromArgb(a, r, g, b);
        }

        internal static void RenderArrowInternal(Graphics g, Rectangle dropDownRect, ArrowDirection direction, Brush brush)
        {
            Point point = new Point(dropDownRect.Left + (dropDownRect.Width / 2), dropDownRect.Top + (dropDownRect.Height / 2));
            Point[] points = null;
            switch (direction)
            {
                case ArrowDirection.Left:
                    points = new Point[] { new Point(point.X + 2, point.Y - 3), new Point(point.X + 2, point.Y + 3), new Point(point.X - 1, point.Y) };
                    break;

                case ArrowDirection.Up:
                    points = new Point[] { new Point(point.X - 3, point.Y + 2), new Point(point.X + 3, point.Y + 2), new Point(point.X, point.Y - 2) };
                    break;

                case ArrowDirection.Right:
                    points = new Point[] { new Point(point.X - 2, point.Y - 3), new Point(point.X - 2, point.Y + 3), new Point(point.X + 1, point.Y) };
                    break;

                default:
                    points = new Point[] { new Point(point.X - 3, point.Y - 1), new Point(point.X + 3, point.Y - 1), new Point(point.X, point.Y + 2) };
                    break;
            }
            g.FillPolygon(brush, points);
        }

        internal static void RenderBackgroundInternal(Graphics g, Rectangle rect, Color baseColor, Color borderColor, Color innerBorderColor, RoundStyle style, bool drawBorder, bool drawGlass, LinearGradientMode mode)
        {
            RenderBackgroundInternal(g, rect, baseColor, borderColor, innerBorderColor, style, 8, drawBorder, drawGlass, mode);
        }

        internal static void RenderBackgroundInternal(Graphics g, Rectangle rect, Color baseColor, Color borderColor, Color innerBorderColor, RoundStyle style, int roundWidth, bool drawBorder, bool drawGlass, LinearGradientMode mode)
        {
            RenderBackgroundInternal(g, rect, baseColor, borderColor, innerBorderColor, style, roundWidth, 0.45f, drawBorder, drawGlass, mode);
        }

        internal static void RenderBackgroundInternal(Graphics g, Rectangle rect, Color baseColor, Color borderColor, Color innerBorderColor, RoundStyle style, int roundWidth, float basePosition, bool drawBorder, bool drawGlass, LinearGradientMode mode)
        {
            if (drawBorder)
            {
                rect.Width--;
                rect.Height--;
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Transparent, Color.Transparent, mode))
            {
                Color[] colors = new Color[] { GetColor(baseColor, 0, 0x23, 0x18, 9), GetColor(baseColor, 0, 13, 8, 3), baseColor, GetColor(baseColor, 0, 0x23, 0x18, 9) };
                ColorBlend blend = new ColorBlend();
                float[] CS_0_0000 = new float[4];
                CS_0_0000[1] = basePosition;
                CS_0_0000[2] = basePosition + 0.05f;
                CS_0_0000[3] = 1f;
                blend.Positions = CS_0_0000;
                blend.Colors = colors;
                brush.InterpolationColors = blend;
                if (style != RoundStyle.None)
                {
                    using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, roundWidth, style, false))
                    {
                        g.FillPath(brush, path);
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
                        CCWin.SkinControl.ControlPaintEx.DrawGlass(g, glassRect, 170, 0);
                        if (baseColor.A > 0)
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
                            using (GraphicsPath pathTop = GraphicsPathHelper.CreatePath(rectTop, roundWidth, RoundStyle.Top, false))
                            {
                                using (SolidBrush brushAlpha = new SolidBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff)))
                                {
                                    g.FillPath(brushAlpha, pathTop);
                                }
                            }
                        }
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
                    CCWin.SkinControl.ControlPaintEx.DrawGlass(g, glassRect, 200, 0);
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
                        using (SolidBrush brushAlpha = new SolidBrush(Color.FromArgb(0x80, 0xff, 0xff, 0xff)))
                        {
                            g.FillRectangle(brushAlpha, rectTop);
                        }
                    }
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
    }
}

