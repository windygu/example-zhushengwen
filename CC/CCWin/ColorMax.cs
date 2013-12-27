namespace CCWin
{
    using System;
    using System.Drawing;

    public class ColorMax
    {
        private System.Drawing.Color color;
        private int number;

        public System.Drawing.Color Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
            }
        }

        public int Number
        {
            get
            {
                return this.number;
            }
            set
            {
                this.number = value;
            }
        }
    }
}

