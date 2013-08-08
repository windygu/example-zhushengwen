using System;
using System.ComponentModel;
using System.Web;

namespace HuaweiSoftware.IPSPBD.UI
{
	/// <summary>
	/// Global 的摘要说明。
	/// </summary>
	public class Global : HttpApplication
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private IContainer components = null;

		public Global()
		{
			InitializeComponent();
		}

		protected void Application_Start(Object sender, EventArgs e)
		{
		}

		protected void Session_Start(Object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{
		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{
		}

		protected void Application_Error(Object sender, EventArgs e)
		{
		}

		protected void Session_End(Object sender, EventArgs e)
		{
		}

		protected void Application_End(Object sender, EventArgs e)
		{
		}

		#region Web 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new Container();
		}

		#endregion
	}
}