namespace CCWin
{
    using CCWin.SkinClass;
    using CCWin.Win32;
    using CCWin.Win32.Struct;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public class CCSkinMain : Form
    {
        private bool _active;
        private int _borderWidth = 3;
        private bool _canResize = true;
        private Font _captionFont = SystemFonts.CaptionFont;
        private int _captionHeight = 0x18;
        private System.Drawing.Size _closeBoxSize = new System.Drawing.Size(0x20, 0x12);
        private CCWin.ControlBoxManager _controlBoxManager;
        private System.Drawing.Point _controlBoxOffset = new System.Drawing.Point(6, 0);
        private int _controlBoxSpace;
        private Rectangle _deltaRect;
        private bool _inPosChanged;
        private System.Drawing.Size _maxBoxSize = new System.Drawing.Size(0x20, 0x12);
        private System.Drawing.Size _miniSize = new System.Drawing.Size(0x20, 0x12);
        private MobileStyle _mobile = MobileStyle.Mobile;
        private System.Windows.Forms.Padding _padding;
        private int _radius = 6;
        private SkinFormRenderer _renderer;
        private CCWin.SkinClass.RoundStyle _roundStyle = CCWin.SkinClass.RoundStyle.All;
        private bool _showSystemMenu;
        private System.Windows.Forms.ToolTip _toolTip;
        public AnchorStyles Aanhor;
        private Image back;
        private BackEventHandler BackChanged;
        private bool backLayout = true;
        private Image backpalace;
        private Rectangle backrectangle = new Rectangle(10, 10, 10, 10);
        private bool backtocolor = true;
        private Image borderpalace;
        private Rectangle borderrectangle = new Rectangle(10, 10, 10, 10);
        private Image closedownback;
        private Image closemouseback;
        private Image closenormlback;
        private const int CS_DROPSHADOW = 0x20000;
        private bool dropback = true;
        private Color effectback = Color.White;
        private bool effectcaption = true;
        private int effectWidth = 6;
        private static readonly object EventRendererChanged = new object();
        private const int GCL_STYLE = -26;
        public bool isMouseDown;
        private Image maxdownback;
        private Image maxmouseback;
        private Image maxnormlback;
        private Image minidownback;
        private Image minimouseback;
        private Image mininormlback;
        private Image restoredownback;
        private Image restoremouseback;
        private Image restorenormlback;
        private bool shadow = true;
        private bool showborder = true;
        private bool showdrawicon = true;
        public CCSkinForm skin;
        private double skinOpacity = 1.0;
        private bool special = true;
        public SysBottomEventHandler SysBottomClick;
        private Image sysBottomDown;
        private Image sysBottomMouse;
        private Image sysBottomNorml;
        private System.Drawing.Size sysBottomSize = new System.Drawing.Size(0x1c, 20);
        private string sysBottomToolTip;
        private bool sysBottomVisibale;

        //[Category("Skin"), Description("Back属性值更改时引发的事件")]
        //public event BackEventHandler BackChanged
        //{
        //    add
        //    {
        //        BackEventHandler handler2;
        //        BackEventHandler backChanged = this.BackChanged;
        //        do
        //        {
        //            handler2 = backChanged;
        //            BackEventHandler handler3 = (BackEventHandler) Delegate.Combine(handler2, value);
        //            backChanged = Interlocked.CompareExchange<BackEventHandler>(ref this.BackChanged, handler3, handler2);
        //        }
        //        while (backChanged != handler2);
        //    }
        //    remove
        //    {
        //        BackEventHandler handler2;
        //        BackEventHandler backChanged = this.BackChanged;
        //        do
        //        {
        //            handler2 = backChanged;
        //            BackEventHandler handler3 = (BackEventHandler) Delegate.Remove(handler2, value);
        //            backChanged = Interlocked.CompareExchange<BackEventHandler>(ref this.BackChanged, handler3, handler2);
        //        }
        //        while (backChanged != handler2);
        //    }
        //}

        public event EventHandler RendererChangled
        {
            add
            {
                base.Events.AddHandler(EventRendererChanged, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventRendererChanged, value);
            }
        }

        //[Description("自定义按钮被点击时引发的事件"), Category("Skin")]
        //public event SysBottomEventHandler SysBottomClick
        //{
        //    add
        //    {
        //        SysBottomEventHandler handler2;
        //        SysBottomEventHandler sysBottomClick = this.SysBottomClick;
        //        do
        //        {
        //            handler2 = sysBottomClick;
        //            SysBottomEventHandler handler3 = (SysBottomEventHandler) Delegate.Combine(handler2, value);
        //            sysBottomClick = Interlocked.CompareExchange<SysBottomEventHandler>(ref this.SysBottomClick, handler3, handler2);
        //        }
        //        while (sysBottomClick != handler2);
        //    }
        //    remove
        //    {
        //        SysBottomEventHandler handler2;
        //        SysBottomEventHandler sysBottomClick = this.SysBottomClick;
        //        do
        //        {
        //            handler2 = sysBottomClick;
        //            SysBottomEventHandler handler3 = (SysBottomEventHandler) Delegate.Remove(handler2, value);
        //            sysBottomClick = Interlocked.CompareExchange<SysBottomEventHandler>(ref this.SysBottomClick, handler3, handler2);
        //        }
        //        while (sysBottomClick != handler2);
        //    }
        //}

        public CCSkinMain()
        {
            this.SetStyles();
            this.Init();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this._controlBoxManager != null)
                {
                    this._controlBoxManager.Dispose();
                    this._controlBoxManager = null;
                }
                this._renderer = null;
                this._toolTip.Dispose();
            }
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);
        private void Init()
        {
            this._toolTip = new System.Windows.Forms.ToolTip();
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            base.BackgroundImageLayout = ImageLayout.None;
            this.Renderer.InitSkinForm(this);
            base.Padding = this.DefaultPadding;
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleMode = AutoScaleMode.None;
            base.ClientSize = new System.Drawing.Size(0x11c, 0x106);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            base.Name = "SkinForm";
            this.Text = "SkinForm";
            base.ResumeLayout(false);
        }

        private void mStopAnthor()
        {
            if (base.Left <= 0)
            {
                this.Aanhor = AnchorStyles.Left;
            }
            else if (base.Left >= (Screen.PrimaryScreen.Bounds.Width - base.Width))
            {
                this.Aanhor = AnchorStyles.Right;
            }
            else if (base.Top <= 0)
            {
                this.Aanhor = AnchorStyles.Top;
            }
            else
            {
                this.Aanhor = AnchorStyles.None;
            }
        }

        protected virtual void OnBackChanged(BackEventArgs e)
        {
            if (this.BackChanged != null)
            {
                this.BackChanged(this, e);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (this.skin != null)
            {
                this.skin.Close();
            }
            if (this.Special)
            {
                base.Opacity = 1.0;
                CCWin.Win32.NativeMethods.AnimateWindow(base.Handle, 150, 0x90000);
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.SetReion();
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (this.DropBack)
            {
                string[] myFiles = (string[]) drgevent.Data.GetData(DataFormats.FileDrop);
                FileInfo f = new FileInfo(myFiles[0]);
                if (myFiles != null)
                {
                    string Type = f.Extension.Substring(1);
                    string[] TypeList = new string[] { "png", "bmp", "jpg", "jpeg", "gif" };
                    if (new List<string>(TypeList).Contains(Type.ToLower()))
                    {
                        this.Back = Image.FromFile(myFiles[0]);
                    }
                }
            }
            base.OnDragDrop(drgevent);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            if (this.DropBack)
            {
                drgevent.Effect = DragDropEffects.Link;
            }
            base.OnDragEnter(drgevent);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            this.mStopAnthor();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            System.Drawing.Point point = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                if (((!this.ControlBoxManager.CloseBoxRect.Contains(point) && !this.ControlBoxManager.MaximizeBoxRect.Contains(point)) && (!this.ControlBoxManager.MinimizeBoxRect.Contains(point) && !this.ControlBoxManager.SysBottomRect.Contains(point))) && (this.Mobile != MobileStyle.None))
                {
                    this.isMouseDown = true;
                    if (this.Mobile == MobileStyle.Mobile)
                    {
                        CCWin.Win32.NativeMethods.ReleaseCapture();
                        CCWin.Win32.NativeMethods.SendMessage(base.Handle, 0x112, 0xf011, 0);
                    }
                    else if ((this.Mobile == MobileStyle.TitleMobile) && (point.Y < this.CaptionHeight))
                    {
                        CCWin.Win32.NativeMethods.ReleaseCapture();
                        CCWin.Win32.NativeMethods.SendMessage(base.Handle, 0x112, 0xf011, 0);
                    }
                    this.OnMouseUp(e);
                }
                else
                {
                    this.ControlBoxManager.ProcessMouseOperate(e.Location, MouseOperate.Down);
                }
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            this.ControlBoxManager.ProcessMouseOperate(base.PointToClient(Control.MousePosition), MouseOperate.Hover);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.ControlBoxManager.ProcessMouseOperate(System.Drawing.Point.Empty, MouseOperate.Leave);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.ControlBoxManager.ProcessMouseOperate(e.Location, MouseOperate.Move);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.isMouseDown = false;
            base.OnMouseUp(e);
            this.ControlBoxManager.ProcessMouseOperate(e.Location, MouseOperate.Up);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            if (this.Back != null)
            {
                if (this.BackLayout)
                {
                    g.DrawImage(this.Back, 0, 0, this.Back.Width, this.Back.Height);
                }
                else
                {
                    g.DrawImage(this.Back, -(this.Back.Width - base.Width), 0, this.Back.Width, this.Back.Height);
                }
            }
            if ((this.Back != null) && this.BackToColor)
            {
                if (this.BackLayout)
                {
                    LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(this.Back.Width - 50, 0, 50, this.Back.Height), this.BackColor, Color.Transparent, 180f);
                    LinearGradientBrush brushTwo = new LinearGradientBrush(new Rectangle(0, this.Back.Height - 50, this.Back.Width, 50), this.BackColor, Color.Transparent, 270f);
                    g.FillRectangle(brush, (this.Back.Width - brush.Rectangle.Width) + 1f, 0f, brush.Rectangle.Width, brush.Rectangle.Height);
                    g.FillRectangle(brushTwo, 0f, (this.Back.Height - brushTwo.Rectangle.Height) + 1f, brushTwo.Rectangle.Width, brushTwo.Rectangle.Height);
                }
                else
                {
                    LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(-(this.Back.Width - base.Width), 0, 50, this.Back.Height), this.BackColor, Color.Transparent, 360f);
                    LinearGradientBrush brushTwo = new LinearGradientBrush(new Rectangle(-(this.Back.Width - base.Width), this.Back.Height - 50, this.Back.Width, 50), this.BackColor, Color.Transparent, 270f);
                    g.FillRectangle(brush, (float) -(this.Back.Width - base.Width), 0f, brush.Rectangle.Width, brush.Rectangle.Height);
                    g.FillRectangle(brushTwo, (float) -(this.Back.Width - base.Width), (float) (this.Back.Height - 50), brushTwo.Rectangle.Width, brushTwo.Rectangle.Height);
                }
            }
            base.OnPaint(e);
            Rectangle rect = base.ClientRectangle;
            SkinFormRenderer renderer = this.Renderer;
            if (this.ControlBoxManager.CloseBoxVisibale)
            {
                renderer.DrawSkinFormControlBox(new SkinFormControlBoxRenderEventArgs(this, g, this.ControlBoxManager.CloseBoxRect, this._active, ControlBoxStyle.Close, this.ControlBoxManager.CloseBoxState));
            }
            if (this.ControlBoxManager.MaximizeBoxVisibale)
            {
                renderer.DrawSkinFormControlBox(new SkinFormControlBoxRenderEventArgs(this, g, this.ControlBoxManager.MaximizeBoxRect, this._active, ControlBoxStyle.Maximize, this.ControlBoxManager.MaximizeBoxState));
            }
            if (this.ControlBoxManager.MinimizeBoxVisibale)
            {
                renderer.DrawSkinFormControlBox(new SkinFormControlBoxRenderEventArgs(this, g, this.ControlBoxManager.MinimizeBoxRect, this._active, ControlBoxStyle.Minimize, this.ControlBoxManager.MinimizeBoxState));
            }
            if (this.ControlBoxManager.SysBottomVisibale)
            {
                renderer.DrawSkinFormControlBox(new SkinFormControlBoxRenderEventArgs(this, g, this.ControlBoxManager.SysBottomRect, this._active, ControlBoxStyle.SysBottom, this.ControlBoxManager.SysBottomState));
            }
            if (this.ShowBorder)
            {
                renderer.DrawSkinFormBorder(new SkinFormBorderRenderEventArgs(this, g, rect, this._active));
            }
            if (this.BackPalace != null)
            {
                CCWin.ImageDrawRect.DrawRect(g, (Bitmap) this.BackPalace, new Rectangle(base.ClientRectangle.X - 5, base.ClientRectangle.Y - 5, base.ClientRectangle.Width + 10, base.ClientRectangle.Height + 10), Rectangle.FromLTRB(this.BackRectangle.X, this.BackRectangle.Y, this.BackRectangle.Width, this.BackRectangle.Height), 1, 1);
            }
            if (this.BorderPalace != null)
            {
                CCWin.ImageDrawRect.DrawRect(g, (Bitmap) this.BorderPalace, new Rectangle(base.ClientRectangle.X - 5, base.ClientRectangle.Y - 5, base.ClientRectangle.Width + 10, base.ClientRectangle.Height + 10), Rectangle.FromLTRB(this.BorderRectangle.X, this.BorderRectangle.Y, this.BorderRectangle.Width, this.BorderRectangle.Height), 1, 1);
            }
            renderer.DrawSkinFormCaption(new SkinFormCaptionRenderEventArgs(this, g, this.CaptionRect, this._active));
        }

        protected virtual void OnRendererChanged(EventArgs e)
        {
            this.Renderer.InitSkinForm(this);
            EventHandler handler = base.Events[EventRendererChanged] as EventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
            base.Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.SetReion();
        }

        protected virtual void OnSysBottomClick(object e)
        {
            if (this.SysBottomClick != null)
            {
                this.SysBottomClick(this);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (base.Visible)
            {
                if (this.Special && !base.DesignMode)
                {
                    CCWin.Win32.NativeMethods.AnimateWindow(base.Handle, 150, 0xa0000);
                    base.Opacity = this.SkinOpacity;
                }
                if ((!base.DesignMode && (this.skin == null)) && this.Shadow)
                {
                    this.skin = new CCSkinForm(this);
                    this.skin.Show(this);
                }
                base.OnVisibleChanged(e);
            }
            else
            {
                base.OnVisibleChanged(e);
                if (this.Special)
                {
                    base.Opacity = 1.0;
                    CCWin.Win32.NativeMethods.AnimateWindow(base.Handle, 150, 0x90000);
                }
            }
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        private void SetReion()
        {
            if (base.Region != null)
            {
                base.Region.Dispose();
            }
            UpdateForm.CreateRegion(this, this.RealClientRect, this.Radius, this.RoundStyle);
        }

        private void SetStyles()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.UpdateStyles();
            base.AutoScaleMode = AutoScaleMode.None;
        }

        public void SysbottomAv(object e)
        {
            this.OnSysBottomClick(e);
        }

        private void WmGetMinMaxInfo(ref Message m)
        {
            CCWin.Win32.Struct.MINMAXINFO minmax = (CCWin.Win32.Struct.MINMAXINFO) Marshal.PtrToStructure(m.LParam, typeof(CCWin.Win32.Struct.MINMAXINFO));
            if (this.MaximumSize != System.Drawing.Size.Empty)
            {
                minmax.maxTrackSize = this.MaximumSize;
            }
            else
            {
                Rectangle rect = Screen.GetWorkingArea(this);
                minmax.maxPosition = new System.Drawing.Point(rect.X - this.BorderWidth, rect.Y);
                minmax.maxTrackSize = new System.Drawing.Size(rect.Width + (this.BorderWidth * 2), rect.Height + this.BorderWidth);
            }
            if (this.MinimumSize != System.Drawing.Size.Empty)
            {
                minmax.minTrackSize = this.MinimumSize;
            }
            else
            {
                minmax.minTrackSize = new System.Drawing.Size(((((((this.CloseBoxSize.Width + this.MiniSize.Width) + this.MaxSize.Width) + this.ControlBoxOffset.X) + (this.ControlBoxSpace * 2)) + SystemInformation.SmallIconSize.Width) + (this.BorderWidth * 2)) + 3, this.CaptionHeight);
            }
            Marshal.StructureToPtr(minmax, m.LParam, false);
        }

        private void WmNcActive(ref Message m)
        {
            if (m.WParam.ToInt32() == 1)
            {
                this._active = true;
            }
            else
            {
                this._active = false;
            }
            m.Result = Result.TRUE;
            base.Invalidate();
        }

        private void WmNcHitTest(ref Message m)
        {
            System.Drawing.Point point = new System.Drawing.Point(m.LParam.ToInt32());
            point = base.PointToClient(point);
            if (this.IconRect.Contains(point) && this.ShowSystemMenu)
            {
                m.Result = new IntPtr(3);
            }
            else if (this._canResize)
            {
                if ((point.X < 5) && (point.Y < 5))
                {
                    m.Result = new IntPtr(13);
                }
                else if ((point.X > (base.Width - 5)) && (point.Y < 5))
                {
                    m.Result = new IntPtr(14);
                }
                else if ((point.X < 5) && (point.Y > (base.Height - 5)))
                {
                    m.Result = new IntPtr(0x10);
                }
                else if ((point.X > (base.Width - 5)) && (point.Y > (base.Height - 5)))
                {
                    m.Result = new IntPtr(0x11);
                }
                else if (point.Y < 3)
                {
                    m.Result = new IntPtr(12);
                }
                else if (point.Y > (base.Height - 3))
                {
                    m.Result = new IntPtr(15);
                }
                else if (point.X < 3)
                {
                    m.Result = new IntPtr(10);
                }
                else if (point.X > (base.Width - 3))
                {
                    m.Result = new IntPtr(11);
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x24:
                    this.WmGetMinMaxInfo(ref m);
                    return;

                case 0x47:
                    this._inPosChanged = true;
                    base.WndProc(ref m);
                    this._inPosChanged = false;
                    return;

                case 0x83:
                case 0x85:
                    break;

                case 0x84:
                    this.WmNcHitTest(ref m);
                    return;

                case 0x86:
                    this.WmNcActive(ref m);
                    return;

                case 0xae:
                case 0xaf:
                    m.Result = Result.TRUE;
                    return;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        [Description("背景"), Category("Skin")]
        public Image Back
        {
            get
            {
                return this.back;
            }
            set
            {
                if (this.back != value)
                {
                    this.OnBackChanged(new BackEventArgs(this.back, value));
                    this.back = value;
                    if (this.BackToColor && (this.back != null))
                    {
                        this.BackColor = BitmapHelper.GetImageAverageColor((Bitmap) this.back);
                    }
                    base.Invalidate();
                }
            }
        }

        [Category("Skin"), Description("是否从左绘制背景")]
        public bool BackLayout
        {
            get
            {
                return this.backLayout;
            }
            set
            {
                if (this.backLayout != value)
                {
                    this.backLayout = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Skin"), Description("质感层背景")]
        public Image BackPalace
        {
            get
            {
                return this.backpalace;
            }
            set
            {
                if (this.backpalace != value)
                {
                    this.backpalace = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Skin"), Description("质感层九宫绘画区域"), DefaultValue(typeof(Rectangle), "10,10,10,10")]
        public Rectangle BackRectangle
        {
            get
            {
                return this.backrectangle;
            }
            set
            {
                if (this.backrectangle != value)
                {
                    this.backrectangle = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(true), Description("是否根据背景图决定背景色，并加入背景渐变效果"), Category("Skin")]
        public bool BackToColor
        {
            get
            {
                return this.backtocolor;
            }
            set
            {
                if (this.backtocolor != value)
                {
                    this.backtocolor = value;
                    base.Invalidate();
                }
            }
        }

        [Description("边框层背景"), Category("Skin")]
        public Image BorderPalace
        {
            get
            {
                return this.borderpalace;
            }
            set
            {
                if (this.borderpalace != value)
                {
                    this.borderpalace = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(Rectangle), "10,10,10,10"), Description("边框质感层九宫绘画区域"), Category("Skin")]
        public Rectangle BorderRectangle
        {
            get
            {
                return this.borderrectangle;
            }
            set
            {
                if (this.borderrectangle != value)
                {
                    this.borderrectangle = value;
                    base.Invalidate();
                }
            }
        }

        [Description("设置或获取窗体的边框的宽度"), DefaultValue(3), Category("Skin")]
        public int BorderWidth
        {
            get
            {
                return this._borderWidth;
            }
            set
            {
                if (this._borderWidth != value)
                {
                    this._borderWidth = (value < 1) ? 1 : value;
                }
            }
        }

        [Description("设置或获取窗体是否可以改变大小"), Category("Skin"), DefaultValue(true)]
        public bool CanResize
        {
            get
            {
                return this._canResize;
            }
            set
            {
                this._canResize = value;
            }
        }

        [Description("设置或获取窗体标题的字体"), DefaultValue(typeof(Font), "CaptionFont"), Category("Caption")]
        public Font CaptionFont
        {
            get
            {
                return this._captionFont;
            }
            set
            {
                if (value == null)
                {
                    this._captionFont = SystemFonts.CaptionFont;
                }
                else
                {
                    this._captionFont = value;
                }
                base.Invalidate(this.CaptionRect);
            }
        }

        [DefaultValue(0x18), Description("设置或获取窗体标题栏的高度"), Category("Skin")]
        public int CaptionHeight
        {
            get
            {
                return this._captionHeight;
            }
            set
            {
                if (this._captionHeight != value)
                {
                    this._captionHeight = (value < this._borderWidth) ? this._borderWidth : value;
                    base.Invalidate();
                }
            }
        }

        public Rectangle CaptionRect
        {
            get
            {
                return new Rectangle(0, 0, base.Width, this.CaptionHeight);
            }
        }

        [Description("设置或获取关闭按钮的大小"), Category("CloseBox"), DefaultValue(typeof(System.Drawing.Size), "32, 18")]
        public System.Drawing.Size CloseBoxSize
        {
            get
            {
                return this._closeBoxSize;
            }
            set
            {
                if (this._closeBoxSize != value)
                {
                    this._closeBoxSize = value;
                    base.Invalidate();
                }
            }
        }

        [Category("CloseBox"), Description("关闭按钮点击时背景")]
        public Image CloseDownBack
        {
            get
            {
                return this.closedownback;
            }
            set
            {
                if (this.closedownback != value)
                {
                    this.closedownback = value;
                    base.Invalidate();
                }
            }
        }

        [Category("CloseBox"), Description("关闭按钮悬浮时背景")]
        public Image CloseMouseBack
        {
            get
            {
                return this.closemouseback;
            }
            set
            {
                if (this.closemouseback != value)
                {
                    this.closemouseback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("关闭按钮初始时背景"), Category("CloseBox")]
        public Image CloseNormlBack
        {
            get
            {
                return this.closenormlback;
            }
            set
            {
                if (this.closenormlback != value)
                {
                    this.closenormlback = value;
                    base.Invalidate();
                }
            }
        }

        public CCWin.ControlBoxManager ControlBoxManager
        {
            get
            {
                if (this._controlBoxManager == null)
                {
                    this._controlBoxManager = new CCWin.ControlBoxManager(this);
                }
                return this._controlBoxManager;
            }
        }

        [Category("Skin"), Description("设置或获取控制按钮的偏移"), DefaultValue(typeof(System.Drawing.Point), "6, 0")]
        public System.Drawing.Point ControlBoxOffset
        {
            get
            {
                return this._controlBoxOffset;
            }
            set
            {
                if (this._controlBoxOffset != value)
                {
                    this._controlBoxOffset = value;
                    base.Invalidate();
                }
            }
        }

        [Description("设置或获取控制按钮的间距"), DefaultValue(0), Category("Skin")]
        public int ControlBoxSpace
        {
            get
            {
                return this._controlBoxSpace;
            }
            set
            {
                if (this._controlBoxSpace != value)
                {
                    this._controlBoxSpace = value;
                    base.Invalidate();
                }
            }
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams cp = base.CreateParams;
                if (!base.DesignMode)
                {
                    cp.Style |= 0x40000;
                    if (base.ControlBox)
                    {
                        cp.Style |= 0x80000;
                    }
                    if (base.MinimizeBox)
                    {
                        cp.Style |= 0x20000;
                    }
                    if (!base.MaximizeBox)
                    {
                        cp.Style &= -65537;
                    }
                    if (this._inPosChanged)
                    {
                        cp.Style &= -786433;
                        cp.ExStyle &= -258;
                    }
                }
                return cp;
            }
        }

        protected override System.Windows.Forms.Padding DefaultPadding
        {
            get
            {
                return new System.Windows.Forms.Padding(this.BorderWidth, this.CaptionHeight, this.BorderWidth, this.BorderWidth);
            }
        }

        [Description("指示控件是否可以将用户拖动到背景上的图片作为背景(注意:开启前请设置AllowDrop为true,否则无效)"), DefaultValue(true), Category("Skin")]
        public bool DropBack
        {
            get
            {
                return this.dropback;
            }
            set
            {
                if (this.dropback != value)
                {
                    this.dropback = value;
                }
            }
        }

        [Category("Caption"), Description("发光字体背景色"), DefaultValue(typeof(Color), "White")]
        public Color EffectBack
        {
            get
            {
                return this.effectback;
            }
            set
            {
                if (this.effectback != value)
                {
                    this.effectback = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(true), Description("是否绘制发光标题"), Category("Caption")]
        public bool EffectCaption
        {
            get
            {
                return this.effectcaption;
            }
            set
            {
                if (this.effectcaption != value)
                {
                    this.effectcaption = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Caption"), Description("光圈大小"), DefaultValue(typeof(int), "6")]
        public int EffectWidth
        {
            get
            {
                return this.effectWidth;
            }
            set
            {
                if (this.effectWidth != value)
                {
                    this.effectWidth = value;
                    base.Invalidate();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Forms.FormBorderStyle FormBorderStyle
        {
            get
            {
                return base.FormBorderStyle;
            }
            set
            {
                base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }

        public Rectangle IconRect
        {
            get
            {
                if (!this.ShowDrawIcon || (base.Icon == null))
                {
                    return Rectangle.Empty;
                }
                int width = SystemInformation.SmallIconSize.Width;
                if (((this.CaptionHeight - this.BorderWidth) - 4) < width)
                {
                    width = (this.CaptionHeight - this.BorderWidth) - 4;
                }
                return new Rectangle(this.BorderWidth, this.BorderWidth + (((this.CaptionHeight - this.BorderWidth) - width) / 2), width, width);
            }
        }

        [Description("最大化按钮点击时背景"), Category("MaximizeBox")]
        public Image MaxDownBack
        {
            get
            {
                return this.maxdownback;
            }
            set
            {
                if (this.maxdownback != value)
                {
                    this.maxdownback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("最大化按钮悬浮时背景"), Category("MaximizeBox")]
        public Image MaxMouseBack
        {
            get
            {
                return this.maxmouseback;
            }
            set
            {
                if (this.maxmouseback != value)
                {
                    this.maxmouseback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("最大化按钮初始时背景"), Category("MaximizeBox")]
        public Image MaxNormlBack
        {
            get
            {
                return this.maxnormlback;
            }
            set
            {
                if (this.maxnormlback != value)
                {
                    this.maxnormlback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("设置或获取最大化（还原）按钮的大小"), DefaultValue(typeof(System.Drawing.Size), "32, 18"), Category("MaximizeBox")]
        public System.Drawing.Size MaxSize
        {
            get
            {
                return this._maxBoxSize;
            }
            set
            {
                if (this._maxBoxSize != value)
                {
                    this._maxBoxSize = value;
                    base.Invalidate();
                }
            }
        }

        [Description("最小化按钮点击时背景"), Category("MinimizeBox")]
        public Image MiniDownBack
        {
            get
            {
                return this.minidownback;
            }
            set
            {
                if (this.minidownback != value)
                {
                    this.minidownback = value;
                    base.Invalidate();
                }
            }
        }

        [Category("MinimizeBox"), Description("最小化按钮悬浮时背景")]
        public Image MiniMouseBack
        {
            get
            {
                return this.minimouseback;
            }
            set
            {
                if (this.minimouseback != value)
                {
                    this.minimouseback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("最小化按钮初始时背景"), Category("MinimizeBox")]
        public Image MiniNormlBack
        {
            get
            {
                return this.mininormlback;
            }
            set
            {
                if (this.mininormlback != value)
                {
                    this.mininormlback = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(typeof(System.Drawing.Size), "32, 18"), Description("设置或获取最小化按钮的大小"), Category("MinimizeBox")]
        public System.Drawing.Size MiniSize
        {
            get
            {
                return this._miniSize;
            }
            set
            {
                if (this._miniSize != value)
                {
                    this._miniSize = value;
                    base.Invalidate();
                }
            }
        }

        [Description("移动窗体的条件"), DefaultValue(typeof(MobileStyle), "2"), Category("Skin")]
        public MobileStyle Mobile
        {
            get
            {
                return this._mobile;
            }
            set
            {
                if (this._mobile != value)
                {
                    this._mobile = value;
                }
            }
        }

        [DefaultValue(typeof(System.Windows.Forms.Padding), "0")]
        public System.Windows.Forms.Padding Padding
        {
            get
            {
                return this._padding;
            }
            set
            {
                this._padding = value;
                base.Padding = new System.Windows.Forms.Padding(this.BorderWidth + this._padding.Left, this.CaptionHeight + this._padding.Top, this.BorderWidth + this._padding.Right, this.BorderWidth + this._padding.Bottom);
            }
        }

        [DefaultValue(6), Description("设置或获取窗体的圆角的大小"), Category("Skin")]
        public int Radius
        {
            get
            {
                return this._radius;
            }
            set
            {
                if (this._radius != value)
                {
                    this._radius = value;
                    this.SetReion();
                    base.Invalidate();
                }
            }
        }

        protected Rectangle RealClientRect
        {
            get
            {
                if (base.WindowState == FormWindowState.Maximized)
                {
                    return new Rectangle(this._deltaRect.X, this._deltaRect.Y, base.Width - this._deltaRect.Width, base.Height - this._deltaRect.Height);
                }
                return new Rectangle(System.Drawing.Point.Empty, base.Size);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("设置或获取窗体的绘制方法")]
        public SkinFormRenderer Renderer
        {
            get
            {
                if (this._renderer == null)
                {
                    this._renderer = new SkinFormProfessionalRenderer();
                }
                return this._renderer;
            }
            set
            {
                this._renderer = value;
                this.OnRendererChanged(EventArgs.Empty);
            }
        }

        [Category("MaximizeBox"), Description("还原按钮点击时背景")]
        public Image RestoreDownBack
        {
            get
            {
                return this.restoredownback;
            }
            set
            {
                if (this.restoredownback != value)
                {
                    this.restoredownback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("还原按钮悬浮时背景"), Category("MaximizeBox")]
        public Image RestoreMouseBack
        {
            get
            {
                return this.restoremouseback;
            }
            set
            {
                if (this.restoremouseback != value)
                {
                    this.restoremouseback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("还原按钮初始时背景"), Category("MaximizeBox")]
        public Image RestoreNormlBack
        {
            get
            {
                return this.restorenormlback;
            }
            set
            {
                if (this.restorenormlback != value)
                {
                    this.restorenormlback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("设置或获取窗体的圆角样式"), Category("Skin"), DefaultValue(typeof(CCWin.SkinClass.RoundStyle), "1")]
        public CCWin.SkinClass.RoundStyle RoundStyle
        {
            get
            {
                return this._roundStyle;
            }
            set
            {
                if (this._roundStyle != value)
                {
                    this._roundStyle = value;
                    this.SetReion();
                    base.Invalidate();
                }
            }
        }

        [Description("是否启用窗体阴影"), DefaultValue(true), Category("Skin")]
        public bool Shadow
        {
            get
            {
                return this.shadow;
            }
            set
            {
                if (this.shadow != value)
                {
                    this.shadow = value;
                }
            }
        }

        [DefaultValue(true), Description("是否在窗体上绘画边框"), Category("Skin")]
        public bool ShowBorder
        {
            get
            {
                return this.showborder;
            }
            set
            {
                if (this.showborder != value)
                {
                    this.showborder = value;
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(true), Description("是否在窗体上绘画ICO图标"), Category("窗口样式")]
        public bool ShowDrawIcon
        {
            get
            {
                return this.showdrawicon;
            }
            set
            {
                if (this.showdrawicon != value)
                {
                    this.showdrawicon = value;
                    base.Invalidate();
                }
            }
        }

        [Description("获取或设置窗体是否显示系统菜单"), Category("Skin"), DefaultValue(false)]
        public bool ShowSystemMenu
        {
            get
            {
                return this._showSystemMenu;
            }
            set
            {
                this._showSystemMenu = value;
            }
        }

        [Category("Skin"), Description("窗体渐变后透明度")]
        public double SkinOpacity
        {
            get
            {
                return this.skinOpacity;
            }
            set
            {
                if (this.skinOpacity != value)
                {
                    this.skinOpacity = value;
                }
            }
        }

        [Category("Skin"), DefaultValue(true), Description("是否启用窗口淡入淡出")]
        public bool Special
        {
            get
            {
                return this.special;
            }
            set
            {
                if (this.special != value)
                {
                    this.special = value;
                }
            }
        }

        [Description("自定义系统按钮点击时"), Category("SysBottom")]
        public Image SysBottomDown
        {
            get
            {
                return this.sysBottomDown;
            }
            set
            {
                if (this.sysBottomDown != value)
                {
                    this.sysBottomDown = value;
                    base.Invalidate();
                }
            }
        }

        [Description("自定义系统按钮悬浮时"), Category("SysBottom")]
        public Image SysBottomMouse
        {
            get
            {
                return this.sysBottomMouse;
            }
            set
            {
                if (this.sysBottomMouse != value)
                {
                    this.sysBottomMouse = value;
                    base.Invalidate();
                }
            }
        }

        [Description("自定义系统按钮初始时"), Category("SysBottom")]
        public Image SysBottomNorml
        {
            get
            {
                return this.sysBottomNorml;
            }
            set
            {
                if (this.sysBottomNorml != value)
                {
                    this.sysBottomNorml = value;
                    base.Invalidate();
                }
            }
        }

        [Category("SysBottom"), DefaultValue(typeof(System.Drawing.Size), "28, 20"), Description("设置或获取自定义系统按钮的大小")]
        public System.Drawing.Size SysBottomSize
        {
            get
            {
                return this.sysBottomSize;
            }
            set
            {
                if (this.sysBottomSize != value)
                {
                    this.sysBottomSize = value;
                    base.Invalidate();
                }
            }
        }

        [Description("自定义系统按钮悬浮提示"), Category("SysBottom")]
        public string SysBottomToolTip
        {
            get
            {
                return this.sysBottomToolTip;
            }
            set
            {
                if (this.sysBottomToolTip != value)
                {
                    this.sysBottomToolTip = value;
                    base.Invalidate();
                }
            }
        }

        [Description("自定义系统按钮是否显示"), Category("SysBottom")]
        public bool SysBottomVisibale
        {
            get
            {
                return this.sysBottomVisibale;
            }
            set
            {
                if (this.sysBottomVisibale != value)
                {
                    this.sysBottomVisibale = value;
                    base.Invalidate();
                }
            }
        }

        [Category("Caption")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                base.Invalidate(new Rectangle(0, 0, base.Width, this.CaptionHeight + 1));
            }
        }

        public System.Windows.Forms.ToolTip ToolTip
        {
            get
            {
                return this._toolTip;
            }
        }

        public delegate void BackEventHandler(object sender, BackEventArgs e);

        public delegate void SysBottomEventHandler(object sender);
    }
}

