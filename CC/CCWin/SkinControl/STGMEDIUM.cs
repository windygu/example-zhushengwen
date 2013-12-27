namespace CCWin.SkinControl
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct STGMEDIUM
    {
        public int tymed;
        public IntPtr unionmember;
        public IntPtr pUnkForRelease;
    }
}

