namespace CCWin.SkinControl
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public sealed class tagLOGPALETTE
    {
        [MarshalAs(UnmanagedType.U2)]
        public short palVersion;
        [MarshalAs(UnmanagedType.U2)]
        public short palNumEntries;
    }
}

