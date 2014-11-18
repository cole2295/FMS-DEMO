<%@ Page Title="" Language="C#" MasterPageFile="~/FinancialManage/DeliverFee.Master" AutoEventWireup="true"
	CodeBehind="AuditDeliverFee.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.AuditDeliverFee"
	Theme="default" %>

<%@ MasterType VirtualPath="~/FinancialManage/DeliverFee.Master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    商家配送费审核
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperatorArea" runat="server">
    <asp:Button ID="btnAuditPrice" runat="server" Text="审核收入配送价格" 
		onclick="btnAuditPrice_Click" />
	<asp:Button ID="btnAuditBasic" runat="server" Text="审核收入结算基础信息" 
		onclick="btnAuditBasic_Click" OnClientClick="return ClientConfirm('确认审核选中的记录？')" />
	<asp:Button ID="btnResetBaic" runat="server" Text="置回收入结算基础信息" 
		onclick="btnResetBaic_Click" OnClientClick="return ClientConfirm('确认置回选中的记录？')" />

    <asp:Button ID="btAuditEffectBasic" runat="server" Text="审核待生效基础信息" 
    onclick="btAuditEffectBasic_Click" />
    <asp:Button ID="btnResetEffectBasic" runat="server" Text="置回待生效基础信息" 
    onclick="btnResetEffectBasic_Click" />
</asp:Content>

<%--<asp:Content ID="Content2" ContentPlaceHolderID="link" runat="server">
	<a href="MerchantDeliverFeeHistory.aspx?mid=<%# Eval("MerchantID") %>&op=a&status=<%# Eval("CurrentStatus") %>">
		查看</a>
</asp:Content>
--%>