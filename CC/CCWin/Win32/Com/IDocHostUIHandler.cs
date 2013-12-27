namespace CCWin.Win32.Com
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    [ComImport, Guid("bd3f23c0-d43e-11cf-893b-00aa00bdce1a"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDocHostUIHandler
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowContextMenu([In, MarshalAs(UnmanagedType.U4)] uint dwID, [In, MarshalAs(UnmanagedType.Struct)] ref tagPOINT pt, [In, MarshalAs(UnmanagedType.IUnknown)] object pcmdtReserved, [In, MarshalAs(UnmanagedType.IDispatch)] object pdispReserved);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetHostInfo([In, Out, MarshalAs(UnmanagedType.Struct)] ref DOCHOSTUIINFO info);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowUI([In, MarshalAs(UnmanagedType.I4)] int dwID, [In, MarshalAs(UnmanagedType.Interface)] IOleInPlaceActiveObject activeObject, [In, MarshalAs(UnmanagedType.Interface)] IOleCommandTarget commandTarget, [In, MarshalAs(UnmanagedType.Interface)] IOleInPlaceFrame frame, [In, MarshalAs(UnmanagedType.Interface)] IOleInPlaceUIWindow doc);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int HideUI();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int UpdateUI();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnableModeless([In, MarshalAs(UnmanagedType.Bool)] bool fEnable);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnDocWindowActivate([In, MarshalAs(UnmanagedType.Bool)] bool fActivate);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnFrameWindowActivate([In, MarshalAs(UnmanagedType.Bool)] bool fActivate);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ResizeBorder([In, MarshalAs(UnmanagedType.Struct)] ref tagRECT rect, [In, MarshalAs(UnmanagedType.Interface)] IOleInPlaceUIWindow doc, [In, MarshalAs(UnmanagedType.Bool)] bool fFrameWindow);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int TranslateAccelerator([In, MarshalAs(UnmanagedType.Struct)] ref tagMSG msg, [In] ref Guid group, [In, MarshalAs(UnmanagedType.U4)] uint nCmdID);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetOptionKeyPath([MarshalAs(UnmanagedType.LPWStr)] out string pbstrKey, [In, MarshalAs(UnmanagedType.U4)] uint dw);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetDropTarget([In, MarshalAs(UnmanagedType.Interface)] IDropTarget pDropTarget, [MarshalAs(UnmanagedType.Interface)] out IDropTarget ppDropTarget);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetExternal([MarshalAs(UnmanagedType.IDispatch)] out object ppDispatch);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int TranslateUrl([In, MarshalAs(UnmanagedType.U4)] uint dwTranslate, [In, MarshalAs(UnmanagedType.LPWStr)] string strURLIn, [MarshalAs(UnmanagedType.LPWStr)] out string pstrURLOut);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int FilterDataObject([In, MarshalAs(UnmanagedType.Interface)] IDataObject pDO, [MarshalAs(UnmanagedType.Interface)] out IDataObject ppDORet);
    }
}

