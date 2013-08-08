<%@ Page language="c#" Codebehind="ListGps_alarm_area.aspx.cs" AutoEventWireup="false" Inherits="HuaweiSoftware.IPSPBD.UI.Pages.Gps_alarm_area.ListGps_alarm_area" %>
<%@ Register TagPrefix="cc1" Namespace="HuaweiSoftware.Common.WebPager"  Assembly="WebPager" %>
<HTML>
	<HEAD>
		<LINK href="../../CSS/Style.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../../Script/Calendar.js"></script>
	</HEAD>
	<body>
		<form id="Form1" method="post" runat="server">
			<table id="Table6" cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td align="center">
					    					    
<fieldset style="WIDTH: 98%"><legend>功能</legend>
	<table id="Table3" cellSpacing="0" cellPadding="0" width="100%" border="1" bordercolor="lavender">
	    <TR>
			<TD class="AlternatingItem">ALARM_ID：</TD>
            	         <TD class="Item" width="106">&nbsp;
		<asp:textbox id="txtAlarm_id" runat="server" width="200px" cssclass="input"></asp:textbox>
	</TD>
	
				<TD class="AlternatingItem">AREA_NAME：</TD>
            	         <TD class="Item" width="106">&nbsp;
		<asp:textbox id="txtArea_name" runat="server" width="200px" cssclass="input"></asp:textbox>
	</TD>
	
	 </TR><TR>				<TD class="AlternatingItem">ALARM_TYPE：</TD>
            	         <TD class="Item" width="106">&nbsp;
		<asp:textbox id="txtAlarm_type" runat="server" width="200px" cssclass="input"></asp:textbox>
	</TD>
	
				<TD class="AlternatingItem">START_TIME：</TD>
            	         <TD class="Item" width="106">&nbsp;
		<asp:textbox id="txtStart_time" runat="server" width="200px" cssclass="input"></asp:textbox>
	</TD>
	
				<TD class="AlternatingItem">END_TIME：</TD>
            	         <TD class="Item" width="106">&nbsp;
		<asp:textbox id="txtEnd_time" runat="server" width="200px" cssclass="input"></asp:textbox>
	</TD>
	
		   </tr>
		<tr>
			<td style="HEIGHT: 15px" vAlign="top" align="right" bgColor="#ffffff" colspan=4>
			    <asp:Button id="btnSearch" runat="server" Text="查询" CssClass="btn"></asp:Button>&nbsp;&nbsp;
			    <asp:Button id="btnAdd" runat="server" Text="增加" CssClass="btn"></asp:Button>&nbsp;&nbsp;
			    <asp:Button id="btnDelete" runat="server" Text="删除" CssClass="btn"></asp:Button>
			</td>
		</tr>
	</table>
</fieldset>					</td>
				</tr>
				<tr>
					<td> 
					    					    <table id="Table7" cellSpacing="0" cellPadding="0" width="98%" align="center" border="0">
<tr>
	<td>
		<asp:datagrid id="dg" runat="server" DataKeyField="ID" allowsorting="True" pagesize="20" autogeneratecolumns="False" width="100%" cssclass="dg">
			<HeaderStyle Wrap="False" CssClass="Header"></HeaderStyle>
			<ItemStyle CssClass="Item"></ItemStyle>
			<AlternatingItemStyle CssClass="AlternatingItem"></AlternatingItemStyle>
			<FooterStyle CssClass="Footer"></FooterStyle>
			<EditItemStyle CssClass="EditItem"></EditItemStyle>
			<Columns>
				<asp:TemplateColumn HeaderImageUrl="../../Images/delete.GIF">
					<HeaderStyle Wrap="False" Width="20px"></HeaderStyle>
					<ItemTemplate>
						<asp:checkbox runat="server" id="cbxDelete"></asp:checkbox>
					</ItemTemplate>
				</asp:TemplateColumn>

    
				<asp:TemplateColumn HeaderText="ALARM_ID">
					<ItemTemplate>
						<asp:Label id="lblAlarm_id" style="word-wrap:break-word;overflow: hidden;width:80px" runat="server" 
							Text='<%# DataBinder.Eval(Container, "DataItem.Alarm_id") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
    
				<asp:TemplateColumn HeaderText="AREA_NAME">
					<ItemTemplate>
						<asp:Label id="lblArea_name" style="word-wrap:break-word;overflow: hidden;width:80px" runat="server" 
							Text='<%# DataBinder.Eval(Container, "DataItem.Area_name") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
    
				<asp:TemplateColumn HeaderText="ALARM_TYPE">
					<ItemTemplate>
						<asp:Label id="lblAlarm_type" style="word-wrap:break-word;overflow: hidden;width:80px" runat="server" 
							Text='<%# DataBinder.Eval(Container, "DataItem.Alarm_type") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
    
				<asp:TemplateColumn HeaderText="START_TIME">
					<ItemTemplate>
						<asp:Label id="lblStart_time" style="word-wrap:break-word;overflow: hidden;width:80px" runat="server" 
							Text='<%# DataBinder.Eval(Container, "DataItem.Start_time") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
    
				<asp:TemplateColumn HeaderText="END_TIME">
					<ItemTemplate>
						<asp:Label id="lblEnd_time" style="word-wrap:break-word;overflow: hidden;width:80px" runat="server" 
							Text='<%# DataBinder.Eval(Container, "DataItem.End_time") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
    
				<asp:TemplateColumn HeaderText="IS_SEND_SMS">
					<ItemTemplate>
						<asp:Label id="lblIs_send_sms" style="word-wrap:break-word;overflow: hidden;width:80px" runat="server" 
							Text='<%# DataBinder.Eval(Container, "DataItem.Is_send_sms") %>'></asp:Label>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:TemplateColumn HeaderImageUrl="../../Images/edititem.gif">
					<HeaderStyle Wrap="False" Width="32px"></HeaderStyle>
					<ItemTemplate>
						<asp:HyperLink id="lnkEdit" runat="server" text="编辑" imageurl="../../Images/edititem.gif" 
						     navigateurl='<%# "AddGps_alarm_area.aspx?id=" + DataBinder.Eval(Container, "DataItem.ID").ToString() %>'>编辑</asp:HyperLink>
					</ItemTemplate>
				</asp:TemplateColumn>
			</Columns>
		</asp:datagrid>
	</td>
</tr>
<tr>
	<td align="right">
		<cc1:Pager id="wp" runat="server" Width="509px" BindControlID="dg"></cc1:Pager>
		<SelectedStyle ForeColor="Red" Font-Bold="True"></SelectedStyle>
		<NormalStyle ForeColor="Black"></NormalStyle>
	</td>
</tr>
</table>					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
