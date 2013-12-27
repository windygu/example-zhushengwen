namespace CCWin.SkinControl
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class LayoutUtils
    {
        public static Rectangle DeflateRect(Rectangle rect, Padding padding)
        {
            rect.X += padding.Left;
            rect.Y += padding.Top;
            rect.Width -= padding.Horizontal;
            rect.Height -= padding.Vertical;
            return rect;
        }

        public static bool IsEmptyRect(Rectangle rect)
        {
            if (rect.Width > 0)
            {
                return (rect.Height <= 0);
            }
            return true;
        }

        public static Rectangle RTLTranslate(Rectangle bounds, Rectangle withinBounds)
        {
            bounds.X = withinBounds.Width - bounds.Right;
            return bounds;
        }
    }
}

