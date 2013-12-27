namespace CCWin.SkinControl
{
    using CCWin.SkinClass;
    using CCWin.Win32;
    using CCWin.Win32.Struct;
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class ScrollBarManager : NativeWindow, IDisposable
    {
        private bool _bPainting;
        private bool _disposed;
        private ScrollBarHistTest _lastMouseDownHistTest;
        private ScrollBarMaskControl _maskControl;
        private ScrollBar _owner;

        internal ScrollBarManager(ScrollBar owner)
        {
            this._owner = owner;
            this.CreateHandle();
        }

        private void CalculateRect(IntPtr scrollBarHWnd, bool bHorizontal, out Rectangle bounds, out Rectangle trackRect, out Rectangle topLeftArrowRect, out Rectangle bottomRightArrowRect, out Rectangle thumbRect)
        {
            CCWin.Win32.Struct.RECT rect = new CCWin.Win32.Struct.RECT();
            CCWin.Win32.NativeMethods.GetWindowRect(scrollBarHWnd, ref rect);
            CCWin.Win32.NativeMethods.OffsetRect(ref rect, -rect.Left, -rect.Top);
            int arrowWidth = bHorizontal ? this.ArrowCx : this.ArrowCy;
            bounds = rect.Rect;
            System.Drawing.Point point = this.GetScrollBarThumb(bounds, bHorizontal, arrowWidth);
            trackRect = bounds;
            if (bHorizontal)
            {
                topLeftArrowRect = new Rectangle(0, 0, arrowWidth, bounds.Height);
                bottomRightArrowRect = new Rectangle(bounds.Width - arrowWidth, 0, arrowWidth, bounds.Height);
                if (this._owner.RightToLeft == RightToLeft.Yes)
                {
                    thumbRect = new Rectangle(point.Y, 0, point.X - point.Y, bounds.Height);
                }
                else
                {
                    thumbRect = new Rectangle(point.X, 0, point.Y - point.X, bounds.Height);
                }
            }
            else
            {
                topLeftArrowRect = new Rectangle(0, 0, bounds.Width, arrowWidth);
                bottomRightArrowRect = new Rectangle(0, bounds.Height - arrowWidth, bounds.Width, arrowWidth);
                thumbRect = new Rectangle(0, point.X, bounds.Width, point.Y - point.X);
            }
        }

        protected void CreateHandle()
        {
            base.AssignHandle(this.OwnerHWnd);
            this._maskControl = new ScrollBarMaskControl(this);
            this._maskControl.OnCreateHandle();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (this._maskControl != null)
                    {
                        this._maskControl.Dispose();
                        this._maskControl = null;
                    }
                    this._owner = null;
                }
                this.ReleaseHandleInternal();
            }
            this._disposed = true;
        }

        private void DrawScrollBar(IntPtr scrollBarHWnd, IntPtr maskHWnd)
        {
            this.DrawScrollBar(scrollBarHWnd, maskHWnd, false, false);
        }

        private void DrawScrollBar(ControlState topLeftArrowState, ControlState bottomRightArrowState, ControlState thumbState)
        {
            Rectangle bounds;
            Rectangle trackRect;
            Rectangle topLeftArrowRect;
            Rectangle bottomRightArrowRect;
            Rectangle thumbRect;
            Orientation direction = this.Direction;
            bool bHorizontal = direction == Orientation.Horizontal;
            this.CalculateRect(this.OwnerHWnd, bHorizontal, out bounds, out trackRect, out topLeftArrowRect, out bottomRightArrowRect, out thumbRect);
            this.DrawScrollBar(this._maskControl.Handle, bounds, trackRect, topLeftArrowRect, bottomRightArrowRect, thumbRect, topLeftArrowState, bottomRightArrowState, thumbState, direction);
        }

        private void DrawScrollBar(IntPtr scrollBarHWnd, IntPtr maskHWnd, bool sbm, bool styleChanged)
        {
            Rectangle bounds;
            Rectangle trackRect;
            Rectangle topLeftArrowRect;
            Rectangle bottomRightArrowRect;
            Rectangle thumbRect;
            ControlState topLeftArrowState;
            ControlState bottomRightArrowState;
            ControlState thumbState;
            ScrollBarHistTest histTest;
            Orientation direction = this.Direction;
            bool bHorizontal = direction == Orientation.Horizontal;
            this.CalculateRect(scrollBarHWnd, bHorizontal, out bounds, out trackRect, out topLeftArrowRect, out bottomRightArrowRect, out thumbRect);
            this.GetState(scrollBarHWnd, bHorizontal, out histTest, out topLeftArrowState, out bottomRightArrowState, out thumbState);
            if (sbm)
            {
                if (histTest == ScrollBarHistTest.None)
                {
                    thumbState = ControlState.Pressed;
                }
                else if (this._lastMouseDownHistTest == ScrollBarHistTest.Track)
                {
                    thumbState = ControlState.Normal;
                }
            }
            if (styleChanged)
            {
                thumbState = ControlState.Normal;
            }
            this.DrawScrollBar(maskHWnd, bounds, trackRect, topLeftArrowRect, bottomRightArrowRect, thumbRect, topLeftArrowState, bottomRightArrowState, thumbState, direction);
        }

        private void DrawScrollBar(IntPtr maskHWnd, Rectangle bounds, Rectangle trackRect, Rectangle topLeftArrowRect, Rectangle bottomRightArrowRect, Rectangle thumbRect, ControlState topLeftArrowState, ControlState bottomRightArrowState, ControlState thumbState, Orientation direction)
        {
            bool bHorizontal = direction == Orientation.Horizontal;
            bool bEnabled = this._owner.Enabled;
            IScrollBarPaint paint = this._owner as IScrollBarPaint;
            if (paint != null)
            {
                ImageDc tempDc = new ImageDc(bounds.Width, bounds.Height);
                IntPtr hdc = CCWin.Win32.NativeMethods.GetDC(maskHWnd);
                try
                {
                    using (Graphics g = Graphics.FromHdc(tempDc.Hdc))
                    {
                        using (PaintScrollBarTrackEventArgs te = new PaintScrollBarTrackEventArgs(g, trackRect, direction, bEnabled))
                        {
                            paint.OnPaintScrollBarTrack(te);
                        }
                        ArrowDirection arrowDirection = bHorizontal ? ArrowDirection.Left : ArrowDirection.Up;
                        using (PaintScrollBarArrowEventArgs te = new PaintScrollBarArrowEventArgs(g, topLeftArrowRect, topLeftArrowState, arrowDirection, direction, bEnabled))
                        {
                            paint.OnPaintScrollBarArrow(te);
                        }
                        arrowDirection = bHorizontal ? ArrowDirection.Right : ArrowDirection.Down;
                        using (PaintScrollBarArrowEventArgs te = new PaintScrollBarArrowEventArgs(g, bottomRightArrowRect, bottomRightArrowState, arrowDirection, direction, bEnabled))
                        {
                            paint.OnPaintScrollBarArrow(te);
                        }
                        using (PaintScrollBarThumbEventArgs te = new PaintScrollBarThumbEventArgs(g, thumbRect, thumbState, direction, bEnabled))
                        {
                            paint.OnPaintScrollBarThumb(te);
                        }
                    }
                    CCWin.Win32.NativeMethods.BitBlt(hdc, 0, 0, bounds.Width, bounds.Height, tempDc.Hdc, 0, 0, 0xcc0020);
                }
                finally
                {
                    CCWin.Win32.NativeMethods.ReleaseDC(maskHWnd, hdc);
                    tempDc.Dispose();
                }
            }
        }

        ~ScrollBarManager()
        {
            this.Dispose(false);
        }

        private SCROLLBARINFO GetScrollBarInfo(IntPtr hWnd)
        {
            SCROLLBARINFO sbi = new SCROLLBARINFO();
            sbi.cbSize = Marshal.SizeOf(sbi);
            CCWin.Win32.NativeMethods.SendMessage(hWnd, 0xeb, 0, ref sbi);
            return sbi;
        }

        private SCROLLBARINFO GetScrollBarInfo(IntPtr hWnd, uint objid)
        {
            SCROLLBARINFO sbi = new SCROLLBARINFO();
            sbi.cbSize = Marshal.SizeOf(sbi);
            CCWin.Win32.NativeMethods.GetScrollBarInfo(hWnd, objid, ref sbi);
            return sbi;
        }

        private System.Drawing.Point GetScrollBarThumb()
        {
            bool bHorizontal = this.Direction == Orientation.Horizontal;
            int arrowWidth = bHorizontal ? this.ArrowCx : this.ArrowCy;
            return this.GetScrollBarThumb(this._owner.ClientRectangle, bHorizontal, arrowWidth);
        }

        private System.Drawing.Point GetScrollBarThumb(Rectangle rect, bool bHorizontal, int arrowWidth)
        {
            int width;
            ScrollBar scrollBar = this._owner;
            System.Drawing.Point point = new System.Drawing.Point();
            if (bHorizontal)
            {
                width = rect.Width - (arrowWidth * 2);
            }
            else
            {
                width = rect.Height - (arrowWidth * 2);
            }
            int value = ((scrollBar.Maximum - scrollBar.Minimum) - scrollBar.LargeChange) + 1;
            float thumbWidth = ((float) width) / ((((float) value) / ((float) scrollBar.LargeChange)) + 1f);
            if (thumbWidth < 8f)
            {
                thumbWidth = 8f;
            }
            if (value != 0)
            {
                int curValue = scrollBar.Value - scrollBar.Minimum;
                if (curValue > value)
                {
                    curValue = value;
                }
                point.X = (int) (curValue * ((width - thumbWidth) / ((float) value)));
            }
            point.X += arrowWidth;
            point.Y = point.X + ((int) Math.Ceiling((double) thumbWidth));
            if (bHorizontal && (scrollBar.RightToLeft == RightToLeft.Yes))
            {
                point.X = scrollBar.Width - point.X;
                point.Y = scrollBar.Width - point.Y;
            }
            return point;
        }

        private void GetState(IntPtr scrollBarHWnd, bool bHorizontal, out ScrollBarHistTest histTest, out ControlState topLeftArrowState, out ControlState bottomRightArrowState, out ControlState thumbState)
        {
            histTest = this.ScrollBarHitTest(scrollBarHWnd);
            bool bLButtonDown = Helper.LeftKeyPressed();
            bool bEnabled = this._owner.Enabled;
            topLeftArrowState = ControlState.Normal;
            bottomRightArrowState = ControlState.Normal;
            thumbState = ControlState.Normal;
            switch (histTest)
            {
                case ScrollBarHistTest.TopArrow:
                case ScrollBarHistTest.LeftArrow:
                    if (!bEnabled)
                    {
                        break;
                    }
                    topLeftArrowState = bLButtonDown ? ControlState.Pressed : ControlState.Hover;
                    return;

                case ScrollBarHistTest.BottomArrow:
                case ScrollBarHistTest.RightArrow:
                    if (!bEnabled)
                    {
                        break;
                    }
                    bottomRightArrowState = bLButtonDown ? ControlState.Pressed : ControlState.Hover;
                    return;

                case ScrollBarHistTest.Thumb:
                    if (bEnabled)
                    {
                        thumbState = bLButtonDown ? ControlState.Pressed : ControlState.Hover;
                    }
                    break;

                default:
                    return;
            }
        }

        private void InvalidateWindow(bool messaged)
        {
            this.InvalidateWindow(this.OwnerHWnd, messaged);
        }

        private void InvalidateWindow(IntPtr hWnd, bool messaged)
        {
            if (messaged)
            {
                CCWin.Win32.NativeMethods.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, 2);
            }
            else
            {
                CCWin.Win32.NativeMethods.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, 0x101);
            }
        }

        internal void ReleaseHandleInternal()
        {
            if (base.Handle != IntPtr.Zero)
            {
                base.ReleaseHandle();
            }
        }

        private ScrollBarHistTest ScrollBarHitTest(IntPtr hWnd)
        {
            CCWin.Win32.NativeMethods.Point point = new CCWin.Win32.NativeMethods.Point();
            CCWin.Win32.Struct.RECT rect = new CCWin.Win32.Struct.RECT();
            System.Drawing.Point thumbPoint = this.GetScrollBarThumb();
            int arrowCx = this.ArrowCx;
            int arrowCy = this.ArrowCy;
            CCWin.Win32.NativeMethods.GetWindowRect(hWnd, ref rect);
            CCWin.Win32.NativeMethods.OffsetRect(ref rect, -rect.Left, -rect.Top);
            CCWin.Win32.Struct.RECT tp = rect;
            CCWin.Win32.NativeMethods.GetCursorPos(ref point);
            CCWin.Win32.NativeMethods.ScreenToClient(hWnd, ref point);
            if (this.Direction == Orientation.Horizontal)
            {
                if (CCWin.Win32.NativeMethods.PtInRect(ref rect, point))
                {
                    tp.Right = arrowCx;
                    if (CCWin.Win32.NativeMethods.PtInRect(ref tp, point))
                    {
                        return ScrollBarHistTest.LeftArrow;
                    }
                    tp.Left = rect.Right - arrowCx;
                    tp.Right = rect.Right;
                    if (CCWin.Win32.NativeMethods.PtInRect(ref tp, point))
                    {
                        return ScrollBarHistTest.RightArrow;
                    }
                    if (this._owner.RightToLeft == RightToLeft.Yes)
                    {
                        tp.Left = point.y;
                        tp.Right = point.x;
                    }
                    else
                    {
                        tp.Left = thumbPoint.X;
                        tp.Right = thumbPoint.Y;
                    }
                    if (CCWin.Win32.NativeMethods.PtInRect(ref tp, point))
                    {
                        return ScrollBarHistTest.Thumb;
                    }
                    return ScrollBarHistTest.Track;
                }
            }
            else if (CCWin.Win32.NativeMethods.PtInRect(ref rect, point))
            {
                tp.Bottom = arrowCy;
                if (CCWin.Win32.NativeMethods.PtInRect(ref tp, point))
                {
                    return ScrollBarHistTest.TopArrow;
                }
                tp.Top = rect.Bottom - arrowCy;
                tp.Bottom = rect.Bottom;
                if (CCWin.Win32.NativeMethods.PtInRect(ref tp, point))
                {
                    return ScrollBarHistTest.BottomArrow;
                }
                tp.Top = thumbPoint.X;
                tp.Bottom = thumbPoint.Y;
                if (CCWin.Win32.NativeMethods.PtInRect(ref tp, point))
                {
                    return ScrollBarHistTest.Thumb;
                }
                return ScrollBarHistTest.Track;
            }
            return ScrollBarHistTest.None;
        }

        protected override void WndProc(ref Message m)
        {
            try
            {
                switch (m.Msg)
                {
                    case 0x200:
                    case 0x202:
                        this.DrawScrollBar(m.HWnd, this._maskControl.Handle);
                        base.WndProc(ref m);
                        return;

                    case 0x201:
                        this._lastMouseDownHistTest = this.ScrollBarHitTest(m.HWnd);
                        this.DrawScrollBar(m.HWnd, this._maskControl.Handle);
                        base.WndProc(ref m);
                        return;

                    case 0x2a3:
                        this.DrawScrollBar(m.HWnd, this._maskControl.Handle);
                        base.WndProc(ref m);
                        return;

                    case 0xe9:
                        this.DrawScrollBar(m.HWnd, this._maskControl.Handle, true, false);
                        base.WndProc(ref m);
                        return;

                    case 15:
                        if (!this._bPainting)
                        {
                            CCWin.Win32.Struct.PAINTSTRUCT ps = new CCWin.Win32.Struct.PAINTSTRUCT();
                            this._bPainting = true;
                            CCWin.Win32.NativeMethods.BeginPaint(m.HWnd, ref ps);
                            this.DrawScrollBar(m.HWnd, this._maskControl.Handle);
                            CCWin.Win32.NativeMethods.ValidateRect(m.HWnd, ref ps.rcPaint);
                            CCWin.Win32.NativeMethods.EndPaint(m.HWnd, ref ps);
                            this._bPainting = false;
                            m.Result = Result.TRUE;
                        }
                        else
                        {
                            base.WndProc(ref m);
                        }
                        return;

                    case 0x47:
                    {
                        CCWin.Win32.Struct.WINDOWPOS pos = (CCWin.Win32.Struct.WINDOWPOS) Marshal.PtrToStructure(m.LParam, typeof(CCWin.Win32.Struct.WINDOWPOS));
                        bool hide = (pos.flags & 0x80) != 0;
                        bool show = (pos.flags & 0x40) != 0;
                        if (hide)
                        {
                            this._maskControl.SetVisibale(false);
                        }
                        else if (show)
                        {
                            this._maskControl.SetVisibale(true);
                        }
                        this._maskControl.CheckBounds(m.HWnd);
                        base.WndProc(ref m);
                        return;
                    }
                    case 0x7d:
                        this.DrawScrollBar(m.HWnd, this._maskControl.Handle, false, true);
                        base.WndProc(ref m);
                        return;
                }
                base.WndProc(ref m);
            }
            catch
            {
            }
        }

        private int ArrowCx
        {
            get
            {
                return SystemInformation.HorizontalScrollBarArrowWidth;
            }
        }

        private int ArrowCy
        {
            get
            {
                return SystemInformation.VerticalScrollBarArrowHeight;
            }
        }

        private Orientation Direction
        {
            get
            {
                if (this._owner is HScrollBar)
                {
                    return Orientation.Horizontal;
                }
                return Orientation.Vertical;
            }
        }

        private IntPtr OwnerHWnd
        {
            get
            {
                return this._owner.Handle;
            }
        }

        private class ScrollBarMaskControl : MaskControlBase
        {
            private ScrollBarManager _owner;

            public ScrollBarMaskControl(ScrollBarManager owner) : base(owner.OwnerHWnd)
            {
                this._owner = owner;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this._owner = null;
                }
                base.Dispose(disposing);
            }

            protected override void OnPaint(IntPtr hWnd)
            {
                this._owner.DrawScrollBar(this._owner.OwnerHWnd, hWnd);
            }
        }
    }
}

