using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using HuaweiSoftware.Common.WebPager;

namespace HuaweiSoftware.IPSPBD.UI.Pages.Gps_alarm_area
{
	/// <summary>
	/// ListGps_alarm_area
	/// </summary>
	public class ListGps_alarm_area : Page
	{
		#region Variable members

			protected TextBox txtAlarm_id;
				protected TextBox txtArea_name;
				protected TextBox txtAlarm_type;
				protected TextBox txtStart_time;
				protected TextBox txtEnd_time;
	
		protected DataGrid dg;
		protected Pager wp;
		protected HtmlForm Form1;

		protected Button btnSearch;
		protected Button btnAdd;
		protected Button btnDelete;

		//private Gps_alarm_area gps_alarm_area = new Gps_alarm_area();

		#endregion

		private void Page_Load(object sender, System.EventArgs e)
        {
			if (!IsPostBack)
            {
				ViewState["Filter"] = "";
				BindData();

				// Show tips before delete
              	btnDelete.Attributes.Add("onclick", "javascript:return confirm(\"此删除将不能恢复,删除时会将所属于此记录的信息全部删除．\\n请按确定继续，按取消撤消删除!\");");
            }
        }


		private void BindData()
		{
			try
			{
				/*
				wp.ConnectionString = Helper.ConnectionString;
				string sql = gps_alarm_area.GetPaginationSQL(ViewState["Filter"].ToString()) ;
				wp.SQL = sql;
				*/
			}
			catch (Exception ex)
			{
				Helper.ShowError(this, ex, false);
				return;
			}
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
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

			this.Load += new System.EventHandler(this.Page_Load);
		}
		
        #endregion

        private void btnDelete_Click(object sender, System.EventArgs e)
        {	
            try
            {
				string idstring = Helper.GetDatagridItems(dg);
				if (idstring != string.Empty)
				{
					string filter = string.Format("ID in ({0})", idstring);
					//gps_alarm_area.Delete(filter);
				}
            }
            catch (Exception ex)
			{
				Helper.ShowError(this, ex, false);
				return;
			}        
            Response.Redirect("ListGps_alarm_area.aspx");
        }


		private void btnAdd_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("AddGps_alarm_area.aspx");
        }

		private void SetFilter()
		{
			string filter = "";

			//Set the filter to search
			ViewState["Filter"] = filter;
		}
	
		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			SetFilter();
			BindData();
		}
    }
}
