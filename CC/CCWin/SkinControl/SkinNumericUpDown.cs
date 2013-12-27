namespace CCWin.SkinControl
{
    using CCWin.Win32;
    using CCWin.Win32.Struct;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(NumericUpDown))]
    public class SkinNumericUpDown : NumericUpDown
    {
        private Color _arrowColor = Color.FromArgb(0, 0x5f, 0x98);
        private Color _baseColor = Color.FromArgb(0xa6, 0xde, 0xff);
        private Color _borderColor = Color.FromArgb(0x17, 0xa9, 0xfe);
        private UpDownButtonNativeWindow _upDownButtonNativeWindow;
        private static readonly object EventPaintUpDownButton = new object();

        public event UpDownButtonPaintEventHandler PaintUpDownButton
        {
            add
            {
                base.Events.AddHandler(EventPaintUpDownButton, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventPaintUpDownButton, value);
            }
        }

        private Color GetColor(Color colorBase, int a, int r, int g, int b)
        {
            int a0 = colorBase.A;
            int r0 = colorBase.R;
            int g0 = colorBase.G;
            int b0 = colorBase.B;
            if ((a + a0) > 0xff)
            {
                a = 0xff;
            }
            else
            {
                a = Math.Max(a + a0, 0);
            }
            if ((r + r0) > 0xff)
            {
                r = 0xff;
            }
            else
            {
                r = Math.Max(r + r0, 0);
            }
            if ((g + g0) > 0xff)
            {
                g = 0xff;
            }
            else
            {
                g = Math.Max(g + g0, 0);
            }
            if ((b + b0) > 0xff)
            {
                b = 0xff;
            }
            else
            {
                b = Math.Max(b + b0, 0);
            }
            return Color.FromArgb(a, r, g, b);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this._upDownButtonNativeWindow == null)
            {
                this._upDownButtonNativeWindow = new UpDownButtonNativeWindow(this);
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            if (this._upDownButtonNativeWindow != null)
            {
                this._upDownButtonNativeWindow.Dispose();
                this._upDownButtonNativeWindow = null;
            }
        }

        protected virtual void OnPaintUpDownButton(UpDownButtonPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.ClipRectangle;
            Color upButtonBaseColor = this._baseColor;
            Color upButtonBorderColor = this._borderColor;
            Color upButtonArrowColor = this._arrowColor;
            Color downButtonBaseColor = this._baseColor;
            Color downButtonBorderColor = this._borderColor;
            Color downButtonArrowColor = this._arrowColor;
            Rectangle upButtonRect = rect;
            upButtonRect.Y++;
            upButtonRect.Width -= 2;
            upButtonRect.Height = (rect.Height / 2) - 2;
            Rectangle downButtonRect = rect;
            downButtonRect.Y = upButtonRect.Bottom + 2;
            downButtonRect.Height = (rect.Height - upButtonRect.Bottom) - 4;
            downButtonRect.Width -= 2;
            if (base.Enabled)
            {
                if (e.MouseOver)
                {
                    if (e.MousePress)
                    {
                        if (e.MouseInUpButton)
                        {
                            upButtonBaseColor = this.GetColor(this._baseColor, 0, -35, -24, -9);
                        }
                        else
                        {
                            downButtonBaseColor = this.GetColor(this._baseColor, 0, -35, -24, -9);
                        }
                    }
                    else if (e.MouseInUpButton)
                    {
                        upButtonBaseColor = this.GetColor(this._baseColor, 0, 0x23, 0x18, 9);
                    }
                    else
                    {
                        downButtonBaseColor = this.GetColor(this._baseColor, 0, 0x23, 0x18, 9);
                    }
                }
            }
            else
            {
                upButtonBaseColor = SystemColors.Control;
                upButtonBorderColor = SystemColors.ControlDark;
                upButtonArrowColor = SystemColors.ControlDark;
                downButtonBaseColor = SystemColors.Control;
                downButtonBorderColor = SystemColors.ControlDark;
                downButtonArrowColor = SystemColors.ControlDark;
            }
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color backColor = base.Enabled ? base.BackColor : SystemColors.Control;
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                rect.Inflate(1, 1);
                g.FillRectangle(brush, rect);
            }
            this.RenderButton(g, upButtonRect, upButtonBaseColor, upButtonBorderColor, upButtonArrowColor, ArrowDirection.Up);
            this.RenderButton(g, downButtonRect, downButtonBaseColor, downButtonBorderColor, downButtonArrowColor, ArrowDirection.Down);
            UpDownButtonPaintEventHandler handler = base.Events[EventPaintUpDownButton] as UpDownButtonPaintEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void RenderArrowInternal(Graphics g, Rectangle dropDownRect, ArrowDirection direction, Brush brush)
        {
            System.Drawing.Point point = new System.Drawing.Point(dropDownRect.Left + (dropDownRect.Width / 2), dropDownRect.Top + (dropDownRect.Height / 2));
            System.Drawing.Point[] points = null;
            switch (direction)
            {
                case ArrowDirection.Left:
                    points = new System.Drawing.Point[] { new System.Drawing.Point(point.X + 2, point.Y - 3), new System.Drawing.Point(point.X + 2, point.Y + 3), new System.Drawing.Point(point.X - 1, point.Y) };
                    break;

                case ArrowDirection.Up:
                    points = new System.Drawing.Point[] { new System.Drawing.Point(point.X - 3, point.Y + 1), new System.Drawing.Point(point.X + 3, point.Y + 1), new System.Drawing.Point(point.X, point.Y - 1) };
                    break;

                case ArrowDirection.Right:
                    points = new System.Drawing.Point[] { new System.Drawing.Point(point.X - 2, point.Y - 3), new System.Drawing.Point(point.X - 2, point.Y + 3), new System.Drawing.Point(point.X + 1, point.Y) };
                    break;

                default:
                    points = new System.Drawing.Point[] { new System.Drawing.Point(point.X - 3, point.Y - 1), new System.Drawing.Point(point.X + 3, point.Y - 1), new System.Drawing.Point(point.X, point.Y + 1) };
                    break;
            }
            g.FillPolygon(brush, points);
        }

        public void RenderBackgroundInternal(Graphics g, Rectangle rect, Color baseColor, Color borderColor, float basePosition, bool drawBorder, LinearGradientMode mode)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Transparent, Color.Transparent, mode))
            {
                Color[] colors = new Color[] { this.GetColor(baseColor, 0, 0x23, 0x18, 9), this.GetColor(baseColor, 0, 13, 8, 3), baseColor, this.GetColor(baseColor, 0, 0x44, 0x45, 0x36) };
                ColorBlend blend = new ColorBlend();
                float[] CS_0_0000 = new float[4];
                CS_0_0000[1] = basePosition;
                CS_0_0000[2] = basePosition + 0.05f;
                CS_0_0000[3] = 1f;
                blend.Positions = CS_0_0000;
                blend.Colors = colors;
                brush.InterpolationColors = blend;
                g.FillRectangle(brush, rect);
            }
            if (baseColor.A > 80)
            {
                Rectangle rectTop = rect;
                if (mode == LinearGradientMode.Vertical)
                {
                    rectTop.Height = (int) (rectTop.Height * basePosition);
                }
                else
                {
                    rectTop.Width = (int) (rect.Width * basePosition);
                }
                using (SolidBrush brushAlpha = new SolidBrush(Color.FromArgb(80, 0xff, 0xff, 0xff)))
                {
                    g.FillRectangle(brushAlpha, rectTop);
                }
            }
            if (drawBorder)
            {
                using (Pen pen = new Pen(borderColor))
                {
                    g.DrawRectangle(pen, rect);
                }
            }
        }

        public void RenderButton(Graphics g, Rectangle rect, Color baseColor, Color borderColor, Color arrowColor, ArrowDirection direction)
        {
            this.RenderBackgroundInternal(g, rect, baseColor, borderColor, 0.45f, true, LinearGradientMode.Vertical);
            using (SolidBrush brush = new SolidBrush(arrowColor))
            {
                this.RenderArrowInternal(g, rect, direction, brush);
            }
        }

        protected override void WndProc(ref Message m)
        {
            int CS_0_0000 = m.Msg;
            if (CS_0_0000 != 15)
            {
                if (CS_0_0000 != 0x85)
                {
                    base.WndProc(ref m);
                    return;
                }
            }
            else
            {
                base.WndProc(ref m);
                if (base.BorderStyle == BorderStyle.None)
                {
                    return;
                }
                Color borderColor = base.Enabled ? this._borderColor : SystemColors.ControlDark;
                using (Graphics g = Graphics.FromHwnd(m.HWnd))
                {
                    ControlPaint.DrawBorder(g, base.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
                    return;
                }
            }
            if (base.BorderStyle != BorderStyle.None)
            {
                Color backColor = base.Enabled ? base.BackColor : SystemColors.Control;
                Rectangle rect = new Rectangle(0, 0, base.Width, base.Height);
                IntPtr hdc = CCWin.Win32.NativeMethods.GetWindowDC(m.HWnd);
                if (hdc == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
                try
                {
                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        using (Brush brush = new SolidBrush(backColor))
                        {
                            g.FillRectangle(brush, rect);
                        }
                    }
                }
                finally
                {
                    CCWin.Win32.NativeMethods.ReleaseDC(m.HWnd, hdc);
                }
            }
            m.Result = IntPtr.Zero;
        }

        [DefaultValue(typeof(Color), "0, 95, 152"), Description("箭头颜色"), Category("Skin")]
        public Color ArrowColor
        {
            get
            {
                return this._arrowColor;
            }
            set
            {
                this._arrowColor = value;
                this.UpDownButton.Invalidate();
            }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                base.Invalidate(true);
            }
        }

        [DefaultValue(typeof(Color), "166, 222, 255"), Category("Skin"), Description("色调")]
        public Color BaseColor
        {
            get
            {
                return this._baseColor;
            }
            set
            {
                this._baseColor = value;
                this.UpDownButton.Invalidate();
            }
        }

        [Description("边框颜色"), Category("Skin"), DefaultValue(typeof(Color), "23, 169, 254")]
        public Color BorderColor
        {
            get
            {
                return this._borderColor;
            }
            set
            {
                this._borderColor = value;
                base.Invalidate(true);
            }
        }

        public Control UpDownButton
        {
            get
            {
                return base.Controls[0];
            }
        }

        private class UpDownButtonNativeWindow : NativeWindow, IDisposable
        {
            private bool _bPainting;
            private SkinNumericUpDown _owner;
            private Control _upDownButton;
            private IntPtr _upDownButtonWnd;
            private static readonly IntPtr TRUE = new IntPtr(1);
            private const int VK_LBUTTON = 1;
            private const int VK_RBUTTON = 2;
            private const int WM_PAINT = 15;

            public UpDownButtonNativeWindow(SkinNumericUpDown owner)
            {
                this._owner = owner;
                this._upDownButton = owner.UpDownButton;
                this._upDownButtonWnd = this._upDownButton.Handle;
                if ((Environment.OSVersion.Version.Major > 5) && CCWin.Win32.NativeMethods.IsAppThemed())
                {
                    CCWin.Win32.NativeMethods.SetWindowTheme(this._upDownButtonWnd, "", "");
                }
                base.AssignHandle(this._upDownButtonWnd);
            }

            public void Dispose()
            {
                this._owner = null;
                this._upDownButton = null;
                base.ReleaseHandle();
            }

            private void DrawUpDownButton()
            {
                bool mouseOver = false;
                bool mousePress = this.LeftKeyPressed();
                bool mouseInUpButton = false;
                Rectangle clipRect = this._upDownButton.ClientRectangle;
                CCWin.Win32.Struct.RECT windowRect = new CCWin.Win32.Struct.RECT();
                CCWin.Win32.NativeMethods.Point cursorPoint = new CCWin.Win32.NativeMethods.Point();
                CCWin.Win32.NativeMethods.GetCursorPos(ref cursorPoint);
                CCWin.Win32.NativeMethods.GetWindowRect(this._upDownButtonWnd, ref windowRect);
                mouseOver = CCWin.Win32.NativeMethods.PtInRect(ref windowRect, cursorPoint);
                cursorPoint.x -= windowRect.Left;
                cursorPoint.y -= windowRect.Top;
                mouseInUpButton = cursorPoint.y < (clipRect.Height / 2);
                using (Graphics g = Graphics.FromHwnd(this._upDownButtonWnd))
                {
                    UpDownButtonPaintEventArgs e = new UpDownButtonPaintEventArgs(g, clipRect, mouseOver, mousePress, mouseInUpButton);
                    this._owner.OnPaintUpDownButton(e);
                }
            }

            private bool LeftKeyPressed()
            {
                if (SystemInformation.MouseButtonsSwapped)
                {
                    return (CCWin.Win32.NativeMethods.GetKeyState(2) < 0);
                }
                return (CCWin.Win32.NativeMethods.GetKeyState(1) < 0);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 15)
                {
                    if (!this._bPainting)
                    {
                        this._bPainting = true;
                        CCWin.Win32.Struct.PAINTSTRUCT ps = new CCWin.Win32.Struct.PAINTSTRUCT();
                        CCWin.Win32.NativeMethods.BeginPaint(m.HWnd, ref ps);
                        this.DrawUpDownButton();
                        CCWin.Win32.NativeMethods.EndPaint(m.HWnd, ref ps);
                        this._bPainting = false;
                        m.Result = TRUE;
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
        }
    }
}

