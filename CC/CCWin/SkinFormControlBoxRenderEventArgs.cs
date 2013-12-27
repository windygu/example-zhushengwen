namespace CCWin
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class SkinFormControlBoxRenderEventArgs : PaintEventArgs
    {
        private bool _active;
        private ControlBoxState _controlBoxState;
        private CCWin.ControlBoxStyle _controlBoxStyle;
        private CCSkinMain _form;

        public SkinFormControlBoxRenderEventArgs(CCSkinMain form, Graphics graphics, Rectangle clipRect, bool active, CCWin.ControlBoxStyle controlBoxStyle, ControlBoxState controlBoxState) : base(graphics, clipRect)
        {
            this._form = form;
            this._active = active;
            this._controlBoxState = controlBoxState;
            this._controlBoxStyle = controlBoxStyle;
        }

        public bool Active
        {
            get
            {
                return this._active;
            }
        }

        public CCWin.ControlBoxStyle ControlBoxStyle
        {
            get
            {
                return this._controlBoxStyle;
            }
        }

        public ControlBoxState ControlBoxtate
        {
            get
            {
                return this._controlBoxState;
            }
        }

        public CCSkinMain Form
        {
            get
            {
                return this._form;
            }
        }
    }
}

