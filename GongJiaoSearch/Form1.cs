using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StringTool;
namespace GongJiaoSearch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MyClass.connectionString = "server= localhost;User Id= root;password=admin;Persist Security Info=True;port=3306;database=test;charset=gbk;";
            comboBox1.AutoCompleteMode = comboBox2.AutoCompleteMode = AutoCompleteMode.Suggest;
            comboBox1.AutoCompleteSource = comboBox2.AutoCompleteSource = AutoCompleteSource.ListItems;
            string [] strs= MyClass.SelectArray("SELECT name from stations");
            comboBox1.DataSource = MyClass.SelectArray("SELECT name from stations");
            comboBox2.DataSource = strs;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = MyClass.ExecuteScalar("SELECT GetBusIndex(\"化工路西环路\",\"-化工路西环路-化工路白庄-冉屯路秦岭路-冉屯路五龙口-冉屯路桐柏路-\")").ToString();
        }
    }
}
