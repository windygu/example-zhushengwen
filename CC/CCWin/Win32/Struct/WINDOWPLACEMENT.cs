namespace CCWin.Win32.Struct
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public Point ptMinPosition;
        public Point ptMaxPosition;
        public CCWin.Win32.Struct.RECT rcNormalPosition;
        public static WINDOWPLACEMENT Default
        {
            get
            {
                WINDOWPLACEMENT structure = new WINDOWPLACEMENT();
                structure.length = Marshal.SizeOf(structure);
                return structure;
            }
        }
    }
}

