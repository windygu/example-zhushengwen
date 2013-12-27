namespace CCWin.SkinControl
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class MouseHook
    {
        private GCHandle gc;
        private IntPtr hHook = IntPtr.Zero;
        public MHookEventHandler MHookEvent;
        private const int WH_MOUSE_LL = 14;
        private const uint WM_LBUTTONDOWN = 0x201;
        private const uint WM_LBUTTONUP = 0x202;
        private const uint WM_RBUTTONDOWN = 0x204;
        private const uint WM_RBUTTONUP = 0x205;

        public event MHookEventHandler mHookEvent
        {
            add
            {
                MHookEventHandler handler2;
                MHookEventHandler mHookEvent = this.MHookEvent;
                do
                {
                    handler2 = mHookEvent;
                    MHookEventHandler handler3 = (MHookEventHandler) Delegate.Combine(handler2, value);
                    mHookEvent = Interlocked.CompareExchange<MHookEventHandler>(ref this.MHookEvent, handler3, handler2);
                }
                while (mHookEvent != handler2);
            }
            remove
            {
                MHookEventHandler handler2;
                MHookEventHandler mHookEvent = this.MHookEvent;
                do
                {
                    handler2 = mHookEvent;
                    MHookEventHandler handler3 = (MHookEventHandler) Delegate.Remove(handler2, value);
                    mHookEvent = Interlocked.CompareExchange<MHookEventHandler>(ref this.MHookEvent, handler3, handler2);
                }
                while (mHookEvent != handler2);
            }
        }

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        private int MouseHookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if ((nCode >= 0) && (this.MHookEvent != null))
            {
                MSLLHOOTSTRUCT stMSLL = (MSLLHOOTSTRUCT) Marshal.PtrToStructure(lParam, typeof(MSLLHOOTSTRUCT));
                ButtonStatus btnStatus = ButtonStatus.None;
                if (wParam == ((IntPtr) 0x201))
                {
                    btnStatus = ButtonStatus.LeftDown;
                }
                else if (wParam == ((IntPtr) 0x202))
                {
                    btnStatus = ButtonStatus.LeftUp;
                }
                else if (wParam == ((IntPtr) 0x204))
                {
                    btnStatus = ButtonStatus.RightDown;
                }
                else if (wParam == ((IntPtr) 0x205))
                {
                    btnStatus = ButtonStatus.RightUp;
                }
                this.MHookEvent(this, new MHookEventArgs(btnStatus, stMSLL.pt.X, stMSLL.pt.Y));
            }
            return CallNextHookEx(this.hHook, nCode, wParam, lParam);
        }

        public bool SetHook()
        {
            if (this.hHook == IntPtr.Zero)
            {
                HookProc mouseCallBack = new HookProc(this.MouseHookProcedure);
                this.hHook = SetWindowsHookEx(14, mouseCallBack, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                if (this.hHook != IntPtr.Zero)
                {
                    this.gc = GCHandle.Alloc(mouseCallBack);
                    return true;
                }
            }
            return false;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hmod, int dwThreadid);
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hHook);
        public bool UnLoadHook()
        {
            if ((this.hHook != IntPtr.Zero) && UnhookWindowsHookEx(this.hHook))
            {
                this.hHook = IntPtr.Zero;
                this.gc.Free();
                return true;
            }
            return false;
        }

        public IntPtr HHook
        {
            get
            {
                return this.hHook;
            }
        }

        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate void MHookEventHandler(object sender, MHookEventArgs e);

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOTSTRUCT
        {
            public MouseHook.POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }
    }
}

