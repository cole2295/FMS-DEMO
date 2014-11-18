<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryPriceSearch.aspx.cs" Inherits="RFD.FMS.WEB.COD.DeliveryPriceSearch" MasterPageFile="~/COD/DeliveryPrice.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/COD/DeliveryPrice.Master" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
配送商配送费价格查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperateContent" runat="server">
	<div>
		<asp:Button ID="DownLoad" runat="server" Text="下载" onclick="DownLoad_Click" />
		<asp:Button ID="SearchHistory" runat="server" Text="查询历史" 
			onclick="SearchHistory_Click" />
		<asp:Button ID="SearchLog" runat="server" Text="查询操作日志" 
			onclick="SearchLog_Click" />
	</div>
</asp:Content>