<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountSearch.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.AccountSearch"  MasterPageFile="~/FinancialManage/Account.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/FinancialManage/Account.Master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
配送费收入结算查询
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="OperatorPlace">
<asp:Button ID="btViewAccountDetail" runat="server" Text="查看结算单明细" 
		onclick="btViewAccountDetail_Click" />
		<asp:Button ID="btViewAccountAreaSummary" runat="server" Text="区域汇总表" 
		onclick="btViewAccountAreaSummary_Click" />
</asp:Content>