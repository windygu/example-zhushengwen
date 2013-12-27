namespace CCWin.SkinControl
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(RichTextBox))]
    public class SkinRichTextBox : RichTextBox
    {
        private Dictionary<int, REOBJECT> _oleObjectList;
        private CCWin.SkinControl.RichEditOle _richEditOle;

        public bool InsertImageUseGifBox(string path)
        {
            try
            {
                SkinGifBox gif = new SkinGifBox();
                gif.BackColor = base.BackColor;
                gif.Image = Image.FromFile(path);
                this.RichEditOle.InsertControl(gif);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Dictionary<int, REOBJECT> OleObjectList
        {
            get
            {
                if (this._oleObjectList == null)
                {
                    this._oleObjectList = new Dictionary<int, REOBJECT>(10);
                }
                return this._oleObjectList;
            }
        }

        public CCWin.SkinControl.RichEditOle RichEditOle
        {
            get
            {
                if ((this._richEditOle == null) && base.IsHandleCreated)
                {
                    this._richEditOle = new CCWin.SkinControl.RichEditOle(this);
                }
                return this._richEditOle;
            }
        }
    }
}

