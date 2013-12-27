namespace CCWin.Win32.Com
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    [ComImport, Guid("00000122-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true)]
    public interface IDropTarget
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DragEnter([In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObj, [In, MarshalAs(UnmanagedType.U4)] uint grfKeyState, [In, MarshalAs(UnmanagedType.Struct)] tagPOINT pt, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint pdwEffect);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DragOver([In, MarshalAs(UnmanagedType.U4)] uint grfKeyState, [In, MarshalAs(UnmanagedType.Struct)] tagPOINT pt, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint pdwEffect);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DragLeave();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Drop([In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObj, [In, MarshalAs(UnmanagedType.U4)] uint grfKeyState, [In, MarshalAs(UnmanagedType.Struct)] tagPOINT pt, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint pdwEffect);
    }
}

