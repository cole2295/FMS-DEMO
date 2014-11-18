<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountAudit.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.AccountAudit" MasterPageFile="~/FinancialManage/Account.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/FinancialManage/Account.Master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
配送费收入结算审核
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="OperatorPlace">
	<asp:Button ID="btAudit" runat="server" Text="审核" onclick="btAudit_Click" />
	<asp:Button ID="btReset" runat="server" Text="置回" onclick="btReset_Click" />
	<asp:Button ID="btViewDetail" runat="server" Text="查看结算单明细" 
	onclick="btViewDetail_Click" />
	<asp:Button ID="btViewAccountAreaSummary" runat="server" Text="区域汇总表" 
		onclick="btViewAccountAreaSummary_Click" />
</asp:Content>