<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
	CodeBehind="FinanceCompare.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.FinanceCompare" %>

<%@ Import Namespace="RFD.FMS.MODEL.BasicSetting" %>
<%@ Register Src="~/UserControl/SelectStation.ascx" TagName="SelectStation" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>财务收款确认对碰</title>

	<script type="text/javascript">
        //根据查询结果显示按钮是否可用
        function enableButtons() {
            var totalCount = $("#<%=hidTotalCount.ClientID %>").val();
            var importCount = $("#<%=hidImportSign.ClientID %>").val();
            $("#<%=btnExportSysOrder.ClientID %>").attr("disabled", totalCount == "0" || importCount == "0");
            $("#<%=btnExportManualOrder.ClientID %>").attr("disabled", totalCount == "0" || importCount == "0");
            $("#<%=btnImport.ClientID %>").attr("disabled", "disabled");
        }
        $(function() {
            enableButtons();    
            checkUploadFileFormat("<%=txtFile.ClientID %>", "<%=btnImport.ClientID %>");
        })
	</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<table cellpadding="0" cellspacing="5" border="0" width="100%">
		<tr>
			<td>
				选择站点：
			</td>
			<td>
				<uc:SelectStation ID="station" runat="server" />
			</td>
			<td>
				提交配送日期：
				<asp:TextBox ID="txtDeliverTime" CssClass="Wdate" runat="server" autocomplete="off"></asp:TextBox>
			</td>
			<td>
				<asp:Button ID="btnSearch" runat="server" Text="查  询" OnClick="btnSearch_Click" />
				<a href="../UpFile/运单信息模板.xls" style="color: blue;">下载模板</a>
				<input type="file" style="width: 180px" runat="server" id="txtFile" title="请你先下载模板到本地，然后填写相关的运单信息再进行导入！" />
				<asp:Button ID="btnImport" runat="server" Text="导入该站点手工订单" OnClick="btnImport_Click"
					ToolTip="请你先下载模板到本地，然后填写相关的运单信息再进行导入！" />
				<asp:Button ID="btnExportSysOrder" runat="server" Text="导出系统对碰订单" OnClick="btnExportSysOrder_Click" />
				<asp:Button ID="btnExportManualOrder" runat="server" Text="导出手工对碰订单" OnClick="btnExportManualOrder_Click" />
			</td>
		</tr>
	</table>
	<asp:UpdatePanel ID="pnlData" runat="server">
		<ContentTemplate>
			<asp:GridView ID="gvSysOrders" runat="server" AutoGenerateColumns="False" Width="100%"
				OnRowDataBound="gvSysOrders_RowDataBound">
				<RowStyle HorizontalAlign="Center" />
				<EmptyDataRowStyle HorizontalAlign="Center" />
				<Columns>
					<asp:TemplateField HeaderText="序号">
						<ItemTemplate>
							<%# Container.DataItemIndex + 1%>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField HeaderText="运单号" DataField="运单号" />
					<asp:BoundField HeaderText="发货时间" DataField="发货时间" DataFormatString="{0:G}" />
					<asp:BoundField HeaderText="入站时间" DataField="入站时间" DataFormatString="{0:G}" />
					<asp:BoundField HeaderText="归班时间" DataField="归班时间" DataFormatString="{0:G}" />
					<asp:BoundField HeaderText="客户订单号" DataField="客户订单号" />
					<asp:TemplateField HeaderText="订单类型">
						<ItemTemplate>
							<asp:Label ID="lblWaybillType" runat="server" Text='<%# GetStatusName(StatusType.WaybillType, Eval("订单类型")) %>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:TemplateField HeaderText="订单状态">
						<ItemTemplate>
							<asp:Label ID="lblWaybillStatus" runat="server" Text='<%# GetStatusName(StatusType.WaybillStatus, Eval("订单状态")) %>'></asp:Label>
						</ItemTemplate>
					</asp:TemplateField>
					<asp:BoundField HeaderText="应收金额" DataField="应收金额" />
					<asp:BoundField HeaderText="应退金额" DataField="应退金额" />
					<asp:BoundField HeaderText="订单重量" DataField="订单重量" />
					<asp:BoundField HeaderText="付款方式" DataField="付款方式" />
					<asp:BoundField HeaderText="配送员" DataField="配送员" />
					<asp:BoundField HeaderText="POS机终端号" DataField="POS机终端号" />
				</Columns>
				<EmptyDataTemplate>
					<asp:Label ID="lblResult" runat="server" ForeColor="Red">查找不到任何关于此站点的数据，请重新设定条件查询！</asp:Label>
				</EmptyDataTemplate>
			</asp:GridView>
			<asp:HiddenField ID="hidTotalCount" runat="server" Value="0" />
			<asp:HiddenField ID="hidImportSign" runat="server" Value="0" />
			<asp:HiddenField ID="hidCompareFields" runat="server" Value="应收金额,应退金额,付款方式,配送员,POS机终端号" />
		</ContentTemplate>
		<Triggers>
			<asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
		</Triggers>
	</asp:UpdatePanel>
</asp:Content>
