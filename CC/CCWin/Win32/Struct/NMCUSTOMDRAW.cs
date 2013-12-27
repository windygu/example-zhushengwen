namespace CCWin.Win32.Struct
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct NMCUSTOMDRAW
    {
        public NMHDR hdr;
        public uint dwDrawStage;
        public IntPtr hdc;
        public RECT rc;
        public IntPtr dwItemSpec;
        public uint uItemState;
        public IntPtr lItemlParam;
    }
}

