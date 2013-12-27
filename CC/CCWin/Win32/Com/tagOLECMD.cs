namespace CCWin.Win32.Com
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct tagOLECMD
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint cmdID;
        [MarshalAs(UnmanagedType.U4)]
        public uint cmdf;
    }
}

