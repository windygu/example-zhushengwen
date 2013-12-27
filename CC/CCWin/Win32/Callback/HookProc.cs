namespace CCWin.Win32.Callback
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate int HookProc(int ncode, IntPtr wParam, IntPtr lParam);
}

