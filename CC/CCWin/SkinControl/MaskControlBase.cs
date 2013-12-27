namespace CCWin.SkinControl
{
    using CCWin.Win32;
    using CCWin.Win32.Const;
    using CCWin.Win32.Struct;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal abstract class MaskControlBase : NativeWindow, IDisposable
    {
        private System.Windows.Forms.CreateParams _createParams;
        private bool _disposed;

        protected MaskControlBase(IntPtr hWnd)
        {
            this.CreateParamsInternal(hWnd);
        }

        protected MaskControlBase(IntPtr hWnd, Rectangle rect)
        {
            this.CreateParamsInternal(hWnd, rect);
        }

        internal void CheckBounds(IntPtr hWnd)
        {
            CCWin.Win32.Struct.RECT controlRect = new CCWin.Win32.Struct.RECT();
            CCWin.Win32.Struct.RECT maskRect = new CCWin.Win32.Struct.RECT();
            CCWin.Win32.NativeMethods.GetWindowRect(base.Handle, ref maskRect);
            CCWin.Win32.NativeMethods.GetWindowRect(hWnd, ref controlRect);
            uint uFlag = 0x214;
            if (!CCWin.Win32.NativeMethods.EqualRect(ref controlRect, ref maskRect))
            {
                CCWin.Win32.NativeMethods.Point point = new CCWin.Win32.NativeMethods.Point(controlRect.Left, controlRect.Top);
                CCWin.Win32.NativeMethods.ScreenToClient(CCWin.Win32.NativeMethods.GetParent(base.Handle), ref point);
                CCWin.Win32.NativeMethods.SetWindowPos(base.Handle, IntPtr.Zero, point.x, point.y, controlRect.Right - controlRect.Left, controlRect.Bottom - controlRect.Top, uFlag);
            }
        }

        internal void CreateParamsInternal(IntPtr hWnd)
        {
            IntPtr hParent = CCWin.Win32.NativeMethods.GetParent(hWnd);
            CCWin.Win32.Struct.RECT rect = new CCWin.Win32.Struct.RECT();
            CCWin.Win32.NativeMethods.Point point = new CCWin.Win32.NativeMethods.Point();
            CCWin.Win32.NativeMethods.GetWindowRect(hWnd, ref rect);
            point.x = rect.Left;
            point.y = rect.Top;
            CCWin.Win32.NativeMethods.ScreenToClient(hParent, ref point);
            int dwStyle = 0x5400000d;
            int dwExStyle = 0x88;
            this._createParams = new System.Windows.Forms.CreateParams();
            this._createParams.Parent = hParent;
            this._createParams.ClassName = "STATIC";
            this._createParams.Caption = null;
            this._createParams.Style = dwStyle;
            this._createParams.ExStyle = dwExStyle;
            this._createParams.X = point.x;
            this._createParams.Y = point.y;
            this._createParams.Width = rect.Right - rect.Left;
            this._createParams.Height = rect.Bottom - rect.Top;
        }

        internal void CreateParamsInternal(IntPtr hWnd, Rectangle rect)
        {
            IntPtr hParent = CCWin.Win32.NativeMethods.GetParent(hWnd);
            int dwStyle = 0x5400000d;
            int dwExStyle = 0x88;
            this._createParams = new System.Windows.Forms.CreateParams();
            this._createParams.Parent = hParent;
            this._createParams.ClassName = "STATIC";
            this._createParams.Caption = null;
            this._createParams.Style = dwStyle;
            this._createParams.ExStyle = dwExStyle;
            this._createParams.X = rect.X;
            this._createParams.Y = rect.Y;
            this._createParams.Width = rect.Width;
            this._createParams.Height = rect.Height;
        }

        internal void DestroyHandleInternal()
        {
            if (this.IsHandleCreated)
            {
                base.DestroyHandle();
            }
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
                    this._createParams = null;
                }
                this.DestroyHandleInternal();
            }
            this._disposed = true;
        }

        ~MaskControlBase()
        {
            this.Dispose(false);
        }

        protected internal void OnCreateHandle()
        {
            base.CreateHandle(this.CreateParams);
            this.SetZorder();
        }

        protected virtual void OnPaint(IntPtr hWnd)
        {
        }

        internal void SetVisibale(bool visibale)
        {
            if (visibale)
            {
                CCWin.Win32.NativeMethods.ShowWindow(base.Handle, 1);
            }
            else
            {
                CCWin.Win32.NativeMethods.ShowWindow(base.Handle, 0);
            }
        }

        internal void SetVisibale(IntPtr hWnd)
        {
            bool bVisible = (CCWin.Win32.NativeMethods.GetWindowLong(hWnd, -16) & 0x10000000) == 0x10000000;
            this.SetVisibale(bVisible);
        }

        private void SetZorder()
        {
            uint uFlags = 0x213;
            CCWin.Win32.NativeMethods.SetWindowPos(base.Handle, HWND.HWND_TOP, 0, 0, 0, 0, uFlags);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            try
            {
                switch (m.Msg)
                {
                    case 15:
                    case 0x85:
                        this.OnPaint(m.HWnd);
                        break;
                }
            }
            catch
            {
            }
        }

        protected virtual System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                return this._createParams;
            }
        }

        protected bool IsHandleCreated
        {
            get
            {
                return (base.Handle != IntPtr.Zero);
            }
        }
    }
}

