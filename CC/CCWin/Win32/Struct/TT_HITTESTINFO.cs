namespace CCWin.Win32.Struct
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TT_HITTESTINFO
    {
        public IntPtr hwnd;
        public POINT pt;
        public TOOLINFO ti;
    }
}

