<%@ Page language="c#" Codebehind="AddGps_alarm_area.aspx.cs" AutoEventWireup="false" Inherits="HuaweiSoftware.IPSPBD.UI.Pages.Gps_alarm_area.AddGps_alarm_area" %>
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
				    				    
<TABLE id="Table9" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
	<TR>
		<TD>
			<TABLE class="table" id="Table11" cellSpacing="1" cellPadding="0" width="100%" align="center" border="0">
			    								    				    				<TR class="trBGColor">
					<TD class="item" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">ALARM_ID：</DIV>
					</TD>
						         <TD class="item" width="106">&nbsp;
				<asp:textbox id="txtAlarm_id" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="AlternatingItem" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">AREA_NAME：</DIV>
					</TD>
						         <TD class="AlternatingItem" width="106">&nbsp;
				<asp:textbox id="txtArea_name" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="item" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">ALARM_TYPE：</DIV>
					</TD>
						         <TD class="item" width="106">&nbsp;
				<asp:textbox id="txtAlarm_type" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="AlternatingItem" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">START_TIME：</DIV>
					</TD>
						         <TD class="AlternatingItem" width="106">&nbsp;
				<asp:textbox id="txtStart_time" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="item" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">END_TIME：</DIV>
					</TD>
						         <TD class="item" width="106">&nbsp;
				<asp:textbox id="txtEnd_time" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="AlternatingItem" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">IS_SEND_SMS：</DIV>
					</TD>
						         <TD class="AlternatingItem" width="106">&nbsp;
				<asp:textbox id="txtIs_send_sms" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="item" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">AREA_DEPARTMENT：</DIV>
					</TD>
						         <TD class="item" width="106">&nbsp;
				<asp:textbox id="txtArea_department" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="AlternatingItem" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">AREA_ENTRI：</DIV>
					</TD>
						         <TD class="AlternatingItem" width="106">&nbsp;
				<asp:textbox id="txtArea_entri" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="item" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">AREA_GEOMERTRY_TYPE：</DIV>
					</TD>
						         <TD class="item" width="106">&nbsp;
				<asp:textbox id="txtArea_geomertry_type" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="AlternatingItem" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">AREA_LINECOLOR：</DIV>
					</TD>
						         <TD class="AlternatingItem" width="106">&nbsp;
				<asp:textbox id="txtArea_linecolor" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="item" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">AREA_LINEWIDTH：</DIV>
					</TD>
						         <TD class="item" width="106">&nbsp;
				<asp:textbox id="txtArea_linewidth" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								    				    				<TR class="trBGColor">
					<TD class="AlternatingItem" style="WIDTH: 106px" width="106">
						<DIV class="style6" align="right">AREA_DATA：</DIV>
					</TD>
						         <TD class="AlternatingItem" width="106">&nbsp;
				<asp:textbox id="txtArea_data" runat="server" width="200px" cssclass="input"></asp:textbox>
		 </TD>
	
				</TR>
								
			</TABLE>
		</TD>
	</TR>
	<TR>
		<TD align="right">
			<asp:button id="btnSave" runat="server" cssclass="btn" text="保存"></asp:button>&nbsp;&nbsp;
			<asp:button id="btnCancel" runat="server" cssclass="btn" text="取消"></asp:button>
		</TD>
	</TR>
</TABLE>
				</td>
			</tr>			
		</table>
	</form>
	</body>
</HTML>
