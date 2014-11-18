<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IncomeAreaExpressLevelSearch.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.AreaExpressLevelIncomeSearch" MasterPageFile="~/Main/main.Master" Theme="default" %>

<%@ Import Namespace="System.Data" %>
<%@ Register Src="~/UserControl/PCASerach.ascx" TagName="UCPCASerach" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/WareHouseCheckBox.ascx" TagName="UCWareHouseCheckBox" TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc4" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    商家区域类型查询
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
	<script language="javascript" type="text/javascript" >
	    function fnOpenModalDialog(areaId, expressCompanyId, wareHouseId, merchantId, distributionCode) {
	        window.showModalDialog("IncomeAreaExpressLevelViewLog.aspx?areaId=" + areaId + "&expressCompanyId="
				+ expressCompanyId + "&wareHouseId=" + wareHouseId + "&merchantId=" + merchantId + "&distributionCode=" + distributionCode
			, window, "dialogWidth=800px;dialogHeight=400px;center:yes;resizable:no;scroll:auto;help=no;location=no;status=no;");
	    }
</script>
<table>
	<tr>
		<td>省市区：</td>
		<td><uc1:UCPCASerach ID="UCPCASerach" runat="server" /></td>
		<td>区域类型：</td>
		<td><asp:DropDownList ID="ddlAreaType" runat="server"></asp:DropDownList></td>
		<td>商家：</td>
		<td><asp:DropDownList ID="ddlMerchant" runat="server"></asp:DropDownList></td>
	</tr>
	<tr>
		<td>分拣中心：</td>
		<td><uc3:UCWareHouseCheckBox ID="UCWareHouseCheckBox" runat="server" LoadDataType="SortCenter" HouseCheckType="all" /></td>
		<td>审批状态：</td>
		<td><asp:DropDownList ID="ddlAudit" runat="server">
                <asp:ListItem Value="">请选择</asp:ListItem>
               <asp:ListItem Value="0">未审核</asp:ListItem>
                <asp:ListItem Value="1">已审核</asp:ListItem>
                <asp:ListItem Value="2">已生效</asp:ListItem>
                <asp:ListItem Value="3">已置回</asp:ListItem>
        </asp:DropDownList></td>
		<td colspan="2" align="left">
			<asp:Button ID="btSearch" runat="server" Text="查询" onclick="btSearch_Click" />
			<asp:Button ID="btExport" runat="server" Text="导出" onclick="btExport_Click" />
		</td>
	</tr>
</table>
<asp:Label ID="noData" runat="server" ForeColor="Red"></asp:Label>
<asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false" DataKeyNames="AutoID">
	<Columns>
		<%--<asp:BoundField HeaderText="配送公司" DataField="CompanyName" />--%>
		<asp:BoundField HeaderText="省份" DataField="ProvinceName" />
		<asp:BoundField HeaderText="城市" DataField="CityName" />
		<asp:BoundField HeaderText="地区" DataField="AreaName" />
		<asp:BoundField HeaderText="分拣中心" DataField="SortCenterName" />
		<asp:BoundField HeaderText="商家" DataField="MerchantName" />
		<asp:BoundField HeaderText="生效区域类型" DataField="AreaType" />
		<asp:BoundField HeaderText="首次新增区域类型" DataField="EffectAreaType" />
		<asp:BoundField HeaderText="生效状态" DataField="EnableStr" />
		<asp:BoundField HeaderText="审批状态" DataField="AuditStatusStr" />
		<asp:TemplateField HeaderText="查看操作日志">
			<ItemTemplate>
				<a href="javascript:fnOpenModalDialog('<%#((DataRowView)Container.DataItem)["AreaID"] %>',
														'<%#((DataRowView)Container.DataItem)["ExpressCompanyID"] %>',
														'<%#((DataRowView)Container.DataItem)["WareHouseID"] %>',
														'<%#((DataRowView)Container.DataItem)["MerchantID"] %>', 
                                                        '<%#((DataRowView)Container.DataItem)["DistributionCode"] %>')">查看操作日志</a>
			</ItemTemplate>
		</asp:TemplateField>		
	</Columns>
</asp:GridView>
<uc4:UCPager ID="UCPager" runat="server" />
</asp:Content>
