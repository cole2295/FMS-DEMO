<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
	CodeBehind="DelayOrderChecked.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.DelayOrderChecked"
	Theme="default" %>

<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>滞留订单核对</title>

	<script src="../Scripts/import/check.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
// <![CDATA[

        function import_onclick() {

        }

// ]]>
    </script>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
滞留订单核对
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<table cellpadding="0" cellspacing="5" border="0" width="100%">
		<tr>
			<td style="float: left;">
				<a href="../UpFile/滞留订单数据模板.xlsx" style="color: blue;">下载模板</a>
				<input type="file" style="width: 180px" runat="server" id="txtFile" title="请你先下载模板到本地，然后填写相关的订单信息再进行导入！" />
				<input id="import" type="button" value="核对导出" title="请你先下载模板到本地，然后填写相关的订单信息再进行导入！"
					disabled="disabled" onclick="return import_onclick()" />
				<asp:Button ID="btnImport" runat="server" Text="核对导出" OnClick="btnImport_Click" Style="display: none;" />
			</td>
			<%--<td colspan="2" style="width:100%;">
				导出格式：<asp:RadioButtonList ID="rblExportFormat" runat="server" RepeatDirection="Horizontal"
					RepeatLayout="Flow">
				</asp:RadioButtonList>
			</td>--%>
		</tr>
		<tr>
			<td valign="top">
			    <asp:Literal ID="Literal12" runat="server"></asp:Literal>
				<asp:Literal ID="ltlResult" runat="server"></asp:Literal>
			</td>
		</tr>
		<%--<tr>
			<td valign="top" width="30%">
				<fieldset>
					<legend>导入列表<asp:Literal ID="ltlTotalInfo" runat="server"></asp:Literal></legend>
					<div>
						<asp:Button ID="btnCheck" runat="server" Text="核对" OnClick="btnCheck_Click" />
					</div>
					<div style="overflow: auto; width: 350px; height: 365px; padding-top: 5px;">
						<table border="0">
							<tr>
								<td>
									<asp:GridView ID="gvTotalData" runat="server" AutoGenerateColumns="false" AllowPaging="false"
										Width="100%">
										<HeaderStyle Wrap="false" />
										<RowStyle Wrap="false" />
										<EmptyDataRowStyle HorizontalAlign="Center" />
										<Columns>
											<asp:BoundField DataField="ID" HeaderText="序号" />
											<asp:BoundField DataField="CompanyName" HeaderText="配送站" />
											<asp:BoundField DataField="WaybillNo" HeaderText="订单号" />
											<asp:BoundField DataField="Amount" HeaderText="订单总价" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="FactAmount" HeaderText="已收款" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="NeedAmount" HeaderText="应收款" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="WayBillInfoWeight" HeaderText="订单重量" />
											<asp:BoundField DataField="WayBillBoxNo" HeaderText="包装箱型号" />
											<asp:BoundField DataField="WarehouseName" HeaderText="出库仓" />
											<asp:BoundField DataField="ReceiveAddress" HeaderText="收货人地址" />
											<asp:BoundField DataField="CreatTime" HeaderText="发货日期" />
											<asp:BoundField DataField="WaybillStatus" HeaderText="订单状态" />
										</Columns>
									</asp:GridView>
								</td>
							</tr>
							<tr>
								<td>
									<uc2:Pager ID="totalPager" runat="server" />
									<asp:HiddenField ID="hidTotalCount" runat="server" Value="0" />
								</td>
							</tr>
						</table>
					</div>
				</fieldset>
			</td>
			<td valign="top" width="35%">
				<fieldset>
					<legend>妥投/拒收入库/退换货入库订单<asp:Literal ID="ltlSuccessInfo" runat="server"></asp:Literal></legend>
					<div>
						<asp:Button ID="btnSuccessOrder" runat="server" Text="导出订单" OnClick="btnSuccessOrder_Click" />
					</div>
					<div style="overflow: auto; width: 370px; height: 365px; padding-top: 5px;">
						<table border="0">
							<tr>
								<td>
									<asp:GridView ID="gvSuccessOrders" runat="server" AutoGenerateColumns="false" AllowPaging="false"
										Width="100%">
										<HeaderStyle Wrap="false" />
										<RowStyle Wrap="false" />
										<EmptyDataRowStyle HorizontalAlign="Center" />
										<Columns>
											<asp:BoundField DataField="ID" HeaderText="序号" />
											<asp:BoundField DataField="CompanyName" HeaderText="配送站" />
											<asp:BoundField DataField="WaybillNo" HeaderText="订单号" />
											<asp:BoundField DataField="Amount" HeaderText="订单总价" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="FactAmount" HeaderText="已收款" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="NeedAmount" HeaderText="应收款" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="WayBillInfoWeight" HeaderText="订单重量" />
											<asp:BoundField DataField="WayBillBoxNo" HeaderText="包装箱型号" />
											<asp:BoundField DataField="WarehouseName" HeaderText="出库仓" />
											<asp:BoundField DataField="ReceiveAddress" HeaderText="收货人地址" />
											<asp:BoundField DataField="CreatTime" HeaderText="发货日期" />
											<asp:BoundField DataField="WaybillStatus" HeaderText="订单状态" />
										</Columns>
									</asp:GridView>
								</td>
							</tr>
							<tr>
								<td>
									<uc2:Pager ID="successPager" runat="server" />
									<asp:HiddenField ID="hidSuccessCount" runat="server" Value="0" />
								</td>
							</tr>
						</table>
					</div>
				</fieldset>
			</td>
			<td valign="top" width="35%">
				<fieldset>
					<legend>滞留订单<asp:Literal ID="ltlDelayInfo" runat="server"></asp:Literal></legend>
					<div>
						<asp:Button ID="btnDelayOrder" runat="server" Text="导出订单" OnClick="btnDelayOrder_Click" />
					</div>
					<div style="overflow: auto; width: 370px; height: 365px; padding-top: 5px;">
						<table border="0">
							<tr>
								<td>
									<asp:GridView ID="gvDelayOrders" runat="server" AutoGenerateColumns="false" AllowPaging="false"
										Width="100%">
										<HeaderStyle Wrap="false" />
										<RowStyle Wrap="false" />
										<EmptyDataRowStyle HorizontalAlign="Center" />
										<Columns>
											<asp:BoundField DataField="ID" HeaderText="序号" />
											<asp:BoundField DataField="CompanyName" HeaderText="配送站" />
											<asp:BoundField DataField="WaybillNo" HeaderText="订单号" />
											<asp:BoundField DataField="Amount" HeaderText="订单总价" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="FactAmount" HeaderText="已收款" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="NeedAmount" HeaderText="应收款" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="WayBillInfoWeight" HeaderText="订单重量" />
											<asp:BoundField DataField="WayBillBoxNo" HeaderText="包装箱型号" />
											<asp:BoundField DataField="WarehouseName" HeaderText="出库仓" />
											<asp:BoundField DataField="ReceiveAddress" HeaderText="收货人地址" />
											<asp:BoundField DataField="CreatTime" HeaderText="发货日期" />
											<asp:BoundField DataField="WaybillStatus" HeaderText="订单状态" />
										</Columns>
									</asp:GridView>
								</td>
							</tr>
							<tr>
								<td>
									<uc2:Pager ID="delayPager" runat="server" />
									<asp:HiddenField ID="hidDelayCount" runat="server" Value="0" />
								</td>
							</tr>
						</table>
					</div>
				</fieldset>
			</td>
		</tr>--%>
	</table>
</asp:Content>
