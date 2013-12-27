namespace CCWin.SkinControl
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    [ToolboxBitmap(typeof(RichTextBox))]
    public class RtfRichTextBox : RichTextBox
    {
        private Dictionary<string, Bitmap> emotions;
        private static bool hasGdiPlus;
        private RtfColor highlightColor;
        private const string RTF_HEADER = @"{\rtf1\ansi\ansicpg936\deff0\deflang1033\deflangfe2052";
        private Dictionary<RtfColor, string> rtfColor;
        private Dictionary<string, string> rtfFontFamily;
        private RtfColor textColor;
        private float xDpi;
        private float yDpi;

        static RtfRichTextBox()
        {
            try
            {
                GdipEmfToWmfBits(IntPtr.Zero, 0, null, 0, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
                hasGdiPlus = true;
            }
            catch (Exception)
            {
            }
        }

        public RtfRichTextBox()
        {
            this.highlightColor = RtfColor.White;
            this.emotions = new Dictionary<string, Bitmap>();
            this.rtfColor = new Dictionary<RtfColor, string>();
            this.rtfFontFamily = new Dictionary<string, string>();
            this.rtfColor.Add(RtfColor.Aqua, @"\red0\green255\blue255");
            this.rtfColor.Add(RtfColor.Black, @"\red0\green0\blue0");
            this.rtfColor.Add(RtfColor.Blue, @"\red0\green0\blue255");
            this.rtfColor.Add(RtfColor.Fuchsia, @"\red255\green0\blue255");
            this.rtfColor.Add(RtfColor.Gray, @"\red128\green128\blue128");
            this.rtfColor.Add(RtfColor.Green, @"\red0\green128\blue0");
            this.rtfColor.Add(RtfColor.Lime, @"\red0\green255\blue0");
            this.rtfColor.Add(RtfColor.Maroon, @"\red128\green0\blue0");
            this.rtfColor.Add(RtfColor.Navy, @"\red0\green0\blue128");
            this.rtfColor.Add(RtfColor.Olive, @"\red128\green128\blue0");
            this.rtfColor.Add(RtfColor.Purple, @"\red128\green0\blue128");
            this.rtfColor.Add(RtfColor.Red, @"\red255\green0\blue0");
            this.rtfColor.Add(RtfColor.Silver, @"\red192\green192\blue192");
            this.rtfColor.Add(RtfColor.Teal, @"\red0\green128\blue128");
            this.rtfColor.Add(RtfColor.White, @"\red255\green255\blue255");
            this.rtfColor.Add(RtfColor.Yellow, @"\red255\green255\blue0");
            this.rtfFontFamily.Add(FontFamily.GenericMonospace.Name, @"\fmodern");
            this.rtfFontFamily.Add(FontFamily.GenericSansSerif.Name, @"\fswiss");
            this.rtfFontFamily.Add(FontFamily.GenericSerif.Name, @"\froman");
            this.rtfFontFamily.Add("UNKNOWN", @"\fnil");
            using (Graphics graphics = base.CreateGraphics())
            {
                this.xDpi = graphics.DpiX;
                this.yDpi = graphics.DpiY;
            }
        }

        public RtfRichTextBox(RtfColor _textColor) : this()
        {
            this.textColor = _textColor;
        }

        public RtfRichTextBox(RtfColor _textColor, RtfColor _highlightColor) : this()
        {
            this.textColor = _textColor;
            this.highlightColor = _highlightColor;
        }

        public void AppendRtf(string _rtf)
        {
            base.Select(this.TextLength, 0);
            base.SelectionColor = Color.Black;
            base.SelectedRtf = _rtf;
        }

        public void AppendTextAsRtf(string _text)
        {
            this.AppendTextAsRtf(_text, this.Font);
        }

        public void AppendTextAsRtf(string _text, Font _font)
        {
            this.AppendTextAsRtf(_text, _font, this.textColor);
        }

        public void AppendTextAsRtf(string _text, Font _font, RtfColor _textColor)
        {
            this.AppendTextAsRtf(_text, _font, _textColor, this.highlightColor);
        }

        public void AppendTextAsRtf(string _text, Font _font, RtfColor _textColor, RtfColor _backColor)
        {
            base.Select(this.TextLength, 0);
            this.InsertTextAsRtf(_text, _font, _textColor, _backColor);
        }

        [DllImport("gdiplus.dll")]
        private static extern uint GdipEmfToWmfBits(IntPtr _hEmf, uint _bufferSize, byte[] _buffer, int _mappingMode, EmfToWmfBitsFlags _flags);
        private string GetColorTable(RtfColor _textColor, RtfColor _backColor)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"{\colortbl ;");
            builder.Append(this.rtfColor[_textColor]);
            builder.Append(";");
            builder.Append(this.rtfColor[_backColor]);
            builder.Append(@";}\n");
            return builder.ToString();
        }

        private string GetDocumentArea(string _text, Font _font)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"\viewkind4\uc1\pard\cf1\f0\fs20");
            builder.Append(@"\highlight2");
            if (_font.Bold)
            {
                builder.Append(@"\b");
            }
            if (_font.Italic)
            {
                builder.Append(@"\i");
            }
            if (_font.Strikeout)
            {
                builder.Append(@"\strike");
            }
            if (_font.Underline)
            {
                builder.Append(@"\ul");
            }
            builder.Append(@"\f0");
            builder.Append(@"\fs");
            builder.Append((int) Math.Round((double) (2f * _font.SizeInPoints)));
            builder.Append(" ");
            builder.Append(_text.Replace("\n", @"\par "));
            builder.Append(@"\highlight0");
            if (_font.Bold)
            {
                builder.Append(@"\b0");
            }
            if (_font.Italic)
            {
                builder.Append(@"\i0");
            }
            if (_font.Strikeout)
            {
                builder.Append(@"\strike0");
            }
            if (_font.Underline)
            {
                builder.Append(@"\ulnone");
            }
            builder.Append(@"\f0");
            builder.Append(@"\fs20");
            builder.Append(@"\cf0\fs17}");
            return builder.ToString();
        }

        private string GetFontTable(Font _font)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"{\fonttbl{\f0");
            builder.Append(@"\");
            if (this.rtfFontFamily.ContainsKey(_font.FontFamily.Name))
            {
                builder.Append(this.rtfFontFamily[_font.FontFamily.Name]);
            }
            else
            {
                builder.Append(this.rtfFontFamily["UNKNOWN"]);
            }
            builder.Append(@"\fcharset134 ");
            builder.Append(_font.Name);
            builder.Append(";}}");
            return builder.ToString();
        }

        private string GetImagePrefix(Image _image)
        {
            StringBuilder builder = new StringBuilder();
            int picw = (int) Math.Round((double) ((((float) _image.Width) / this.xDpi) * 2540f));
            int pich = (int) Math.Round((double) ((((float) _image.Height) / this.yDpi) * 2540f));
            int picwgoal = (int) Math.Round((double) ((((float) _image.Width) / this.xDpi) * 1440f));
            int pichgoal = (int) Math.Round((double) ((((float) _image.Height) / this.yDpi) * 1440f));
            builder.Append(@"{\pict\wmetafile8");
            builder.Append(@"\picw");
            builder.Append(picw);
            builder.Append(@"\pich");
            builder.Append(pich);
            builder.Append(@"\picwgoal");
            builder.Append(picwgoal);
            builder.Append(@"\pichgoal");
            builder.Append(pichgoal);
            builder.Append(" ");
            return builder.ToString();
        }

        private string GetRtfImage(Image _image)
        {
            MemoryStream stream = null;
            Graphics graphics = null;
            Metafile image = null;
            string ret;
            try
            {
                stream = new MemoryStream();
                using (graphics = base.CreateGraphics())
                {
                    IntPtr hdc = graphics.GetHdc();
                    image = new Metafile(stream, hdc);
                    graphics.ReleaseHdc(hdc);
                }
                using (graphics = Graphics.FromImage(image))
                {
                    graphics.DrawImage(_image, new Rectangle(0, 0, _image.Width, _image.Height));
                }
                IntPtr henhmetafile = image.GetHenhmetafile();
                uint num = GdipEmfToWmfBits(henhmetafile, 0, null, 1, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
                byte[] buffer = new byte[num];
                GdipEmfToWmfBits(henhmetafile, num, buffer, 1, EmfToWmfBitsFlags.EmfToWmfBitsFlagsDefault);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < buffer.Length; i++)
                {
                    builder.Append(string.Format("{0:X2}", buffer[i]));
                }
                ret = builder.ToString();
            }
            finally
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                }
                if (image != null)
                {
                    image.Dispose();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return ret;
        }

        public void InsertEmotion()
        {
            if (hasGdiPlus)
            {
                foreach (string emoticon in this.emotions.Keys)
                {
                    int start = base.Find(emoticon, RichTextBoxFinds.None);
                    if (start > -1)
                    {
                        base.Select(start, emoticon.Length);
                        this.InsertImage(this.emotions[emoticon]);
                    }
                }
            }
        }

        public void InsertImage(Image _image)
        {
            if (hasGdiPlus)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(@"{\rtf1\ansi\ansicpg936\deff0\deflang1033\deflangfe2052");
                builder.Append(this.GetFontTable(this.Font));
                builder.Append(this.GetImagePrefix(_image));
                builder.Append(this.GetRtfImage(_image));
                builder.Append("}");
                base.SelectedRtf = builder.ToString();
            }
        }

        public void InsertRtf(string _rtf)
        {
            base.SelectedRtf = _rtf;
        }

        public void InsertTextAsRtf(string _text)
        {
            this.InsertTextAsRtf(_text, this.Font);
        }

        public void InsertTextAsRtf(string _text, Font _font)
        {
            this.InsertTextAsRtf(_text, _font, this.textColor);
        }

        public void InsertTextAsRtf(string _text, Font _font, RtfColor _textColor)
        {
            this.InsertTextAsRtf(_text, _font, _textColor, this.highlightColor);
        }

        public void InsertTextAsRtf(string _text, Font _font, RtfColor _textColor, RtfColor _backColor)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(@"{\rtf1\ansi\ansicpg936\deff0\deflang1033\deflangfe2052");
            builder.Append(this.GetFontTable(_font));
            builder.Append(this.GetColorTable(_textColor, _backColor));
            builder.Append(this.GetDocumentArea(_text, _font));
            base.SelectedRtf = builder.ToString();
        }

        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string lpFileName);
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
        }

        private string RemoveBadChars(string _originalRtf)
        {
            return _originalRtf.Replace("\0", "");
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams prams = base.CreateParams;
                if (LoadLibrary("msftedit.dll") != IntPtr.Zero)
                {
                    prams.ExStyle |= 0x20;
                    prams.ClassName = "RICHEDIT50W";
                }
                return prams;
            }
        }

        public Dictionary<string, Bitmap> Emotions
        {
            get
            {
                return this.emotions;
            }
        }

        public bool HasEmotion
        {
            get
            {
                if (hasGdiPlus)
                {
                    foreach (string emoticon in this.emotions.Keys)
                    {
                        if (this.Text.IndexOf(emoticon, StringComparison.CurrentCultureIgnoreCase) > -1)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public RtfColor HiglightColor
        {
            get
            {
                return this.highlightColor;
            }
            set
            {
                this.highlightColor = value;
            }
        }

        public string Rtf
        {
            get
            {
                return this.RemoveBadChars(base.Rtf);
            }
            set
            {
                base.Rtf = value;
            }
        }

        public RtfColor TextColor
        {
            get
            {
                return this.textColor;
            }
            set
            {
                this.textColor = value;
            }
        }

        [Flags]
        private enum EmfToWmfBitsFlags
        {
            EmfToWmfBitsFlagsDefault = 0,
            EmfToWmfBitsFlagsEmbedEmf = 1,
            EmfToWmfBitsFlagsIncludePlaceable = 2,
            EmfToWmfBitsFlagsNoXORClip = 4
        }

        public enum RtfColor
        {
            Black,
            Maroon,
            Green,
            Olive,
            Navy,
            Purple,
            Teal,
            Gray,
            Silver,
            Red,
            Lime,
            Yellow,
            Blue,
            Fuchsia,
            Aqua,
            White
        }
    }
}

