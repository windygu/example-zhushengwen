namespace CCWin.SkinClass
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class SmoothingModeGraphics : IDisposable
    {
        private Graphics _graphics;
        private SmoothingMode _oldMode;

        public SmoothingModeGraphics(Graphics graphics) : this(graphics, SmoothingMode.AntiAlias)
        {
        }

        public SmoothingModeGraphics(Graphics graphics, SmoothingMode newMode)
        {
            this._graphics = graphics;
            this._oldMode = graphics.SmoothingMode;
            graphics.SmoothingMode = newMode;
        }

        public void Dispose()
        {
            this._graphics.SmoothingMode = this._oldMode;
        }
    }
}

