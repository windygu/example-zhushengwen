namespace CCWin.SkinClass
{
    using CCWin.Win32;
    using System;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
    public class ImageDc : IDisposable
    {
        private int _height;
        private IntPtr _pBmp;
        private IntPtr _pBmpOld;
        private IntPtr _pHdc;
        private int _width;

        public ImageDc(int width, int height)
        {
            this._pHdc = IntPtr.Zero;
            this._pBmp = IntPtr.Zero;
            this._pBmpOld = IntPtr.Zero;
            this.CreateImageDc(width, height, IntPtr.Zero);
        }

        public ImageDc(int width, int height, IntPtr hBmp)
        {
            this._pHdc = IntPtr.Zero;
            this._pBmp = IntPtr.Zero;
            this._pBmpOld = IntPtr.Zero;
            this.CreateImageDc(width, height, hBmp);
        }

        private void CreateImageDc(int width, int height, IntPtr hBmp)
        {
            IntPtr pHdc = IntPtr.Zero;
            pHdc = NativeMethods.CreateDCA("DISPLAY", "", "", 0);
            this._pHdc = NativeMethods.CreateCompatibleDC(pHdc);
            if (hBmp != IntPtr.Zero)
            {
                this._pBmp = hBmp;
            }
            else
            {
                this._pBmp = NativeMethods.CreateCompatibleBitmap(pHdc, width, height);
            }
            this._pBmpOld = NativeMethods.SelectObject(this._pHdc, this._pBmp);
            if (this._pBmpOld == IntPtr.Zero)
            {
                this.ImageDestroy();
            }
            else
            {
                this._width = width;
                this._height = height;
            }
            NativeMethods.DeleteDC(pHdc);
            pHdc = IntPtr.Zero;
        }

        public void Dispose()
        {
            this.ImageDestroy();
        }

        private void ImageDestroy()
        {
            if (this._pBmpOld != IntPtr.Zero)
            {
                NativeMethods.SelectObject(this._pHdc, this._pBmpOld);
                this._pBmpOld = IntPtr.Zero;
            }
            if (this._pBmp != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(this._pBmp);
                this._pBmp = IntPtr.Zero;
            }
            if (this._pHdc != IntPtr.Zero)
            {
                NativeMethods.DeleteDC(this._pHdc);
                this._pHdc = IntPtr.Zero;
            }
        }

        public IntPtr HBmp
        {
            get
            {
                return this._pBmp;
            }
        }

        public IntPtr Hdc
        {
            get
            {
                return this._pHdc;
            }
        }
    }
}

