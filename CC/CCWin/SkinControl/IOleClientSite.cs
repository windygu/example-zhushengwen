namespace CCWin.SkinControl
{
    using System;
    using System.Runtime.InteropServices;

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true), Guid("00000118-0000-0000-C000-000000000046")]
    public interface IOleClientSite
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SaveObject();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwAssign, [In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [MarshalAs(UnmanagedType.Interface)] out object ppmk);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetContainer([MarshalAs(UnmanagedType.Interface)] out IOleContainer container);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int ShowObject();
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int OnShowWindow([In, MarshalAs(UnmanagedType.I4)] int fShow);
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int RequestNewObjectLayout();
    }
}

