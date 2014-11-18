<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MerchantDeliverFeeMain.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.MerchantDeliverFeeMain"
	 MasterPageFile="~/FinancialManage/MerchantDeliverFee.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/FinancialManage/MerchantDeliverFee.Master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
商家配送费设置
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperatorArea" runat="server">
	<asp:Button ID="btnAddFee" runat="server" Text="新增配送价格" 
		onclick="btnAddFee_Click" />
	<asp:Button ID="btnEditFee" runat="server" Text="修改配送价格" 
		onclick="btnEditFee_Click" />
	<asp:Button ID="btnDelFee" runat="server" Text="选中删除" 
		onclick="btnDelFee_Click" OnClientClick="return ClientConfirm('确定删除？')" />

    <asp:Button ID="btnAddFeeWait" runat="server" Text="添加待生效" 
        onclick="btnAddFeeWait_Click" />
    <asp:Button ID="btnUpdateFeeWait" runat="server" Text="更新待生效" 
        onclick="btnUpdateFeeWait_Click" />
	<br />
	<asp:Button ID="btDownTemplet" runat="server" Text="导入模板下载" 
		onclick="btDownTemplet_Click" />
	<asp:FileUpload ID="FileUpload1" runat="server" />
	<asp:Button ID="btImport" runat="server" Text="导入" onclick="btImport_Click" />
</asp:Content>