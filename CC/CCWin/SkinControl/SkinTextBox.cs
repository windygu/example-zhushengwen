namespace CCWin.SkinControl
{
    using CCWin.Properties;
    using CCWin.SkinClass;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Threading;
    using System.Windows.Forms;

    public class SkinTextBox : UserControl
    {
        private System.Windows.Forms.Cursor _cursor = Cursors.IBeam;
        private Image _icon;
        private bool _iconIsButton;
        private ControlState _iconMouseState;
        private ControlState _mouseState;
        private WaterTextBox BaseText;
        private IContainer components;
        private bool flag;
        private EventHandler IconClick;
        private Bitmap mouseback;
        private Bitmap normlback;

        public event EventHandler iconClick
        {
            add
            {
                EventHandler handler2;
                EventHandler iconClick = this.IconClick;
                do
                {
                    handler2 = iconClick;
                    EventHandler handler3 = (EventHandler) Delegate.Combine(handler2, value);
                    iconClick = Interlocked.CompareExchange<EventHandler>(ref this.IconClick, handler3, handler2);
                }
                while (iconClick != handler2);
            }
            remove
            {
                EventHandler handler2;
                EventHandler iconClick = this.IconClick;
                do
                {
                    handler2 = iconClick;
                    EventHandler handler3 = (EventHandler) Delegate.Remove(handler2, value);
                    iconClick = Interlocked.CompareExchange<EventHandler>(ref this.IconClick, handler3, handler2);
                }
                while (iconClick != handler2);
            }
        }

        public SkinTextBox()
        {
            this.InitializeComponent();
            this.Init();
            this.InitEvents();
            this.BackColor = Color.Transparent;
        }

        public void AppendText(string str)
        {
            this.BaseText.AppendText(str);
        }

        private void BaseText_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        private void BaseText_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        private void BaseText_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }

        private void BaseText_MouseEnter(object sender, EventArgs e)
        {
            this.MouseState = ControlState.Hover;
        }

        protected void BaseText_MouseLeave(object sender, EventArgs e)
        {
            this.MouseState = ControlState.Normal;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void Init()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            base.UpdateStyles();
        }

        private void InitEvents()
        {
            this.BaseText.MouseEnter += new EventHandler(this.BaseText_MouseEnter);
            this.BaseText.MouseLeave += new EventHandler(this.BaseText_MouseLeave);
            this.BaseText.KeyDown += new KeyEventHandler(this.BaseText_KeyDown);
            this.BaseText.KeyPress += new KeyPressEventHandler(this.BaseText_KeyPress);
            this.BaseText.KeyUp += new KeyEventHandler(this.BaseText_KeyUp);
        }

        private void InitializeComponent()
        {
            this.BaseText = new WaterTextBox();
            base.SuspendLayout();
            this.BaseText.BorderStyle = BorderStyle.None;
            this.BaseText.Dock = DockStyle.Fill;
            this.BaseText.Font = new System.Drawing.Font("微软雅黑", 9.75f);
            this.BaseText.Location = new Point(5, 5);
            this.BaseText.Margin = new Padding(0);
            this.BaseText.Name = "BaseText";
            this.BaseText.Size = new Size(0xaf, 0x12);
            this.BaseText.TabIndex = 1;
            this.BaseText.WaterColor = Color.DarkGray;
            this.BaseText.WaterText = "";
            base.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.Black;
            base.Controls.Add(this.BaseText);
            this.Cursor = Cursors.IBeam;
            this.DoubleBuffered = true;
            base.Margin = new Padding(0);
            this.MinimumSize = new Size(0, 0x1c);
            base.Name = "SkinTextBox";
            base.Padding = new Padding(5);
            base.Size = new Size(0xb9, 0x1c);
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void OnIconClick()
        {
            if (this.IconClick != null)
            {
                this.IconClick(this, EventArgs.Empty);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (((this._icon != null) && this._iconIsButton) && ((e.Button == MouseButtons.Left) && this.IconRect.Contains(e.Location)))
            {
                this.IconMouseState = ControlState.Pressed;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.MouseState = ControlState.Hover;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.MouseState = ControlState.Normal;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if ((this._icon != null) && this.IconRect.Contains(e.Location))
            {
                if (this._iconIsButton)
                {
                    this.Cursor = Cursors.Hand;
                }
                else
                {
                    this.Cursor = Cursors.IBeam;
                }
            }
            else
            {
                this.Cursor = Cursors.IBeam;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if ((this._icon != null) && this._iconIsButton)
            {
                this.IconMouseState = ControlState.Hover;
                if ((e.Button == MouseButtons.Left) && this.IconRect.Contains(e.Location))
                {
                    this.OnIconClick();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Bitmap mouse = (this.MouseBack == null) ? Resources.frameBorderEffect_mouseDownDraw : this.MouseBack;
            Bitmap norml = (this.NormlBack == null) ? Resources.frameBorderEffect_normalDraw : this.NormlBack;
            Bitmap btm = (this._mouseState == ControlState.Hover) ? mouse : norml;
            if (btm != null)
            {
                DrawHelper.RendererBackground(g, base.ClientRectangle, btm, true);
            }
            if (this._icon != null)
            {
                Rectangle iconRect = this.IconRect;
                if (this._iconMouseState == ControlState.Pressed)
                {
                    iconRect.X++;
                    iconRect.Y++;
                }
                g.DrawImage(this._icon, iconRect, 0, 0, this._icon.Width, this._icon.Height, GraphicsUnit.Pixel);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (base.Height > 0x1c)
            {
                this.BaseText.Multiline = true;
            }
            else
            {
                this.BaseText.Multiline = false;
            }
        }

        protected virtual void PositionTextBox()
        {
            if ((this._icon != null) && !this.flag)
            {
                base.Padding = new Padding(base.Padding.Left, base.Padding.Top, base.Padding.Right + 0x17, base.Padding.Bottom);
                this.flag = true;
            }
            else if ((this._icon == null) && this.flag)
            {
                base.Padding = new Padding(base.Padding.Left, base.Padding.Top, base.Padding.Right - 0x17, base.Padding.Bottom);
                this.flag = false;
            }
        }

        public override System.Windows.Forms.Cursor Cursor
        {
            get
            {
                return this._cursor;
            }
            set
            {
                this._cursor = value;
            }
        }

        [Category("Skin"), Description("用于显示控件中文本的字体。")]
        public override System.Drawing.Font Font
        {
            get
            {
                return this.BaseText.Font;
            }
            set
            {
                base.Font = this.BaseText.Font = value;
            }
        }

        [Description("此组件的前景色，用于显示文本。"), Category("Skin")]
        public override Color ForeColor
        {
            get
            {
                return this.BaseText.ForeColor;
            }
            set
            {
                base.ForeColor = this.BaseText.ForeColor = value;
            }
        }

        [Category("Skin"), Description("文本框的图标")]
        public Image Icon
        {
            get
            {
                return this._icon;
            }
            set
            {
                if (this._icon != value)
                {
                    this._icon = value;
                    base.Invalidate(this.IconRect);
                    this.PositionTextBox();
                }
            }
        }

        [Category("Skin"), Description("文本框的图标是否是按钮")]
        public bool IconIsButton
        {
            get
            {
                return this._iconIsButton;
            }
            set
            {
                this._iconIsButton = value;
            }
        }

        protected ControlState IconMouseState
        {
            get
            {
                return this._iconMouseState;
            }
            set
            {
                this._iconMouseState = value;
                base.Invalidate();
            }
        }

        protected Rectangle IconRect
        {
            get
            {
                return new Rectangle(base.Width - 0x17, 3, 20, 20);
            }
        }

        [Description("指示将为单行编辑控件的密码输入显示的字符。"), Category("Skin")]
        public char IsPasswordChat
        {
            get
            {
                return this.BaseText.PasswordChar;
            }
            set
            {
                this.BaseText.PasswordChar = value;
            }
        }

        [Category("Skin"), Description("指示编辑控件中的文本是否以默认的密码字符显示。")]
        public bool IsSystemPasswordChar
        {
            get
            {
                return this.BaseText.UseSystemPasswordChar;
            }
            set
            {
                this.BaseText.UseSystemPasswordChar = value;
            }
        }

        [Description("多行编辑中的文本行，作为字符串值的数组。"), Category("Skin")]
        public string[] Lines
        {
            get
            {
                return this.BaseText.Lines;
            }
            set
            {
                this.BaseText.Lines = value;
            }
        }

        [Category("Skin"), Description("指定可以在编辑控件中输入的最大字符数。")]
        public int MaxLength
        {
            get
            {
                return this.BaseText.MaxLength;
            }
            set
            {
                this.BaseText.MaxLength = value;
            }
        }

        [Description("悬浮时背景框。"), Category("Skin")]
        public Bitmap MouseBack
        {
            get
            {
                return this.mouseback;
            }
            set
            {
                if (this.mouseback != value)
                {
                    this.mouseback = value;
                    base.Invalidate();
                }
            }
        }

        protected ControlState MouseState
        {
            get
            {
                return this._mouseState;
            }
            set
            {
                this._mouseState = value;
                base.Invalidate();
            }
        }

        [Category("Skin"), Description("控件编辑控件的文本是否能够跨越多行。")]
        public bool Multiline
        {
            get
            {
                return this.BaseText.Multiline;
            }
            set
            {
                this.BaseText.Multiline = value;
            }
        }

        [Category("Skin"), Description("正常状态时背景框。")]
        public Bitmap NormlBack
        {
            get
            {
                return this.normlback;
            }
            set
            {
                if (this.normlback != value)
                {
                    this.normlback = value;
                    base.Invalidate();
                }
            }
        }

        [Description("控制能否更改编辑控件中的文本。"), Category("Skin")]
        public bool ReadOnly
        {
            get
            {
                return this.BaseText.ReadOnly;
            }
            set
            {
                this.BaseText.ReadOnly = value;
            }
        }

        [Category("Skin"), Description("指示对于多行编辑控件，将为此控件显示哪些滚动条。")]
        public System.Windows.Forms.ScrollBars ScrollBars
        {
            get
            {
                return this.BaseText.ScrollBars;
            }
            set
            {
                this.BaseText.ScrollBars = value;
            }
        }

        [Category("Skin"), Description("与控件关联的文本"), Browsable(true)]
        public override string Text
        {
            get
            {
                return this.BaseText.Text;
            }
            set
            {
                this.BaseText.Text = value;
            }
        }

        [Description("指示应该如何对齐编辑控件的文本。"), Category("Skin")]
        public HorizontalAlignment TextAlign
        {
            get
            {
                return this.BaseText.TextAlign;
            }
            set
            {
                this.BaseText.TextAlign = value;
            }
        }

        [Description("水印颜色"), Category("Skin")]
        public Color WaterColor
        {
            get
            {
                return this.BaseText.WaterColor;
            }
            set
            {
                this.BaseText.WaterColor = value;
            }
        }

        [Description("水印文字"), Category("Skin")]
        public string WaterText
        {
            get
            {
                return this.BaseText.WaterText;
            }
            set
            {
                this.BaseText.WaterText = value;
            }
        }

        [Description("指示多行编辑控件是否自动换行。"), Category("Skin")]
        public bool WordWrap
        {
            get
            {
                return this.BaseText.WordWrap;
            }
            set
            {
                this.BaseText.WordWrap = value;
            }
        }
    }
}

