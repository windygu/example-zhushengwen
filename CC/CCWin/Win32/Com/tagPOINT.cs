namespace CCWin.Win32.Com
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct tagPOINT
    {
        [MarshalAs(UnmanagedType.I4)]
        public int X;
        [MarshalAs(UnmanagedType.I4)]
        public int Y;
    }
}

