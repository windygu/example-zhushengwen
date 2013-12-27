namespace CCWin.Win32.Const
{
    using System;

    public class TernaryRasterOperations
    {
        public const int BLACKNESS = 0x42;
        public const int DSTINVERT = 0x550009;
        public const int MERGECOPY = 0xc000ca;
        public const int MERGEPAINT = 0xbb0226;
        public const int NOTSRCCOPY = 0x330008;
        public const int NOTSRCERASE = 0x1100a6;
        public const int PATCOPY = 0xf00021;
        public const int PATINVERT = 0x5a0049;
        public const int PATPAINT = 0xfb0a09;
        public const int SRCAND = 0x8800c6;
        public const int SRCCOPY = 0xcc0020;
        public const int SRCERASE = 0x440328;
        public const int SRCINVERT = 0x660046;
        public const int SRCPAINT = 0xee0086;
        public const int WHITENESS = 0xff0062;

        private TernaryRasterOperations()
        {
        }
    }
}

