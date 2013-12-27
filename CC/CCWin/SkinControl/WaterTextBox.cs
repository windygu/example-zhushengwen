namespace CCWin.SkinControl
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(TextBox))]
    public class WaterTextBox : TextBox
    {
        private Color _waterColor = Color.FromArgb(0x7f, 0x7f, 0x7f);
        private string _waterText = string.Empty;

        private void WmPaintWater(ref Message m)
        {
            using (Graphics g = Graphics.FromHwnd(base.Handle))
            {
                if (((this.Text.Length == 0) && !string.IsNullOrEmpty(this._waterText)) && !this.Focused)
                {
                    TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter;
                    if (this.RightToLeft == RightToLeft.Yes)
                    {
                        flags |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
                    }
                    TextRenderer.DrawText(g, this._waterText, new Font("微软雅黑", 8.5f), base.ClientRectangle, this._waterColor, flags);
                }
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 15)
            {
                this.WmPaintWater(ref m);
            }
        }

        [Description("水印的颜色"), Category("Skin")]
        public Color WaterColor
        {
            get
            {
                return this._waterColor;
            }
            set
            {
                this._waterColor = value;
                base.Invalidate();
            }
        }

        [Category("Skin"), Description("水印文字")]
        public string WaterText
        {
            get
            {
                return this._waterText;
            }
            set
            {
                this._waterText = value;
                base.Invalidate();
            }
        }
    }
}

