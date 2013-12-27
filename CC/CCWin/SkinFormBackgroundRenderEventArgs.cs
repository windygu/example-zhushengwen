namespace CCWin
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class SkinFormBackgroundRenderEventArgs : PaintEventArgs
    {
        private CCSkinMain _skinForm;

        public SkinFormBackgroundRenderEventArgs(CCSkinMain skinForm, Graphics g, Rectangle clipRect) : base(g, clipRect)
        {
            this._skinForm = skinForm;
        }

        public CCSkinMain SkinForm
        {
            get
            {
                return this._skinForm;
            }
        }
    }
}

