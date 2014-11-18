<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
	Theme="default" CodeBehind="OrderMoneyStoreReport.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.OrderMoneyStoreReport" %>

<%@ Register Src="~/UserControl/SelectStation.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

	<script type="text/javascript">
		$(function() {
			//判断是否显示商家来源
			showMerchant({
				source: $("#<%=ddlOrderSource.ClientID %>"),
				merchant: $("#<%=ddlMerchantList.ClientID %>").parent()
			});
			render();
		})
		function check() {
			var stationTime = $.trim($("#<%=txtIntoStationTime.ClientID %>").val());
			var signTime = $.trim($("#<%=txtSignTime.ClientID %>").val());
			var beginTime = $.trim($("#<%=txtBegTime.ClientID %>").val());
			var endTime = $.trim($("#<%=txtEndTime.ClientID %>").val());
			if (stationTime != "" && !isdate(stationTime)) {
				alert("请输入合法的入站时间！");
				return false;
			}
			if (signTime != "" && !isdate(signTime)) {
				alert("请输入合法的签收时间！");
				return false;
			}
			if (beginTime != "" && !isdate(beginTime)) {
				alert("请输入合法的发货开始时间！");
				return false;
			}
			if (endTime != "" && !isdate(endTime)) {
				alert("请输入合法的发货结束时间！");
				return false;
			}
			if (beginTime == "" || endTime == "") {
				return true;
			}
			var seconds = seconddiff(beginTime, endTime);
			if (seconds < 0) {
				alert("发货结束时间必须大于等于发货开始时间！");
				return false;
			}
			return true;
		}
	</script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
入库状态查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table cellpadding="0" cellspacing="5" border="0" width="100%">
		<tr>
			<td align="right">
				运单号：
			</td>
			<td>
				<asp:TextBox ID="txtWayBillNO" runat="server"></asp:TextBox>
			</td>
			<td align="right">
				选择站点：
			</td>
			<td>
				<uc:SelectStation ID="station" runat="server" />
			</td>
			<td align="right">
				入库状态：
			</td>
			<td>
				<asp:DropDownList ID="ddlInBoundStatus" runat="server">
					<asp:ListItem Value="">请选择</asp:ListItem>
					<asp:ListItem Value="0">未入库</asp:ListItem>
					<asp:ListItem Value="1">已入库</asp:ListItem>
				</asp:DropDownList>
			</td>
			<td align="right">
				收款状态：
			</td>
			<td>
				<asp:DropDownList ID="ddlMoneyStatus" runat="server">
					<asp:ListItem Value="">请选择</asp:ListItem>
					<asp:ListItem Value="0">未付款</asp:ListItem>
					<asp:ListItem Value="1">已付款</asp:ListItem>
				</asp:DropDownList>
			</td>
			<td>
				订单来源：
			</td>
			<td>
				<asp:DropDownList ID="ddlOrderSource" runat="server">
				</asp:DropDownList>
			</td>
			<td>
				商家来源：
				<asp:DropDownList ID="ddlMerchantList" runat="server">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right">
				入站时间：
			</td>
			<td>
				<asp:TextBox ID="txtIntoStationTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>
			</td>
			<td align="right">
				签收时间：
			</td>
			<td>
				<asp:TextBox ID="txtSignTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>
			</td>
			<td align="right">
				发货时间：
			</td>
			<td colspan="6">
				<asp:TextBox ID="txtBegTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>
				—<asp:TextBox ID="txtEndTime" runat="server" CssClass="Wdate text" autocomplete="off"></asp:TextBox></td>
		</tr>
		<tr>
			<td colspan="11" align="right">
                <asp:Button ID="btnQuery" runat="server" Text="查询" OnClick="btnQuery_Click" OnClientClick="return check();" />&nbsp;&nbsp;
				<asp:Label ID="lblMessage" runat="server"></asp:Label>
				<a href="../UpFile/批量查询模板.xls">下载模板</a>
				<input type="file" style="width: 180px" runat="server" id="txtFile" title="请你先下载模板到本地，然后填写相关的订单信息再进行导入！" />
                <asp:RadioButton ID="rbWaybillno" runat="server" GroupName="rbSearchType" Text="运单号" Checked="true" />
                <asp:RadioButton ID="rbOrderId" runat="server" GroupName="rbSearchType" Text="订单号" />
				<asp:Button ID="btnBatchQuery" runat="server" Text="批量查询" OnClick="btnBatchQuery_Click"
					OnClientClick="return check();" Visible="false" />&nbsp;&nbsp;
                <asp:Button ID="btnBatchQueryV2" runat="server" Text="批量查询V2"
					OnClientClick="return check();" onclick="btnBatchQueryV2_Click" />&nbsp;&nbsp;
				
				<asp:Button ID="btnReportData" runat="server" Text="导出" OnClick="btnReportData_Click" />
			</td>
		</tr>
		<tr>
			<td colspan="11">
				<fieldset>
					<legend>配送站单量汇总</legend>
					<table style="width: 100%; border-width: 0px;">
						<tr>
							<td>
								<label for="">
									已收款单量：
								</label>
								<asp:Label ID="lblMoneyOrderCount" runat="server"></asp:Label>
							</td>
							<td>
								<label for="">
									未收款单量：
								</label>
								<asp:Label ID="lblNoMoneyOrderCount" runat="server"></asp:Label>
							</td>
							<td>
								<label for="">
									已入库单量：
								</label>
								<asp:Label ID="lblInBoundCount" runat="server"></asp:Label>
							</td>
							<td>
								<label for="">
									未入库单量：
								</label>
								<asp:Label ID="lblNotInBoundCount" runat="server"></asp:Label>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
	</table>
	<asp:GridView ID="gv" runat="server" Width="100%" AutoGenerateColumns="false" AllowPaging="false"
		EnableViewState="true" OnRowEditing="gv_RowEditing" DataKeyNames="WaybillSignInfoID">
		<HeaderStyle Wrap="false" />
		<RowStyle Wrap="false" />
		<Columns>
			<asp:BoundField DataField="RowNumber" HeaderText="序号" />
			<asp:BoundField DataField="SendTime" HeaderText="发货时间" />
			<asp:BoundField DataField="IntoTime" HeaderText="入站时间" />
			<asp:BoundField DataField="BackStationTime" HeaderText="归班结算时间" />
			<asp:BoundField DataField="WaybillNO" HeaderText="运单号" />
            <asp:BoundField DataField="CustomerOrder" HeaderText="订单号" />
			<asp:BoundField DataField="WaybillTypeName" HeaderText="订单类型" />
			<asp:BoundField DataField="StatusName" HeaderText="状态" />
			<asp:BoundField DataField="NeedAmount" HeaderText="应收金额" />
			<asp:BoundField DataField="NeedBackAmount" HeaderText="应退金额" />
			<asp:BoundField DataField="WayBillInfoWeight" HeaderText="订单重量" />
			<asp:BoundField DataField="AcceptType" HeaderText="付款方式" />
			<asp:BoundField DataField="EmployeeName" HeaderText="配送员" />
			<asp:BoundField DataField="FinancialStatus" HeaderText="财务收款状态" />
			<asp:BoundField DataField="FinancialTime" HeaderText="收款确认时间" />
			<asp:BoundField DataField="POSCode" HeaderText="POS机终端号" />
			<asp:BoundField DataField="BackStatusName" HeaderText="入库状态" />
			<asp:BoundField DataField="ISBackBound" HeaderText="是否入库" />
			<asp:BoundField DataField="CompanyName" HeaderText="站点" />
			<asp:BoundField DataField="MerchantName" HeaderText="商家名称" />
			<asp:CommandField EditText="确认" HeaderText="确认" ShowEditButton="true" />
		</Columns>
		<EmptyDataTemplate>
			<label id="NOData">
				没有数据</label>
		</EmptyDataTemplate>
	</asp:GridView>
	<uc2:Pager ID="pager" runat="server" />
	<asp:HiddenField ID="hidBatchData" runat="server" Value="0"/>

</asp:Content>
