namespace CCWin.Win32.Const
{
    using System;

    public class RDW
    {
        public const int RDW_ALLCHILDREN = 0x80;
        public const int RDW_ERASE = 4;
        public const int RDW_ERASENOW = 0x200;
        public const int RDW_FRAME = 0x400;
        public const int RDW_INTERNALPAINT = 2;
        public const int RDW_INVALIDATE = 1;
        public const int RDW_NOCHILDREN = 0x40;
        public const int RDW_NOERASE = 0x20;
        public const int RDW_NOFRAME = 0x800;
        public const int RDW_NOINTERNALPAINT = 0x10;
        public const int RDW_UPDATENOW = 0x100;
        public const int RDW_VALIDATE = 8;

        private RDW()
        {
        }
    }
}

