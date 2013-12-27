namespace CCWin.SkinControl
{
    using System;
    using System.Runtime.InteropServices;

    [Flags, ComVisible(false)]
    public enum REOOBJECTFLAGS : uint
    {
        REO_BELOWBASELINE = 2,
        REO_BLANK = 0x10,
        REO_DONTNEEDPALETTE = 0x20,
        REO_DYNAMICSIZE = 8,
        REO_GETMETAFILE = 0x400000,
        REO_HILITED = 0x1000000,
        REO_INPLACEACTIVE = 0x2000000,
        REO_INVERTEDSELECT = 4,
        REO_LINK = 0x80000000,
        REO_LINKAVAILABLE = 0x800000,
        REO_NULL = 0,
        REO_OPEN = 0x4000000,
        REO_READWRITEMASK = 0x3f,
        REO_RESIZABLE = 1,
        REO_SELECTED = 0x8000000,
        REO_STATIC = 0x40000000
    }
}

