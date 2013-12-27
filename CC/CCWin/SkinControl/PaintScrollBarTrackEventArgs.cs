namespace CCWin.SkinControl
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class PaintScrollBarTrackEventArgs : IDisposable
    {
        private bool _enabled;
        private System.Drawing.Graphics _graphics;
        private System.Windows.Forms.Orientation _orientation;
        private Rectangle _trackRect;

        public PaintScrollBarTrackEventArgs(System.Drawing.Graphics graphics, Rectangle trackRect, System.Windows.Forms.Orientation orientation) : this(graphics, trackRect, orientation, true)
        {
        }

        public PaintScrollBarTrackEventArgs(System.Drawing.Graphics graphics, Rectangle trackRect, System.Windows.Forms.Orientation orientation, bool enabled)
        {
            this._graphics = graphics;
            this._trackRect = trackRect;
            this._orientation = orientation;
            this._enabled = enabled;
        }

        public void Dispose()
        {
            this._graphics = null;
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

        public Rectangle TrackRectangle
        {
            get
            {
                return this._trackRect;
            }
            set
            {
                this._trackRect = value;
            }
        }
    }
}

