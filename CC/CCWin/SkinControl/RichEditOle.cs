namespace CCWin.SkinControl
{
    using CCWin.Win32;
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class RichEditOle
    {
        private SkinRichTextBox _richEdit;
        private CCWin.SkinControl.IRichEditOle _richEditOle;

        public RichEditOle(SkinRichTextBox richEdit)
        {
            this._richEdit = richEdit;
        }

        private System.Drawing.Size GetSizeFromMillimeter(REOBJECT lpreobject)
        {
            using (Graphics graphics = Graphics.FromHwnd(this._richEdit.Handle))
            {
                System.Drawing.Point[] pts = new System.Drawing.Point[1];
                graphics.PageUnit = GraphicsUnit.Millimeter;
                pts[0] = new System.Drawing.Point(lpreobject.sizel.Width / 100, lpreobject.sizel.Height / 100);
                graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.Page, pts);
                return new System.Drawing.Size(pts[0]);
            }
        }

        public void InsertControl(Control control)
        {
            if (control != null)
            {
                CCWin.SkinControl.ILockBytes bytes;
                CCWin.SkinControl.IStorage storage;
                CCWin.SkinControl.IOleClientSite site;
                Guid guid = Marshal.GenerateGuidForType(control.GetType());
                CCWin.Win32.NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out bytes);
                CCWin.Win32.NativeMethods.StgCreateDocfileOnILockBytes(bytes, 0x1012, 0, out storage);
                this.IRichEditOle.GetClientSite(out site);
                REOBJECT lpreobject = new REOBJECT();
                lpreobject.cp = this._richEdit.TextLength;
                lpreobject.clsid = guid;
                lpreobject.pstg = storage;
                lpreobject.poleobj = Marshal.GetIUnknownForObject(control);
                lpreobject.polesite = site;
                lpreobject.dvAspect = 1;
                lpreobject.dwFlags = 2;
                lpreobject.dwUser = 1;
                this.IRichEditOle.InsertObject(lpreobject);
                Marshal.ReleaseComObject(bytes);
                Marshal.ReleaseComObject(site);
                Marshal.ReleaseComObject(storage);
            }
        }

        public bool InsertImageFromFile(string strFilename)
        {
            CCWin.SkinControl.ILockBytes bytes;
            CCWin.SkinControl.IStorage storage;
            CCWin.SkinControl.IOleClientSite site;
            object obj2;
            CCWin.Win32.NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out bytes);
            CCWin.Win32.NativeMethods.StgCreateDocfileOnILockBytes(bytes, 0x1012, 0, out storage);
            this.IRichEditOle.GetClientSite(out site);
            FORMATETC pFormatEtc = new FORMATETC();
            pFormatEtc.cfFormat = (CLIPFORMAT) 0;
            pFormatEtc.ptd = IntPtr.Zero;
            pFormatEtc.dwAspect = DVASPECT.DVASPECT_CONTENT;
            pFormatEtc.lindex = -1;
            pFormatEtc.tymed = TYMED.TYMED_NULL;
            Guid riid = new Guid("{00000112-0000-0000-C000-000000000046}");
            Guid rclsid = new Guid("{00000000-0000-0000-0000-000000000000}");
            CCWin.Win32.NativeMethods.OleCreateFromFile(ref rclsid, strFilename, ref riid, 1, ref pFormatEtc, site, storage, out obj2);
            if (obj2 == null)
            {
                Marshal.ReleaseComObject(bytes);
                Marshal.ReleaseComObject(site);
                Marshal.ReleaseComObject(storage);
                return false;
            }
            CCWin.SkinControl.IOleObject pUnk = (CCWin.SkinControl.IOleObject) obj2;
            Guid pClsid = new Guid();
            pUnk.GetUserClassID(ref pClsid);
            CCWin.Win32.NativeMethods.OleSetContainedObject(pUnk, true);
            REOBJECT lpreobject = new REOBJECT();
            lpreobject.cp = this._richEdit.TextLength;
            lpreobject.clsid = pClsid;
            lpreobject.pstg = storage;
            lpreobject.poleobj = Marshal.GetIUnknownForObject(pUnk);
            lpreobject.polesite = site;
            lpreobject.dvAspect = 1;
            lpreobject.dwFlags = 2;
            lpreobject.dwUser = 0;
            this.IRichEditOle.InsertObject(lpreobject);
            Marshal.ReleaseComObject(bytes);
            Marshal.ReleaseComObject(site);
            Marshal.ReleaseComObject(storage);
            Marshal.ReleaseComObject(pUnk);
            return true;
        }

        public REOBJECT InsertOleObject(CCWin.SkinControl.IOleObject oleObject, int index)
        {
            CCWin.SkinControl.ILockBytes pLockBytes;
            CCWin.SkinControl.IStorage pStorage;
            CCWin.SkinControl.IOleClientSite pOleClientSite;
            if (oleObject == null)
            {
                return null;
            }
            CCWin.Win32.NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out pLockBytes);
            CCWin.Win32.NativeMethods.StgCreateDocfileOnILockBytes(pLockBytes, 0x1012, 0, out pStorage);
            this.IRichEditOle.GetClientSite(out pOleClientSite);
            Guid guid = new Guid();
            oleObject.GetUserClassID(ref guid);
            CCWin.Win32.NativeMethods.OleSetContainedObject(oleObject, true);
            REOBJECT reoObject = new REOBJECT();
            reoObject.cp = this._richEdit.TextLength;
            reoObject.clsid = guid;
            reoObject.pstg = pStorage;
            reoObject.poleobj = Marshal.GetIUnknownForObject(oleObject);
            reoObject.polesite = pOleClientSite;
            reoObject.dvAspect = 1;
            reoObject.dwFlags = 2;
            reoObject.dwUser = (uint) index;
            this.IRichEditOle.InsertObject(reoObject);
            Marshal.ReleaseComObject(pLockBytes);
            Marshal.ReleaseComObject(pOleClientSite);
            Marshal.ReleaseComObject(pStorage);
            return reoObject;
        }

        public void UpdateObjects()
        {
            int objectCount = this.IRichEditOle.GetObjectCount();
            for (int i = 0; i < objectCount; i++)
            {
                REOBJECT lpreobject = new REOBJECT();
                this.IRichEditOle.GetObject(i, lpreobject, GETOBJECTOPTIONS.REO_GETOBJ_ALL_INTERFACES);
                System.Drawing.Point positionFromCharIndex = this._richEdit.GetPositionFromCharIndex(lpreobject.cp);
                Rectangle rc = new Rectangle(positionFromCharIndex.X, positionFromCharIndex.Y, 50, 50);
                this._richEdit.Invalidate(rc, false);
            }
        }

        public void UpdateObjects(REOBJECT reObj)
        {
            System.Drawing.Point positionFromCharIndex = this._richEdit.GetPositionFromCharIndex(reObj.cp);
            System.Drawing.Size size = this.GetSizeFromMillimeter(reObj);
            Rectangle rc = new Rectangle(positionFromCharIndex, size);
            this._richEdit.Invalidate(rc, false);
        }

        public void UpdateObjects(int pos)
        {
            REOBJECT lpreobject = new REOBJECT();
            this.IRichEditOle.GetObject(pos, lpreobject, GETOBJECTOPTIONS.REO_GETOBJ_ALL_INTERFACES);
            this.UpdateObjects(lpreobject);
        }

        public CCWin.SkinControl.IRichEditOle IRichEditOle
        {
            get
            {
                if (this._richEditOle == null)
                {
                    this._richEditOle = CCWin.Win32.NativeMethods.SendMessage(this._richEdit.Handle, 0x43c, 0);
                }
                return this._richEditOle;
            }
        }
    }
}

