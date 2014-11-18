<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
	CodeBehind="StationDeliverFeeHistory.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.StationDeliverFeeHistory"
	Theme="default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>查看站点配送费历史</title>

	<script src="../Scripts/finance/finance.deliverfee.js" type="text/javascript"></script>

	<script type="text/javascript">
		$(function() {
			deliver.init.ready({ update: false, basic: false });
		})
	</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<table cellpadding="0" cellspacing="5" border="0" width="100%" id="container">
		<tr>
			<td>
				<asp:Label ID="lblMerchantName" runat="server" Text="" CssClass="title"></asp:Label>
			</td>
			<td align="right">
				<asp:Button ID="btnReturn" runat="server" Text="返回" OnClick="btnReturn_Click" />
			</td>
		</tr>
	</table>
	<div style="overflow: auto; height: 410px; padding-top: 10px; width: 100%;">
		<asp:GridView ID="gvDeliverFeeHistory" runat="server" AutoGenerateColumns="False"
			Width="100%" OnRowDataBound="gvDeliverFeeHistory_RowDataBound">
			<EmptyDataRowStyle HorizontalAlign="Center" />
			<HeaderStyle Wrap="false" />
			<RowStyle Wrap="false" />
			<Columns>
				<asp:BoundField HeaderText="部门名称" DataField="CompanyName" />
				<asp:BoundField HeaderText="配送费(元)" DataField="BasicDeliverFee" />
				<asp:BoundField HeaderText="修改人" DataField="UpdateName" />
				<asp:BoundField HeaderText="修改人编号" DataField="UpdateCode" />
				<asp:BoundField HeaderText="修改时间" DataField="UpdateTime" />
				<asp:BoundField HeaderText="审核人" DataField="AuditName" />
				<asp:BoundField HeaderText="审核人编号" DataField="AuditCode" />
				<asp:BoundField HeaderText="审核时间" DataField="AuditTime" />
				<asp:BoundField HeaderText="审核结果" DataField="AuditResult" />
				<asp:BoundField HeaderText="状态" DataField="Status" />
			</Columns>
		</asp:GridView>
	</div>
</asp:Content>
