<%@ Page Title="" Language="C#" MasterPageFile="~/FinancialManage/DeliverFee.Master" AutoEventWireup="true"
	CodeBehind="QueryDeliverFee.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.QueryDeliverFee"
	Theme="default" %>

<%@ MasterType VirtualPath="~/FinancialManage/DeliverFee.Master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
商家配送费查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperatorArea" runat="server">
	<asp:Button ID="btnViewPrice" runat="server" Text="查询收入配送价格" 
		onclick="btnViewPrice_Click" />
</asp:Content>
<%--<asp:Content ID="Content2" ContentPlaceHolderID="link" runat="server">
	<a href="MerchantDeliverFeeHistory.aspx?mid=<%# Eval("MerchantID") %>&op=q">
		查看</a>
</asp:Content>--%>