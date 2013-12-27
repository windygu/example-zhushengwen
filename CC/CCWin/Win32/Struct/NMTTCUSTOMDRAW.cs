namespace CCWin.Win32.Struct
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NMTTCUSTOMDRAW
    {
        public NMCUSTOMDRAW nmcd;
        public uint uDrawFlags;
    }
}

