﻿namespace CCWin.SkinControl
{
    using CCWin.SkinClass;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    public sealed class ControlPaintEx
    {
        private ControlPaintEx()
        {
        }

        internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout)
        {
            Rectangle rectangle = bounds;
            if (backgroundImage != null)
            {
                switch (imageLayout)
                {
                    case ImageLayout.None:
                        rectangle.Size = backgroundImage.Size;
                        return rectangle;

                    case ImageLayout.Tile:
                        return rectangle;

                    case ImageLayout.Center:
                    {
                        rectangle.Size = backgroundImage.Size;
                        Size size = bounds.Size;
                        if (size.Width > rectangle.Width)
                        {
                            rectangle.X = (size.Width - rectangle.Width) / 2;
                        }
                        if (size.Height > rectangle.Height)
                        {
                            rectangle.Y = (size.Height - rectangle.Height) / 2;
                        }
                        return rectangle;
                    }
                    case ImageLayout.Stretch:
                        rectangle.Size = bounds.Size;
                        return rectangle;

                    case ImageLayout.Zoom:
                    {
                        Size size2 = backgroundImage.Size;
                        float num = ((float) bounds.Width) / ((float) size2.Width);
                        float num2 = ((float) bounds.Height) / ((float) size2.Height);
                        if (num >= num2)
                        {
                            rectangle.Height = bounds.Height;
                            rectangle.Width = (int) ((size2.Width * num2) + 0.5);
                            if (bounds.X >= 0)
                            {
                                rectangle.X = (bounds.Width - rectangle.Width) / 2;
                            }
                            return rectangle;
                        }
                        rectangle.Width = bounds.Width;
                        rectangle.Height = (int) ((size2.Height * num) + 0.5);
                        if (bounds.Y >= 0)
                        {
                            rectangle.Y = (bounds.Height - rectangle.Height) / 2;
                        }
                        return rectangle;
                    }
                }
            }
            return rectangle;
        }

        public static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect)
        {
            DrawBackgroundImage(g, backgroundImage, backColor, backgroundImageLayout, bounds, clipRect, Point.Empty, RightToLeft.No);
        }

        public static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset)
        {
            DrawBackgroundImage(g, backgroundImage, backColor, backgroundImageLayout, bounds, clipRect, scrollOffset, RightToLeft.No);
        }

        public static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset, RightToLeft rightToLeft)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }
            if (backgroundImageLayout == ImageLayout.Tile)
            {
                using (TextureBrush brush = new TextureBrush(backgroundImage, WrapMode.Tile))
                {
                    if (scrollOffset != Point.Empty)
                    {
                        Matrix transform = brush.Transform;
                        transform.Translate((float) scrollOffset.X, (float) scrollOffset.Y);
                        brush.Transform = transform;
                    }
                    g.FillRectangle(brush, clipRect);
                    return;
                }
            }
            Rectangle rect = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);
            if ((rightToLeft == RightToLeft.Yes) && (backgroundImageLayout == ImageLayout.None))
            {
                rect.X += clipRect.Width - rect.Width;
            }
            using (SolidBrush brush2 = new SolidBrush(backColor))
            {
                g.FillRectangle(brush2, clipRect);
            }
            if (!clipRect.Contains(rect))
            {
                if ((backgroundImageLayout == ImageLayout.Stretch) || (backgroundImageLayout == ImageLayout.Zoom))
                {
                    rect.Intersect(clipRect);
                    g.DrawImage(backgroundImage, rect);
                }
                else if (backgroundImageLayout == ImageLayout.None)
                {
                    rect.Offset(clipRect.Location);
                    Rectangle destRect = rect;
                    destRect.Intersect(clipRect);
                    Rectangle rectangle3 = new Rectangle(Point.Empty, destRect.Size);
                    g.DrawImage(backgroundImage, destRect, rectangle3.X, rectangle3.Y, rectangle3.Width, rectangle3.Height, GraphicsUnit.Pixel);
                }
                else
                {
                    Rectangle rectangle4 = rect;
                    rectangle4.Intersect(clipRect);
                    Rectangle rectangle5 = new Rectangle(new Point(rectangle4.X - rect.X, rectangle4.Y - rect.Y), rectangle4.Size);
                    g.DrawImage(backgroundImage, rectangle4, rectangle5.X, rectangle5.Y, rectangle5.Width, rectangle5.Height, GraphicsUnit.Pixel);
                }
            }
            else
            {
                ImageAttributes imageAttr = new ImageAttributes();
                imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                g.DrawImage(backgroundImage, rect, 0, 0, backgroundImage.Width, backgroundImage.Height, GraphicsUnit.Pixel, imageAttr);
                imageAttr.Dispose();
            }
        }

        public static void DrawCheckedFlag(Graphics g, Rectangle rect, Color color)
        {
            PointF[] points = new PointF[] { new PointF(rect.X + (((float) rect.Width) / 4.5f), rect.Y + (((float) rect.Height) / 2.5f)), new PointF(rect.X + (((float) rect.Width) / 2.5f), rect.Bottom - (((float) rect.Height) / 3f)), new PointF(rect.Right - (((float) rect.Width) / 4f), rect.Y + (((float) rect.Height) / 4.5f)) };
            using (Pen pen = new Pen(color, 2f))
            {
                g.DrawLines(pen, points);
            }
        }

        public static void DrawGlass(Graphics g, RectangleF glassRect, int alphaCenter, int alphaSurround)
        {
            DrawGlass(g, glassRect, Color.White, alphaCenter, alphaSurround);
        }

        public static void DrawGlass(Graphics g, RectangleF glassRect, Color glassColor, int alphaCenter, int alphaSurround)
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

        internal static void DrawGradientRect(Graphics g, Rectangle rect, Color begin, Color end, Color border, Color innerBorder, Blend blend, LinearGradientMode mode, bool drawBorder, bool drawInnerBorder)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, begin, end, mode))
            {
                brush.Blend = blend;
                g.FillRectangle(brush, rect);
            }
            if (drawBorder)
            {
                ControlPaint.DrawBorder(g, rect, border, ButtonBorderStyle.Solid);
            }
            if (drawInnerBorder)
            {
                rect.Inflate(-1, -1);
                ControlPaint.DrawBorder(g, rect, border, ButtonBorderStyle.Solid);
            }
        }

        internal static void DrawGradientRoundRect(Graphics g, Rectangle rect, Color begin, Color end, Color border, Color innerBorder, Blend blend, LinearGradientMode mode, int radios, RoundStyle roundStyle, bool drawBorder, bool drawInnderBorder)
        {
            using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, radios, roundStyle, true))
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(rect, begin, end, mode))
                {
                    brush.Blend = blend;
                    g.FillPath(brush, path);
                }
                if (drawBorder)
                {
                    using (Pen pen = new Pen(border))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
            if (drawInnderBorder)
            {
                rect.Inflate(-1, -1);
                using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, radios, roundStyle, true))
                {
                    using (Pen pen = new Pen(innerBorder))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        public static void DrawScrollBarArraw(Graphics g, Rectangle rect, Color begin, Color end, Color border, Color innerBorder, Color fore, Orientation orientation, ArrowDirection arrowDirection, bool changeColor)
        {
            if (changeColor)
            {
                Color tmp = begin;
                begin = end;
                end = tmp;
            }
            LinearGradientMode mode = (orientation == Orientation.Horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
            rect.Inflate(-1, -1);
            Blend blend = new Blend();
            float[] CS_0_0000 = new float[3];
            CS_0_0000[0] = 1f;
            CS_0_0000[1] = 0.5f;
            blend.Factors = CS_0_0000;
            float[] CS_0_0001 = new float[3];
            CS_0_0001[1] = 0.5f;
            CS_0_0001[2] = 1f;
            blend.Positions = CS_0_0001;
            DrawGradientRoundRect(g, rect, begin, end, border, innerBorder, blend, mode, 4, RoundStyle.All, true, true);
            using (SolidBrush brush = new SolidBrush(fore))
            {
                RenderHelper.RenderArrowInternal(g, rect, arrowDirection, brush);
            }
        }

        public static void DrawScrollBarSizer(Graphics g, Rectangle rect, Color begin, Color end)
        {
            Blend blend = new Blend();
            float[] CS_0_0000 = new float[3];
            CS_0_0000[0] = 1f;
            CS_0_0000[1] = 0.5f;
            blend.Factors = CS_0_0000;
            float[] CS_0_0001 = new float[3];
            CS_0_0001[1] = 0.5f;
            CS_0_0001[2] = 1f;
            blend.Positions = CS_0_0001;
            DrawGradientRect(g, rect, begin, end, begin, begin, blend, LinearGradientMode.Horizontal, true, false);
        }

        public static void DrawScrollBarThumb(Graphics g, Rectangle rect, Color begin, Color end, Color border, Color innerBorder, Orientation orientation, bool changeColor)
        {
            if (changeColor)
            {
                Color tmp = begin;
                begin = end;
                end = tmp;
            }
            bool bHorizontal = orientation == Orientation.Horizontal;
            LinearGradientMode mode = bHorizontal ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
            Blend blend = new Blend();
            float[] CS_0_0000 = new float[3];
            CS_0_0000[0] = 1f;
            CS_0_0000[1] = 0.5f;
            blend.Factors = CS_0_0000;
            float[] CS_0_0001 = new float[3];
            CS_0_0001[1] = 0.5f;
            CS_0_0001[2] = 1f;
            blend.Positions = CS_0_0001;
            if (bHorizontal)
            {
                rect.Inflate(0, -1);
            }
            else
            {
                rect.Inflate(-1, 0);
            }
            DrawGradientRoundRect(g, rect, begin, end, border, innerBorder, blend, mode, 4, RoundStyle.All, true, true);
        }

        public static void DrawScrollBarTrack(Graphics g, Rectangle rect, Color begin, Color end, Orientation orientation)
        {
            LinearGradientMode mode = (orientation == Orientation.Horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
            Blend blend = new Blend();
            float[] CS_0_0000 = new float[3];
            CS_0_0000[0] = 1f;
            CS_0_0000[1] = 0.5f;
            blend.Factors = CS_0_0000;
            float[] CS_0_0001 = new float[3];
            CS_0_0001[1] = 0.5f;
            CS_0_0001[2] = 1f;
            blend.Positions = CS_0_0001;
            DrawGradientRect(g, rect, begin, end, begin, begin, blend, mode, true, false);
        }
    }
}
