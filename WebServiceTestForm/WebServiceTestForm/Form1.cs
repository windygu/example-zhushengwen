using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;


namespace WebServiceTestForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = MyClass.GetNowTime();
        }
        MethodInfo[] dataSource;
        string AssUrl = "";
        Assembly _WebServiceAssembly;
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> mpinfo = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        private void Form1_Load(object sender, EventArgs e)
        {
            /*  StringBuilder _Code = new StringBuilder();
              _Code.AppendLine("using System;");
              _Code.AppendLine("namespace TestName");
              _Code.AppendLine("{");
              _Code.AppendLine("public class TestClass");
              _Code.AppendLine("{");
              _Code.AppendLine("public string Test()");
              _Code.AppendLine("{");
              _Code.AppendLine("return \"Test\";");
              _Code.AppendLine("}");
              _Code.AppendLine("}");
              _Code.AppendLine("}");

              IList<string> _List = new List<string>();
              _List.Add("System.dll");

              Assembly _Assembly = Zgke.Run.GetAssembly.GetCodeAssembly(_Code.ToString(), _List);
              Type _Class =_Assembly.GetType("TestName.TestClass");
              MethodInfo _Method = _Class.GetMethod("Test");

              object _Object = Activator.CreateInstance(_Class);  
              object _Returun = _Method.Invoke(_Object, new object[] { });
              MessageBox.Show(_Returun.ToString());
             * */


            // MessageBox.Show(comboBox1.Text);
            // object _ReturnData = _Method.Invoke(_Object, new object[] {"",0,10,"true"});  
            string str = InitUrl();
            if (str != "")
                textBox1.Text = str;
            ValidUrl(textBox1);

        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            ValidUrl(sender);
        }

        private void ValidUrl(object sender)
        {
            TextBox tb = (TextBox)sender;
            Regex r = new Regex(@"^(http|https)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$", RegexOptions.IgnoreCase);
            Match m = r.Match(tb.Text); // 在字符串中匹配
            if (m.Success && tb.Text.ToLower().EndsWith(".asmx"))
            {
                SetText("WebService地址格式正确!"); //输入匹配字符的位置
                tb.Tag = true;
                tb.BackColor = SystemColors.Window;
                button1.Enabled = true;
                //  label1.ForeColor = Color.Blue;
            }
            else
            {
                SetText("WebService地址有误!");
                tb.Tag = false;
                tb.BackColor = Color.BurlyWood;
                button1.Enabled = false;
                //  label1.ForeColor = Color.Red;
            }

            //Set the state of the OK button
            ValidateOK();

        }
        private void ValidateOK()
        {
            //Set the OK button to enabled if all the Tags are true
            this.button1.Enabled = ((bool)(textBox1.Tag));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidUrl(sender);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetService();
            
        }
        public static string OsPath
        {
            get
            {
                return Path.Combine(System.Environment.OSVersion.Version.Major > 5 ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : Environment.GetFolderPath(Environment.SpecialFolder.System), "WST.ini");
            }
        }
        private void SaveUrl(string url)
        {

            FileInfo f = new FileInfo(OsPath);
            string content = "";
            if (f.Exists)
            {
                FileStream fs = new FileStream(OsPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                content = sr.ReadToEnd();
                if (content.Contains(url + "\r\n"))
                    content = content.Replace(url + "\r\n", "");
                else if (content.Contains(url))
                    content = content.Replace(url, "");
                sr.Close();
            }
            content = content.Trim();
            if (content != "")
                content += "\r\n";
            content += url;

            FileStream wn1 = f.Create();
            byte[] b = System.Text.Encoding.Default.GetBytes(content);
            wn1.Write(b, 0, b.Length);
            wn1.Close();
        }
        private string InitUrl()
        {
            if (!File.Exists(OsPath))
            {
                return "";
            }
            FileInfo fi = new FileInfo(OsPath);
            StreamReader sr = fi.OpenText();
            string temp = "";
            while (!sr.EndOfStream)
            {
                temp = sr.ReadLine();
                if (!textBox1.AutoCompleteCustomSource.Contains(temp))
                    textBox1.AutoCompleteCustomSource.Add(temp);
            }
            sr.Close();
            return temp;
        }
        /*      private void GetService()
              {
                  Thread t = new Thread(RunService);
                  t.Start();
                  textBox1.Enabled = false;
                  SetText("正在努力获取服务...");
                  Thread.Sleep(10000);
                  if (t.ThreadState != ThreadState.Stopped)
                  {
                      try
                      {
                          t.Abort();
                      }
                      catch (Exception)
                      {
                      }
                      SetText("获取服务超时...");
                      MessageBox.Show("在规定时间内远程无法连接!", "服务异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       textBox1.Enabled = true;
                  }
              }
         */
        private void GetService()
        {
            try
            {
                textBox1.Enabled = false;
                SetText("正在努力获取服务...");
                _WebServiceAssembly = Zgke.Run.GetAssembly.GetWebServiceAssembly(textBox1.Text + "?WSDL", "BMTWebServiceV1");
                SetText("获取服务成功!");
                
                textBox1.Enabled = true;
            }
            catch (Exception e)
            {
                SetText("获取服务发生异常!");
                MessageBox.Show(e.Message, "服务异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Enabled = true;
                return;
            }

            if (_WebServiceAssembly.GetTypes().Length == 0)
            {
                SetText("未发现任何程序集!");
                MessageBox.Show("程序集为空", "未发现任何程序集!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AssUrl = textBox1.Text;
            SaveUrl(textBox1.Text);
            if (!textBox1.AutoCompleteCustomSource.Contains(textBox1.Text))
                textBox1.AutoCompleteCustomSource.Add(textBox1.Text);

            button1.Enabled = false;

            Type _Type = _WebServiceAssembly.GetTypes()[0];

            dataSource = _Type.GetMethods()
                .Where(p => p.Name != "Discover" && p.Name != "GetType" && !p.IsVirtual && !(p.Name.StartsWith("Begin") || p.Name.StartsWith("End")))
                .Where(p => !p.Name.StartsWith("set_") && !p.Name.StartsWith("get_")).OrderBy(p => p.Name).ToArray();
            string[] strs = dataSource.Select(p => p.Name).ToArray();

            string showstr = "";
            if (strs.Contains(comboBox1.Text))
            {
                showstr = comboBox1.Text;
            }

            comboBox1.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBox1.DataSource = strs;

            if (showstr != "")
            {
                comboBox1.Text = showstr;
            }
            comboBox1_TextChanged(comboBox1, null);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void UserControlValue_TextChanged(object sender, EventArgs e)
        {
            if (!mpinfo.ContainsKey(AssUrl))
            {
                mpinfo[AssUrl] = new Dictionary<string, Dictionary<string, string>>();
            }
            if (!mpinfo[AssUrl].ContainsKey(comboBox1.Text))
            {
                mpinfo[AssUrl][comboBox1.Text] = new Dictionary<string, string>();
            }

            UserControl1 uc = (UserControl1)((Control)sender).Parent;
            mpinfo[AssUrl][comboBox1.Text][uc.p_Name.Text] = uc.p_value.Text;

        }
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            string str = cb.Text;
            if (dataSource.Select(p => p.Name).ToArray().Contains(str))
            {
                Type _Type = _WebServiceAssembly.GetTypes()[0];

                MethodInfo _Method = _Type.GetMethod(cb.Text);

                ParameterInfo[] pis = _Method.GetParameters();
                for (int x = 0; x < Controls.Count; x++)
                {
                    if (Controls.ContainsKey("Val" + x))
                        Controls.RemoveByKey("Val" + x);
                    else break;
                }
                for (int i = 0; i < pis.Length; i++)
                {
                    UserControl1 uc = new UserControl1();
                    uc.Name = "Val" + i;
                    uc.p_Name.Text = pis[i].Name;
                    uc.p_type.Text = pis[i].ParameterType.Name;
                    if (pis[i].ParameterType.Name == "String")
                    {
                    }
                    else if (pis[i].ParameterType.Name == "Boolean")
                    {
                        uc.p_value.Text = "true";
                    }
                    else
                    {
                        if (uc.p_Name.Text == "PageNum")
                        {
                            uc.p_value.Text = "0";
                        }
                        else if (uc.p_Name.Text == "Count")
                        {
                            uc.p_value.Text = "10";
                        }
                        else
                            uc.p_value.Text = "1";
                    }


                    if (mpinfo.ContainsKey(AssUrl))
                    {
                        if (mpinfo[AssUrl].ContainsKey(cb.Text))
                        {
                            if (mpinfo[AssUrl][cb.Text].ContainsKey(uc.p_Name.Text))
                                uc.p_value.Text = mpinfo[AssUrl][cb.Text][uc.p_Name.Text];
                        }
                    }
                    uc.Location = new Point(18 + i % 5 * (uc.Width + 2), 82 + i / 5 * (uc.Height + 2));
                    uc.p_value.KeyDown += comboBox1_KeyDown;
                    uc.p_value.TextChanged += UserControlValue_TextChanged;
                    Controls.Add(uc);
                }
                richTextBox1.Location = new Point(richTextBox1.Location.X, 82 + 43 * ((pis.Length + 4) / 5));
                richTextBox1.Height = Height - (82 + 43 * ((pis.Length + 4) / 5)) - 45;
                SetText("调用方法正确!");
                cb.BackColor = SystemColors.Window;
                //   label1.ForeColor = Color.Blue;
                button2.Enabled = true;
            }
            else
            {
                SetText("调用方法未找到!");
                cb.BackColor = Color.BurlyWood;
                //   label1.ForeColor = Color.Red;
                button2.Enabled = false;
            }
        }
        private void SetText(string title)
        {
            this.Text = "TestWebService  --  " + title +"  --  ZhuShengWen";
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InvokeMethod();
        }

        private void InvokeMethod()
        {
            List<object> objl = new List<object>();
            bool exp = false;
            for (int x = 0; x < Controls.Count; x++)
            {
                if (Controls.ContainsKey("Val" + x))
                {

                    UserControl1 uc = (UserControl1)Controls["Val" + x];
                    try
                    {
                        Type tp = Type.GetType("System." + uc.p_type.Text);
                        MethodInfo mi = tp.GetMethod("Parse", new Type[] { typeof(string) });


                        if (mi != null)
                        {
                            object _Object = Activator.CreateInstance(tp);
                            object _ReturnData = mi.Invoke(_Object, new object[] { uc.p_value.Text });
                            objl.Add(_ReturnData);
                        }
                        else
                        {
                            if (uc.p_type.Text == "String[]")
                            {
                                objl.Add(uc.p_value.Text.Split(','));
                            }
                            else
                                objl.Add(uc.p_value.Text);
                        }

                        uc.p_value.BackColor = SystemColors.Window;
                        SetText("调用方法成功!");
                    }
                    catch (Exception)
                    {
                        SetText(uc.p_Name + " 字段类型错误!");
                        uc.p_value.BackColor = Color.BurlyWood;
                        exp = true;
                    }

                }
            }
            if (exp) return;
            Type _Type = _WebServiceAssembly.GetTypes()[0];
            object _ObjectW = Activator.CreateInstance(_Type);
            MethodInfo _Method = _Type.GetMethod(comboBox1.Text);
            object _ReturnDataW = "";
            try
            {
                _ReturnDataW = _Method.Invoke(_ObjectW, objl.ToArray());
            }
            catch (Exception e)
            {
                SetText("调用异常!");
                MessageBox.Show(e.Message, "调用异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_ReturnDataW.GetType().Name == "List`1" || _ReturnDataW.GetType().Name == "String[]" || _ReturnDataW.GetType().Name == "Array")
            {
                int i = 0;
                richTextBox1.Text = "";
                foreach (var item in (IEnumerable<object>)_ReturnDataW)
                {
                    if (i != 0) richTextBox1.Text += ",";
                    richTextBox1.Text += ("[" + item + "]");
                    i++;
                }
                richTextBox1.Text += "\r\n总共:" + i.ToString() + "\r\n";
            }
            else
                richTextBox1.Text = _ReturnDataW.ToString();
            toolStripStatusLabel1.Text = MyClass.GetNowTime();

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValidUrl(sender);
                if (button1.Enabled)
                {
                    GetService();
                    comboBox1.Focus();
                }
            }
        }

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (button2.Enabled && e.KeyCode == Keys.Enter)
            {
                InvokeMethod();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (button1.Enabled)
            {
                GetService();
                comboBox1.Focus();
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            ValidUrl(textBox1);
        }
        public string TempTxtPath
        {
            get
            {
                return Path.GetFullPath(Path.Combine(System.Environment.GetEnvironmentVariable("TEMP"), MyClass.GetNowTime().Replace(':', '-')));
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string path = TempTxtPath;


            string str = comboBox1.Text;
            if (dataSource.Select(p => p.Name).ToArray().Contains(str))
            {
                Type _Type = _WebServiceAssembly.GetTypes()[0];

                MethodInfo _Method = _Type.GetMethod(str);

                str = _Method.ReturnType.Name + " " + str;
                ParameterInfo[] pis = _Method.GetParameters();
                str += "(";
                for (int i = 0; i < pis.Length; i++)
                {
                    str += pis[i].ParameterType.Name;
                    str += " ";
                    str += pis[i].Name;
                    if (i != pis.Length - 1) str += ",";
                }
                str += ");";
                SetText("原型已生成!");
                comboBox1.BackColor = SystemColors.Window;
                button2.Enabled = true;
                MyClass.WriteFile(path, str);
                System.Diagnostics.Process.Start("notepad", path);

            }
            else
            {
                SetText("调用方法未找到,原型无法生成!");
                comboBox1.BackColor = Color.BurlyWood;
                button2.Enabled = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                button3_Click(null, null);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text=MyClass.GetNowTime();
        }


    }
    public class MyClass
    {
        public static string GetNowTime()
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }
        public static string TempDir
        {
            get
            {
                return Path.GetFullPath(Path.Combine(System.Environment.GetEnvironmentVariable("TEMP")));
            }
        }
        public static string TempFile
        {
            get
            {
                return Path.GetFullPath(Path.Combine(TempDir, GetNowTime().Replace(':', '-')));
            }
        }
        public static void CreateFile(string path)
        {
            if (!File.Exists(path))
            {
                FileInfo f = new FileInfo(path);
                FileStream wn = f.OpenWrite();
                wn.Close();
            }
        }
        public static void CreateFile(string path, string content, bool isRecreate, bool isGBK = true)
        {
            if (File.Exists(path))
            {
                if (!isRecreate)
                {
                    FileInfo f = new FileInfo(path);
                    try
                    {
                        StreamWriter wn = f.AppendText();
                        wn.WriteLine(content);
                        wn.Close();

                    }
                    catch (Exception)
                    {
                    }
                    return;
                }
                else
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception)
                    {

                        return;
                    }
                }
            }
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            FileInfo f1 = new FileInfo(path);
            FileStream wn1 = f1.Create();
            byte[] b = isGBK ? System.Text.Encoding.Default.GetBytes(content) : System.Text.Encoding.UTF8.GetBytes(content);
            wn1.Write(b, 0, b.Length);
            wn1.Close();
        }

        public static void WriteFile(string path, string content, bool isReCreate = false, bool isGBK = true)//string path,
        {
            CreateFile(path, content, isReCreate, isGBK);
        }
    }
}
