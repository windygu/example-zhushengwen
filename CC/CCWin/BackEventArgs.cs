namespace CCWin
{
    using System;
    using System.Drawing;

    public class BackEventArgs
    {
        private Image afterBack;
        private Image beforeBack;

        public BackEventArgs(Image beforeBack, Image afterBack)
        {
            this.beforeBack = beforeBack;
            this.afterBack = afterBack;
        }

        public Image AfterBack
        {
            get
            {
                return this.afterBack;
            }
        }

        public Image BeforeBack
        {
            get
            {
                return this.beforeBack;
            }
        }
    }
}

