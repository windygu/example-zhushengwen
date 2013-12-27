namespace CCWin.Win32.Struct
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TCHITTESTINFO
    {
        public System.Drawing.Point Point;
        public int Flags;
        public TCHITTESTINFO(System.Drawing.Point location)
        {
            this.Point = location;
            this.Flags = 6;
        }
    }
}

