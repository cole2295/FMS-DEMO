<%@ Page Title="" Language="C#" MasterPageFile="~/FinancialManage/DeliverFee.Master" AutoEventWireup="true"
	CodeBehind="MaintainDeliverFee.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.MaintainDeliverFee"
	Theme="default" %>

<%@ MasterType VirtualPath="~/FinancialManage/DeliverFee.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
	<title>配送费维护</title>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
商家配送费设置
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperatorArea" runat="server">
	<asp:Button ID="btnEditPrice" runat="server" Text="维护收入配送价格" 
		onclick="btnEditPrice_Click" />
	<asp:Button ID="btnEditBasic" runat="server" Text="维护收入结算基础信息" 
		onclick="btnEditBasic_Click" />
    <asp:Button ID="btnAddEffectBasic" runat="server" Text="增加待生效基础信息" 
        onclick="btnAddEffectBasic_Click" />
    <asp:Button ID="btnUpdateEffectBasic" runat="server" Text="更新待生效基础信息" 
        onclick="btnUpdateEffectBasic_Click" />
</asp:Content>