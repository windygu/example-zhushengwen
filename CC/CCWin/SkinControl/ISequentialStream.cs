namespace CCWin.SkinControl
{
    using System;
    using System.Runtime.InteropServices;

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0c733a30-2a1c-11ce-ade5-00aa0044773d")]
    public interface ISequentialStream
    {
        int Read(IntPtr pv, uint cb, out uint pcbRead);
        int Write(IntPtr pv, uint cb, out uint pcbWritten);
    }
}

