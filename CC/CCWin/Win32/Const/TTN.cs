namespace CCWin.Win32.Const
{
    using System;
    using System.Runtime.InteropServices;

    public static class TTN
    {
        public const int TTN_FIRST = -520;
        public static readonly int TTN_GETDISPINFO;
        public const int TTN_GETDISPINFOA = -520;
        public const int TTN_GETDISPINFOW = -530;
        public const int TTN_LAST = -549;
        public const int TTN_LINKCLICK = -523;
        public static readonly int TTN_NEEDTEXT;
        public const int TTN_NEEDTEXTA = -520;
        public const int TTN_NEEDTEXTW = -530;
        public const int TTN_POP = -522;
        public const int TTN_SHOW = -521;

        static TTN()
        {
            if (Marshal.SystemDefaultCharSize != 1)
            {
                TTN_GETDISPINFO = -530;
                TTN_NEEDTEXT = -530;
            }
            else
            {
                TTN_GETDISPINFO = -520;
                TTN_NEEDTEXT = -520;
            }
        }
    }
}

