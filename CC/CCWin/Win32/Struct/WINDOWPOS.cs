namespace CCWin.Win32.Struct
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPOS
    {
        public IntPtr hWnd;
        public IntPtr hWndInsertAfter;
        public int x;
        public int y;
        public int cx;
        public int cy;
        public int flags;
    }
}

