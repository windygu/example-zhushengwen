namespace CCWin.SkinControl
{
    using CCWin.Properties;
    using CCWin.Win32;
    using CCWin.Win32.Struct;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class FrmCapture : Form
    {
        private ColorBox colorBox1;
        private IContainer components;
        private ImageProcessBox imgpb;
        private bool isCaptureCursor;
        private Bitmap m_bmpLayerCurrent;
        private Bitmap m_bmpLayerShow;
        private bool m_isStartDraw;
        private List<Bitmap> m_layer;
        private MouseHook m_MHook;
        private System.Drawing.Point m_ptCurrent;
        private System.Drawing.Point m_ptOriginal;
        private Panel panel1;
        private Panel panel2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private RtfRichTextBox RcTxt;
        private ToolButton tBtn_Arrow;
        private ToolButton tBtn_Brush;
        private ToolButton tBtn_Cancel;
        private ToolButton tBtn_Close;
        private ToolButton tBtn_Ellipse;
        private ToolButton tBtn_Finish;
        private ToolButton tBtn_Rect;
        private ToolButton tBtn_Save;
        private ToolButton tBtn_Text;
        private TextBox textBox1;
        private Timer timer1;
        private ToolButton toolButton1;
        private ToolButton toolButton2;
        private ToolButton toolButton3;

        public FrmCapture()
        {
            this.InitializeComponent();
            base.FormBorderStyle = FormBorderStyle.None;
            base.Location = new System.Drawing.Point(0, 0);
            base.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            base.TopMost = true;
            base.ShowInTaskbar = false;
            this.m_MHook = new MouseHook();
            base.FormClosing += delegate (object s, FormClosingEventArgs e) {
                this.m_MHook.UnLoadHook();
                this.DelResource();
            };
            this.imgpb.MouseLeave += delegate (object s, EventArgs e) {
                this.Cursor = Cursors.Default;
            };
            this.m_layer = new List<Bitmap>();
        }

        public FrmCapture(RtfRichTextBox rcTxt)
        {
            this.InitializeComponent();
            this.RcTxt = rcTxt;
            base.FormBorderStyle = FormBorderStyle.None;
            base.Location = new System.Drawing.Point(0, 0);
            base.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            base.TopMost = true;
            base.ShowInTaskbar = false;
            this.m_MHook = new MouseHook();
            base.FormClosing += delegate (object s, FormClosingEventArgs e) {
                this.m_MHook.UnLoadHook();
                this.DelResource();
            };
            this.imgpb.MouseLeave += delegate (object s, EventArgs e) {
                this.Cursor = Cursors.Default;
            };
            this.m_layer = new List<Bitmap>();
        }

        private void ClearToolBarBtnSelected()
        {
            this.tBtn_Rect.IsSelected = this.tBtn_Ellipse.IsSelected = this.tBtn_Arrow.IsSelected = this.tBtn_Brush.IsSelected = this.tBtn_Text.IsSelected = false;
        }

        private void DelResource()
        {
            if (this.m_bmpLayerCurrent != null)
            {
                this.m_bmpLayerCurrent.Dispose();
            }
            if (this.m_bmpLayerShow != null)
            {
                this.m_bmpLayerShow.Dispose();
            }
            this.m_layer.Clear();
            this.imgpb.DeleResource();
            GC.Collect();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FoundAndDrawWindowRect()
        {
            CCWin.Win32.NativeMethods.Point pt = new CCWin.Win32.NativeMethods.Point();
            pt.x = Control.MousePosition.X;
            pt.y = Control.MousePosition.Y;
            IntPtr hWnd = CCWin.Win32.NativeMethods.ChildWindowFromPointEx(CCWin.Win32.NativeMethods.GetDesktopWindow(), pt, 3);
            if (hWnd != IntPtr.Zero)
            {
                IntPtr hTemp = hWnd;
                while (true)
                {
                    CCWin.Win32.NativeMethods.ScreenToClient(hTemp, ref pt);
                    hTemp = CCWin.Win32.NativeMethods.ChildWindowFromPointEx(hTemp, pt, 0);
                    if ((hTemp == IntPtr.Zero) || (hTemp == hWnd))
                    {
                        break;
                    }
                    hWnd = hTemp;
                    pt.x = Control.MousePosition.X;
                    pt.y = Control.MousePosition.Y;
                }
                CCWin.Win32.Struct.RECT rect = new CCWin.Win32.Struct.RECT();
                CCWin.Win32.NativeMethods.GetWindowRect(hWnd, ref rect);
                this.imgpb.SetSelectRect(new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top));
            }
        }

        private void FrmCapture_Load(object sender, EventArgs e)
        {
            this.InitMember();
            this.imgpb.BaseImage = this.GetScreen();
            this.m_MHook.SetHook();
            this.m_MHook.MHookEvent += new MouseHook.MHookEventHandler(this.m_MHook_MHookEvent);
            this.imgpb.IsDrawOperationDot = false;
            base.BeginInvoke((System.Threading.ThreadStart)delegate() {
                base.Enabled = false;
            });
            this.timer1.Interval = 500;
            this.timer1.Enabled = true;
        }

        private Bitmap GetScreen()
        {
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            if (this.isCaptureCursor)
            {
                using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
                {
                    CCWin.Win32.NativeMethods.PCURSORINFO pci;
                    pci.cbSize = Marshal.SizeOf(typeof(CCWin.Win32.NativeMethods.PCURSORINFO));
                    CCWin.Win32.NativeMethods.GetCursorInfo(out pci);
                    if (pci.hCursor != IntPtr.Zero)
                    {
                        Cursor cur = new Cursor(pci.hCursor);
                        g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                        cur.Draw(g, new Rectangle((System.Drawing.Point) (((System.Drawing.Size) Control.MousePosition) - ((System.Drawing.Size) cur.HotSpot)), cur.Size));
                    }
                }
            }
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            }
            return bmp;
        }

        private string GetTimeString()
        {
            DateTime time = DateTime.Now;
            return (time.Date.ToShortDateString().Replace("/", "") + "_" + time.ToLongTimeString().Replace(":", ""));
        }

        private bool HaveSelectedToolButton()
        {
            if ((!this.tBtn_Rect.IsSelected && !this.tBtn_Ellipse.IsSelected) && (!this.tBtn_Arrow.IsSelected && !this.tBtn_Brush.IsSelected))
            {
                return this.tBtn_Text.IsSelected;
            }
            return true;
        }

        private void imageProcessBox1_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetImage(this.m_bmpLayerCurrent);
            if (this.RcTxt != null)
            {
                this.RcTxt.Paste();
            }
            base.Close();
        }

        private void imageProcessBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if ((this.imgpb.Cursor != Cursors.SizeAll) && (this.imgpb.Cursor != Cursors.Default))
            {
                this.panel1.Visible = false;
            }
            if (((e.Button == MouseButtons.Left) && this.imgpb.IsDrawed) && (this.HaveSelectedToolButton() && this.imgpb.SelectedRectangle.Contains(e.Location)))
            {
                if (this.tBtn_Text.IsSelected)
                {
                    this.textBox1.Location = e.Location;
                    this.textBox1.Visible = true;
                    this.textBox1.Focus();
                    return;
                }
                this.m_isStartDraw = true;
                Cursor.Clip = this.imgpb.SelectedRectangle;
            }
            this.m_ptOriginal = e.Location;
        }

        private void imageProcessBox1_MouseMove(object sender, MouseEventArgs e)
        {
            this.m_ptCurrent = e.Location;
            if ((this.imgpb.SelectedRectangle.Contains(e.Location) && this.HaveSelectedToolButton()) && this.imgpb.IsDrawed)
            {
                this.Cursor = Cursors.Cross;
            }
            else if (!this.imgpb.SelectedRectangle.Contains(e.Location))
            {
                this.Cursor = Cursors.Default;
            }
            if (this.imgpb.IsStartDraw && this.panel1.Visible)
            {
                this.SetToolBarLocation();
            }
            if (this.m_isStartDraw && (this.m_bmpLayerShow != null))
            {
                using (Graphics g = Graphics.FromImage(this.m_bmpLayerShow))
                {
                    int tempWidth = 1;
                    if (this.toolButton2.IsSelected)
                    {
                        tempWidth = 3;
                    }
                    if (this.toolButton3.IsSelected)
                    {
                        tempWidth = 5;
                    }
                    Pen p = new Pen(this.colorBox1.SelectedColor, (float) tempWidth);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    if (this.tBtn_Rect.IsSelected)
                    {
                        int tempX = ((e.X - this.m_ptOriginal.X) > 0) ? this.m_ptOriginal.X : e.X;
                        int tempY = ((e.Y - this.m_ptOriginal.Y) > 0) ? this.m_ptOriginal.Y : e.Y;
                        g.Clear(Color.Transparent);
                        g.DrawRectangle(p, tempX - this.imgpb.SelectedRectangle.Left, tempY - this.imgpb.SelectedRectangle.Top, Math.Abs((int) (e.X - this.m_ptOriginal.X)), Math.Abs((int) (e.Y - this.m_ptOriginal.Y)));
                        this.imgpb.Invalidate();
                    }
                    if (this.tBtn_Ellipse.IsSelected)
                    {
                        g.DrawLine(Pens.Red, 0, 0, 200, 200);
                        g.Clear(Color.Transparent);
                        g.DrawEllipse(p, (int) (this.m_ptOriginal.X - this.imgpb.SelectedRectangle.Left), (int) (this.m_ptOriginal.Y - this.imgpb.SelectedRectangle.Top), (int) (e.X - this.m_ptOriginal.X), (int) (e.Y - this.m_ptOriginal.Y));
                        this.imgpb.Invalidate();
                    }
                    if (this.tBtn_Arrow.IsSelected)
                    {
                        g.Clear(Color.Transparent);
                        AdjustableArrowCap lineArrow = new AdjustableArrowCap(4f, 4f, true);
                        p.CustomEndCap = lineArrow;
                        g.DrawLine(p, (System.Drawing.Point) (((System.Drawing.Size) this.m_ptOriginal) - ((System.Drawing.Size) this.imgpb.SelectedRectangle.Location)), (System.Drawing.Point) (((System.Drawing.Size) this.m_ptCurrent) - ((System.Drawing.Size) this.imgpb.SelectedRectangle.Location)));
                        this.imgpb.Invalidate();
                    }
                    if (this.tBtn_Brush.IsSelected)
                    {
                        System.Drawing.Point ptTemp = (System.Drawing.Point) (((System.Drawing.Size) this.m_ptOriginal) - ((System.Drawing.Size) this.imgpb.SelectedRectangle.Location));
                        p.LineJoin = LineJoin.Round;
                        g.DrawLine(p, ptTemp, (System.Drawing.Point) (((System.Drawing.Size) e.Location) - ((System.Drawing.Size) this.imgpb.SelectedRectangle.Location)));
                        this.m_ptOriginal = e.Location;
                        this.imgpb.Invalidate();
                    }
                    p.Dispose();
                }
            }
        }

        private void imageProcessBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!this.imgpb.IsDrawed)
            {
                base.Enabled = false;
                this.imgpb.IsDrawOperationDot = false;
            }
            else if (!this.panel1.Visible)
            {
                this.SetToolBarLocation();
                this.panel1.Visible = true;
                this.m_bmpLayerCurrent = this.imgpb.GetResultBmp();
                this.m_bmpLayerShow = new Bitmap(this.m_bmpLayerCurrent.Width, this.m_bmpLayerCurrent.Height);
            }
            if ((this.imgpb.Cursor == Cursors.SizeAll) && (this.m_ptOriginal != e.Location))
            {
                this.m_bmpLayerCurrent = this.imgpb.GetResultBmp();
            }
            if (this.m_isStartDraw)
            {
                Cursor.Clip = Rectangle.Empty;
                this.m_isStartDraw = false;
                if ((e.Location != this.m_ptOriginal) || this.tBtn_Brush.IsSelected)
                {
                    this.SetLayer();
                }
            }
        }

        private void imageProcessBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (this.m_layer.Count > 0)
            {
                g.DrawImage(this.m_layer[this.m_layer.Count - 1], this.imgpb.SelectedRectangle.Location);
            }
            if (this.m_bmpLayerShow != null)
            {
                g.DrawImage(this.m_bmpLayerShow, this.imgpb.SelectedRectangle.Location);
            }
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.panel1 = new Panel();
            this.pictureBox2 = new PictureBox();
            this.pictureBox1 = new PictureBox();
            this.panel2 = new Panel();
            this.textBox1 = new TextBox();
            this.timer1 = new Timer(this.components);
            this.toolButton1 = new ToolButton();
            this.toolButton3 = new ToolButton();
            this.toolButton2 = new ToolButton();
            this.colorBox1 = new ColorBox();
            this.tBtn_Finish = new ToolButton();
            this.tBtn_Close = new ToolButton();
            this.tBtn_Save = new ToolButton();
            this.tBtn_Cancel = new ToolButton();
            this.tBtn_Text = new ToolButton();
            this.tBtn_Brush = new ToolButton();
            this.tBtn_Arrow = new ToolButton();
            this.tBtn_Ellipse = new ToolButton();
            this.tBtn_Rect = new ToolButton();
            this.imgpb = new ImageProcessBox();
            this.panel1.SuspendLayout();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            this.panel2.SuspendLayout();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.tBtn_Finish);
            this.panel1.Controls.Add(this.tBtn_Close);
            this.panel1.Controls.Add(this.tBtn_Save);
            this.panel1.Controls.Add(this.tBtn_Cancel);
            this.panel1.Controls.Add(this.tBtn_Text);
            this.panel1.Controls.Add(this.tBtn_Brush);
            this.panel1.Controls.Add(this.tBtn_Arrow);
            this.panel1.Controls.Add(this.tBtn_Ellipse);
            this.panel1.Controls.Add(this.tBtn_Rect);
            this.panel1.Location = new System.Drawing.Point(12, 0x53);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(0x126, 0x19);
            this.panel1.TabIndex = 1;
            this.pictureBox2.Image = Resources.separator;
            this.pictureBox2.Location = new System.Drawing.Point(0xc7, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1, 0x11);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            this.pictureBox1.Image = Resources.separator;
            this.pictureBox1.Location = new System.Drawing.Point(0x8a, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1, 0x11);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.panel2.Controls.Add(this.toolButton1);
            this.panel2.Controls.Add(this.toolButton3);
            this.panel2.Controls.Add(this.toolButton2);
            this.panel2.Controls.Add(this.colorBox1);
            this.panel2.Location = new System.Drawing.Point(12, 0x72);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(250, 0x20);
            this.panel2.TabIndex = 2;
            this.textBox1.Location = new System.Drawing.Point(12, 0x18);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 0x13);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextChanged += new EventHandler(this.textBox1_TextChanged);
            this.textBox1.Resize += new EventHandler(this.textBox1_Resize);
            this.textBox1.Validating += new CancelEventHandler(this.textBox1_Validating);
            this.timer1.Tick += new EventHandler(this.timer1_Tick);
            this.toolButton1.BtnImage = Resources.small;
            this.toolButton1.IsSelected = true;
            this.toolButton1.IsSelectedBtn = true;
            this.toolButton1.IsSingleSelectedBtn = true;
            this.toolButton1.Location = new System.Drawing.Point(3, 6);
            this.toolButton1.Name = "toolButton1";
            this.toolButton1.Size = new System.Drawing.Size(0x15, 0x15);
            this.toolButton1.TabIndex = 4;
            this.toolButton3.BtnImage = Resources.large;
            this.toolButton3.IsSelected = false;
            this.toolButton3.IsSelectedBtn = true;
            this.toolButton3.IsSingleSelectedBtn = true;
            this.toolButton3.Location = new System.Drawing.Point(0x39, 6);
            this.toolButton3.Name = "toolButton3";
            this.toolButton3.Size = new System.Drawing.Size(0x15, 0x15);
            this.toolButton3.TabIndex = 3;
            this.toolButton2.BtnImage = Resources.middle;
            this.toolButton2.IsSelected = false;
            this.toolButton2.IsSelectedBtn = true;
            this.toolButton2.IsSingleSelectedBtn = true;
            this.toolButton2.Location = new System.Drawing.Point(30, 6);
            this.toolButton2.Name = "toolButton2";
            this.toolButton2.Size = new System.Drawing.Size(0x15, 0x15);
            this.toolButton2.TabIndex = 2;
            this.colorBox1.Location = new System.Drawing.Point(0x55, 0);
            this.colorBox1.Name = "colorBox1";
            this.colorBox1.Size = new System.Drawing.Size(0xa5, 0x23);
            this.colorBox1.TabIndex = 0;
            this.colorBox1.Text = "colorBox1";
            this.tBtn_Finish.BtnImage = Resources.ok;
            this.tBtn_Finish.IsSelected = false;
            this.tBtn_Finish.IsSelectedBtn = false;
            this.tBtn_Finish.IsSingleSelectedBtn = false;
            this.tBtn_Finish.Location = new System.Drawing.Point(0xe9, 3);
            this.tBtn_Finish.Name = "tBtn_Finish";
            this.tBtn_Finish.Size = new System.Drawing.Size(0x44, 0x15);
            this.tBtn_Finish.TabIndex = 8;
            this.tBtn_Finish.Text = "Finish ";
            this.tBtn_Finish.Click += new EventHandler(this.tBtn_Finish_Click);
            this.tBtn_Close.BtnImage = Resources.close;
            this.tBtn_Close.IsSelected = false;
            this.tBtn_Close.IsSelectedBtn = false;
            this.tBtn_Close.IsSingleSelectedBtn = false;
            this.tBtn_Close.Location = new System.Drawing.Point(0xce, 3);
            this.tBtn_Close.Name = "tBtn_Close";
            this.tBtn_Close.Size = new System.Drawing.Size(0x15, 0x15);
            this.tBtn_Close.TabIndex = 7;
            this.tBtn_Save.BtnImage = Resources.save;
            this.tBtn_Save.IsSelected = false;
            this.tBtn_Save.IsSelectedBtn = false;
            this.tBtn_Save.IsSingleSelectedBtn = false;
            this.tBtn_Save.Location = new System.Drawing.Point(0xac, 3);
            this.tBtn_Save.Name = "tBtn_Save";
            this.tBtn_Save.Size = new System.Drawing.Size(0x15, 0x15);
            this.tBtn_Save.TabIndex = 6;
            this.tBtn_Save.Click += new EventHandler(this.tBtn_Save_Click);
            this.tBtn_Cancel.BtnImage = Resources.cancel;
            this.tBtn_Cancel.IsSelected = false;
            this.tBtn_Cancel.IsSelectedBtn = false;
            this.tBtn_Cancel.IsSingleSelectedBtn = false;
            this.tBtn_Cancel.Location = new System.Drawing.Point(0x91, 3);
            this.tBtn_Cancel.Name = "tBtn_Cancel";
            this.tBtn_Cancel.Size = new System.Drawing.Size(0x15, 0x15);
            this.tBtn_Cancel.TabIndex = 5;
            this.tBtn_Cancel.Click += new EventHandler(this.tBtn_Cancel_Click);
            this.tBtn_Text.BtnImage = Resources.text;
            this.tBtn_Text.IsSelected = false;
            this.tBtn_Text.IsSelectedBtn = true;
            this.tBtn_Text.IsSingleSelectedBtn = false;
            this.tBtn_Text.Location = new System.Drawing.Point(0x6f, 3);
            this.tBtn_Text.Name = "tBtn_Text";
            this.tBtn_Text.Size = new System.Drawing.Size(0x15, 0x15);
            this.tBtn_Text.TabIndex = 4;
            this.tBtn_Brush.BtnImage = Resources.brush;
            this.tBtn_Brush.IsSelected = false;
            this.tBtn_Brush.IsSelectedBtn = true;
            this.tBtn_Brush.IsSingleSelectedBtn = false;
            this.tBtn_Brush.Location = new System.Drawing.Point(0x54, 3);
            this.tBtn_Brush.Name = "tBtn_Brush";
            this.tBtn_Brush.Size = new System.Drawing.Size(0x15, 0x15);
            this.tBtn_Brush.TabIndex = 3;
            this.tBtn_Arrow.BtnImage = Resources.arrow;
            this.tBtn_Arrow.IsSelected = false;
            this.tBtn_Arrow.IsSelectedBtn = true;
            this.tBtn_Arrow.IsSingleSelectedBtn = false;
            this.tBtn_Arrow.Location = new System.Drawing.Point(0x39, 3);
            this.tBtn_Arrow.Name = "tBtn_Arrow";
            this.tBtn_Arrow.Size = new System.Drawing.Size(0x15, 0x15);
            this.tBtn_Arrow.TabIndex = 2;
            this.tBtn_Ellipse.BtnImage = Resources.ellips;
            this.tBtn_Ellipse.IsSelected = false;
            this.tBtn_Ellipse.IsSelectedBtn = true;
            this.tBtn_Ellipse.IsSingleSelectedBtn = false;
            this.tBtn_Ellipse.Location = new System.Drawing.Point(30, 3);
            this.tBtn_Ellipse.Name = "tBtn_Ellipse";
            this.tBtn_Ellipse.Size = new System.Drawing.Size(0x15, 0x15);
            this.tBtn_Ellipse.TabIndex = 1;
            this.tBtn_Rect.BtnImage = Resources.rect;
            this.tBtn_Rect.IsSelected = false;
            this.tBtn_Rect.IsSelectedBtn = true;
            this.tBtn_Rect.IsSingleSelectedBtn = false;
            this.tBtn_Rect.Location = new System.Drawing.Point(3, 3);
            this.tBtn_Rect.Name = "tBtn_Rect";
            this.tBtn_Rect.Size = new System.Drawing.Size(0x15, 0x15);
            this.tBtn_Rect.TabIndex = 0;
            this.imgpb.BackColor = Color.Black;
            this.imgpb.BaseImage = null;
            this.imgpb.CanReset = true;
            this.imgpb.Cursor = Cursors.Default;
            this.imgpb.Dock = DockStyle.Fill;
            this.imgpb.ForeColor = Color.White;
            this.imgpb.Location = new System.Drawing.Point(0, 0);
            this.imgpb.Name = "imgpb";
            this.imgpb.Size = new System.Drawing.Size(0x16b, 0xf7);
            this.imgpb.TabIndex = 0;
            this.imgpb.Text = "imageProcessBox1";
            this.imgpb.Paint += new PaintEventHandler(this.imageProcessBox1_Paint);
            this.imgpb.DoubleClick += new EventHandler(this.imageProcessBox1_DoubleClick);
            this.imgpb.MouseDown += new MouseEventHandler(this.imageProcessBox1_MouseDown);
            this.imgpb.MouseMove += new MouseEventHandler(this.imageProcessBox1_MouseMove);
            this.imgpb.MouseUp += new MouseEventHandler(this.imageProcessBox1_MouseUp);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(0x16b, 0xf7);
            base.Controls.Add(this.textBox1);
            base.Controls.Add(this.panel2);
            base.Controls.Add(this.panel1);
            base.Controls.Add(this.imgpb);
            this.Cursor = Cursors.Default;
            base.Name = "FrmCapture";
            this.Text = "FrmCapture";
            base.Load += new EventHandler(this.FrmCapture_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((ISupportInitialize) this.pictureBox2).EndInit();
            ((ISupportInitialize) this.pictureBox1).EndInit();
            this.panel2.ResumeLayout(false);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void InitMember()
        {
            this.panel1.Visible = false;
            this.panel2.Visible = false;
            this.panel1.BackColor = Color.White;
            this.panel2.BackColor = Color.White;
            this.panel1.Height = this.tBtn_Finish.Bottom + 3;
            this.panel1.Width = this.tBtn_Finish.Right + 3;
            this.panel2.Height = this.colorBox1.Height;
            this.panel1.Paint += delegate (object s, PaintEventArgs e) {
                e.Graphics.DrawRectangle(Pens.SteelBlue, 0, 0, this.panel1.Width - 1, this.panel1.Height - 1);
            };
            this.panel2.Paint += delegate (object s, PaintEventArgs e) {
                e.Graphics.DrawRectangle(Pens.SteelBlue, 0, 0, this.panel2.Width - 1, this.panel2.Height - 1);
            };
            this.tBtn_Rect.Click += new EventHandler(this.selectToolButton_Click);
            this.tBtn_Ellipse.Click += new EventHandler(this.selectToolButton_Click);
            this.tBtn_Arrow.Click += new EventHandler(this.selectToolButton_Click);
            this.tBtn_Brush.Click += new EventHandler(this.selectToolButton_Click);
            this.tBtn_Text.Click += new EventHandler(this.selectToolButton_Click);
            this.tBtn_Close.Click += delegate (object s, EventArgs e) {
                base.Close();
            };
            this.textBox1.BorderStyle = BorderStyle.None;
            this.textBox1.Visible = false;
            this.textBox1.ForeColor = Color.Red;
            this.colorBox1.ColorChanged += delegate (object s, ColorChangedEventArgs e) {
                this.textBox1.ForeColor = e.Color;
            };
        }

        private void m_MHook_MHookEvent(object sender, MHookEventArgs e)
        {
            if (!base.Enabled)
            {
                this.imgpb.SetInfoPoint(Control.MousePosition.X, Control.MousePosition.Y);
            }
            if ((e.MButton == ButtonStatus.LeftDown) || (e.MButton == ButtonStatus.RightDown))
            {
                base.Enabled = true;
                this.imgpb.IsDrawOperationDot = true;
            }
            if (e.MButton == ButtonStatus.RightUp)
            {
                if (!this.imgpb.IsDrawed)
                {
                    base.BeginInvoke((System.Threading.ThreadStart)delegate
                    {
                        base.Close();
                    });
                }
                base.Enabled = false;
                this.imgpb.ClearDraw();
                this.imgpb.CanReset = true;
                this.imgpb.IsDrawOperationDot = false;
                this.m_layer.Clear();
                this.m_bmpLayerCurrent = null;
                this.m_bmpLayerShow = null;
                this.ClearToolBarBtnSelected();
                this.panel1.Visible = false;
                this.panel2.Visible = false;
            }
            if (!base.Enabled)
            {
                this.FoundAndDrawWindowRect();
            }
        }

        private void selectToolButton_Click(object sender, EventArgs e)
        {
            this.panel2.Visible = ((ToolButton) sender).IsSelected;
            if (this.panel2.Visible)
            {
                this.imgpb.CanReset = false;
            }
            else
            {
                this.imgpb.CanReset = this.m_layer.Count == 0;
            }
            this.SetToolBarLocation();
        }

        private void SetLayer()
        {
            if (!base.IsDisposed)
            {
                using (Graphics g = Graphics.FromImage(this.m_bmpLayerCurrent))
                {
                    g.DrawImage(this.m_bmpLayerShow, 0, 0);
                }
                Bitmap bmpTemp = this.m_bmpLayerCurrent.Clone(new Rectangle(0, 0, this.m_bmpLayerCurrent.Width, this.m_bmpLayerCurrent.Height), this.m_bmpLayerCurrent.PixelFormat);
                this.m_layer.Add(bmpTemp);
            }
        }

        private void SetToolBarLocation()
        {
            int tempX = this.imgpb.SelectedRectangle.Left;
            int tempY = this.imgpb.SelectedRectangle.Bottom + 5;
            int tempHeight = this.panel2.Visible ? (this.panel2.Height + 2) : 0;
            if (((tempY + this.panel1.Height) + tempHeight) >= base.Height)
            {
                tempY = ((this.imgpb.SelectedRectangle.Top - this.panel1.Height) - 10) - this.imgpb.Font.Height;
            }
            if ((tempY - tempHeight) <= 0)
            {
                if (((this.imgpb.SelectedRectangle.Top - 5) - this.imgpb.Font.Height) >= 0)
                {
                    tempY = this.imgpb.SelectedRectangle.Top + 5;
                }
                else
                {
                    tempY = (this.imgpb.SelectedRectangle.Top + 10) + this.imgpb.Font.Height;
                }
            }
            if ((tempX + this.panel1.Width) >= base.Width)
            {
                tempX = (base.Width - this.panel1.Width) - 5;
            }
            this.panel1.Left = tempX;
            this.panel2.Left = tempX;
            this.panel1.Top = tempY;
            this.panel2.Top = (this.imgpb.SelectedRectangle.Top > tempY) ? (tempY - tempHeight) : (this.panel1.Bottom + 2);
        }

        private void tBtn_Cancel_Click(object sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(this.m_bmpLayerShow))
            {
                g.Clear(Color.Transparent);
            }
            if (this.m_layer.Count > 0)
            {
                this.m_layer.RemoveAt(this.m_layer.Count - 1);
                if (this.m_layer.Count > 0)
                {
                    this.m_bmpLayerCurrent = this.m_layer[this.m_layer.Count - 1].Clone(new Rectangle(0, 0, this.m_bmpLayerCurrent.Width, this.m_bmpLayerCurrent.Height), this.m_bmpLayerCurrent.PixelFormat);
                }
                else
                {
                    this.m_bmpLayerCurrent = this.imgpb.GetResultBmp();
                }
                this.imgpb.Invalidate();
                this.imgpb.CanReset = (this.m_layer.Count == 0) && !this.HaveSelectedToolButton();
            }
            else
            {
                base.Enabled = false;
                this.imgpb.ClearDraw();
                this.imgpb.IsDrawOperationDot = false;
                this.panel1.Visible = false;
                this.panel2.Visible = false;
            }
        }

        private void tBtn_Finish_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(this.m_bmpLayerCurrent);
            if (this.RcTxt != null)
            {
                this.RcTxt.Paste();
            }
            base.Close();
        }

        private void tBtn_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "位图(*.bmp)|*.bmp|JPEG(*.jpg)|*.jpg";
            saveDlg.FilterIndex = 1;
            saveDlg.FileName = "CAPTURE_" + this.GetTimeString();
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                switch (saveDlg.FilterIndex)
                {
                    case 1:
                        this.m_bmpLayerCurrent.Clone(new Rectangle(0, 0, this.m_bmpLayerCurrent.Width, this.m_bmpLayerCurrent.Height), PixelFormat.Format24bppRgb).Save(saveDlg.FileName, ImageFormat.Bmp);
                        base.Close();
                        return;

                    case 2:
                        this.m_bmpLayerCurrent.Save(saveDlg.FileName, ImageFormat.Jpeg);
                        base.Close();
                        return;

                    default:
                        return;
                }
            }
        }

        private void textBox1_Resize(object sender, EventArgs e)
        {
            int se = 10;
            if (this.toolButton2.IsSelected)
            {
                se = 12;
            }
            if (this.toolButton3.IsSelected)
            {
                se = 14;
            }
            if (this.textBox1.Font.Height != se)
            {
                this.textBox1.Font = new Font(this.Font.FontFamily, (float) se);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            System.Drawing.Size se = TextRenderer.MeasureText(this.textBox1.Text, this.textBox1.Font);
            this.textBox1.Size = se.IsEmpty ? new System.Drawing.Size(50, this.textBox1.Font.Height) : se;
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            this.textBox1.Visible = false;
            if (string.IsNullOrEmpty(this.textBox1.Text.Trim()))
            {
                this.textBox1.Text = "";
            }
            else
            {
                using (Graphics g = Graphics.FromImage(this.m_bmpLayerCurrent))
                {
                    SolidBrush sb = new SolidBrush(this.colorBox1.SelectedColor);
                    g.DrawString(this.textBox1.Text, this.textBox1.Font, sb, (float) (this.textBox1.Left - this.imgpb.SelectedRectangle.Left), (float) (this.textBox1.Top - this.imgpb.SelectedRectangle.Top));
                    sb.Dispose();
                    this.textBox1.Text = "";
                    this.SetLayer();
                    this.imgpb.Invalidate();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!base.Enabled)
            {
                this.imgpb.SetInfoPoint(Control.MousePosition.X, Control.MousePosition.Y);
            }
        }

        public Color ImgProcessBoxDotColor
        {
            get
            {
                return this.imgpb.DotColor;
            }
            set
            {
                this.imgpb.DotColor = value;
            }
        }

        public bool ImgProcessBoxIsShowInfo
        {
            get
            {
                return this.imgpb.IsShowInfo;
            }
            set
            {
                this.imgpb.IsShowInfo = value;
            }
        }

        public Color ImgProcessBoxLineColor
        {
            get
            {
                return this.imgpb.LineColor;
            }
            set
            {
                this.imgpb.LineColor = value;
            }
        }

        public System.Drawing.Size ImgProcessBoxMagnifySize
        {
            get
            {
                return this.imgpb.MagnifySize;
            }
            set
            {
                this.imgpb.MagnifySize = value;
            }
        }

        public int ImgProcessBoxMagnifyTimes
        {
            get
            {
                return this.imgpb.MagnifyTimes;
            }
            set
            {
                this.imgpb.MagnifyTimes = value;
            }
        }

        public bool IsCaptureCursor
        {
            get
            {
                return this.isCaptureCursor;
            }
            set
            {
                this.isCaptureCursor = value;
            }
        }
    }
}

