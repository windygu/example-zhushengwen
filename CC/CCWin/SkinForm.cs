namespace CCWin
{
    using CCWin.Win32;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class SkinForm : Form
    {
        private IContainer components;
        private bool isMouseDown;
        private SkinMain Main;
        private System.Drawing.Point mouseOffset;

        public SkinForm(SkinMain main)
        {
            this.InitializeComponent();
            this.Main = main;
            this.SetStyles();
            this.Init();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Frm_LocationChanged(object sender, EventArgs e)
        {
            Form frm = (Form) sender;
            if (frm == this)
            {
                this.Main.Location = frm.Location;
            }
            else
            {
                base.Location = frm.Location;
            }
        }

        private void Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Main.SkinMobile && (e.Button == MouseButtons.Left))
            {
                this.mouseOffset = new System.Drawing.Point(-e.X, -e.Y);
                this.isMouseDown = true;
            }
        }

        private void Frm_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Main.SkinMobile)
            {
                Form frm = (Form) sender;
                if (this.isMouseDown)
                {
                    System.Drawing.Point mousePos = Control.MousePosition;
                    mousePos.Offset(this.mouseOffset.X, this.mouseOffset.Y);
                    frm.Location = mousePos;
                }
            }
        }

        private void Frm_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Main.SkinMobile && (e.Button == MouseButtons.Left))
            {
                this.isMouseDown = false;
                if (base.Top < 0)
                {
                    base.Top = this.Main.Top = 0;
                }
            }
        }

        private void Init()
        {
            base.TopMost = this.Main.TopMost;
            base.ShowInTaskbar = this.Main.SkinShowInTaskbar;
            base.FormBorderStyle = FormBorderStyle.None;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            base.Location = this.Main.Location;
            base.Icon = this.Main.Icon;
            base.ShowIcon = this.Main.ShowIcon;
            base.Size = this.Main.Size;
            this.Text = this.Main.Text;
            Bitmap bitmaps = new Bitmap(this.Main.SkinBack, base.Size);
            if (this.Main.SkinTrankColor != Color.Transparent)
            {
                bitmaps.MakeTransparent(this.Main.SkinTrankColor);
            }
            this.BackgroundImage = bitmaps;
            this.Main.Owner = this;
            base.MouseDown += new MouseEventHandler(this.Frm_MouseDown);
            base.MouseMove += new MouseEventHandler(this.Frm_MouseMove);
            base.MouseUp += new MouseEventHandler(this.Frm_MouseUp);
            base.LocationChanged += new EventHandler(this.Frm_LocationChanged);
            this.Main.MouseDown += new MouseEventHandler(this.Frm_MouseDown);
            this.Main.MouseMove += new MouseEventHandler(this.Frm_MouseMove);
            this.Main.MouseUp += new MouseEventHandler(this.Frm_MouseUp);
            this.Main.LocationChanged += new EventHandler(this.Frm_LocationChanged);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(0x103, 0x10f);
            base.FormBorderStyle = FormBorderStyle.None;
            base.Name = "SkinForm";
            this.Text = "SkinForm";
            base.ResumeLayout(false);
        }

        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            base.OnBackgroundImageChanged(e);
            this.SetBits();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.SetBits();
        }

        public void SetBits()
        {
            if (this.BackgroundImage != null)
            {
                Bitmap bitmap = new Bitmap(this.BackgroundImage, base.Width, base.Height);
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
        }

        private void SetStyles()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.UpdateStyles();
            base.AutoScaleMode = AutoScaleMode.None;
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
    }
}

