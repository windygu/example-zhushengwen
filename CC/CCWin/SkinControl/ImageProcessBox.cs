namespace CCWin.SkinControl
{
    using CCWin.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class ImageProcessBox : Control
    {
        private bool autoSizeFromImage;
        private Image baseImage;
        private bool canReset;
        private IContainer components;
        private Color dotColor;
        private bool isDrawed;
        private bool isDrawOperationDot;
        private bool isMoving;
        private bool isSetClip;
        private bool isShowInfo;
        private bool isStartDraw;
        private Color lineColor;
        private bool m_bLockH;
        private bool m_bLockW;
        private bool m_bMouseEnter;
        private Bitmap m_bmpDark;
        private Pen m_pen;
        private System.Drawing.Point m_ptCurrent;
        private System.Drawing.Point m_ptOriginal;
        private System.Drawing.Point m_ptTempStarPos;
        private Rectangle m_rectClip;
        private Rectangle[] m_rectDots;
        private SolidBrush m_sb;
        private System.Drawing.Size magnifySize;
        private int magnifyTimes;
        private Rectangle selectedRectangle;

        public ImageProcessBox()
        {
            this.InitializeComponent();
            this.InitMember();
            this.ForeColor = Color.White;
            this.BackColor = Color.Black;
            this.Dock = DockStyle.Fill;
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        private void BuildBitmap()
        {
            if (this.baseImage != null)
            {
                this.m_bmpDark = new Bitmap(this.baseImage);
                using (Graphics g = Graphics.FromImage(this.m_bmpDark))
                {
                    SolidBrush sb = new SolidBrush(Color.FromArgb(0x7d, 0, 0, 0));
                    g.FillRectangle(sb, 0, 0, this.m_bmpDark.Width, this.m_bmpDark.Height);
                    sb.Dispose();
                }
            }
        }

        public void ClearDraw()
        {
            this.isDrawed = false;
            this.selectedRectangle.X = this.selectedRectangle.Y = -100;
            this.selectedRectangle.Width = this.selectedRectangle.Height = 0;
            this.Cursor = Cursors.Default;
            base.Invalidate();
        }

        public void DeleResource()
        {
            this.m_pen.Dispose();
            this.m_sb.Dispose();
            if (this.baseImage != null)
            {
                this.m_bmpDark.Dispose();
                this.baseImage.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual void DrawInfo(Graphics g)
        {
            int tempX = this.m_ptCurrent.X + 20;
            int tempY = this.m_ptCurrent.Y + 20;
            int tempW = (this.magnifySize.Width * this.magnifyTimes) + 8;
            int tempH = ((this.magnifySize.Width * this.magnifyTimes) + 12) + (this.Font.Height * 3);
            if (!this.m_rectClip.IsEmpty)
            {
                if ((tempX + tempW) >= this.m_rectClip.Right)
                {
                    tempX -= tempW + 30;
                }
                if ((tempY + tempH) >= this.m_rectClip.Bottom)
                {
                    tempY -= tempH + 30;
                }
            }
            else
            {
                if ((tempX + tempW) >= base.ClientRectangle.Width)
                {
                    tempX -= tempW + 30;
                }
                if ((tempY + tempH) >= base.ClientRectangle.Height)
                {
                    tempY -= tempH + 30;
                }
            }
            Rectangle tempRectBorder = new Rectangle(tempX + 2, tempY + 2, tempW - 4, (this.magnifySize.Width * this.magnifyTimes) + 4);
            this.m_sb.Color = Color.FromArgb(200, 0, 0, 0);
            g.FillRectangle(this.m_sb, tempX, tempY, tempW, tempH);
            this.m_pen.Width = 2f;
            this.m_pen.Color = Color.White;
            g.DrawRectangle(this.m_pen, tempRectBorder);
            using (Bitmap bmpSrc = new Bitmap(this.magnifySize.Width, this.magnifySize.Height, PixelFormat.Format32bppArgb))
            {
                using (Graphics gp = Graphics.FromImage(bmpSrc))
                {
                    gp.SetClip(new Rectangle(0, 0, this.magnifySize.Width, this.magnifySize.Height));
                    gp.DrawImage(this.baseImage, -(this.m_ptCurrent.X - (this.magnifySize.Width / 2)), -(this.m_ptCurrent.Y - (this.magnifySize.Height / 2)));
                }
                using (Bitmap bmpInfo = this.MagnifyImage(bmpSrc, this.magnifyTimes))
                {
                    g.DrawImage(bmpInfo, (int) (tempX + 4), (int) (tempY + 4));
                }
            }
            this.m_pen.Width = this.magnifyTimes - 2;
            this.m_pen.Color = Color.FromArgb(0x7d, 0, 0xff, 0xff);
            int tempCenterX = tempX + ((tempW + (((this.magnifySize.Width % 2) == 0) ? this.magnifyTimes : 0)) / 2);
            int tempCenterY = (tempY + 2) + ((tempRectBorder.Height + (((this.MagnifySize.Height % 2) == 0) ? this.magnifyTimes : 0)) / 2);
            g.DrawLine(this.m_pen, tempCenterX, tempY + 4, tempCenterX, tempRectBorder.Bottom - 2);
            g.DrawLine(this.m_pen, tempX + 4, tempCenterY, (tempX + tempW) - 4, tempCenterY);
            this.m_sb.Color = this.ForeColor;
            Color clr = ((Bitmap) this.baseImage).GetPixel(this.m_ptCurrent.X, this.m_ptCurrent.Y);
            g.DrawString(string.Concat(new object[] { "Size: ", this.selectedRectangle.Width + 1, " x ", this.selectedRectangle.Height + 1 }), this.Font, this.m_sb, (float) (tempX + 2), (float) (tempRectBorder.Bottom + 2));
            g.DrawString(string.Concat(new object[] { clr.A, ",", clr.R, ",", clr.G, ",", clr.B }), this.Font, this.m_sb, (float) (tempX + 2), (float) ((tempRectBorder.Bottom + 2) + this.Font.Height));
            g.DrawString("0x" + clr.A.ToString("X").PadLeft(2, '0') + clr.R.ToString("X").PadLeft(2, '0') + clr.G.ToString("X").PadLeft(2, '0') + clr.B.ToString("X").PadLeft(2, '0'), this.Font, this.m_sb, (float) (tempX + 2), (float) ((tempRectBorder.Bottom + 2) + (this.Font.Height * 2)));
            this.m_sb.Color = clr;
            g.FillRectangle(this.m_sb, ((tempX + tempW) - 2) - this.Font.Height, ((tempY + tempH) - 2) - this.Font.Height, this.Font.Height, this.Font.Height);
            g.DrawRectangle(Pens.Cyan, ((tempX + tempW) - 2) - this.Font.Height, ((tempY + tempH) - 2) - this.Font.Height, this.Font.Height, this.Font.Height);
            g.FillRectangle(this.m_sb, tempCenterX - (this.magnifyTimes / 2), tempCenterY - (this.magnifyTimes / 2), this.magnifyTimes, this.magnifyTimes);
            g.DrawRectangle(Pens.Cyan, (int) (tempCenterX - (this.magnifyTimes / 2)), (int) (tempCenterY - (this.magnifyTimes / 2)), (int) (this.magnifyTimes - 1), (int) (this.magnifyTimes - 1));
        }

        protected virtual void DrawOperationBox(Graphics g)
        {
            string strDrawSize = string.Concat(new object[] { "X:", this.selectedRectangle.X, " Y:", this.selectedRectangle.Y, " W:", this.selectedRectangle.Width + 1, " H:", this.selectedRectangle.Height + 1 });
            System.Drawing.Size seStr = TextRenderer.MeasureText(strDrawSize, this.Font);
            int tempX = this.selectedRectangle.X;
            int tempY = (this.selectedRectangle.Y - seStr.Height) - 5;
            if (!this.m_rectClip.IsEmpty && ((tempX + seStr.Width) >= this.m_rectClip.Right))
            {
                tempX -= seStr.Width;
            }
            if (tempY <= 0)
            {
                tempY += seStr.Height + 10;
            }
            this.m_sb.Color = Color.FromArgb(0x7d, 0, 0, 0);
            g.FillRectangle(this.m_sb, tempX, tempY, seStr.Width, seStr.Height);
            this.m_sb.Color = this.ForeColor;
            g.DrawString(strDrawSize, this.Font, this.m_sb, (float) tempX, (float) tempY);
            if (!this.isDrawOperationDot)
            {
                this.m_pen.Width = 3f;
                this.m_pen.Color = this.lineColor;
                g.DrawRectangle(this.m_pen, this.selectedRectangle);
            }
            else
            {
                int CS_0_0001;
                int CS_0_0003;
                int CS_0_0005;
                int CS_0_0007;
                this.m_rectDots[2].Y = CS_0_0001 = this.selectedRectangle.Y - 2;
                this.m_rectDots[0].Y = this.m_rectDots[1].Y = CS_0_0001;
                this.m_rectDots[7].Y = CS_0_0003 = this.selectedRectangle.Bottom - 2;
                this.m_rectDots[5].Y = this.m_rectDots[6].Y = CS_0_0003;
                this.m_rectDots[5].X = CS_0_0005 = this.selectedRectangle.X - 2;
                this.m_rectDots[0].X = this.m_rectDots[3].X = CS_0_0005;
                this.m_rectDots[7].X = CS_0_0007 = this.selectedRectangle.Right - 2;
                this.m_rectDots[2].X = this.m_rectDots[4].X = CS_0_0007;
                this.m_rectDots[3].Y = this.m_rectDots[4].Y = (this.selectedRectangle.Y + (this.selectedRectangle.Height / 2)) - 2;
                this.m_rectDots[1].X = this.m_rectDots[6].X = (this.selectedRectangle.X + (this.selectedRectangle.Width / 2)) - 2;
                this.m_pen.Width = 1f;
                this.m_pen.Color = this.lineColor;
                g.DrawRectangle(this.m_pen, this.selectedRectangle);
                this.m_sb.Color = this.dotColor;
                foreach (Rectangle rect in this.m_rectDots)
                {
                    g.FillRectangle(this.m_sb, rect);
                }
                if ((this.selectedRectangle.Width <= 10) || (this.selectedRectangle.Height <= 10))
                {
                    g.DrawRectangle(this.m_pen, this.selectedRectangle);
                }
            }
        }

        ~ImageProcessBox()
        {
            this.m_pen.Dispose();
            this.m_sb.Dispose();
            this.m_bmpDark.Dispose();
            this.baseImage.Dispose();
        }

        public Bitmap GetResultBmp()
        {
            if (this.baseImage == null)
            {
                return null;
            }
            Bitmap bmp = new Bitmap(this.selectedRectangle.Width + 1, this.selectedRectangle.Height + 1);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(this.baseImage, -this.selectedRectangle.X, -this.selectedRectangle.Y);
            }
            return bmp;
        }

        private void InitializeComponent()
        {
            this.components = new Container();
        }

        private void InitMember()
        {
            this.dotColor = Color.Yellow;
            this.lineColor = Color.Cyan;
            this.magnifySize = new System.Drawing.Size(15, 15);
            this.magnifyTimes = 7;
            this.isDrawOperationDot = true;
            this.isSetClip = true;
            this.isShowInfo = true;
            this.autoSizeFromImage = true;
            this.canReset = true;
            this.m_pen = new Pen(this.lineColor, 1f);
            this.m_sb = new SolidBrush(this.dotColor);
            this.selectedRectangle = new Rectangle();
            this.ClearDraw();
            this.m_rectDots = new Rectangle[8];
            for (int i = 0; i < 8; i++)
            {
                this.m_rectDots[i] = new Rectangle(-10, -10, 5, 5);
            }
        }

        private Bitmap MagnifyImage(Bitmap bmpSrc, int times)
        {
            Bitmap bmpNew = new Bitmap(bmpSrc.Width * times, bmpSrc.Height * times, PixelFormat.Format32bppArgb);
            BitmapData bmpSrcData = bmpSrc.LockBits(new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmpNewData = bmpNew.LockBits(new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] bySrcData = new byte[bmpSrcData.Height * bmpSrcData.Stride];
            Marshal.Copy(bmpSrcData.Scan0, bySrcData, 0, bySrcData.Length);
            byte[] byNewData = new byte[bmpNewData.Height * bmpNewData.Stride];
            Marshal.Copy(bmpNewData.Scan0, byNewData, 0, byNewData.Length);
            int y = 0;
            int lenY = bmpSrc.Height;
            while (y < lenY)
            {
                int x = 0;
                int lenX = bmpSrc.Width;
                while (x < lenX)
                {
                    for (int cy = 0; cy < times; cy++)
                    {
                        for (int cx = 0; cx < times; cx++)
                        {
                            byNewData[(((x * times) + cx) * 4) + (((y * times) + cy) * bmpNewData.Stride)] = bySrcData[(x * 4) + (y * bmpSrcData.Stride)];
                            byNewData[((((x * times) + cx) * 4) + (((y * times) + cy) * bmpNewData.Stride)) + 1] = bySrcData[((x * 4) + (y * bmpSrcData.Stride)) + 1];
                            byNewData[((((x * times) + cx) * 4) + (((y * times) + cy) * bmpNewData.Stride)) + 2] = bySrcData[((x * 4) + (y * bmpSrcData.Stride)) + 2];
                            byNewData[((((x * times) + cx) * 4) + (((y * times) + cy) * bmpNewData.Stride)) + 3] = bySrcData[((x * 4) + (y * bmpSrcData.Stride)) + 3];
                        }
                    }
                    x++;
                }
                y++;
            }
            Marshal.Copy(byNewData, 0, bmpNewData.Scan0, byNewData.Length);
            bmpSrc.UnlockBits(bmpSrcData);
            bmpNew.UnlockBits(bmpNewData);
            return bmpNew;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
            {
                CCWin.Win32.NativeMethods.SetCursorPos(Control.MousePosition.X, Control.MousePosition.Y - 1);
            }
            else if (e.KeyChar == 's')
            {
                CCWin.Win32.NativeMethods.SetCursorPos(Control.MousePosition.X, Control.MousePosition.Y + 1);
            }
            else if (e.KeyChar == 'a')
            {
                CCWin.Win32.NativeMethods.SetCursorPos(Control.MousePosition.X - 1, Control.MousePosition.Y);
            }
            else if (e.KeyChar == 'd')
            {
                CCWin.Win32.NativeMethods.SetCursorPos(Control.MousePosition.X + 1, Control.MousePosition.Y);
            }
            base.OnKeyPress(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) && (!this.IsDrawed || (this.Cursor != Cursors.Default)))
            {
                this.m_rectClip = this.DisplayRectangle;
                if ((this.baseImage != null) && this.isSetClip)
                {
                    if ((e.X > this.baseImage.Width) || (e.Y > this.baseImage.Height))
                    {
                        return;
                    }
                    this.m_rectClip.Intersect(new Rectangle(0, 0, this.baseImage.Width, this.baseImage.Height));
                }
                Cursor.Clip = base.RectangleToScreen(this.m_rectClip);
                this.isStartDraw = true;
                this.m_ptOriginal = e.Location;
            }
            base.Focus();
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.m_bMouseEnter = false;
            base.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.m_ptCurrent = e.Location;
            this.m_bMouseEnter = true;
            if (this.isDrawed && this.canReset)
            {
                this.SetCursorStyle(e.Location);
                if (this.isStartDraw && this.isDrawOperationDot)
                {
                    if (this.m_rectDots[0].Contains(e.Location))
                    {
                        this.isDrawed = false;
                        this.m_ptOriginal.X = this.selectedRectangle.Right;
                        this.m_ptOriginal.Y = this.selectedRectangle.Bottom;
                    }
                    else if (this.m_rectDots[1].Contains(e.Location))
                    {
                        this.isDrawed = false;
                        this.m_ptOriginal.Y = this.selectedRectangle.Bottom;
                        this.m_bLockW = true;
                    }
                    else if (this.m_rectDots[2].Contains(e.Location))
                    {
                        this.isDrawed = false;
                        this.m_ptOriginal.X = this.selectedRectangle.X;
                        this.m_ptOriginal.Y = this.selectedRectangle.Bottom;
                    }
                    else if (this.m_rectDots[3].Contains(e.Location))
                    {
                        this.isDrawed = false;
                        this.m_ptOriginal.X = this.selectedRectangle.Right;
                        this.m_bLockH = true;
                    }
                    else if (this.m_rectDots[4].Contains(e.Location))
                    {
                        this.isDrawed = false;
                        this.m_ptOriginal.X = this.selectedRectangle.X;
                        this.m_bLockH = true;
                    }
                    else if (this.m_rectDots[5].Contains(e.Location))
                    {
                        this.isDrawed = false;
                        this.m_ptOriginal.X = this.selectedRectangle.Right;
                        this.m_ptOriginal.Y = this.selectedRectangle.Y;
                    }
                    else if (this.m_rectDots[6].Contains(e.Location))
                    {
                        this.isDrawed = false;
                        this.m_ptOriginal.Y = this.selectedRectangle.Y;
                        this.m_bLockW = true;
                    }
                    else if (this.m_rectDots[7].Contains(e.Location))
                    {
                        this.isDrawed = false;
                        this.m_ptOriginal = this.selectedRectangle.Location;
                    }
                    else if (this.selectedRectangle.Contains(e.Location))
                    {
                        this.isDrawed = false;
                        this.isMoving = true;
                    }
                }
                base.OnMouseMove(e);
            }
            else
            {
                if (this.isStartDraw)
                {
                    if (this.isMoving)
                    {
                        System.Drawing.Point ptLast = this.selectedRectangle.Location;
                        this.selectedRectangle.X = (this.m_ptTempStarPos.X + e.X) - this.m_ptOriginal.X;
                        this.selectedRectangle.Y = (this.m_ptTempStarPos.Y + e.Y) - this.m_ptOriginal.Y;
                        if (this.selectedRectangle.X < 0)
                        {
                            this.selectedRectangle.X = 0;
                        }
                        if (this.selectedRectangle.Y < 0)
                        {
                            this.selectedRectangle.Y = 0;
                        }
                        if (this.selectedRectangle.Right > this.m_rectClip.Width)
                        {
                            this.selectedRectangle.X = (this.m_rectClip.Width - this.selectedRectangle.Width) - 1;
                        }
                        if (this.selectedRectangle.Bottom > this.m_rectClip.Height)
                        {
                            this.selectedRectangle.Y = (this.m_rectClip.Height - this.selectedRectangle.Height) - 1;
                        }
                        if (base.Location == ptLast)
                        {
                            return;
                        }
                    }
                    else if ((Math.Abs((int) (e.X - this.m_ptOriginal.X)) > 1) || (Math.Abs((int) (e.Y - this.m_ptOriginal.Y)) > 1))
                    {
                        if (!this.m_bLockW)
                        {
                            this.selectedRectangle.X = ((this.m_ptOriginal.X - e.X) < 0) ? this.m_ptOriginal.X : e.X;
                            this.selectedRectangle.Width = Math.Abs((int) (this.m_ptOriginal.X - e.X));
                        }
                        if (!this.m_bLockH)
                        {
                            this.selectedRectangle.Y = ((this.m_ptOriginal.Y - e.Y) < 0) ? this.m_ptOriginal.Y : e.Y;
                            this.selectedRectangle.Height = Math.Abs((int) (this.m_ptOriginal.Y - e.Y));
                        }
                    }
                    base.Invalidate();
                }
                if (((this.baseImage != null) && !this.isDrawed) && (!this.isMoving && this.isShowInfo))
                {
                    base.Invalidate();
                }
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if ((this.selectedRectangle.Width >= 4) && (this.selectedRectangle.Height >= 4))
                {
                    this.isDrawed = true;
                }
                else
                {
                    this.ClearDraw();
                }
                this.isMoving = this.m_bLockH = this.m_bLockW = false;
                this.isStartDraw = false;
                this.m_ptTempStarPos = this.selectedRectangle.Location;
                Cursor.Clip = new Rectangle();
            }
            else if (e.Button == MouseButtons.Right)
            {
                this.ClearDraw();
            }
            base.Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (this.baseImage != null)
            {
                g.DrawImage(this.m_bmpDark, 0, 0);
                g.DrawImage(this.baseImage, this.selectedRectangle, this.selectedRectangle, GraphicsUnit.Pixel);
            }
            this.DrawOperationBox(g);
            if ((((this.baseImage != null) && !this.isDrawed) && (!this.isMoving && this.m_bMouseEnter)) && this.isShowInfo)
            {
                this.DrawInfo(e.Graphics);
            }
            base.OnPaint(e);
        }

        private void SetCursorStyle(System.Drawing.Point loc)
        {
            if (this.m_rectDots[0].Contains(loc) || this.m_rectDots[7].Contains(loc))
            {
                this.Cursor = Cursors.SizeNWSE;
            }
            else if (this.m_rectDots[1].Contains(loc) || this.m_rectDots[6].Contains(loc))
            {
                this.Cursor = Cursors.SizeNS;
            }
            else if (this.m_rectDots[2].Contains(loc) || this.m_rectDots[5].Contains(loc))
            {
                this.Cursor = Cursors.SizeNESW;
            }
            else if (this.m_rectDots[3].Contains(loc) || this.m_rectDots[4].Contains(loc))
            {
                this.Cursor = Cursors.SizeWE;
            }
            else if (this.selectedRectangle.Contains(loc))
            {
                this.Cursor = Cursors.SizeAll;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void SetInfoPoint(System.Drawing.Point pt)
        {
            if (this.m_ptCurrent != pt)
            {
                this.m_ptCurrent = pt;
                this.m_bMouseEnter = true;
                base.Invalidate();
            }
        }

        public void SetInfoPoint(int x, int y)
        {
            if ((this.m_ptCurrent.X != x) || (this.m_ptCurrent.Y != y))
            {
                this.m_ptCurrent.X = x;
                this.m_ptCurrent.Y = y;
                this.m_bMouseEnter = true;
                base.Invalidate();
            }
        }

        public void SetSelectRect(Rectangle rect)
        {
            rect.Intersect(this.DisplayRectangle);
            if (!rect.IsEmpty)
            {
                rect.Width--;
                rect.Height--;
                if (this.selectedRectangle != rect)
                {
                    this.selectedRectangle = rect;
                    base.Invalidate();
                }
            }
        }

        public void SetSelectRect(System.Drawing.Point pt, System.Drawing.Size se)
        {
            Rectangle rectTemp = new Rectangle(pt, se);
            rectTemp.Intersect(this.DisplayRectangle);
            if (!rectTemp.IsEmpty)
            {
                rectTemp.Width--;
                rectTemp.Height--;
                if (this.selectedRectangle != rectTemp)
                {
                    this.selectedRectangle = rectTemp;
                    base.Invalidate();
                }
            }
        }

        public void SetSelectRect(int x, int y, int w, int h)
        {
            Rectangle rectTemp = new Rectangle(x, y, w, h);
            rectTemp.Intersect(this.DisplayRectangle);
            if (!rectTemp.IsEmpty)
            {
                rectTemp.Width--;
                rectTemp.Height--;
                if (this.selectedRectangle != rectTemp)
                {
                    this.selectedRectangle = rectTemp;
                    base.Invalidate();
                }
            }
        }

        [Description("获取或设置是否根据图像大小自动调整控件尺寸"), Category("Custom"), DefaultValue(true)]
        public bool AutoSizeFromImage
        {
            get
            {
                return this.autoSizeFromImage;
            }
            set
            {
                if (value && (this.baseImage != null))
                {
                    base.Width = this.baseImage.Width;
                    base.Height = this.baseImage.Height;
                }
                this.autoSizeFromImage = value;
            }
        }

        [Category("Custom"), Description("获取或设置用于被操作的图像")]
        public Image BaseImage
        {
            get
            {
                return this.baseImage;
            }
            set
            {
                this.baseImage = value;
                this.BuildBitmap();
            }
        }

        [Browsable(false)]
        public bool CanReset
        {
            get
            {
                return this.canReset;
            }
            set
            {
                this.canReset = value;
                if (!this.canReset)
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        [DefaultValue(typeof(Color), "Yellow"), Category("Custom"), Description("获取或设置操作框点的颜色")]
        public Color DotColor
        {
            get
            {
                return this.dotColor;
            }
            set
            {
                this.dotColor = value;
            }
        }

        [Browsable(false)]
        public bool IsDrawed
        {
            get
            {
                return this.isDrawed;
            }
        }

        [DefaultValue(true), Description("获取或设置是否绘制操作框点"), Category("Custom")]
        public bool IsDrawOperationDot
        {
            get
            {
                return this.isDrawOperationDot;
            }
            set
            {
                if (value != this.isDrawOperationDot)
                {
                    this.isDrawOperationDot = value;
                    base.Invalidate();
                }
            }
        }

        [Browsable(false)]
        public bool IsMoving
        {
            get
            {
                return this.isMoving;
            }
        }

        [Category("Custom"), DefaultValue(true), Description("获取或设置是否限制鼠标操作区域")]
        public bool IsSetClip
        {
            get
            {
                return this.isSetClip;
            }
            set
            {
                this.isSetClip = value;
            }
        }

        [DefaultValue(true), Category("Custom"), Description("获取或设置是否绘制信息展示")]
        public bool IsShowInfo
        {
            get
            {
                return this.isShowInfo;
            }
            set
            {
                this.isShowInfo = value;
            }
        }

        [Browsable(false)]
        public bool IsStartDraw
        {
            get
            {
                return this.isStartDraw;
            }
        }

        [DefaultValue(typeof(Color), "Cyan"), Description("获取或设置操作框线条的颜色"), Category("Custom")]
        public Color LineColor
        {
            get
            {
                return this.lineColor;
            }
            set
            {
                this.lineColor = value;
            }
        }

        [Description("获取或设置放大图像的原图大小尺寸"), DefaultValue(typeof(System.Drawing.Size), "15,15"), Category("Custom")]
        public System.Drawing.Size MagnifySize
        {
            get
            {
                return this.magnifySize;
            }
            set
            {
                this.magnifySize = value;
                if (this.magnifySize.Width < 5)
                {
                    this.magnifySize.Width = 5;
                }
                if (this.magnifySize.Width > 20)
                {
                    this.magnifySize.Width = 20;
                }
                if (this.magnifySize.Height < 5)
                {
                    this.magnifySize.Height = 5;
                }
                if (this.magnifySize.Height > 20)
                {
                    this.magnifySize.Height = 20;
                }
            }
        }

        [Description("获取或设置图像放大的倍数"), DefaultValue(7), Category("Custom")]
        public int MagnifyTimes
        {
            get
            {
                return this.magnifyTimes;
            }
            set
            {
                this.magnifyTimes = value;
                if (this.magnifyTimes < 3)
                {
                    this.magnifyTimes = 3;
                }
                if (this.magnifyTimes > 10)
                {
                    this.magnifyTimes = 10;
                }
            }
        }

        [Browsable(false)]
        public Rectangle SelectedRectangle
        {
            get
            {
                Rectangle rectTemp = this.selectedRectangle;
                rectTemp.Width++;
                rectTemp.Height++;
                return rectTemp;
            }
        }
    }
}

