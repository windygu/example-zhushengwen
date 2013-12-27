namespace CCWin.SkinControl
{
    using System;
    using System.Drawing;

    public class ColorChangedEventArgs : EventArgs
    {
        private System.Drawing.Color color;

        public ColorChangedEventArgs(System.Drawing.Color clr)
        {
            this.color = clr;
        }

        public System.Drawing.Color Color
        {
            get
            {
                return this.color;
            }
        }
    }
}

