namespace CCWin.SkinControl
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public sealed class STATDATA
    {
        [MarshalAs(UnmanagedType.U4)]
        public int advf;
        [MarshalAs(UnmanagedType.U4)]
        public int dwConnection;
    }
}

