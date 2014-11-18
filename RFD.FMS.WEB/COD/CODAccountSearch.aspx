<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CODAccountSearch.aspx.cs" Inherits="RFD.FMS.WEB.COD.CODAccountSearch" MasterPageFile="~/COD/COD.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/COD/COD.Master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
配送费发货结算查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperateContent" runat="server">
	<div>
		<asp:Button ID="btViewDetail" runat="server" Text="查询明细" 
			onclick="btViewDetail_Click" />
		<asp:Button ID="ExprotMsg" runat="server" Text="导出汇总表信息" 
			onclick="ExprotMsg_Click" />
		<asp:Button ID="PrintMsg" runat="server" Text="打印汇总表信息" 
			onclick="PrintMsg_Click" />
		<asp:Button ID="ViewAccountDetail" runat="server" Text="查看结算单明细" 
			onclick="ViewAccountDetail_Click" />
		<asp:Button ID="AreaFare" runat="server" Text="区域运费汇总表" 
			onclick="AreaFare_Click" />
	</div>
</asp:Content>