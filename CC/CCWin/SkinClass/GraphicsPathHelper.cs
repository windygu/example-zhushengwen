namespace CCWin.SkinClass
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public static class GraphicsPathHelper
    {
        public static GraphicsPath CreatePath(Rectangle rect, int radius, RoundStyle style, bool correction)
        {
            GraphicsPath path = new GraphicsPath();
            int radiusCorrection = correction ? 1 : 0;
            switch (style)
            {
                case RoundStyle.None:
                    path.AddRectangle(rect);
                    break;

                case RoundStyle.All:
                    path.AddArc(rect.X, rect.Y, radius, radius, 180f, 90f);
                    path.AddArc((rect.Right - radius) - radiusCorrection, rect.Y, radius, radius, 270f, 90f);
                    path.AddArc((rect.Right - radius) - radiusCorrection, (rect.Bottom - radius) - radiusCorrection, radius, radius, 0f, 90f);
                    path.AddArc(rect.X, (rect.Bottom - radius) - radiusCorrection, radius, radius, 90f, 90f);
                    break;

                case RoundStyle.Left:
                    path.AddArc(rect.X, rect.Y, radius, radius, 180f, 90f);
                    path.AddLine(rect.Right - radiusCorrection, rect.Y, rect.Right - radiusCorrection, rect.Bottom - radiusCorrection);
                    path.AddArc(rect.X, (rect.Bottom - radius) - radiusCorrection, radius, radius, 90f, 90f);
                    break;

                case RoundStyle.Right:
                    path.AddArc((rect.Right - radius) - radiusCorrection, rect.Y, radius, radius, 270f, 90f);
                    path.AddArc((rect.Right - radius) - radiusCorrection, (rect.Bottom - radius) - radiusCorrection, radius, radius, 0f, 90f);
                    path.AddLine(rect.X, rect.Bottom - radiusCorrection, rect.X, rect.Y);
                    break;

                case RoundStyle.Top:
                    path.AddArc(rect.X, rect.Y, radius, radius, 180f, 90f);
                    path.AddArc((rect.Right - radius) - radiusCorrection, rect.Y, radius, radius, 270f, 90f);
                    path.AddLine(rect.Right - radiusCorrection, rect.Bottom - radiusCorrection, rect.X, rect.Bottom - radiusCorrection);
                    break;

                case RoundStyle.Bottom:
                    path.AddArc((rect.Right - radius) - radiusCorrection, (rect.Bottom - radius) - radiusCorrection, radius, radius, 0f, 90f);
                    path.AddArc(rect.X, (rect.Bottom - radius) - radiusCorrection, radius, radius, 90f, 90f);
                    path.AddLine(rect.X, rect.Y, rect.Right - radiusCorrection, rect.Y);
                    break;

                case RoundStyle.BottomLeft:
                    path.AddArc(rect.X, (rect.Bottom - radius) - radiusCorrection, radius, radius, 90f, 90f);
                    path.AddLine(rect.X, rect.Y, rect.Right - radiusCorrection, rect.Y);
                    path.AddLine(rect.Right - radiusCorrection, rect.Y, rect.Right - radiusCorrection, rect.Bottom - radiusCorrection);
                    break;

                case RoundStyle.BottomRight:
                    path.AddArc((rect.Right - radius) - radiusCorrection, (rect.Bottom - radius) - radiusCorrection, radius, radius, 0f, 90f);
                    path.AddLine(rect.X, rect.Bottom - radiusCorrection, rect.X, rect.Y);
                    path.AddLine(rect.X, rect.Y, rect.Right - radiusCorrection, rect.Y);
                    break;
            }
            path.CloseFigure();
            return path;
        }
    }
}

