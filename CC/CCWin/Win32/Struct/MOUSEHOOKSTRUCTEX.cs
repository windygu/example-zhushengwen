namespace CCWin.Win32.Struct
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public class MOUSEHOOKSTRUCTEX
    {
        public MOUSEHOOKSTRUCT Mouse;
        public int mouseData;
    }
}

