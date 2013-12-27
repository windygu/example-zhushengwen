namespace CCWin.SkinClass
{
    using CCWin;
    using CCWin.SkinControl;
    using CCWin.Win32;
    using CCWin.Win32.Struct;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class UpdateForm
    {
        public static Bitmap BothAlpha(Bitmap p_Bitmap, bool p_CentralTransparent, bool p_Crossdirection)
        {
            Bitmap _SetBitmap = new Bitmap(p_Bitmap.Width, p_Bitmap.Height);
            Graphics _GraphisSetBitmap = Graphics.FromImage(_SetBitmap);
            _GraphisSetBitmap.DrawImage(p_Bitmap, new Rectangle(0, 0, p_Bitmap.Width, p_Bitmap.Height));
            _GraphisSetBitmap.Dispose();
            Bitmap _Bitmap = new Bitmap(_SetBitmap.Width, _SetBitmap.Height);
            Graphics _Graphcis = Graphics.FromImage(_Bitmap);
            System.Drawing.Point _Left1 = new System.Drawing.Point(0, 0);
            System.Drawing.Point _Left2 = new System.Drawing.Point(_Bitmap.Width, 0);
            System.Drawing.Point _Left3 = new System.Drawing.Point(_Bitmap.Width, _Bitmap.Height / 2);
            System.Drawing.Point _Left4 = new System.Drawing.Point(0, _Bitmap.Height / 2);
            if (p_Crossdirection)
            {
                _Left1 = new System.Drawing.Point(0, 0);
                _Left2 = new System.Drawing.Point(_Bitmap.Width / 2, 0);
                _Left3 = new System.Drawing.Point(_Bitmap.Width / 2, _Bitmap.Height);
                _Left4 = new System.Drawing.Point(0, _Bitmap.Height);
            }
            System.Drawing.Point[] _Point = new System.Drawing.Point[] { _Left1, _Left2, _Left3, _Left4 };
            PathGradientBrush _SetBruhs = new PathGradientBrush(_Point, WrapMode.TileFlipY);
            _SetBruhs.CenterPoint = new PointF(0f, 0f);
            _SetBruhs.FocusScales = new PointF((float) (_Bitmap.Width / 2), 0f);
            _SetBruhs.CenterColor = Color.FromArgb(0, 0xff, 0xff, 0xff);
            _SetBruhs.SurroundColors = new Color[] { Color.FromArgb(0xff, 0xff, 0xff, 0xff) };
            if (p_Crossdirection)
            {
                _SetBruhs.FocusScales = new PointF(0f, (float) _Bitmap.Height);
                _SetBruhs.WrapMode = WrapMode.TileFlipX;
            }
            if (p_CentralTransparent)
            {
                _SetBruhs.CenterColor = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
                _SetBruhs.SurroundColors = new Color[] { Color.FromArgb(0, 0xff, 0xff, 0xff) };
            }
            _Graphcis.FillRectangle(_SetBruhs, new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height));
            _Graphcis.Dispose();
            BitmapData _NewData = _Bitmap.LockBits(new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height), ImageLockMode.ReadOnly, _Bitmap.PixelFormat);
            byte[] _NewBytes = new byte[_NewData.Stride * _NewData.Height];
            Marshal.Copy(_NewData.Scan0, _NewBytes, 0, _NewBytes.Length);
            _Bitmap.UnlockBits(_NewData);
            BitmapData _SetData = _SetBitmap.LockBits(new Rectangle(0, 0, _SetBitmap.Width, _SetBitmap.Height), ImageLockMode.ReadWrite, _SetBitmap.PixelFormat);
            byte[] _SetBytes = new byte[_SetData.Stride * _SetData.Height];
            Marshal.Copy(_SetData.Scan0, _SetBytes, 0, _SetBytes.Length);
            int _WriteIndex = 0;
            for (int i = 0; i != _SetData.Height; i++)
            {
                _WriteIndex = (i * _SetData.Stride) + 3;
                for (int z = 0; z != _SetData.Width; z++)
                {
                    _SetBytes[_WriteIndex] = _NewBytes[_WriteIndex];
                    _WriteIndex += 4;
                }
            }
            Marshal.Copy(_SetBytes, 0, _SetData.Scan0, _SetBytes.Length);
            _SetBitmap.UnlockBits(_SetData);
            return _SetBitmap;
        }

        public static GraphicsPath CalculateControlGraphicsPath(Bitmap bitmap, int Alpha)
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            int colOpaquePixel = 0;
            for (int row = 0; row < bitmap.Height; row++)
            {
                colOpaquePixel = 0;
                for (int col = 0; col < bitmap.Width; col++)
                {
                    if (bitmap.GetPixel(col, row).A < Alpha)
                    {
                        continue;
                    }
                    colOpaquePixel = col;
                    int colNext = col;
                    colNext = colOpaquePixel;
                    while (colNext < bitmap.Width)
                    {
                        if (bitmap.GetPixel(colNext, row).A < Alpha)
                        {
                            break;
                        }
                        colNext++;
                    }
                    graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1));
                    col = colNext;
                }
            }
            return graphicsPath;
        }

        public static void CreateControlRegion(Control control, Bitmap bitmap, int Alpha)
        {
            if ((control != null) && (bitmap != null))
            {
                control.Width = bitmap.Width;
                control.Height = bitmap.Height;
                if (control is Form)
                {
                    Form form = (Form) control;
                    form.Width = control.Width;
                    form.Height = control.Height;
                    form.FormBorderStyle = FormBorderStyle.None;
                    form.BackgroundImage = bitmap;
                    GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap, Alpha);
                    form.Region = new Region(graphicsPath);
                }
                else if (control is SkinButtom)
                {
                    SkinButtom button = (SkinButtom) control;
                    GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap, Alpha);
                    button.Region = new Region(graphicsPath);
                }
                else if (control is SkinProgressBar)
                {
                    SkinProgressBar Progressbar = (SkinProgressBar) control;
                    GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap, Alpha);
                    Progressbar.Region = new Region(graphicsPath);
                }
            }
        }

        public static void CreateRegion(Control control, Rectangle bounds)
        {
            CreateRegion(control, bounds, 8, RoundStyle.All);
        }

        public static void CreateRegion(Control ctrl, int RgnRadius)
        {
            int Rgn = CCWin.Win32.NativeMethods.CreateRoundRectRgn(0, 0, ctrl.ClientRectangle.Width + 1, ctrl.ClientRectangle.Height + 1, RgnRadius, RgnRadius);
            CCWin.Win32.NativeMethods.SetWindowRgn(ctrl.Handle, Rgn, true);
        }

        public static void CreateRegion(IntPtr hWnd, int radius, RoundStyle roundStyle, bool redraw)
        {
            CCWin.Win32.Struct.RECT bounds = new CCWin.Win32.Struct.RECT();
            CCWin.Win32.NativeMethods.GetWindowRect(hWnd, ref bounds);
            Rectangle rect = new Rectangle(System.Drawing.Point.Empty, bounds.Size);
            if (roundStyle != RoundStyle.None)
            {
                using (GraphicsPath path = GraphicsPathHelper.CreatePath(rect, radius, roundStyle, true))
                {
                    using (Region region = new Region(path))
                    {
                        path.Widen(Pens.White);
                        region.Union(path);
                        IntPtr hDc = CCWin.Win32.NativeMethods.GetWindowDC(hWnd);
                        try
                        {
                            using (Graphics g = Graphics.FromHdc(hDc))
                            {
                                CCWin.Win32.NativeMethods.SetWindowRgn(hWnd, region.GetHrgn(g), redraw);
                            }
                        }
                        finally
                        {
                            CCWin.Win32.NativeMethods.ReleaseDC(hWnd, hDc);
                        }
                    }
                    return;
                }
            }
            IntPtr hRgn = CCWin.Win32.NativeMethods.CreateRectRgn(0, 0, rect.Width, rect.Height);
            CCWin.Win32.NativeMethods.SetWindowRgn(hWnd, hRgn, redraw);
        }

        public static void CreateRegion(Control control, Rectangle bounds, int radius, RoundStyle roundStyle)
        {
            if (roundStyle != RoundStyle.None)
            {
                using (GraphicsPath path = GraphicsPathHelper.CreatePath(bounds, radius, roundStyle, true))
                {
                    Region region = new Region(path);
                    path.Widen(Pens.White);
                    region.Union(path);
                    control.Region = region;
                    return;
                }
            }
            if (control.Region != null)
            {
                control.Region = null;
            }
        }

        public static void CursorClick(int x, int y)
        {
            int MOUSEEVENTF_LEFTDOWN = 2;
            int MOUSEEVENTF_LEFTUP = 4;
            CCWin.Win32.NativeMethods.mouse_event(MOUSEEVENTF_LEFTDOWN, (x * 0x10000) / 0x400, (y * 0x10000) / 0x300, 0, 0);
            CCWin.Win32.NativeMethods.mouse_event(MOUSEEVENTF_LEFTUP, (x * 0x10000) / 0x400, (y * 0x10000) / 0x300, 0, 0);
        }

        public static Bitmap GaryImg(Bitmap b)
        {
            Bitmap bmp = b.Clone(new Rectangle(0, 0, b.Width, b.Height), PixelFormat.Format24bppRgb);
            b.Dispose();
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
            byte[] byColorInfo = new byte[bmp.Height * bmpData.Stride];
            Marshal.Copy(bmpData.Scan0, byColorInfo, 0, byColorInfo.Length);
            int x = 0;
            int xLen = bmp.Width;
            while (x < xLen)
            {
                int y = 0;
                int yLen = bmp.Height;
                while (y < yLen)
                {
                    byte CS_0_0000;
                    byColorInfo[((y * bmpData.Stride) + (x * 3)) + 2] = CS_0_0000 = GetAvg(byColorInfo[(y * bmpData.Stride) + (x * 3)], byColorInfo[((y * bmpData.Stride) + (x * 3)) + 1], byColorInfo[((y * bmpData.Stride) + (x * 3)) + 2]);
                    byColorInfo[(y * bmpData.Stride) + (x * 3)] = byColorInfo[((y * bmpData.Stride) + (x * 3)) + 1] = CS_0_0000;
                    y++;
                }
                x++;
            }
            Marshal.Copy(byColorInfo, 0, bmpData.Scan0, byColorInfo.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private static byte GetAvg(byte b, byte g, byte r)
        {
            return (byte) (((r + g) + b) / 3);
        }

        public static Color GetImageAverageColor(Bitmap back)
        {
            return BitmapHelper.GetImageAverageColor(back);
        }

        public static Image ImageLightEffect(string Str, Font F, Color ColorFore, Color ColorBack, int BlurConsideration)
        {
            Bitmap Var_Bitmap = null;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF Var_Size = g.MeasureString(Str, F);
                using (Bitmap Var_bmp = new Bitmap((int) Var_Size.Width, (int) Var_Size.Height))
                {
                    using (Graphics Var_G_Bmp = Graphics.FromImage(Var_bmp))
                    {
                        using (SolidBrush Var_BrushBack = new SolidBrush(Color.FromArgb(0x10, ColorBack.R, ColorBack.G, ColorBack.B)))
                        {
                            using (SolidBrush Var_BrushFore = new SolidBrush(ColorFore))
                            {
                                Var_G_Bmp.SmoothingMode = SmoothingMode.HighQuality;
                                Var_G_Bmp.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                Var_G_Bmp.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                                Var_G_Bmp.DrawString(Str, F, Var_BrushBack, (float) 0f, (float) 0f);
                                Var_Bitmap = new Bitmap(Var_bmp.Width + BlurConsideration, Var_bmp.Height + BlurConsideration);
                                using (Graphics Var_G_Bitmap = Graphics.FromImage(Var_Bitmap))
                                {
                                    Var_G_Bitmap.SmoothingMode = SmoothingMode.HighQuality;
                                    Var_G_Bitmap.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                    Var_G_Bitmap.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                                    for (int x = 0; x <= BlurConsideration; x++)
                                    {
                                        for (int y = 0; y <= BlurConsideration; y++)
                                        {
                                            Var_G_Bitmap.DrawImageUnscaled(Var_bmp, x, y);
                                        }
                                    }
                                    Var_G_Bitmap.DrawString(Str, F, Var_BrushFore, (float) (BlurConsideration / 2), (float) (BlurConsideration / 2));
                                }
                                return Var_Bitmap;
                            }
                        }
                    }
                }
            }
        }

        public static Image ImageLightEffect(string Str, Font F, Color ColorFore, Color ColorBack, int BlurConsideration, Rectangle rc, bool auto)
        {
            Bitmap Var_Bitmap = null;
            StringFormat sf = new StringFormat(StringFormatFlags.NoWrap);
            sf.Trimming = auto ? StringTrimming.EllipsisWord : StringTrimming.None;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF Var_Size = g.MeasureString(Str, F);
                using (Bitmap Var_bmp = new Bitmap((int) Var_Size.Width, (int) Var_Size.Height))
                {
                    using (Graphics Var_G_Bmp = Graphics.FromImage(Var_bmp))
                    {
                        using (SolidBrush Var_BrushBack = new SolidBrush(Color.FromArgb(0x10, ColorBack.R, ColorBack.G, ColorBack.B)))
                        {
                            using (SolidBrush Var_BrushFore = new SolidBrush(ColorFore))
                            {
                                Var_G_Bmp.SmoothingMode = SmoothingMode.HighQuality;
                                Var_G_Bmp.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                Var_G_Bmp.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                                Var_G_Bmp.DrawString(Str, F, Var_BrushBack, rc, sf);
                                Var_Bitmap = new Bitmap(Var_bmp.Width + BlurConsideration, Var_bmp.Height + BlurConsideration);
                                using (Graphics Var_G_Bitmap = Graphics.FromImage(Var_Bitmap))
                                {
                                    if (ColorBack != Color.Transparent)
                                    {
                                        Var_G_Bitmap.SmoothingMode = SmoothingMode.HighQuality;
                                        Var_G_Bitmap.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                        Var_G_Bitmap.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                                        for (int x = 0; x <= BlurConsideration; x++)
                                        {
                                            for (int y = 0; y <= BlurConsideration; y++)
                                            {
                                                Var_G_Bitmap.DrawImageUnscaled(Var_bmp, x, y);
                                            }
                                        }
                                    }
                                    Var_G_Bitmap.DrawString(Str, F, Var_BrushFore, new Rectangle(new System.Drawing.Point(Convert.ToInt32((int) (BlurConsideration / 2)), Convert.ToInt32((int) (BlurConsideration / 2))), rc.Size), sf);
                                }
                                return Var_Bitmap;
                            }
                        }
                    }
                }
            }
        }

        public static Bitmap ResizeBitmap(Bitmap b, int dstWidth, int dstHeight)
        {
            Bitmap dstImage = new Bitmap(dstWidth, dstHeight);
            Graphics g = Graphics.FromImage(dstImage);
            g.InterpolationMode = InterpolationMode.Bilinear;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(b, new Rectangle(0, 0, dstImage.Width, dstImage.Height), new Rectangle(0, 0, b.Width, b.Height), GraphicsUnit.Pixel);
            g.Save();
            g.Dispose();
            return dstImage;
        }
    }
}

