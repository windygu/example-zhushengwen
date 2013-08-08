using System;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using HuaweiSoftware.Common.Encrypt;
using log4net;

namespace HuaweiSoftware.IPSPBD.UI
{
	/// <summary>
	/// 帮助类
	/// </summary>
	public class Helper
	{
		private static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		public static string ConnectionString
		{
			get
			{
				string connectionString = ConfigurationSettings.AppSettings["IPSPBD_ConnectionString"];
				if (connectionString != null)
				{
					connectionString = EncryptHelper.DecodeBase64(connectionString); //解密字符串
				}
				return connectionString;
			}
		}

		/// <summary>
		/// 在弹出的对话框中显示错误信息,记录日志
		/// </summary>
		/// <param name="page">页面引用</param>
		/// <param name="ex">异常对象</param>
		/// <param name="Close">是否关闭</param>
		public static void ShowError(Page page, Exception ex, bool Close)
		{
			logger.Error("SaveException:" + page.ID, ex);
			if (Close)
			{
				AlertAndClose(page, ex.Message);
			}
			else
			{
				Alerts(page, ex.Message);
			}
		}

		/// <summary>
		/// 弹出提示信息
		/// </summary>
		/// <param name="control">当前请求的page</param>
		/// <param name="message">message</param>
		public static void Alerts(Control control, string message)
		{
			control.Page.RegisterStartupScript("", string.Format(
				"<script>javascript:alert(\"{0}\");</script>", message).Replace("\r\n", ""));
		}

		public static void AlertAndClose(Control control, string message)
		{
			control.Page.RegisterStartupScript("", string.Format(
				"<script>javascript:alert(\"{0}\");window.close();</script>", message).Replace("\r\n", ""));
		}

		/// <summary>
		/// 定位到指定的页面
		/// </summary>
		/// <param name="GoPage">目标页面</param>
		public static void GoTo(string GoPage)
		{
			HttpContext.Current.Response.Redirect(GoPage);
		}

		public static void Location(Control control, string page)
		{
			string js = "<script language='JavaScript'>";
			js += "top.location='" + page + "'";
			js += "</script>";
			control.Page.RegisterStartupScript("", js);
		}

		public static void AlertAndLocation(Control control, string page, string message)
		{
			string js = "<script language='JavaScript'>";
			js += "alert('" + message + "');";
			js += "top.location='" + page + "'";
			js += "</script>";
			control.Page.RegisterStartupScript("", js);
		}

		public static void CloseWin(Control control, string returnValue)
		{
			string js = "<script language='JavaScript'>";
			js += "window.parent.returnValue='" + returnValue + "';";
			js += "window.close();";
			js += "</script>";
			control.Page.RegisterStartupScript("", js);
		}

		public static HtmlAnchor GetHtmlAnchor(string innerText, string href)
		{
			HtmlAnchor htmlAnchor = new HtmlAnchor();
			htmlAnchor.InnerText = innerText;
			htmlAnchor.HRef = href;

			return htmlAnchor;
		}

		/// <summary>
		/// 是否是数值
		/// </summary>
		/// <param name="strValue"></param>
		/// <returns></returns>
		public static bool IsNumerical(string strValue)
		{
			return Regex.IsMatch(strValue, @"^[0-9]*$");
		}

		/// <summary>
		/// 过滤输入的前后空格和分号字符
		/// </summary>
		/// <param name="strValue"></param>
		/// <returns></returns>
		public static string ConvertString(string strValue)
		{
			return strValue.Trim().Replace("'", "''");
		}

		/// <summary>
		/// 获取DataGrid控件中选择的项目的ID字符串(要求DataGrid设置datakeyfield="ID")
		/// </summary>
		/// <returns>如果没有选择, 那么返回为空字符串, 否则返回逗号分隔的ID字符串(如1,2,3)</returns>
		public static string GetDatagridItems(DataGrid dg)
		{
			string idstring = string.Empty;
			foreach (DataGridItem item in dg.Items)
			{
				string key = dg.DataKeys[item.ItemIndex].ToString();
				bool isSelected = ((CheckBox) item.FindControl("cbxDelete")).Checked;
				if (isSelected)
				{
					idstring += key + ",";
				}
			}
			idstring = idstring.Trim(',');

			return idstring;
		}
	}
}