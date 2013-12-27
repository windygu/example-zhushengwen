namespace CCWin
{
    using CCWin.Properties;
    using CCWin.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class CCSkinForm : Form
    {
        private IContainer components;
        private CCSkinMain Main;

        public CCSkinForm(CCSkinMain main)
        {
            this.Main = main;
            this.InitializeComponent();
            this.SetStyles();
            this.Init();
        }

        private void CanPenetrate()
        {
            CCWin.Win32.NativeMethods.GetWindowLong(base.Handle, -20);
            CCWin.Win32.NativeMethods.SetWindowLong(base.Handle, -20, 0x80020);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Init()
        {
            base.TopMost = this.Main.TopMost;
            this.Main.BringToFront();
            base.ShowInTaskbar = false;
            base.FormBorderStyle = FormBorderStyle.None;
            base.Location = new System.Drawing.Point(this.Main.Location.X - 5, this.Main.Location.Y - 5);
            base.Icon = this.Main.Icon;
            base.ShowIcon = this.Main.ShowIcon;
            base.Width = this.Main.Width + 10;
            base.Height = this.Main.Height + 10;
            this.Text = this.Main.Text;
            this.Main.LocationChanged += new EventHandler(this.Main_LocationChanged);
            this.Main.SizeChanged += new EventHandler(this.Main_SizeChanged);
            this.Main.VisibleChanged += new EventHandler(this.Main_VisibleChanged);
            this.SetBits();
            this.CanPenetrate();
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackgroundImageLayout = ImageLayout.None;
            base.ClientSize = new System.Drawing.Size(0x103, 0x10f);
            base.FormBorderStyle = FormBorderStyle.None;
            base.Name = "SkinFormTwo";
            this.Text = "SkinForm";
            base.TopMost = true;
            base.ResumeLayout(false);
        }

        private void Main_LocationChanged(object sender, EventArgs e)
        {
            base.Location = new System.Drawing.Point(this.Main.Left - 5, this.Main.Top - 5);
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            base.Width = this.Main.Width + 10;
            base.Height = this.Main.Height + 10;
            this.SetBits();
        }

        private void Main_VisibleChanged(object sender, EventArgs e)
        {
            base.Visible = this.Main.Visible;
        }

        public void SetBits()
        {
            Bitmap bitmap = new Bitmap(this.Main.Width + 10, this.Main.Height + 10);
            Rectangle _BacklightLTRB = new Rectangle(20, 20, 20, 20);
            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            ImageDrawRect.DrawRect(g, Resources.main_light_bkg_top123, base.ClientRectangle, Rectangle.FromLTRB(_BacklightLTRB.X, _BacklightLTRB.Y, _BacklightLTRB.Width, _BacklightLTRB.Height), 1, 1);
            if (!Image.IsCanonicalPixelFormat(bitmap.PixelFormat) || !Image.IsAlphaPixelFormat(bitmap.PixelFormat))
            {
                throw new ApplicationException("图片必须是32位带Alhpa通道的图片。");
            }
            IntPtr oldBits = IntPtr.Zero;
            IntPtr screenDC = CCWin.Win32.NativeMethods.GetDC(IntPtr.Zero);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr memDc = CCWin.Win32.NativeMethods.CreateCompatibleDC(screenDC);
            try
            {
                CCWin.Win32.NativeMethods.Point topLoc = new CCWin.Win32.NativeMethods.Point(base.Left, base.Top);
                CCWin.Win32.NativeMethods.Size bitMapSize = new CCWin.Win32.NativeMethods.Size(base.Width, base.Height);
                CCWin.Win32.NativeMethods.BLENDFUNCTION blendFunc = new CCWin.Win32.NativeMethods.BLENDFUNCTION();
                CCWin.Win32.NativeMethods.Point srcLoc = new CCWin.Win32.NativeMethods.Point(0, 0);
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBits = CCWin.Win32.NativeMethods.SelectObject(memDc, hBitmap);
                blendFunc.BlendOp = 0;
                blendFunc.SourceConstantAlpha = byte.Parse("255");
                blendFunc.AlphaFormat = 1;
                blendFunc.BlendFlags = 0;
                CCWin.Win32.NativeMethods.UpdateLayeredWindow(base.Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, 2);
            }
            finally
            {
                if (hBitmap != IntPtr.Zero)
                {
                    CCWin.Win32.NativeMethods.SelectObject(memDc, oldBits);
                    CCWin.Win32.NativeMethods.DeleteObject(hBitmap);
                }
                CCWin.Win32.NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
                CCWin.Win32.NativeMethods.DeleteDC(memDc);
            }
        }

        private void SetStyles()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.UpdateStyles();
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams cParms = base.CreateParams;
                cParms.ExStyle |= 0x80000;
                return cParms;
            }
        }

        public class CommonClass
        {
            public static void SetTaskMenu(Form form)
            {
                int windowLong = CCWin.Win32.NativeMethods.GetWindowLong(new HandleRef(form, form.Handle), -16);
                CCWin.Win32.NativeMethods.SetWindowLong(new HandleRef(form, form.Handle), -16, (windowLong | 0x80000) | 0x20000);
            }
        }
    }
}

