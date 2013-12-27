namespace CCWin
{
    using CCWin.SkinClass;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    public class ControlBoxManager : IDisposable
    {
        private ControlBoxState _closBoxState;
        private ControlBoxState _maximizeBoxState;
        private ControlBoxState _minimizeBoxState;
        private bool _mouseDown;
        private CCSkinMain _owner;
        private ControlBoxState _SysBottomState;

        public ControlBoxManager(CCSkinMain owner)
        {
            this._owner = owner;
        }

        public void Dispose()
        {
            this._owner = null;
        }

        private void HideToolTip()
        {
            if (this._owner != null)
            {
                this._owner.ToolTip.Active = false;
            }
        }

        private void Invalidate(Rectangle rect)
        {
            this._owner.Invalidate(rect);
        }

        private void ProcessMouseDown(Point mousePoint, Rectangle closeBoxRect, Rectangle minimizeBoxRect, Rectangle maximizeBoxRect, Rectangle sysbottomRect, bool closeBoxVisibale, bool minimizeBoxVisibale, bool maximizeBoxVisibale, bool sysbottomVisibale)
        {
            this._mouseDown = true;
            if (closeBoxVisibale && closeBoxRect.Contains(mousePoint))
            {
                this.CloseBoxState = ControlBoxState.Pressed;
            }
            else if (minimizeBoxVisibale && minimizeBoxRect.Contains(mousePoint))
            {
                this.MinimizeBoxState = ControlBoxState.Pressed;
            }
            else if (this.SysBottomVisibale && sysbottomRect.Contains(mousePoint))
            {
                this.SysBottomState = ControlBoxState.Pressed;
            }
            else if (maximizeBoxVisibale && maximizeBoxRect.Contains(mousePoint))
            {
                this.MaximizeBoxState = ControlBoxState.Pressed;
            }
        }

        private void ProcessMouseLeave(bool closeBoxVisibale, bool minimizeBoxVisibale, bool maximizeBoxVisibale, bool sysbottomVisibale)
        {
            if (closeBoxVisibale)
            {
                if (this.CloseBoxState == ControlBoxState.Pressed)
                {
                    this.CloseBoxState = ControlBoxState.PressedLeave;
                }
                else
                {
                    this.CloseBoxState = ControlBoxState.Normal;
                }
            }
            if (minimizeBoxVisibale)
            {
                if (this.MinimizeBoxState == ControlBoxState.Pressed)
                {
                    this.MinimizeBoxState = ControlBoxState.PressedLeave;
                }
                else
                {
                    this.MinimizeBoxState = ControlBoxState.Normal;
                }
            }
            if (sysbottomVisibale)
            {
                if (this.SysBottomState == ControlBoxState.Pressed)
                {
                    this.SysBottomState = ControlBoxState.PressedLeave;
                }
                else
                {
                    this.SysBottomState = ControlBoxState.Normal;
                }
            }
            if (maximizeBoxVisibale)
            {
                if (this.MaximizeBoxState == ControlBoxState.Pressed)
                {
                    this.MaximizeBoxState = ControlBoxState.PressedLeave;
                }
                else
                {
                    this.MaximizeBoxState = ControlBoxState.Normal;
                }
            }
            this.HideToolTip();
        }

        private void ProcessMouseMove(Point mousePoint, Rectangle closeBoxRect, Rectangle minimizeBoxRect, Rectangle maximizeBoxRect, Rectangle sysbottomRect, bool closeBoxVisibale, bool minimizeBoxVisibale, bool maximizeBoxVisibale, bool sysbottomVisibale)
        {
            string toolTip = string.Empty;
            bool hide = true;
            if (closeBoxVisibale)
            {
                if (closeBoxRect.Contains(mousePoint))
                {
                    hide = false;
                    if (!this._mouseDown)
                    {
                        if (this.CloseBoxState != ControlBoxState.Hover)
                        {
                            toolTip = "关闭";
                        }
                        this.CloseBoxState = ControlBoxState.Hover;
                    }
                    else if (this.CloseBoxState == ControlBoxState.PressedLeave)
                    {
                        this.CloseBoxState = ControlBoxState.Pressed;
                    }
                }
                else if (!this._mouseDown)
                {
                    this.CloseBoxState = ControlBoxState.Normal;
                }
                else if (this.CloseBoxState == ControlBoxState.Pressed)
                {
                    this.CloseBoxState = ControlBoxState.PressedLeave;
                }
            }
            if (minimizeBoxVisibale)
            {
                if (minimizeBoxRect.Contains(mousePoint))
                {
                    hide = false;
                    if (!this._mouseDown)
                    {
                        if (this.MinimizeBoxState != ControlBoxState.Hover)
                        {
                            toolTip = "最小化";
                        }
                        this.MinimizeBoxState = ControlBoxState.Hover;
                    }
                    else if (this.MinimizeBoxState == ControlBoxState.PressedLeave)
                    {
                        this.MinimizeBoxState = ControlBoxState.Pressed;
                    }
                }
                else if (!this._mouseDown)
                {
                    this.MinimizeBoxState = ControlBoxState.Normal;
                }
                else if (this.MinimizeBoxState == ControlBoxState.Pressed)
                {
                    this.MinimizeBoxState = ControlBoxState.PressedLeave;
                }
            }
            if (maximizeBoxVisibale)
            {
                if (maximizeBoxRect.Contains(mousePoint))
                {
                    hide = false;
                    if (!this._mouseDown)
                    {
                        if (this.MaximizeBoxState != ControlBoxState.Hover)
                        {
                            toolTip = (this._owner.WindowState == FormWindowState.Maximized) ? "还原" : "最大化";
                        }
                        this.MaximizeBoxState = ControlBoxState.Hover;
                    }
                    else if (this.MaximizeBoxState == ControlBoxState.PressedLeave)
                    {
                        this.MaximizeBoxState = ControlBoxState.Pressed;
                    }
                }
                else if (!this._mouseDown)
                {
                    this.MaximizeBoxState = ControlBoxState.Normal;
                }
                else if (this.MaximizeBoxState == ControlBoxState.Pressed)
                {
                    this.MaximizeBoxState = ControlBoxState.PressedLeave;
                }
            }
            if (sysbottomVisibale)
            {
                if (sysbottomRect.Contains(mousePoint))
                {
                    hide = false;
                    if (!this._mouseDown)
                    {
                        if (this.SysBottomState != ControlBoxState.Hover)
                        {
                            toolTip = this._owner.SysBottomToolTip;
                        }
                        this.SysBottomState = ControlBoxState.Hover;
                    }
                    else if (this.SysBottomState == ControlBoxState.PressedLeave)
                    {
                        this.SysBottomState = ControlBoxState.Pressed;
                    }
                }
                else if (!this._mouseDown)
                {
                    this.SysBottomState = ControlBoxState.Normal;
                }
                else if (this.SysBottomState == ControlBoxState.Pressed)
                {
                    this.SysBottomState = ControlBoxState.PressedLeave;
                }
            }
            if (toolTip != string.Empty)
            {
                this.HideToolTip();
                this.ShowTooTip(toolTip);
            }
            if (hide)
            {
                this.HideToolTip();
            }
        }

        public void ProcessMouseOperate(Point mousePoint, MouseOperate operate)
        {
            if (this._owner.ControlBox)
            {
                Rectangle closeBoxRect = this.CloseBoxRect;
                Rectangle minimizeBoxRect = this.MinimizeBoxRect;
                Rectangle maximizeBoxRect = this.MaximizeBoxRect;
                Rectangle sysbottoneBoxRect = this.SysBottomRect;
                bool closeBoxVisibale = this.CloseBoxVisibale;
                bool minimizeBoxVisibale = this.MinimizeBoxVisibale;
                bool maximizeBoxVisibale = this.MaximizeBoxVisibale;
                bool sysbottomVisibale = this.SysBottomVisibale;
                switch (operate)
                {
                    case MouseOperate.Move:
                        this.ProcessMouseMove(mousePoint, closeBoxRect, minimizeBoxRect, maximizeBoxRect, sysbottoneBoxRect, closeBoxVisibale, minimizeBoxVisibale, maximizeBoxVisibale, sysbottomVisibale);
                        return;

                    case MouseOperate.Down:
                        this.ProcessMouseDown(mousePoint, closeBoxRect, minimizeBoxRect, maximizeBoxRect, sysbottoneBoxRect, closeBoxVisibale, minimizeBoxVisibale, maximizeBoxVisibale, sysbottomVisibale);
                        return;

                    case MouseOperate.Up:
                        this.ProcessMouseUP(mousePoint, closeBoxRect, minimizeBoxRect, maximizeBoxRect, sysbottoneBoxRect, closeBoxVisibale, minimizeBoxVisibale, maximizeBoxVisibale, sysbottomVisibale);
                        return;

                    case MouseOperate.Leave:
                        this.ProcessMouseLeave(closeBoxVisibale, minimizeBoxVisibale, maximizeBoxVisibale, sysbottomVisibale);
                        return;

                    case MouseOperate.Hover:
                        return;
                }
            }
        }

        private void ProcessMouseUP(Point mousePoint, Rectangle closeBoxRect, Rectangle minimizeBoxRect, Rectangle maximizeBoxRect, Rectangle sysbottomRect, bool closeBoxVisibale, bool minimizeBoxVisibale, bool maximizeBoxVisibale, bool sysbottomVisible)
        {
            this._mouseDown = false;
            if (closeBoxVisibale)
            {
                if (closeBoxRect.Contains(mousePoint) && (this.CloseBoxState == ControlBoxState.Pressed))
                {
                    this._owner.Close();
                    this.CloseBoxState = ControlBoxState.Normal;
                    return;
                }
                this.CloseBoxState = ControlBoxState.Normal;
            }
            if (minimizeBoxVisibale)
            {
                if (minimizeBoxRect.Contains(mousePoint) && (this.MinimizeBoxState == ControlBoxState.Pressed))
                {
                    if (this._owner.ShowInTaskbar)
                    {
                        this._owner.WindowState = FormWindowState.Minimized;
                    }
                    else
                    {
                        this._owner.Hide();
                    }
                    this.MinimizeBoxState = ControlBoxState.Normal;
                    return;
                }
                this.MinimizeBoxState = ControlBoxState.Normal;
            }
            if (sysbottomVisible)
            {
                if (sysbottomRect.Contains(mousePoint) && (this.SysBottomState == ControlBoxState.Pressed))
                {
                    this._owner.SysbottomAv(this._owner);
                    this.SysBottomState = ControlBoxState.Normal;
                    return;
                }
                this.MinimizeBoxState = ControlBoxState.Normal;
            }
            if (maximizeBoxVisibale)
            {
                if (maximizeBoxRect.Contains(mousePoint) && (this.MaximizeBoxState == ControlBoxState.Pressed))
                {
                    if (this._owner.WindowState == FormWindowState.Maximized)
                    {
                        this._owner.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        this._owner.WindowState = FormWindowState.Maximized;
                    }
                    this.MaximizeBoxState = ControlBoxState.Normal;
                }
                else
                {
                    this.MaximizeBoxState = ControlBoxState.Normal;
                }
            }
        }

        private void ShowTooTip(string toolTipText)
        {
            if (this._owner != null)
            {
                this._owner.ToolTip.Active = true;
                this._owner.ToolTip.SetToolTip(this._owner, toolTipText);
            }
        }

        public Rectangle CloseBoxRect
        {
            get
            {
                if (this.CloseBoxVisibale)
                {
                    Point offset = this.ControlBoxOffset;
                    Size size = this._owner.CloseBoxSize;
                    return new Rectangle((this._owner.Width - offset.X) - size.Width, offset.Y, size.Width, size.Height);
                }
                return Rectangle.Empty;
            }
        }

        public ControlBoxState CloseBoxState
        {
            get
            {
                return this._closBoxState;
            }
            protected set
            {
                if (this._closBoxState != value)
                {
                    this._closBoxState = value;
                    if (this._owner != null)
                    {
                        this.Invalidate(this.CloseBoxRect);
                    }
                }
            }
        }

        public bool CloseBoxVisibale
        {
            get
            {
                return this._owner.ControlBox;
            }
        }

        public Point ControlBoxOffset
        {
            get
            {
                return this._owner.ControlBoxOffset;
            }
        }

        public int ControlBoxSpace
        {
            get
            {
                return this._owner.ControlBoxSpace;
            }
        }

        public Rectangle MaximizeBoxRect
        {
            get
            {
                if (this.MaximizeBoxVisibale)
                {
                    Point offset = this.ControlBoxOffset;
                    Size size = this._owner.MaxSize;
                    return new Rectangle((this.CloseBoxRect.X - this.ControlBoxSpace) - size.Width, offset.Y, size.Width, size.Height);
                }
                return Rectangle.Empty;
            }
        }

        public ControlBoxState MaximizeBoxState
        {
            get
            {
                return this._maximizeBoxState;
            }
            protected set
            {
                if (this._maximizeBoxState != value)
                {
                    this._maximizeBoxState = value;
                    if (this._owner != null)
                    {
                        this.Invalidate(this.MaximizeBoxRect);
                    }
                }
            }
        }

        public bool MaximizeBoxVisibale
        {
            get
            {
                return (this._owner.ControlBox && this._owner.MaximizeBox);
            }
        }

        public Rectangle MinimizeBoxRect
        {
            get
            {
                if (this.MinimizeBoxVisibale)
                {
                    Point offset = this.ControlBoxOffset;
                    Size size = this._owner.MiniSize;
                    return new Rectangle(this.MaximizeBoxVisibale ? ((this.MaximizeBoxRect.X - this.ControlBoxSpace) - size.Width) : ((this.CloseBoxRect.X - this.ControlBoxSpace) - size.Width), offset.Y, size.Width, size.Height);
                }
                return Rectangle.Empty;
            }
        }

        public ControlBoxState MinimizeBoxState
        {
            get
            {
                return this._minimizeBoxState;
            }
            protected set
            {
                if (this._minimizeBoxState != value)
                {
                    this._minimizeBoxState = value;
                    if (this._owner != null)
                    {
                        this.Invalidate(this.MinimizeBoxRect);
                    }
                }
            }
        }

        public bool MinimizeBoxVisibale
        {
            get
            {
                return (this._owner.ControlBox && this._owner.MinimizeBox);
            }
        }

        public Rectangle SysBottomRect
        {
            get
            {
                if (this.SysBottomVisibale)
                {
                    Point offset = this.ControlBoxOffset;
                    Size size = this._owner.SysBottomSize;
                    return new Rectangle(this.MinimizeBoxVisibale ? ((this.MinimizeBoxRect.X - this.ControlBoxSpace) - size.Width) : (this.MaximizeBoxVisibale ? ((this.MaximizeBoxRect.X - this.ControlBoxSpace) - size.Width) : ((this.CloseBoxRect.X - this.ControlBoxSpace) - size.Width)), offset.Y, size.Width, size.Height);
                }
                return Rectangle.Empty;
            }
        }

        public ControlBoxState SysBottomState
        {
            get
            {
                return this._SysBottomState;
            }
            protected set
            {
                if (this._SysBottomState != value)
                {
                    this._SysBottomState = value;
                    if (this._owner != null)
                    {
                        this.Invalidate(this.SysBottomRect);
                    }
                }
            }
        }

        public bool SysBottomVisibale
        {
            get
            {
                return this._owner.SysBottomVisibale;
            }
        }
    }
}

