<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountMain.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.AccountMain" MasterPageFile="~/FinancialManage/Account.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/FinancialManage/Account.Master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
配送费收入结算创建
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="OperatorPlace">
	<asp:Button ID="btBuildAccount" runat="server" Text="新建结算单" 
	onclick="btBuildAccount_Click" />
	<asp:Button ID="btEditAccount" runat="server" Text="编辑结算单" 
		onclick="btEditAccount_Click" />
	<asp:Button ID="btDeleteAccount" runat="server" Text="删除结算单号" 
	onclick="btDeleteAccount_Click" OnClientClick="return ClientConfirm('确认删除选中的结算单号？');" />
	<asp:Button ID="btAccountAreaSummary" runat="server" Text="区域汇总表" 
		onclick="btAccountAreaSummary_Click" />
</asp:Content>