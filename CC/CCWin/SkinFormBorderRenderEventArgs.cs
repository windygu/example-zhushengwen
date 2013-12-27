namespace CCWin
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class SkinFormBorderRenderEventArgs : PaintEventArgs
    {
        private bool _active;
        private CCSkinMain _skinForm;

        public SkinFormBorderRenderEventArgs(CCSkinMain skinForm, Graphics g, Rectangle clipRect, bool active) : base(g, clipRect)
        {
            this._skinForm = skinForm;
            this._active = active;
        }

        public bool Active
        {
            get
            {
                return this._active;
            }
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

