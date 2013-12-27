namespace CCWin.Win32.Struct
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct INITCOMMONCONTROLSEX
    {
        public int dwSize;
        public int dwICC;
        public INITCOMMONCONTROLSEX(int flags)
        {
            this.dwSize = Marshal.SizeOf(typeof(INITCOMMONCONTROLSEX));
            this.dwICC = flags;
        }
    }
}

