namespace CCWin.Win32
{
    using CCWin.SkinControl;
    using CCWin.Win32.Callback;
    using CCWin.Win32.Struct;
    using System;
    using System.Runtime.InteropServices;

    public class NativeMethods
    {
        public const int CWP_All = 0;
        public const int CWP_SKIPDISABLED = 2;
        public const int CWP_SKIPINVISIBL = 1;
        public const int EM_GETOLEINTERFACE = 0x43c;
        public const int ULW_ALPHA = 2;
        public const int WM_USER = 0x400;

        private NativeMethods()
        {
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool AdjustWindowRectEx(ref RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);
        [DllImport("gdi32.dll", EntryPoint="GdiAlphaBlend")]
        public static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, BLENDFUNCTION blendFunction);
        [DllImport("user32.dll")]
        public static extern bool AnimateWindow(IntPtr whnd, int dwtime, int dwflag);
        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
        [DllImport("USER32.dll", CharSet=CharSet.Auto)]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPointEx(IntPtr pHwnd, Point pt, uint uFlgs);
        [DllImport("gdi32.dll", SetLastError=true, ExactSpelling=true)]
        public static extern int CombineRgn(IntPtr hRgn, IntPtr hRgn1, IntPtr hRgn2, int nCombineMode);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, int lpInitData);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDCA([MarshalAs(UnmanagedType.LPStr)] string lpszDriver, [MarshalAs(UnmanagedType.LPStr)] string lpszDevice, [MarshalAs(UnmanagedType.LPStr)] string lpszOutput, int lpInitData);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDCW([MarshalAs(UnmanagedType.LPWStr)] string lpszDriver, [MarshalAs(UnmanagedType.LPWStr)] string lpszDevice, [MarshalAs(UnmanagedType.LPWStr)] string lpszOutput, int lpInitData);
        [DllImport("ole32.dll")]
        public static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out ILockBytes ppLkbyt);
        [DllImport("gdi32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);
        [DllImport("gdi32.dll")]
        public static extern int CreateRoundRectRgn(int x1, int y1, int x2, int y2, int x3, int y3);
        [DllImport("user32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        public static extern IntPtr CreateWindowEx(int exstyle, string lpClassName, string lpWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hwndParent, IntPtr Menu, IntPtr hInstance, IntPtr lpParam);
        [DllImport("user32.dll")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern void DisableProcessWindowsGhosting();
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);
        [DllImport("user32.dll")]
        public static extern bool EnableScrollBar(IntPtr hWnd, int wSBflags, int wArrows);
        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);
        [DllImport("user32.dll")]
        public static extern bool EqualRect([In] ref RECT lprc1, [In] ref RECT lprc2);
        [DllImport("gdi32.dll")]
        public static extern int ExcludeClipRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        [DllImport("shell32.dll", CharSet=CharSet.Auto)]
        public static extern int FindExecutable(string filename, string directory, ref string result);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("User32.dll")]
        public static extern bool FlashWindow(IntPtr hWnd, bool bInvert);
        [DllImport("user32.DLL")]
        public static extern uint GetCaretBlinkTime();
        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
            {
                return GetClassLongPtr64(hWnd, nIndex);
            }
            return GetClassLongPtr32(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint="GetClassLong")]
        private static extern IntPtr GetClassLongPtr32(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint="GetClassLongPtr")]
        private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, ref RECT r);
        [DllImport("user32.dll")]
        public static extern bool GetComboBoxInfo(IntPtr hwndCombo, ref ComboBoxInfo info);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int GetCurrentThreadId();
        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out PCURSORINFO pci);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr handle);
        [DllImport("USER32.dll")]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern short GetKeyState(int nVirtKey);
        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int GetScrollBarInfo(IntPtr hWnd, uint idObject, ref SCROLLBARINFO psbi);
        [DllImport("user32.dll")]
        public static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO scrollInfo);
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr handle);
        [DllImport("user32.dll")]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);
        [DllImport("user32")]
        public static extern int GetWindowLong(IntPtr hwnd, int nIndex);
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int GetWindowLong(HandleRef hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hwnd, int nIndex);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("comctl32.dll", CallingConvention=CallingConvention.StdCall)]
        public static extern bool InitCommonControlsEx(ref INITCOMMONCONTROLSEX iccex);
        [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool InvalidateRect(IntPtr hWnd, ref RECT rect, bool erase);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("uxtheme.dll")]
        public static extern bool IsAppThemed();
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool IsZoomed(IntPtr hWnd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", ExactSpelling=true)]
        public static extern bool KillTimer(IntPtr hWnd, uint uIDEvent);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursorFromFile(string fileName);
        [DllImport("user32.dll")]
        public static extern IntPtr LoadIcon(IntPtr hInstance, int lpIconName);
        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);
        [DllImport("user32.dll")]
        public static extern bool MessageBeep(int uType);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("kernel32.dll")]
        public static extern int MulDiv(int nNumber, int nNumerator, int nDenominator);
        [DllImport("user32.dll")]
        public static extern int OffsetRect(ref RECT lpRect, int x, int y);
        [DllImport("ole32.dll")]
        public static extern int OleCreateFromFile([In] ref Guid rclsid, [MarshalAs(UnmanagedType.LPWStr)] string lpszFileName, [In] ref Guid riid, uint renderopt, ref FORMATETC pFormatEtc, IOleClientSite pClientSite, IStorage pStg, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);
        [DllImport("ole32.dll")]
        public static extern int OleSetContainedObject([MarshalAs(UnmanagedType.IUnknown)] object pUnk, bool fContained);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool PtInRect(ref RECT lprc, Point pt);
        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr rectUpdate, IntPtr hrgnUpdate, int flags);
        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, ref RECT rectUpdate, IntPtr hrgnUpdate, int flags);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr handle, IntPtr hdc);
        [DllImport("kernel32.dll")]
        public static extern int RtlMoveMemory(ref NMCUSTOMDRAW destination, IntPtr Source, int length);
        [DllImport("kernel32.dll")]
        public static extern int RtlMoveMemory(ref NMHDR destination, IntPtr source, int length);
        [DllImport("kernel32.dll")]
        public static extern int RtlMoveMemory(ref NMTTCUSTOMDRAW destination, IntPtr Source, int length);
        [DllImport("kernel32.dll")]
        public static extern int RtlMoveMemory(ref NMTTDISPINFO destination, IntPtr source, int length);
        [DllImport("kernel32.dll")]
        public static extern int RtlMoveMemory(ref CCWin.Win32.Struct.POINT destination, ref RECT Source, int length);
        [DllImport("kernel32.dll")]
        public static extern int RtlMoveMemory(IntPtr destination, ref NMTTDISPINFO Source, int length);
        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);
        [DllImport("gdi32.dll", ExactSpelling=true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("User32.dll", CharSet=CharSet.Auto, PreserveSig=false)]
        public static extern IRichEditOle SendMessage(IntPtr hWnd, int message, int wParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref RECT lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref SCROLLBARINFO lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref TOOLINFO lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref NMHDR lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPTStr)] string lParam);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        public static extern int SetFocus(IntPtr hWnd);
        [DllImport("gdi32.dll")]
        public static extern uint SetPixel(IntPtr hdc, int X, int Y, int crColor);
        [DllImport("user32.dll", ExactSpelling=true)]
        public static extern IntPtr SetTimer(IntPtr hWnd, int nIDEvent, uint uElapse, IntPtr lpTimerFunc);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLongPtr(IntPtr hwnd, int nIndex, IntPtr dwNewLong);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int x, int y, int cx, int cy, uint flags);
        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hwnd, int hRgn, bool bRedraw);
        [DllImport("user32.dll")]
        public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);
        [DllImport("USER32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(int idHook, CCWin.Win32.Callback.HookProc lpfn, int hMod, int dwThreadId);
        [DllImport("uxtheme.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
        [DllImport("shell32.dll", CharSet=CharSet.Auto)]
        public static extern int ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("ole32.dll")]
        public static extern int StgCreateDocfileOnILockBytes(ILockBytes plkbyt, uint grfMode, uint reserved, out IStorage ppstgOpen);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(IntPtr hDest, int X, int Y, int nWidth, int nHeight, IntPtr hdcSrc, int sX, int sY, int nWidthSrc, int nHeightSrc, int dwRop);
        [DllImport("user32.dll")]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);
        [DllImport("user32.dll")]
        public static extern IntPtr TrackPopupMenu(IntPtr hMenu, int uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr par);
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool TrackPopupMenuEx(IntPtr hMenu, uint uFlags, int x, int y, IntPtr hWnd, IntPtr tpmParams);
        [DllImport("USER32.dll", CharSet=CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", SetLastError=true, ExactSpelling=true)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);
        [DllImport("user32.dll")]
        public static extern bool ValidateRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("kernel32.dll")]
        public static extern int WinExec(string lpCmdLine, int nCmdShow);

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        public enum ComboBoxButtonState
        {
            STATE_SYSTEM_INVISIBLE = 0x8000,
            STATE_SYSTEM_NONE = 0,
            STATE_SYSTEM_PRESSED = 8
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ComboBoxInfo
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public NativeMethods.ComboBoxButtonState stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndEdit;
            public IntPtr hwndList;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PCURSORINFO
        {
            public int cbSize;
            public int flag;
            public IntPtr hCursor;
            public NativeMethods.Point ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int x;
            public int y;
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public int cx;
            public int cy;
            public Size(int x, int y)
            {
                this.cx = x;
                this.cy = y;
            }
        }
    }
}

