namespace CCWin.SkinControl
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public sealed class tagOLEVERB
    {
        [MarshalAs(UnmanagedType.I4)]
        public int lVerb;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpszVerbName;
        [MarshalAs(UnmanagedType.U4)]
        public int fuFlags;
        [MarshalAs(UnmanagedType.U4)]
        public int grfAttribs;
    }
}

