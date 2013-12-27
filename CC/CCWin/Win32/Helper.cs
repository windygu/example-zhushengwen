namespace CCWin.Win32
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public static class Helper
    {
        public static int HIWORD(int n)
        {
            return ((n >> 0x10) & 0xffff);
        }

        public static int HIWORD(IntPtr n)
        {
            return HIWORD((int) ((long) n));
        }

        public static bool LeftKeyPressed()
        {
            if (SystemInformation.MouseButtonsSwapped)
            {
                return (CCWin.Win32.NativeMethods.GetKeyState(2) < 0);
            }
            return (CCWin.Win32.NativeMethods.GetKeyState(1) < 0);
        }

        public static int LOWORD(int n)
        {
            return (n & 0xffff);
        }

        public static int LOWORD(IntPtr n)
        {
            return LOWORD((int) ((long) n));
        }

        public static int MAKELONG(int low, int high)
        {
            return ((high << 0x10) | (low & 0xffff));
        }

        public static IntPtr MAKELPARAM(int low, int high)
        {
            return (IntPtr) ((high << 0x10) | (low & 0xffff));
        }

        public static void SetRedraw(IntPtr hWnd, bool redraw)
        {
            IntPtr ptr = redraw ? Result.TRUE : Result.FALSE;
            CCWin.Win32.NativeMethods.SendMessage(hWnd, 11, ptr, 0);
        }

        public static int SignedHIWORD(int n)
        {
            return (short) ((n >> 0x10) & 0xffff);
        }

        public static int SignedHIWORD(IntPtr n)
        {
            return SignedHIWORD((int) ((long) n));
        }

        public static int SignedLOWORD(int n)
        {
            return (short) (n & 0xffff);
        }

        public static int SignedLOWORD(IntPtr n)
        {
            return SignedLOWORD((int) ((long) n));
        }

        public static void Swap(ref int x, ref int y)
        {
            int tmp = x;
            x = y;
            y = tmp;
        }

        public static IntPtr ToIntPtr(object structure)
        {
            IntPtr lparam = IntPtr.Zero;
            lparam = Marshal.AllocCoTaskMem(Marshal.SizeOf(structure));
            Marshal.StructureToPtr(structure, lparam, false);
            return lparam;
        }
    }
}

