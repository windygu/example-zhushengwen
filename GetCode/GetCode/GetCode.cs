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

namespace GetCodes
{
    using System.Net;
    using System.IO;
    using System.Data.OleDb;
    using DotNet.Framework.Common.Algorithm;
    using System.Collections;

    public partial class GetCode : Form
    {
        OleDbConnection DbConn;
        OleDbCommand DbCmd;

        public GetCode()
        {
            InitializeComponent();
            DbConn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source=" + Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\")) + @"\code.mdb;");
            DbCmd = new OleDbCommand();

            try
            {
                DbConn.Open();
                DbCmd.Connection = this.DbConn;
                DbCmd.CommandType = CommandType.Text;
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bitmap1 = GetSourceCode(textBox1.Text);
            pbSource.Image = bitmap1;

            Bitmap bitmap = (Bitmap)bitmap1.Clone();
            UnCodebase ud = new UnCodebase(bitmap);
            bitmap = ud.GrayByPixels();
            if (cbquzao.Checked)
            {
                ud.ClearNoise(int.Parse(updgary.Value.ToString()), int.Parse(updmaxpoint.Value.ToString()));
                //bitmap = ClearNoise(bitmap, 128, 1);
            }
            bitmap = ud.ReSetBitMap();
            //bitmap = new UnCodebase(bitmap).ClearPicBorder(2);
            pbhuidu.Image = bitmap;

            bitmap = CutMap(bitmap);

            Bitmap[] arrmap = SplitImg(bitmap, 5, 1);

            DisplaySplitImg(arrmap);

            textBox6.Text = DrawCode(arrmap);


        }



        private void DisplaySplitImg(Bitmap[] arrmap)
        {
            for (int i = 0; i < arrmap.Length; i++)
            {
                foreach (Control item in groupbox1.Controls)
                {
                    if (item is PictureBox)
                    {
                        PictureBox p = item as PictureBox;
                        int index = i + 1;
                        if (p.Name == "pb" + index)
                        {
                            p.Image = arrmap[i];
                            break;
                        }
                    }
                }
            }
        }

        private string DrawCode(Bitmap[] arrmap)
        {
            groupBox2.Controls.Clear();
            //groupBox2.Refresh();
            StringBuilder code = new StringBuilder();
            Panel p;
            string spchar = "=======";
            for (int i = 0; i < arrmap.Length; i++)
            {
                p = new Panel();
                p.BackColor = Color.White;
                p.Width = 100;
                p.Height = 100;
                p.Location = new Point(i * p.Width + 10, 40);

                groupBox2.Controls.Add(p);
                if (i == arrmap.Length - 1)
                {
                    spchar = string.Empty;
                }
                code.Append(GetCodebybitmap(arrmap[i], p) + spchar);
            }
            return code.ToString();
        }

        /// <summary>
        /// 获取数字对应的二值化代码
        /// </summary>
        /// <param name="_bitmap"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private string GetCodebybitmap(Bitmap _bitmap, Panel p)
        {
            StringBuilder code = new StringBuilder();
            Graphics g = p.CreateGraphics();
            for (int i = 0; i < _bitmap.Width; i++)
            {
                for (int j = 0; j < _bitmap.Height; j++)
                {
                    int r = _bitmap.GetPixel(i, j).R;
                    if (r < 100)//常用的是灰度128
                    {
                        code.Append("1");
                        g.DrawString("-", new Font("宋体", 12), new SolidBrush(Color.Blue), new Rectangle(i * 5, j * 5, 12, 12));
                    }
                    else
                    {
                        code.Append("0");
                    }
                }
            }

            return code.ToString();

        }

        /// <summary>
        /// 切分图片
        /// </summary>
        /// <param name="_bitmap"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 得到图片有效区域，使切分图片更加精确
        /// </summary>
        /// <param name="_bitmap"></param>
        /// <returns></returns>
        private Bitmap CutMap(Bitmap _bitmap)
        {
            Rectangle rg = new Rectangle(int.Parse(updX.Value.ToString()), int.Parse(updY.Value.ToString()), _bitmap.Width - int.Parse(updW.Value.ToString()), _bitmap.Height - int.Parse(updH.Value.ToString()));
            //Rectangle rg = new Rectangle(1, 0, _bitmap.Width - 10, _bitmap.Height );
            Bitmap bitmap = _bitmap.Clone(rg, _bitmap.PixelFormat);
            return bitmap;

        }

        private Bitmap GetSourceCode(string url)
        {
            Bitmap bitmap = null;
            if (url.StartsWith("http"))
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream st = response.GetResponseStream();
                bitmap = (Bitmap)Bitmap.FromStream(st);
            }
            else
            {
                bitmap = (Bitmap)Bitmap.FromFile(url);
            }

            //bitmap.Save(System.Windows.Forms.Application.StartupPath + @"\tmp.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //bitmap = (Bitmap)Bitmap.FromFile(System.Windows.Forms.Application.StartupPath + @"\tmp.bmp");
            return bitmap;
        }



        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string[] arrv = textBox6.Text.Split(new string[] { "=======" }, StringSplitOptions.RemoveEmptyEntries);

                string sql = "insert into Codes(Keynum,CodeValue) values('{0}','{1}')";
                foreach (char item in textBox2.Text)
                {
                    sql = "insert into Codes(Keynum,CodeValue) values('{0}','{1}')";
                    sql = string.Format(sql, item, arrv[0]);
                    DbCmd.CommandText = sql;
                    DbCmd.ExecuteNonQuery();
                }

                //if (!string.IsNullOrEmpty(txt1.Text))
                //{

                //    sql = string.Format(sql, txt1.Text, arrv[0]);
                //    DbCmd.CommandText = sql;
                //    DbCmd.ExecuteNonQuery();
                //}

                //if (!string.IsNullOrEmpty(txt2.Text))
                //{

                //    sql = string.Format(sql, txt2.Text, arrv[1]);
                //    DbCmd.CommandText = sql;
                //    DbCmd.ExecuteNonQuery();
                //}

                //if (!string.IsNullOrEmpty(txt3.Text))
                //{
                //    sql = "insert into Codes(Keynum,CodeValue) values('{0}','{1}')";
                //    sql = string.Format(sql, txt3.Text, arrv[2]);
                //    DbCmd.CommandText = sql;
                //    DbCmd.ExecuteNonQuery();
                //}

                //if (!string.IsNullOrEmpty(txt4.Text))
                //{
                //    sql = "insert into Codes(Keynum,CodeValue) values('{0}','{1}')";
                //    sql = string.Format(sql, txt4.Text, arrv[3]);
                //    DbCmd.CommandText = sql;
                //    DbCmd.ExecuteNonQuery();
                //}
                //if (!string.IsNullOrEmpty(txt5.Text))
                //{
                //    sql = "insert into Codes(Keynum,CodeValue) values('{0}','{1}')";
                //    sql = string.Format(sql, txt5.Text, arrv[4]);
                //    DbCmd.CommandText = sql;
                //    DbCmd.ExecuteNonQuery();
                //}

                //txt1.Text = txt2.Text = txt3.Text = txt4.Text = txt5.Text = string.Empty;
                //MessageBox.Show("入库成功");
                button1_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox6.Text = "";
            StringBuilder sb = new StringBuilder();
            button1_Click(null, null);
            string[] arrv = textBox6.Text.Split(new string[] { "=======" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < arrv.Length; i++)
            {
                sb.Append(new Compar(arrv[i]).GetCode());
            }
            txtcode.Text = sb.ToString();
        }

        private void updW_ValueChanged(object sender, EventArgs e)
        {
            if (updW.Value < updX.Value)
            {
                MessageBox.Show("图片左边沿到第一个字的宽度不能小于X的值");
                updW.Value++;
                return;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length == 5)
            {
                button2_Click(null, null);
                textBox2.Text = "";
            }
        }
    }

    public class SingleCodes
    {
        private DataSet ds;
        OleDbConnection DbConn;
        OleDbCommand DbCmd;

        private void GetDS()
        {

            DbConn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;User ID=Admin;Data Source=" + Application.ExecutablePath.Substring(0, Application.ExecutablePath.LastIndexOf("\\")) + @"\code.mdb;");
            DbCmd = new OleDbCommand();

            try
            {
                ds = new DataSet();
                DbConn.Open();
                DbCmd.Connection = this.DbConn;
                DbCmd.CommandText = "select * from Codes";
                OleDbDataAdapter adapter = new OleDbDataAdapter(DbCmd);
                adapter.Fill(ds);

            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }
        }
        private SingleCodes()
        {
            GetDS();
        }
        public static SingleCodes Instance
        {
            get
            {
                return innerclass.instance;
            }
        }

        public DataTable DataTable
        {
            get
            {
                if (ds == null) GetDS();
                return ds.Tables[0];
            }
        }

        class innerclass
        {
            static innerclass() { }
            public static readonly SingleCodes instance = new SingleCodes();
        }
    }

    public class Compar
    {
        private string code01;
        SortedList sl;
        public Compar(string code01)
        {
            this.code01 = code01;
            sl = new SortedList();
        }

        public string GetCode()
        {
            LevenshteinDistance ld = new LevenshteinDistance();
            DataTable dt = SingleCodes.Instance.DataTable;
            string value = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                value = dt.Rows[i][2].ToString();
                decimal parent = ld.LevenshteinDistancePercent(value, code01);
                if (parent > 0.85m)
                {
                    if (!sl.ContainsKey(parent))
                    {
                        sl.Add(parent, dt.Rows[i][1]);
                    }
                }
            }

            return sl.GetByIndex(sl.Count - 1).ToString();
        }
    }
}
