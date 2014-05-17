using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SoukeyNetget.Plan;
using System.Reflection;
using System.Resources;

namespace SoukeyNetget
{
    public partial class frmTaskPlan : Form
    {
        //是否已保存了任务，如果保存，即便在取消的时候，
        //也需要将任务所述分类进行返回，主要是用在“应用”
        //和“取消”按钮的判断上
        private bool IsSaveTask = false;

        private ResourceManager rm;

        public frmTaskPlan()
        {
            InitializeComponent();
        }

        private cGlobalParas.FormState m_FormState;
        public cGlobalParas.FormState FormState
        {
            get { return m_FormState; }
            set { m_FormState = value; }
        }

        private void cmdAddTask_Click(object sender, EventArgs e)
        {
            frmAddPlanTask f = new frmAddPlanTask();
            f.RTask  = GetTaskInfo;
            f.ShowDialog();
            f.Dispose();

            this.IsSave.Text = "true";
        }

        private void GetTaskInfo(cGlobalParas.RunTaskType rType,string TaskName,string TaskPara)
        {
            ListViewItem Litem = new ListViewItem();

            Litem.Text = cGlobalParas.ConvertName((int)rType);
            Litem.SubItems.Add(TaskName);
            Litem.SubItems.Add(TaskPara);

            this.listTask.Items.Add(Litem);

           
        }

        private void cboxIsDisabled_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cboxIsDisabled.Checked == true)
            {
                this.raNumber.Enabled = true;
                this.raDateTime.Enabled = true;

                if (this.raNumber.Checked == true)
                {
                    this.groupBox4.Enabled = true;
                    this.groupBox5.Enabled = false;
                }
                else if (this.raDateTime.Checked == true)
                {
                    this.groupBox4.Enabled = false;
                    this.groupBox5.Enabled = true;
                }
            }
            else
            {
                this.raNumber.Enabled = false;
                this.raDateTime.Enabled = false;
                this.groupBox4.Enabled = false;
                this.groupBox5.Enabled = false;
            }

           

            this.IsSave.Text = "true";
        }

        private void raNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raNumber.Checked == true)
                this.groupBox4.Enabled = true;
            else
                this.groupBox4.Enabled = false;

            this.IsSave.Text = "true";
        }

        private void raDateTime_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raDateTime.Checked == true)
                this.groupBox5.Enabled = true;
            else
                this.groupBox5.Enabled = false;

            this.IsSave.Text = "true";
        }

        private void raOneTime_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raOneTime.Checked == true)
            {
                this.PanelOne.Visible = true;
                this.PanelDay.Visible = false;
                this.PanelWeekly.Visible = false;
                this.panelCustom.Visible = false;

                //修改过期参数
                this.cboxIsDisabled.Checked = true;
                this.raNumber.Checked = true;
                this.DisabledRunNum.Value = 1; 
            }

            this.IsSave.Text = "true";
        }

        private void raDay_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raDay.Checked == true)
            {
                this.PanelOne.Visible = false;
                this.PanelDay.Visible = true;
                this.PanelWeekly.Visible = false;
                this.panelCustom.Visible = false;
            }

            this.IsSave.Text = "true";
        }

        private void raWeekly_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raWeekly.Checked == true)
            {
                this.PanelOne.Visible = false;
                this.PanelDay.Visible = false;
                this.PanelWeekly.Visible = true;
                this.panelCustom.Visible = false;
            }

            this.IsSave.Text = "true";
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void raRunones_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raRunones.Checked == true)
            {
                this.RunDayOnesTime.Enabled = true;
                this.RunDayTwice1Time.Enabled = false;
                this.RunDayTwice2Time.Enabled = false;

                this.label10.Enabled = false;
                this.label9.Enabled = false;
                this.label8.Enabled = true;
            }

            this.IsSave.Text = "true";
        }

        private void raRuntwice_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raRuntwice.Checked == true)
            {
                this.RunDayOnesTime.Enabled = false;
                this.RunDayTwice1Time.Enabled = true;
                this.RunDayTwice2Time.Enabled = true;

                this.label10.Enabled = true;
                this.label9.Enabled = true;
                this.label8.Enabled = false;
            }

            this.IsSave.Text = "true";
        }

        private void cmdDelTask_Click(object sender, EventArgs e)
        {
            this.listTask.Focus();
            SendKeys.Send("{Del}");

            this.IsSave.Text = "true";
        }

        private void listTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                while (this.listTask.SelectedItems.Count > 0)
                {
                    this.listTask.Items.Remove(this.listTask.SelectedItems[0]);
                }

                this.IsSave.Text = "true";
            }


        }

        #region 控制修改标志
        private void txtPlanName_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtPlanDemo_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cboxIsRun_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void DisabledRunNum_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

       
        private void EnabledDate_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }
       
     

        private void cboxSunday_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cboxMonday_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cboxTuesday_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cboxWednesday_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cboxThursday_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cboxFriday_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cboxSturday_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void DisabledDateTime_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void RunDayOnesTime_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void RunDayTwice1Time_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void RunDayTwice2Time_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void RunOnceTime_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void RunWeeklyTime_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }
        #endregion

        private bool CheckInputvalidity()
        {
            this.errorProvider1.Clear();

            if (this.txtPlanName.Text.ToString() == null || this.txtPlanName.Text.Trim().ToString() == "")
            {
                this.errorProvider1.SetError(this.txtPlanName, rm.GetString ("Error18"));
                return false;
            }
            
            if (this.listTask.Items.Count ==0)
            {
                this.tabControl1.SelectedTab = this.tabControl1.TabPages[1];
                this.errorProvider1.SetError(this.listTask, rm.GetString ("Error19"));
                return false;
            }

            if (this.cboxIsDisabled.Checked == true && this.raNumber.Checked == true)
            {
                if (!(this.DisabledRunNum .Value.ToString () =="1" && this.raOneTime.Checked ==true ))
                {
                    this.tabControl1.SelectedTab = this.tabControl1.TabPages[0];
                    this.errorProvider1.SetError(this.DisabledRunNum, rm.GetString("Error20"));
                    return false;
                }
            }

            return true;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "true")
            {
                if (false == Save())
                    return;
            }

            this.Close();
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            Save();
        }

        private bool Save()
        {
            if (!CheckInputvalidity())
            {
                return false ;
            }

            try
            {
                //判断当前是否为编辑状态，如果是编辑状态则需要删除原有任务
                //但任务的ID不能变化

                Int64 Nid =  SaveTaskPlan();

                this.txtPlanID.Text = Nid.ToString();

                this.IsSave.Text = "false";

                this.IsSaveTask = true;

                if (this.FormState == cGlobalParas.FormState.New)
                {
                    this.FormState = cGlobalParas.FormState.Edit;
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info89") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false ;
            }

            return true;
        }

        private Int64  SaveTaskPlan()
        {
            cPlan NewPlan = new cPlan();

            Int64 Nid = GetNewID();

            if (Nid == 0)
            {
                throw new cSoukeyException(rm.GetString ("Error21"));
            }

            NewPlan.PlanID = Nid;
            NewPlan.PlanName = this.txtPlanName.Text;
            NewPlan.PlanRemark = this.txtPlanDemo.Text;

            //系统自动判断当前任务的状态
            if (this.cboxIsDisabled.Checked == true)
            {
                if (this.raNumber.Checked == true)
                {
                    if (this.DisabledRunNum.Value == 1 && DateTime.Compare(DateTime.Now,this.RunOnceTime.Value) > 0)
                        NewPlan.PlanState =(int) cGlobalParas.PlanState.Disabled;
                    else
                        NewPlan.PlanState = (int)cGlobalParas.PlanState.Enabled;

                }
                else if (this.raDateTime.Checked == true)
                {
                    if (DateTime.Compare(DateTime.Now, this.DisabledDateTime.Value) < 0)
                        NewPlan.PlanState = (int)cGlobalParas.PlanState.Disabled;
                    else
                        NewPlan.PlanState = (int)cGlobalParas.PlanState.Enabled;
                }
            }
            else
            {
                NewPlan.PlanState = (int)cGlobalParas.PlanState.Enabled;
            }

            if (this.cboxIsRun.Checked == true)
                NewPlan.IsOverRun = true;
            else
                NewPlan.IsOverRun = false;

            if (this.cboxIsDisabled.Checked ==true )
                NewPlan.IsDisabled =true ;
            else
                NewPlan .IsDisabled =false ;

            if (this.raNumber.Checked ==true )
                NewPlan.DisabledType =(int) cGlobalParas.PlanDisabledType.RunTime ;
            else if (this.raDateTime .Checked ==true )
                NewPlan.DisabledType =(int) cGlobalParas.PlanDisabledType .RunDateTime ;

            NewPlan.DisabledTime =int.Parse (this.DisabledRunNum .Value.ToString () );
            NewPlan.DisabledDateTime = this.DisabledDateTime.Value;

            cTaskPlan tp;
            for (int i = 0; i < this.listTask.Items.Count; i++)
            {
                tp = new cTaskPlan();
                tp.RunTaskType = cGlobalParas.ConvertID(this.listTask.Items[i].Text);

                if (cGlobalParas.ConvertID(this.listTask.Items[i].Text)==(int)cGlobalParas.RunTaskType.DataTask )
                    tp.RunTaskName = cGlobalParas.ConvertID ( this.listTask.Items[i].SubItems[1].Text.ToString()).ToString ();
                else
                    tp.RunTaskName =this.listTask.Items[i].SubItems[1].Text.ToString();

                tp.RunTaskPara = this.listTask.Items[i].SubItems[2].Text.ToString();

                NewPlan.RunTasks.Add(tp);
            }

            if (this.raOneTime.Checked == true)
                NewPlan.RunTaskPlanType = (int) cGlobalParas.RunTaskPlanType.Ones;
            else if (this.raDay.Checked == true)
            {
                if (this.raRunones.Checked == true)
                    NewPlan.RunTaskPlanType = (int)cGlobalParas.RunTaskPlanType.DayOnes;
                else if (this.raRuntwice.Checked == true)
                    NewPlan.RunTaskPlanType = (int)cGlobalParas.RunTaskPlanType.DayTwice;
            }
            else if (this.raWeekly.Checked == true)
            {
                NewPlan.RunTaskPlanType = (int)cGlobalParas.RunTaskPlanType.Weekly;
            }
            else if (this.raCustom.Checked == true)
            {
                NewPlan.RunTaskPlanType = (int)cGlobalParas.RunTaskPlanType.Custom;
            }


            NewPlan.EnabledDateTime = this.EnabledDate.Value.ToLongDateString ();
            NewPlan.RunOnesTime = this.RunOnceTime.Value.ToString () ;
            NewPlan.RunDayTime = this.RunDayOnesTime.Value.ToLongTimeString();
            NewPlan.RunAMTime = this.RunDayTwice1Time.Value.ToLongTimeString();
            NewPlan.RunPMTime = this.RunDayTwice2Time.Value.ToLongTimeString();
            NewPlan.RunWeeklyTime = this.RunWeeklyTime.Value.ToLongTimeString();
            NewPlan.FirstRunTime = this.FirstRunTime.Value.ToLongTimeString();
            NewPlan.RunInterval = this.udRunInterval.Value.ToString();

            string runWeekly = "";

            if (this.cboxSunday.Checked ==true )
                runWeekly ="0";
            if (this.cboxMonday.Checked == true)
                runWeekly += ",1";
            if (this.cboxTuesday.Checked == true)
                runWeekly += ",2";
            if (this.cboxWednesday.Checked == true)
                runWeekly += ",3";
            if (this.cboxThursday.Checked == true)
                runWeekly += ",4";
            if (this.cboxFriday.Checked == true)
                runWeekly += ",5";
            if (this.cboxSturday.Checked == true)
                runWeekly += ",6";

            NewPlan.RunWeekly = runWeekly;

            cPlans p = new cPlans();

            if (this.FormState == cGlobalParas.FormState.New )
            {
                p.InsertPlan(NewPlan);
            }
            else if (this.FormState == cGlobalParas.FormState.Edit)
            {
                p.EditPlan(NewPlan);
            }

            p = null;

            return Nid;

        }

        private Int64 GetNewID()
        {
            Int64 pid=0;

            if (this.FormState == cGlobalParas.FormState.New)
                pid = Int64.Parse(DateTime.Now.ToFileTime().ToString());
            else if (this.FormState == cGlobalParas.FormState.Edit)
                pid = Int64.Parse(this.txtPlanID.Text);

            return pid;
        }

        private void IsSave_TextChanged(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "true" && this.FormState != cGlobalParas.FormState.Browser)
            {
                this.cmdApply.Enabled = true;
            }
            else if (this.IsSave.Text == "false")
            {
                this.cmdApply.Enabled = false;
            }
        }

        public void LoadPlan(Int64 PlanID)
        {
            cPlans ps = new cPlans();

            cPlan p=ps.GetSinglePlan(PlanID);

            this.txtPlanName.Text = p.PlanName;
            this.txtPlanID.Text = p.PlanID.ToString ();
            this.txtPlanDemo.Text = p.PlanRemark;

            this.txtPlanState.Text = cGlobalParas.ConvertName(p.PlanState);

            if (p.IsOverRun == true)
                this.cboxIsRun.Checked = true;
            else
                this.cboxIsRun.Checked = false;

            if (p.IsDisabled == true)
                this.cboxIsDisabled.Checked = true;
            else
                this.cboxIsDisabled.Checked = false;

            if (p.DisabledType == (int)cGlobalParas.PlanDisabledType.RunTime)
                this.raNumber.Checked = true;
            else if (p.DisabledType == (int)cGlobalParas.PlanDisabledType.RunDateTime)
                this.raDateTime.Checked = true;

            this.DisabledRunNum.Value = p.DisabledTime;
            this.DisabledDateTime.Value = p.DisabledDateTime;

            ListViewItem litem;

            for (int i = 0; i < p.RunTasks.Count; i++)
            {
                litem = new ListViewItem();
                litem.Text = cGlobalParas.ConvertName ( p.RunTasks[i].RunTaskType);
                if (p.RunTasks[i].RunTaskType == (int)cGlobalParas.RunTaskType.DataTask)
                    litem.SubItems.Add(cGlobalParas.ConvertName(int.Parse (p.RunTasks[i].RunTaskName)));
                else
                    litem.SubItems.Add(p.RunTasks[i].RunTaskName);
                litem.SubItems.Add(p.RunTasks[i].RunTaskPara);

                this.listTask.Items.Add(litem);
            }

            this.EnabledDate.Value = DateTime.Parse (p.EnabledDateTime);

            switch (p.RunTaskPlanType)
            {
                case (int)cGlobalParas.RunTaskPlanType .Ones :
                    this.raOneTime.Checked = true;
                    this.RunOnceTime.Value = DateTime.Parse (p.RunOnesTime);
                    break;
                case (int)cGlobalParas.RunTaskPlanType.DayOnes :
                    this.raDay.Checked = true;
                    this.raRunones.Checked = true;
                    this.RunDayOnesTime.Value = DateTime.Parse (p.RunDayTime);
                    break;
                case (int)cGlobalParas.RunTaskPlanType.DayTwice :
                    this.raDay.Checked = true;
                    this.raRuntwice.Checked = true;
                    this.RunDayTwice1Time.Value =DateTime.Parse ( p.RunAMTime);
                    this.RunDayTwice2Time.Value = DateTime.Parse (p.RunPMTime);
                    break;
                case (int)cGlobalParas.RunTaskPlanType.Weekly :
                    this.raWeekly.Checked = true;
                    this.RunWeeklyTime.Value =DateTime.Parse ( p.RunWeeklyTime);
                    string rWeekly = p.RunWeekly;
                    foreach (string sc in rWeekly.Split(','))
                    {
                        string ss = sc.Trim();
                        switch (ss)
                        {
                            case "0":
                                this.cboxSunday.Checked = true;
                                break;
                            case "1":
                                this.cboxMonday.Checked = true;
                                break;
                            case "2":
                                this.cboxTuesday.Checked = true;
                                break;
                            case "3":
                                this.cboxWednesday.Checked = true;
                                break;
                            case "4":
                                this.cboxThursday.Checked = true;
                                break;
                            case "5":
                                this.cboxFriday.Checked = true;
                                break;
                            case "6":
                                this.cboxSunday.Checked = true;
                                break;
                            default :
                                break;
                        }
                    }
                    break;
                case (int)cGlobalParas.RunTaskPlanType.Custom :
                    this.raCustom.Checked = true;
                    this.FirstRunTime.Value = DateTime.Parse(p.FirstRunTime );
                    this.udRunInterval.Value = decimal.Parse( p.RunInterval);
                    break;
            }

            p = null;
            ps = null;
            
        }

        private void frmTaskPlan_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("SoukeyNetget.Resources.globalUI", Assembly.GetExecutingAssembly());

            switch (this.FormState)
            {
                case cGlobalParas.FormState.New:
                    this.txtPlanState.Text = rm.GetString ("Label22");
                    break;
                case cGlobalParas.FormState.Edit:
                    //编辑状态进来不能修改分类
                    this.txtPlanName.ReadOnly = true;
                    break;
                case cGlobalParas.FormState.Browser:

                    break;
                default:
                    break;
            }

            this.IsSave.Text = "false";
        }

        private void raCustom_CheckedChanged(object sender, EventArgs e)
        {
            if (this.raCustom.Checked == true)
            {
                this.PanelOne.Visible = false;
                this.PanelDay.Visible = false;
                this.PanelWeekly.Visible = false;
                this.panelCustom.Visible = true;
            }

            this.IsSave.Text = "true";
        }

      

    }
}