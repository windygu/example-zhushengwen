namespace CCWin.Win32.Com
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("3050F3F0-98B5-11CF-BB82-00AA00BDCE0B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICustomDoc
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void SetUIHandler([In, MarshalAs(UnmanagedType.Interface)] IDocHostUIHandler pUIHandler);
    }
}

