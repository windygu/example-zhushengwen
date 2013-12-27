namespace CCWin.SkinControl
{
    using CCWin.SkinClass;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class PaintScrollBarThumbEventArgs : IDisposable
    {
        private CCWin.SkinClass.ControlState _controlState;
        private bool _enabled;
        private System.Drawing.Graphics _graphics;
        private System.Windows.Forms.Orientation _orientation;
        private Rectangle _thumbRect;

        public PaintScrollBarThumbEventArgs(System.Drawing.Graphics graphics, Rectangle thumbRect, CCWin.SkinClass.ControlState controlState, System.Windows.Forms.Orientation orientation) : this(graphics, thumbRect, controlState, orientation, true)
        {
        }

        public PaintScrollBarThumbEventArgs(System.Drawing.Graphics graphics, Rectangle thumbRect, CCWin.SkinClass.ControlState controlState, System.Windows.Forms.Orientation orientation, bool enabled)
        {
            this._graphics = graphics;
            this._thumbRect = thumbRect;
            this._controlState = controlState;
            this._orientation = orientation;
            this._enabled = enabled;
        }

        public void Dispose()
        {
            this._graphics = null;
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

        public Rectangle ThumbRectangle
        {
            get
            {
                return this._thumbRect;
            }
            set
            {
                this._thumbRect = value;
            }
        }
    }
}

