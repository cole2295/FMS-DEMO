<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CODAccountMain.aspx.cs" Inherits="RFD.FMS.WEB.COD.CODAccountMain" MasterPageFile="~/COD/COD.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/COD/COD.Master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
配送费发货结算创建
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperateContent" runat="server">
	<div>
		<asp:Button ID="btViewDetail" runat="server" Text="查询明细" 
			onclick="btViewDetail_Click" />
		<asp:Button ID="ExprotMsg" runat="server" Text="导出汇总表信息" 
			onclick="ExprotMsg_Click" />
		<asp:Button ID="PrintMsg" runat="server" Text="打印汇总表信息" 
			onclick="PrintMsg_Click" />
		<asp:Button ID="AddAccount" runat="server" Text="新建结算单" 
			onclick="AddAccount_Click" />
		<asp:Button ID="EditAccount" runat="server" Text="编辑结算单" 
			onclick="EditAccount_Click" />
		<asp:Button ID="DeleteAccountNo" runat="server" Text="删除结算单号" 
			onclick="DeleteAccountNo_Click" OnClientClick="return ClientConfirm('确定删除选择的结算单？');" />
		<asp:Button ID="AreaFare" runat="server" Text="区域运费汇总表" 
			onclick="AreaFare_Click" />
	</div>
</asp:Content>