<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryPriceMain.aspx.cs" Inherits="RFD.FMS.WEB.COD.DeliveryPriceMain" MasterPageFile="~/COD/DeliveryPrice.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/COD/DeliveryPrice.Master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
配送商配送费价格设置
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperateContent" runat="server">
	<div>
		<asp:Button ID="ImportExcel" runat="server" Text="导入模板下载"
			onclick="ImportExcel_Click" />
		<asp:FileUpload ID="fuExprot" runat="server" />
		<asp:Button ID="Import" runat="server" Text="导入"
			onclick="Import_Click" />
		<asp:Button ID="DownLoad" runat="server" Text="下载" onclick="DownLoad_Click" />
        <asp:Button ID="BatchAddEffectDeliveryPrice" runat="server" Text="批量添加待生效价格" 
            onclick="BatchAddEffectDeliveryPrice_Click" />
        <asp:Button ID="BatchUpdateEffectDeliveryPrice" runat="server" Text="批量更新待生效价格" 
            onclick="BatchUpdateEffectDeliveryPrice_Click" />
	</div>
	<div>
		<asp:Button ID="AddDeliveryPrice" runat="server" Text="新增价格配置" 
			onclick="AddDeliveryPrice_Click" />
		<asp:Button ID="UpDateDeliveryPrice" runat="server" Text="更新价格配置" 
			onclick="UpDateDeliveryPrice_Click" />
		<asp:Button ID="Delete" runat="server" Text="删除" onclick="Delete_Click" />
		
		<asp:Button ID="SearchHistory" runat="server" Text="查询历史" 
			onclick="SearchHistory_Click" />
		<asp:Button ID="SearchLog" runat="server" Text="查询操作日志" 
			onclick="SearchLog_Click" style="height: 26px" />
		<asp:Button ID="AddEffectDeliveryPrice" runat="server" Text="添加待生效价格" 
			onclick="AddEffectDeliveryPrice_Click" />
		<asp:Button ID="UpdateEffectDeliveryPrice" runat="server" Text="更新待生效价格" 
			onclick="UpdateEffectDeliveryPrice_Click" />		
	</div>
</asp:Content>