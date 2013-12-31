using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using DotNet.Framework.Common.Algorithm;
using System.Collections;



namespace GetCodes
{


    public partial class ImageOpt : Form
    {

        Dictionary<int, SortedList> dic;
        public ImageOpt()
        {
            InitializeComponent();
            dic = new Dictionary<int, SortedList>();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Bitmap bitmap = (Bitmap) Bitmap.FromFile("OKK.png");
            UnCodebase ud = new UnCodebase(bitmap);
            bitmap = ud.GrayByPixels();
           ud.ClearNoise(128, 2);

            pictureBox1.Image = bitmap;
           
            tessnet2.Tesseract ocr = new tessnet2.Tesseract();//声明一个OCR类
            ocr.SetVariable("tessedit_char_whitelist", "0123456789ABCDEFGHIGKLMNOPQRSTUVWXYZ"); //设置识别变量，当前只能识别数字。
            string path = Application.StartupPath + @"\tmpe\";
            ocr.Init(path, "eng", false); //应用当前语言包。注，Tessnet2是支持多国语的。语言包下载链接：http://code.google.com/p/tesseract-ocr/downloads/list
            List<tessnet2.Word> result = ocr.DoOCR(bitmap, Rectangle.Empty);//执行识别操作
            string code = result[0].Text;
            textBox1.Text = code;

            return;

            //System.Drawing.Bitmap img = Run();

            //UnCodebase ud = new UnCodebase(img);
            //img = ud.GrayByPixels();
            //ud.ClearNoise(128, 2);

            //pictureBox1.Image = img;

            //tessnet2.Tesseract ocr = new tessnet2.Tesseract();//声明一个OCR类
            //ocr.SetVariable("tessedit_char_whitelist", "0123456789"); //设置识别变量，当前只能识别数字。
            //ocr.Init(Application.StartupPath + @"\\tmpe", "eng", true); //应用当前语言包。注，Tessnet2是支持多国语的。语言包下载链接：http://code.google.com/p/tesseract-ocr/downloads/list
            //List<tessnet2.Word> result = ocr.DoOCR(img, Rectangle.Empty);//执行识别操作
            //string code = result[0].Text;
            //textBox1.Text = code;
        }

        private Bitmap Run()
        {
            WebRequest request = WebRequest.Create("http://sz.2zf.cn/js/code2.asp");
            WebResponse response = request.GetResponse();
            Stream st = response.GetResponseStream();
            Bitmap bitmap = (Bitmap)Bitmap.FromStream(st);

            foreach (Control item in this.Controls)
            {
                if (item is Panel)
                {
                    this.Controls.Remove(item);
                }
            }
            UnCodebase ud = new UnCodebase(bitmap);
            ud.GrayByPixels();
            ud.ClearNoise(128, 1);
            //("http://localhost:7788/R.home?temp=gv4xa06r");
            //Bitmap bitmap = (Bitmap)Bitmap.FromFile("untitled.bmp");
            //bitmap = ClickExtracted(bitmap);

            //bitmap = ClearNoise(bitmap, 128, 2);

            //bitmap = CutMap(bitmap);

            Bitmap[] arrmap = SplitImg(bitmap, 4, 1);
            PictureBox pb;
            for (int i = 0; i < arrmap.Length; i++)
            {
                pb = new PictureBox();
                pb.Name = i.ToString();
                pb.Image = arrmap[i]; //ClearNoise(arrmap[i], 128, 1);
                pb.Location = new Point(i * 100, 100);
                this.Controls.Add(pb);
            }
            Panel p;
            this.groupBox1.Controls.Clear();
            for (int i = 0; i < arrmap.Length; i++)
            {
                p = new Panel();
                p.BackColor = Color.White;
                p.Width = 100;
                p.Height = 100;
                p.Location = new Point(i * p.Width + 10, 40);
                this.groupBox1.Controls.Add(p);
                GetCode(arrmap[i], p);
            }

            //for (int i = 0; i < arrmap.Length; i++)
            //{
            //    SaveCode(arrmap[i]);
            //}
            dic.Clear();
            StringBuilder code = new StringBuilder();
            for (int i = 0; i < arrmap.Length; i++)
            {
                //code.Append(GetCode(GetCode01(arrmap[i])));
                GetCode(GetCode01(arrmap[i]));
            }

            foreach (var item in dic.Keys)
            {
                SortedList s = dic[item];
                if (s.Count > 0)
                {
                    code.Append(s.GetByIndex(s.Count - 1));
                }

            }
            // textBox1.Text = code.ToString();

            return bitmap;
        }


        int n = -1;

        private string GetCode(string code01)
        {
            n++;
            SortedList s = new SortedList();
            string code = string.Empty;
            string[] arrcode = Consts.ArrCode();
            LevenshteinDistance ld = new LevenshteinDistance();
            decimal parcent;
            for (int i = 0; i < arrcode.Length; i++)
            {
                parcent = ld.LevenshteinDistancePercent(arrcode[i], code01);
                Console.WriteLine("parcent{1}={0}", parcent, n);
                if (parcent > 0.8m)
                {
                    if (!s.ContainsKey(parcent))
                        s.Add(parcent, i);
                    //code = i.ToString();
                    //break;
                }
            }
            dic.Add(n, s);
            return code;
        }

        private string GetCode01(Bitmap _bitmap)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _bitmap.Width; i++)
            {
                for (int j = 0; j < _bitmap.Height; j++)
                {
                    int r = _bitmap.GetPixel(i, j).R;
                    if (r < 100)//常用的是灰度128
                    {
                        sb.Append("1");
                    }
                    else
                    {
                        sb.Append("0");
                    }
                }
            }
            return sb.ToString();
        }

        private void SaveCode(Bitmap _bitmap)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _bitmap.Width; i++)
            {
                for (int j = 0; j < _bitmap.Height; j++)
                {
                    int r = _bitmap.GetPixel(i, j).R;
                    if (r < 128)//常用的是灰度128
                    {
                        sb.Append("1");
                    }
                    else
                    {
                        sb.Append("0");
                    }
                }
            }


        }

        private void GetCode(Bitmap _bitmap, Panel p)
        {
            Graphics g = p.CreateGraphics();
            for (int i = 0; i < _bitmap.Width; i++)
            {
                for (int j = 0; j < _bitmap.Height; j++)
                {
                    int r = _bitmap.GetPixel(i, j).R;
                    if (r < 100)//常用的是灰度128
                    {
                        g.DrawString("-", new Font("宋体", 12), new SolidBrush(Color.Blue), new Rectangle(i * 5, j * 5, 12, 12));
                    }
                }
            }

        }

        private Bitmap CutMap(Bitmap _bitmap)
        {
            Rectangle rg = new Rectangle(1, 0, _bitmap.Width - 10, _bitmap.Height);
            Bitmap bitmap = _bitmap.Clone(rg, _bitmap.PixelFormat);
            return bitmap;

        }


        private Bitmap[] SplitImg(Bitmap _bitmap, int row, int col)
        {
            int singW = _bitmap.Width / row;
            int singH = _bitmap.Height / col;
            Bitmap[] arrmap = new Bitmap[row * col];
            Rectangle rect;
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    rect = new Rectangle(j * singW, i * singH, singW, singH);
                    arrmap[i * row + j] = _bitmap.Clone(rect, _bitmap.PixelFormat);
                }
            }
            return arrmap;
        }

    }

    public class Consts
    {
        public static string[] ArrCode()
        {
            string[] a = new string[] {   "00100000000000000000000000000000000000000000000000000000000000000000000011100000000000001111011110000000001111110001110000000111000000001100000011000000000011000001100000000000110000011000000000011000000110000000111110000001110001101110000000001111111100000000"//0                                         
                                        ,"00000000000000000000000000000000000000000000000000000000000000000000000000000000000000100000000000000000011000000000000000001100000000000000000111011111100000000001111111010111100000111100000001011000001000000000000000000000000000000000000000000000000000000000"//1
                                        ,"00000000000000000000000000000000000000000000000000000000000000000000000000100000000000000000111100000001100000011011000000111000000100110000011000000011001100000110000001100011000001100000110000110000011000001000001100000111011100000011000000111110000000010000"//2
                                        ,"00000000000000000000000000000000000000000000010000000000000000001100000001000000000110000000011000000011000000000011000000110000110000111000001100001100000110000011100111000001100000011111011000011000000100000011111100000000000000011111000000000000000000100000"//3
                                        ,"00000000000000000000000000000000000000000000000000000000000000000000000011100000000000000000011000000000000000110110000000000000011001100000000000011000011000000000101100000111110000011110000011100000000001111111011000000000000000000110000000000000000000000000"//4
                                        ,"00000000000000000000000000000000000000000000000110001000000000000001100011000000000111111000011000000011110100000111000001100011000000110000011000110000001100000110001110000011000001100011110011110000011000001111111000000110000000000100000001100000000000000000"//5
                                        ,"00000000000000000000000000000000000000000000000111111000000000000011111111100000000001100110011100000000110011000001100000011000110000001000000110001100000011000001100011000000110000011000011100011100000011100011101111000000011000000111000000000000000000000000"//6
                                        ,"00001100000000000000000000000000000000000000000000000000000000000000000000000000010000000000000000000110000000000011000001100000000011110000011000000011100000000110000011110000000001100001100000000000011001110000000000000110111000000000000001111000000000000000"//7
                                        ,"00000000000000000000000000000000000000000000000000000000000000000000000000000000001110000000000000000111011001111100000001100011111111110000011000011100011100000110000110000011000001110001100000110000001110111000001100000000111010000010000000000100011011100000"//8
                                        };
            return a;

        }

    }



}
