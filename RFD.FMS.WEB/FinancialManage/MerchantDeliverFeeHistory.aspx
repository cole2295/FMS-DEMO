<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
	CodeBehind="MerchantDeliverFeeHistory.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.MerchantDeliverFeeHistory"
	Theme="default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>查看商家配送费历史</title>

	<script type="text/javascript">
		$(function() {
			var params = getUrlParams();
			//只有"配送费审核"模块中处于"待审核"状态的才可驳回或审核
			if (params["op"] == "a" && params["status"] == "1") {
				$("input.show").show();
			} else {
				$("input.show").hide();
			}
			render();
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
				<asp:Button ID="btnReject" runat="server" Text="驳回" CssClass="show" OnClick="btnReject_Click"
					ToolTip="将当前商家的维护状态变为:待维护" />
				<asp:Button ID="btnAudited" runat="server" Text="审核通过" CssClass="show" OnClick="btnAudited_Click"
					ToolTip="将当前商家的维护状态变为:已审核" />
			</td>
		</tr>
	</table>
	<div style="overflow: auto; height: 410px; padding-top: 10px; width: 100%;">
		<asp:GridView ID="gvDeliverFeeHistory" runat="server" AutoGenerateColumns="False"
			Width="100%">
			<EmptyDataRowStyle HorizontalAlign="Center" />
			<HeaderStyle Wrap="false" />
			<RowStyle Wrap="false" />
			<Columns>
				<asp:BoundField HeaderText="货款结算周期" DataField="PaymentPeriod" />
				<asp:BoundField HeaderText="配送费结算周期" DataField="DeliverFeePeriod" />
				<asp:BoundField HeaderText="计费因素" DataField="FeeFactors" />
				<asp:BoundField HeaderText="各配送站基本配送费是否一致" DataField="IsUniformedFee" />
				<asp:TemplateField HeaderText="基本配送费(元)">
					<ItemTemplate>
						<a href="BasicDeliverFee.aspx?mid=<%#Eval("MerchantID") %>&op=<%=Option %>&status=<%# Eval("CurrentStatus") %>">
							<%#Eval("BasicDeliverFee") %></a>
					</ItemTemplate>
				</asp:TemplateField>
				<asp:BoundField HeaderText="单件计费公式" DataField="FormulaParamters" />
				<asp:BoundField HeaderText="拒收订单配送费(%)" DataField="RefuseFeeRate" />
				<asp:BoundField HeaderText="代收货款手续费(%)" DataField="ReceiveFeeRate" />
				<asp:BoundField HeaderText="修改人" DataField="UpdateBy" />
				<asp:BoundField HeaderText="修改人编号" DataField="UpdateCode" />
				<asp:BoundField HeaderText="修改时间" DataField="UpdateTime" />
				<asp:BoundField HeaderText="审核人" DataField="AuditBy" />
				<asp:BoundField HeaderText="审核人编号" DataField="AuditCode" />
				<asp:BoundField HeaderText="审核时间" DataField="AuditTime" />
				<asp:BoundField HeaderText="审核结果" DataField="AuditResult" />
				<asp:BoundField HeaderText="维护状态" DataField="Status" />
			</Columns>
		</asp:GridView>
	</div>
</asp:Content>
