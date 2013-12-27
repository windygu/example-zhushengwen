namespace CCWin.Win32.Struct
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    [StructLayout(LayoutKind.Sequential)]
    public struct API_MSG
    {
        public IntPtr Hwnd;
        public int Msg;
        public IntPtr WParam;
        public IntPtr LParam;
        public int Time;
        public CCWin.Win32.Struct.POINT Pt;
        public Message ToMessage()
        {
            Message res = new Message();
            res.HWnd = this.Hwnd;
            res.Msg = this.Msg;
            res.WParam = this.WParam;
            res.LParam = this.LParam;
            return res;
        }

        public void FromMessage(ref Message msg)
        {
            this.Hwnd = msg.HWnd;
            this.Msg = msg.Msg;
            this.WParam = msg.WParam;
            this.LParam = msg.LParam;
        }
    }
}

