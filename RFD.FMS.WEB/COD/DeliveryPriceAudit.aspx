<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryPriceAudit.aspx.cs" Inherits="RFD.FMS.WEB.COD.DeliveryPriceAudit" MasterPageFile="~/COD/DeliveryPrice.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/COD/DeliveryPrice.Master" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
配送商配送费价格审核
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperateContent" runat="server">
	<div>
		<asp:Button ID="Reset" runat="server" Text="置回" onclick="Reset_Click" />
		<asp:Button ID="Audit" runat="server" Text="审核" onclick="Audit_Click" />
		<asp:Button ID="DownLoad" runat="server" Text="下载" onclick="DownLoad_Click" />
		<asp:Button ID="SearchHistory" runat="server" Text="查询历史" 
			onclick="SearchHistory_Click" />
		<asp:Button ID="SearchLog" runat="server" Text="查询操作日志"
			onclick="SearchLog_Click" />
		<asp:Button ID="ResetEffect" runat="server" Text="置回待生效" 
			onclick="ResetEffect_Click" />
		<asp:Button ID="AuditEffect" runat="server" Text="审核待生效" 
			onclick="AuditEffect_Click" />		
	</div>
</asp:Content>
