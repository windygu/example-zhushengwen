namespace CCWin.SkinControl
{
    using CCWin.Imaging;
    using CCWin.SkinClass;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class SkinHScrollBar : HScrollBar, IScrollBarPaint
    {
        private Color _backHover = Color.FromArgb(0x79, 0xd8, 0xf3);
        private Color _backNormal = Color.FromArgb(0xeb, 0xf9, 0xfd);
        private Color _backPressed = Color.FromArgb(70, 0xca, 0xef);
        private Color _base = Color.FromArgb(0xab, 230, 0xf7);
        private Color _border = Color.FromArgb(0x59, 210, 0xf9);
        private Color _fore = Color.FromArgb(0x30, 0x87, 0xc0);
        private Color _innerBorder = Color.FromArgb(200, 250, 250, 250);
        private ScrollBarManager _manager;

        void IScrollBarPaint.OnPaintScrollBarArrow(PaintScrollBarArrowEventArgs e)
        {
            this.OnPaintScrollBarArrow(e);
        }

        void IScrollBarPaint.OnPaintScrollBarThumb(PaintScrollBarThumbEventArgs e)
        {
            this.OnPaintScrollBarThumb(e);
        }

        void IScrollBarPaint.OnPaintScrollBarTrack(PaintScrollBarTrackEventArgs e)
        {
            this.OnPaintScrollBarTrack(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this._manager != null))
            {
                this._manager.Dispose();
                this._manager = null;
            }
            base.Dispose(disposing);
        }

        private Color GetGray(Color color)
        {
            return ColorConverterEx.RgbToGray(new RGB(color)).Color;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this._manager != null)
            {
                this._manager.Dispose();
            }
            if (!base.DesignMode)
            {
                this._manager = new ScrollBarManager(this);
            }
        }

        protected virtual void OnPaintScrollBarArrow(PaintScrollBarArrowEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.ArrowRectangle;
            ControlState controlState = e.ControlState;
            ArrowDirection direction = e.ArrowDirection;
            Orientation orientation = e.Orientation;
            bool bEnabled = e.Enabled;
            Color backColor = this.BackNormal;
            Color baseColor = this.Base;
            Color borderColor = this.Border;
            Color innerBorderColor = this.InnerBorder;
            Color foreColor = this.Fore;
            bool changeColor = false;
            if (bEnabled)
            {
                switch (controlState)
                {
                    case ControlState.Hover:
                        baseColor = this.BackHover;
                        goto Label_00BD;

                    case ControlState.Pressed:
                        baseColor = this.BackPressed;
                        changeColor = true;
                        goto Label_00BD;
                }
                baseColor = this.Base;
            }
            else
            {
                backColor = this.GetGray(backColor);
                baseColor = this.GetGray(this.Base);
                borderColor = this.GetGray(borderColor);
                foreColor = this.GetGray(foreColor);
            }
        Label_00BD:
            using (new SmoothingModeGraphics(g))
            {
                CCWin.SkinControl.ControlPaintEx.DrawScrollBarArraw(g, rect, baseColor, backColor, borderColor, innerBorderColor, foreColor, e.Orientation, direction, changeColor);
            }
        }

        protected virtual void OnPaintScrollBarThumb(PaintScrollBarThumbEventArgs e)
        {
            if (e.Enabled)
            {
                Graphics g = e.Graphics;
                Rectangle rect = e.ThumbRectangle;
                ControlState controlState = e.ControlState;
                Color backColor = this.BackNormal;
                Color baseColor = this.Base;
                Color borderColor = this.Border;
                Color innerBorderColor = this.InnerBorder;
                bool changeColor = false;
                switch (controlState)
                {
                    case ControlState.Hover:
                        baseColor = this.BackHover;
                        break;

                    case ControlState.Pressed:
                        baseColor = this.BackPressed;
                        changeColor = true;
                        break;

                    default:
                        baseColor = this.Base;
                        break;
                }
                using (new SmoothingModeGraphics(g))
                {
                    CCWin.SkinControl.ControlPaintEx.DrawScrollBarThumb(g, rect, baseColor, backColor, borderColor, innerBorderColor, e.Orientation, changeColor);
                }
            }
        }

        protected virtual void OnPaintScrollBarTrack(PaintScrollBarTrackEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.TrackRectangle;
            Color baseColor = this.GetGray(this.Base);
            CCWin.SkinControl.ControlPaintEx.DrawScrollBarTrack(g, rect, baseColor, Color.White, e.Orientation);
        }

        public Color BackHover
        {
            get
            {
                return this._backHover;
            }
            set
            {
                if (this._backHover != value)
                {
                    this._backHover = value;
                    base.Invalidate();
                }
            }
        }

        public Color BackNormal
        {
            get
            {
                return this._backNormal;
            }
            set
            {
                if (this._backNormal != value)
                {
                    this._backNormal = value;
                    base.Invalidate();
                }
            }
        }

        public Color BackPressed
        {
            get
            {
                return this._backPressed;
            }
            set
            {
                if (this._backPressed != value)
                {
                    this._backPressed = value;
                    base.Invalidate();
                }
            }
        }

        public Color Base
        {
            get
            {
                return this._base;
            }
            set
            {
                if (this._base != value)
                {
                    this._base = value;
                    base.Invalidate();
                }
            }
        }

        public Color Border
        {
            get
            {
                return this._border;
            }
            set
            {
                if (this._border != value)
                {
                    this._border = value;
                    base.Invalidate();
                }
            }
        }

        public Color Fore
        {
            get
            {
                return this._fore;
            }
            set
            {
                if (this._fore != value)
                {
                    this._fore = value;
                    base.Invalidate();
                }
            }
        }

        public Color InnerBorder
        {
            get
            {
                return this._innerBorder;
            }
            set
            {
                if (this._innerBorder != value)
                {
                    this._innerBorder = value;
                    base.Invalidate();
                }
            }
        }
    }
}

