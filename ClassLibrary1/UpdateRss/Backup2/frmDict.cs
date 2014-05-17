using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace SoukeyNetget
{
    public partial class frmDict : Form
    {

        private string DelContent = "DictClass";
        private ResourceManager rm;
        
        public frmDict()
        {
            InitializeComponent();

        }

        private void IniData()
        {

            DataView dClass = new DataView();
            int i = 0;
            cDict d = new cDict();

            TreeNode newNode;

            dClass = d.GetDictClass();

            for (i = 0; i < dClass.Count; i++)
            {
                newNode = new TreeNode();
                newNode.Name = "C" + i.ToString();
                newNode.Text = dClass[i].Row["Name"].ToString();
                newNode.ImageIndex = 1;
                newNode.SelectedImageIndex = 1;
                this.treeDict.Nodes["nodDict"].Nodes.Add(newNode);
                newNode = null;
            }
            
            dClass = null;

            this.treeDict.Nodes["nodDict"].Expand();

            //设置默认选择的树形结构节点为“正在运行”
            TreeNode SelectNode = new TreeNode();
            SelectNode = this.treeDict.Nodes["nodDict"];
            this.treeDict.SelectedNode = SelectNode;
            SelectNode = null;

            d = null;

        }

        private void frmDict_Load(object sender, EventArgs e)
        {
            IniData();

            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());
        }
     
        private void AddDictClass()
        {
            this.treeDict.LabelEdit = true;

            TreeNode newNode;
            newNode = new TreeNode();
            newNode.Name = this.treeDict.Nodes.Count.ToString ();
            newNode.Text = rm.GetString ("Label19");
            newNode.ImageIndex = 1;
            newNode.SelectedImageIndex = 1;
            this.treeDict.Nodes["nodDict"].Nodes.Add(newNode);
            //this.treeDict.SelectedNode =newNode ;
            newNode.BeginEdit();
            newNode = null;

            //this.listDict.Items.Clear();

           
        }

        private void AddDictContent()
        {
            if (this.treeDict.SelectedNode.Name =="nodDict")
            {
                MessageBox.Show(rm.GetString("Info77"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.listDict.LabelEdit = true;
            ListViewItem item = new ListViewItem();
            item.Text = rm.GetString ("Label20");
            this.listDict.Items.Add(item);
            item.BeginEdit();
            item = null;
            //this.listDict.Items[this.listDict.Items.Count - 1].BeginEdit();
        }

        private void menuAddDict_Click(object sender, EventArgs e)
        {
            AddDictContent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //修改字典内容
        private void listDict_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
            {
                //表示没有进行修改
                this.listDict.Items.Remove(this.listDict.Items[e.Item]);
                this.listDict.LabelEdit = false;
                return;
            }

            if (e.Label.ToString().Trim() == "")
            {
                e.CancelEdit = true;
                this.listDict.Items.Remove(this.listDict.Items[e.Item]);
                this.listDict.LabelEdit = false;
                return;
            }

            try
            {
                cDict d = new cDict();
                d.AddDict(this.treeDict.SelectedNode.Text,e.Label.ToString ());
                d = null;
                this.listDict.LabelEdit = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info78") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
            }

        }

        private void menuDelDict_Click(object sender, EventArgs e)
        {
            DelDictContent();
        }

        private void menuDelDictClass_Click(object sender, EventArgs e)
        {
            DelDictClass();
        }

        private void listDict_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.listDict.Items.Count == 0)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                 DelDictContent();
            }
        }

        private void treeDict_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DataView dName;
            ListViewItem item;
            int i = 0;

            this.listDict.Items.Clear();
            if (this.treeDict.SelectedNode.Name =="nodDict")
            {
                return ;
            }

            cDict d = new cDict();

            dName = d.GetDict(this.treeDict.SelectedNode.Text );

            d = null;

            if (dName == null)
            {
                return;
            }

            for (i = 0; i < dName.Count; i++)
            {
                item = new ListViewItem();
                item.Text = dName[i].Row["Name"].ToString();
                this.listDict.Items.Add(item);
            }

            dName = null;
        }

        private void treeDict_Enter(object sender, EventArgs e)
        {
            DelContent = "DictClass";
        }

        private void listDict_Enter(object sender, EventArgs e)
        {
            DelContent = "Dict";
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (DelContent == "DictClass")
            {
                DelDictClass();
            }
            else if (DelContent =="Dict")
            {
                DelDictContent();
            }
        }

        private void DelDictClass()
        {
            if (this.treeDict.SelectedNode.Name == "nodDict")
            {
                MessageBox.Show(rm.GetString("Info79"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(rm.GetString("Delete") + "“" + this.treeDict.SelectedNode.Text + "”？" + rm.GetString ("Quaere21"),
               rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cDict d = new cDict();

                d.DelDictClass(this.treeDict.SelectedNode.Text);

                d = null;

                this.treeDict.Nodes.Remove(this.treeDict.SelectedNode);
            }
        }

        private void DelDictContent()
        {
            if (this.listDict.Items.Count == 0 || this.listDict.SelectedItems.Count ==0)
            {
                MessageBox.Show(rm.GetString ("Info80"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(rm.GetString("Delete") + "“" + this.listDict.SelectedItems[0].Text + "”？" + rm.GetString("Quaere22"),
                rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cDict d = new cDict();

                d.DelDict(this.treeDict.SelectedNode.Text, this.listDict.SelectedItems[0].Text.ToString());

                d = null;

                this.listDict.Items.Remove(this.listDict.SelectedItems[0]);

            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolAddDictClass_Click(object sender, EventArgs e)
        {
            AddDictClass();
        }

        private void menuAddDictClass_Click(object sender, EventArgs e)
        {
            AddDictClass();
        }

        private void treeDict_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DelDictClass();
            }
        }

        private void treeDict_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Text == "nodDict" || e.Label == null)
            {
                this.treeDict.Nodes.Remove(e.Node);
                this.treeDict.LabelEdit = false;
                return;
            }

            if (e.Label.ToString().Trim() == "")
            {
                this.treeDict.Nodes.Remove(e.Node);
                return;
            }

            cDict d = new cDict();
            d.AddDictClass(e.Label .ToString ());
            d = null;

            this.treeDict.LabelEdit = false;

        }

        private void toolAddDict_Click(object sender, EventArgs e)
        {
            AddDictContent();
        }

        private void frmDict_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

       
     
    }
}