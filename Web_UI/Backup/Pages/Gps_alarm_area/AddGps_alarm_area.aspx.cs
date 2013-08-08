using System;
using System.Web.UI;
using System.Web.UI.WebControls;

//using HuaweiSoftware.IPSPBD.Entity;

namespace HuaweiSoftware.IPSPBD.UI.Pages.Gps_alarm_area
{
	/// <summary>
	/// AddGps_alarm_area 的摘要说明。
	/// </summary>
	public class AddGps_alarm_area : Page
	{
		#region Variable members

		protected TextBox txtAlarm_id;
 		protected TextBox txtArea_name;
 		protected TextBox txtAlarm_type;
 		protected TextBox txtStart_time;
 		protected TextBox txtEnd_time;
 		protected TextBox txtIs_send_sms;
 		protected TextBox txtArea_department;
 		protected TextBox txtArea_entri;
 		protected TextBox txtArea_geomertry_type;
 		protected TextBox txtArea_linecolor;
 		protected TextBox txtArea_linewidth;
 		protected TextBox txtArea_data;
  
		protected Button btnSave;
		protected Button btnCancel;

		#endregion

		private void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
				BindData();	
		    }
  		}

		private void BindData()
		{
			//Add Initial code here

		}

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }
		
        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {    
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

			this.Load += new System.EventHandler(this.Page_Load);
		}
        #endregion

        private void btnSave_Click(object sender, System.EventArgs e)
        {
			try
			{
				/*
				BLLCheck.Gps_alarm_area gps_alarm_area = new BLLCheck.Gps_alarm_area();

				Gps_alarm_areaInfo gps_alarm_areaInfo = new Gps_alarm_areaInfo();
		    	gps_alarm_areaInfo.Alarm_id = Convert.ToDecimal(txtAlarm_id.Text);
 		    	gps_alarm_areaInfo.Area_name = Convert.ToString(txtArea_name.Text);
 		    	gps_alarm_areaInfo.Alarm_type = Convert.ToString(txtAlarm_type.Text);
 		    	gps_alarm_areaInfo.Start_time = Convert.ToString(txtStart_time.Text);
 		    	gps_alarm_areaInfo.End_time = Convert.ToString(txtEnd_time.Text);
 		    	gps_alarm_areaInfo.Is_send_sms = Convert.ToDecimal(txtIs_send_sms.Text);
 		    	gps_alarm_areaInfo.Area_department = Convert.ToString(txtArea_department.Text);
 		    	gps_alarm_areaInfo.Area_entri = Convert.ToString(txtArea_entri.Text);
 		    	gps_alarm_areaInfo.Area_geomertry_type = Convert.ToString(txtArea_geomertry_type.Text);
 		    	gps_alarm_areaInfo.Area_linecolor = Convert.ToString(txtArea_linecolor.Text);
 		    	gps_alarm_areaInfo.Area_linewidth = Convert.ToDecimal(txtArea_linewidth.Text);
 		    	gps_alarm_areaInfo.Area_data = Convert.ToByte[](txtArea_data.Text);
  				gps_alarm_area.Insert(gps_alarm_areaInfo);
				*/
			}
			catch (Exception ex)
			{
				Helper.ShowError(this, ex, false);
				return;
			}
			
        	Response.Redirect("ListGps_alarm_area.aspx");
        }

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("ListGps_alarm_area.aspx");
		}
    }
}
