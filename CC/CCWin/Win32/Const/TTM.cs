namespace CCWin.Win32.Const
{
    using System;
    using System.Runtime.InteropServices;

    public static class TTM
    {
        public const int TTM_ACTIVATE = 0x401;
        public static readonly int TTM_ADDTOOL;
        public const int TTM_ADDTOOLA = 0x404;
        public const int TTM_ADDTOOLW = 0x432;
        public const int TTM_ADJUSTRECT = 0x41f;
        public static readonly int TTM_DELTOOL;
        public const int TTM_DELTOOLA = 0x405;
        public const int TTM_DELTOOLW = 0x433;
        public static readonly int TTM_ENUMTOOLS;
        public const int TTM_ENUMTOOLSA = 0x40e;
        public const int TTM_ENUMTOOLSW = 0x43a;
        public static readonly int TTM_GETCURRENTTOOL;
        public const int TTM_GETCURRENTTOOLA = 0x40f;
        public const int TTM_GETCURRENTTOOLW = 0x43b;
        public const int TTM_GETDELAYTIME = 0x415;
        public const int TTM_GETMARGIN = 0x41b;
        public const int TTM_GETMAXTIPWIDTH = 0x419;
        public static readonly int TTM_GETTEXT;
        public const int TTM_GETTEXTA = 0x40b;
        public const int TTM_GETTEXTW = 0x438;
        public const int TTM_GETTIPBKCOLOR = 0x416;
        public const int TTM_GETTIPTEXTCOLOR = 0x417;
        public const int TTM_GETTOOLCOUNT = 0x40d;
        public static readonly int TTM_GETTOOLINFO;
        public const int TTM_GETTOOLINFOA = 0x408;
        public const int TTM_GETTOOLINFOW = 0x435;
        public static readonly int TTM_HITTEST;
        public const int TTM_HITTESTA = 0x40a;
        public const int TTM_HITTESTW = 0x437;
        public static readonly int TTM_NEWTOOLRECT;
        public const int TTM_NEWTOOLRECTA = 0x406;
        public const int TTM_NEWTOOLRECTW = 0x434;
        public const int TTM_POP = 0x41c;
        public const int TTM_POPUP = 0x422;
        public const int TTM_RELAYEVENT = 0x407;
        public const int TTM_SETDELAYTIME = 0x403;
        public const int TTM_SETMARGIN = 0x41a;
        public const int TTM_SETMAXTIPWIDTH = 0x418;
        public const int TTM_SETTIPBKCOLOR = 0x413;
        public const int TTM_SETTIPTEXTCOLOR = 0x414;
        public static readonly int TTM_SETTITLE;
        public const int TTM_SETTITLEA = 0x420;
        public const int TTM_SETTITLEW = 0x421;
        public static readonly int TTM_SETTOOLINFO;
        public const int TTM_SETTOOLINFOA = 0x409;
        public const int TTM_SETTOOLINFOW = 0x436;
        public const int TTM_TRACKACTIVATE = 0x411;
        public const int TTM_TRACKPOSITION = 0x412;
        public const int TTM_UPDATE = 0x41d;
        public static readonly int TTM_UPDATETIPTEXT;
        public const int TTM_UPDATETIPTEXTA = 0x40c;
        public const int TTM_UPDATETIPTEXTW = 0x439;
        public const int TTM_WINDOWFROMPOINT = 0x410;
        public const int WM_USER = 0x400;

        static TTM()
        {
            if (Marshal.SystemDefaultCharSize != 1)
            {
                TTM_ADDTOOL = 0x432;
                TTM_DELTOOL = 0x433;
                TTM_NEWTOOLRECT = 0x434;
                TTM_GETTOOLINFO = 0x435;
                TTM_SETTOOLINFO = 0x436;
                TTM_HITTEST = 0x437;
                TTM_GETTEXT = 0x438;
                TTM_UPDATETIPTEXT = 0x439;
                TTM_GETCURRENTTOOL = 0x43b;
                TTM_ENUMTOOLS = 0x43a;
                TTM_GETCURRENTTOOL = 0x43b;
                TTM_SETTITLE = 0x421;
            }
            else
            {
                TTM_ADDTOOL = 0x404;
                TTM_DELTOOL = 0x405;
                TTM_NEWTOOLRECT = 0x406;
                TTM_GETTOOLINFO = 0x408;
                TTM_SETTOOLINFO = 0x409;
                TTM_HITTEST = 0x40a;
                TTM_GETTEXT = 0x40b;
                TTM_UPDATETIPTEXT = 0x40c;
                TTM_GETCURRENTTOOL = 0x40f;
                TTM_ENUMTOOLS = 0x40e;
                TTM_GETCURRENTTOOL = 0x40f;
                TTM_SETTITLE = 0x420;
            }
        }
    }
}

