namespace CCWin.SkinControl
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(false), Flags]
    public enum DVASPECT
    {
        DVASPECT_CONTENT = 1,
        DVASPECT_DOCPRINT = 8,
        DVASPECT_ICON = 4,
        DVASPECT_OPAQUE = 0x10,
        DVASPECT_THUMBNAIL = 2,
        DVASPECT_TRANSPARENT = 0x20
    }
}

