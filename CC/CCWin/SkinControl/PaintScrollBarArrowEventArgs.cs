namespace CCWin.SkinControl
{
    using CCWin.SkinClass;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class PaintScrollBarArrowEventArgs : IDisposable
    {
        private System.Windows.Forms.ArrowDirection _arrowDirection;
        private Rectangle _arrowRect;
        private CCWin.SkinClass.ControlState _controlState;
        private bool _enabled;
        private System.Drawing.Graphics _graphics;
        private System.Windows.Forms.Orientation _orientation;

        public PaintScrollBarArrowEventArgs(System.Drawing.Graphics graphics, Rectangle arrowRect, CCWin.SkinClass.ControlState controlState, System.Windows.Forms.ArrowDirection arrowDirection, System.Windows.Forms.Orientation orientation) : this(graphics, arrowRect, controlState, arrowDirection, orientation, true)
        {
        }

        public PaintScrollBarArrowEventArgs(System.Drawing.Graphics graphics, Rectangle arrowRect, CCWin.SkinClass.ControlState controlState, System.Windows.Forms.ArrowDirection arrowDirection, System.Windows.Forms.Orientation orientation, bool enabled)
        {
            this._graphics = graphics;
            this._arrowRect = arrowRect;
            this._controlState = controlState;
            this._arrowDirection = arrowDirection;
            this._orientation = orientation;
            this._enabled = enabled;
        }

        public void Dispose()
        {
            this._graphics = null;
        }

        public System.Windows.Forms.ArrowDirection ArrowDirection
        {
            get
            {
                return this._arrowDirection;
            }
            set
            {
                this._arrowDirection = value;
            }
        }

        public Rectangle ArrowRectangle
        {
            get
            {
                return this._arrowRect;
            }
            set
            {
                this._arrowRect = value;
            }
        }

        public CCWin.SkinClass.ControlState ControlState
        {
            get
            {
                return this._controlState;
            }
            set
            {
                this._controlState = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                this._enabled = value;
            }
        }

        public System.Drawing.Graphics Graphics
        {
            get
            {
                return this._graphics;
            }
            set
            {
                this._graphics = value;
            }
        }

        public System.Windows.Forms.Orientation Orientation
        {
            get
            {
                return this._orientation;
            }
            set
            {
                this._orientation = value;
            }
        }
    }
}

