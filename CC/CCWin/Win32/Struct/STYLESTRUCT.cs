namespace CCWin.Win32.Struct
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct STYLESTRUCT
    {
        public int styleOld;
        public int styleNew;
    }
}

