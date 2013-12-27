namespace CCWin
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public class AntiAliasGraphics : IDisposable
    {
        private Graphics _graphics;
        private SmoothingMode _oldMode;

        public AntiAliasGraphics(Graphics graphics)
        {
            this._graphics = graphics;
            this._oldMode = graphics.SmoothingMode;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        public void Dispose()
        {
            this._graphics.SmoothingMode = this._oldMode;
        }
    }
}

