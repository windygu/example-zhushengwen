namespace CCWin.SkinClass
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class InterpolationModeGraphics : IDisposable
    {
        private Graphics _graphics;
        private InterpolationMode _oldMode;

        public InterpolationModeGraphics(Graphics graphics) : this(graphics, InterpolationMode.HighQualityBicubic)
        {
        }

        public InterpolationModeGraphics(Graphics graphics, InterpolationMode newMode)
        {
            this._graphics = graphics;
            this._oldMode = graphics.InterpolationMode;
            graphics.InterpolationMode = newMode;
        }

        public void Dispose()
        {
            this._graphics.InterpolationMode = this._oldMode;
        }
    }
}

