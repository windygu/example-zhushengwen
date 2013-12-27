﻿namespace CCWin.SkinControl
{
    using System;

    public class MHookEventArgs : EventArgs
    {
        private ButtonStatus mButton;
        private int x;
        private int y;

        public MHookEventArgs(ButtonStatus btn, int cx, int cy)
        {
            this.mButton = btn;
            this.x = cx;
            this.y = cy;
        }

        public ButtonStatus MButton
        {
            get
            {
                return this.mButton;
            }
        }

        public int X
        {
            get
            {
                return this.x;
            }
        }

        public int Y
        {
            get
            {
                return this.y;
            }
        }
    }
}

