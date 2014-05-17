using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Data ;

///���ܣ��Զ���datagridview�ؼ�
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������Ҫ���ڲɼ����ݵ��Զ���ʾ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.CustomControl
{
    public partial class cMyDataGridView : DataGridView 
    {
        private string m_FileName;

        public cMyDataGridView()
        {
            InitializeComponent();
            m_gData = new DataTable();
            base.ReadOnly = false;
            base.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            base.MultiSelect = true;
            base.DataSource = m_gData;
            m_gData.AcceptChanges();

            base.AllowUserToAddRows = false;
            base.AllowUserToDeleteRows = true ;
        }

        public cMyDataGridView(string FileName)
        {
            InitializeComponent();
            m_gData = new DataTable();
            base.ReadOnly = false;
            base.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            base.DataSource = m_gData;
            m_gData.AcceptChanges();

            base.AllowUserToAddRows = false;
            base.AllowUserToDeleteRows = false;

            m_FileName = FileName;

        }

        public void Clear()
        {
            m_gData = null;
            m_gData = new DataTable();
            base.DataSource = m_gData;
        }

        public cMyDataGridView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private Int64 m_TaskRunID;
        public Int64 TaskRunID
        {
            get { return m_TaskRunID; }
            set { m_TaskRunID = value; }
        }

        private DataTable m_gData;
        public DataTable gData
        {
            get { return this.m_gData; }
            set 
            {
                DataTable tmp = new DataTable();
                tmp=value;
                try
                {
                    if (tmp != null)
                    {
                        m_gData.Merge(tmp);
                        m_gData.AcceptChanges();

                        base.FirstDisplayedScrollingRowIndex = base.Rows.Count - 1;

                    }
                }
                catch (System.Exception)
                {
                    //��������󣬲����κδ���ֻҪ���ϲ��������򼴿�
                }
            }
        }

        private string FileName
        {
            get
            {
                string FileName = Program.getPrjPath() + "data\\" + this.TaskName + "-" + this.TaskRunID + ".xml";
                return FileName;
            }

        }
    }
}
