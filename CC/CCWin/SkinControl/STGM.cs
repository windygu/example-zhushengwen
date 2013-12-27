namespace CCWin.SkinControl
{
    using System;
    using System.Runtime.InteropServices;

    [Flags, ComVisible(false)]
    public enum STGM
    {
        STGM_CONVERT = 0x20000,
        STGM_CREATE = 0x1000,
        STGM_DELETEONRELEASE = 0x4000000,
        STGM_DIRECT = 0,
        STGM_FAILIFTHERE = 0,
        STGM_NOSCRATCH = 0x100000,
        STGM_NOSNAPSHOT = 0x200000,
        STGM_PRIORITY = 0x40000,
        STGM_READ = 0,
        STGM_READWRITE = 2,
        STGM_SHARE_DENY_NONE = 0x40,
        STGM_SHARE_DENY_READ = 0x30,
        STGM_SHARE_DENY_WRITE = 0x20,
        STGM_SHARE_EXCLUSIVE = 0x10,
        STGM_SIMPLE = 0x8000000,
        STGM_TRANSACTED = 0x10000,
        STGM_WRITE = 1
    }
}

