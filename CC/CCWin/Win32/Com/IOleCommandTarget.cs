namespace CCWin.Win32.Com
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("B722BCCB-4E68-101B-A2BC-00AA00404770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true)]
    public interface IOleCommandTarget
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryStatus([In] IntPtr pguidCmdGroup, [In, MarshalAs(UnmanagedType.U4)] uint cCmds, [In, Out, MarshalAs(UnmanagedType.Struct)] ref tagOLECMD prgCmds, [In, Out] IntPtr pCmdText);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Exec([In] IntPtr pguidCmdGroup, [In, MarshalAs(UnmanagedType.U4)] uint nCmdID, [In, MarshalAs(UnmanagedType.U4)] uint nCmdexecopt, [In] IntPtr pvaIn, [In, Out] IntPtr pvaOut);
    }
}

