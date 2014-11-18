<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
	CodeBehind="BankPosChecked.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.BankPosChecked" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>银行POS机刷卡核对</title>

	<script type="text/javascript">
		//根据查询结果显示按钮是否可用
		function enableButtons() {
			var totalCount = $("#<%=hidTotalCount.ClientID %>").val();
			var successCount = $("#<%=hidSuccessCount.ClientID %>").val();
			var failCount = $("#<%=hidFailCount.ClientID %>").val();
			$("#<%=btnCheck.ClientID %>").attr("disabled", totalCount == "0");
			$("#<%=btnSucOutExcel.ClientID %>").attr("disabled", totalCount == "0" || successCount == "0");
			$("#<%=btnSucPDF.ClientID %>").attr("disabled", totalCount == "0" || successCount == "0");
			$("#<%=btnFailExcel.ClientID %>").attr("disabled", totalCount == "0" || failCount == "0");
			$("#<%=btnFailPDF.ClientID %>").attr("disabled", totalCount == "0" || failCount == "0");
			$("#<%=btnBatchQuery.ClientID %>").attr("disabled", "disabled");
		}

		$(function() {
			enableButtons();
			checkUploadFileFormat("<%=fileUpload.ClientID %>", "<%=btnBatchQuery.ClientID %>");
			$("#<%=btnCheck.ClientID %>").click(function() {
				//判断对账时间段
				var beginTime = $("#<%=txtBeginTime.ClientID %>").val();
				var endTime = $("#<%=txtEndTime.ClientID %>").val();
				return checkDate(beginTime, endTime,
		               {
		               	diffdays: 2,
		               	intevalText: "订单数据量较大，建议只查询间隔2天的数据请重新选择交易时间!"
		               });
			});
		})
	</script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
POS刷卡核对
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<table cellpadding="0" cellspacing="5" border="0" width="100%">
		<tr>
			<td colspan="3">
				<asp:Label ID="lblMessage" runat="server"></asp:Label>
				<a href="../UpFile/POS对账银行数据模板.xls" style="color: Blue;">下载模板</a>
				<asp:FileUpload ID="fileUpload" runat="server" Style="width: 180px" ToolTip="请你先下载模板到本地，然后填写相关的信息再进行导入！" />
				<asp:Button ID="btnBatchQuery" runat="server" Text="导入" OnClick="btnBatchQuery_Click"
					title="请你先下载模板到本地，然后填写相关的信息再进行导入！" />
			</td>
		</tr>
		<tr>
			<td style="width: 30%;" valign="top">
				<fieldset>
					<legend>
						<asp:Label ID="lblInTitle" runat="server">导入列表</asp:Label></legend>
					<div>
						<table cellpadding="0" cellspacing="0" border="0" width="100%" style="height: 55px">
							<tr>
								<td>
									交易时间：<asp:TextBox ID="txtBeginTime" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d 00:00:00',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"
										runat="server"></asp:TextBox>至<asp:TextBox ID="txtEndTime" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d 23:59:59',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"
											runat="server"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									配送部门：<asp:DropDownList ID="ddlStation" runat="server">
									</asp:DropDownList>
									<asp:Button ID="btnCheck" runat="server" Text="核对" OnClick="btnCheck_Click" />
								</td>
							</tr>
						</table>
					</div>
					<div style="overflow: auto; width: 100%; height: 500px;">
						<asp:GridView ID="gvInData" runat="server" AutoGenerateColumns="false" AllowPaging="false"
							Width="100%" EnableViewState="true">
							<Columns>
								<asp:BoundField DataField="ClientCode" HeaderText="终端编号" />
								<asp:BoundField DataField="CardNumber" HeaderText="卡号" />
								<asp:BoundField DataField="TradeDate" HeaderText="交易日期" />
								<asp:BoundField DataField="TradeTime" HeaderText="交易时间" />
								<asp:BoundField DataField="TradeMoney" HeaderText="交易金额" />
							</Columns>
							<EmptyDataTemplate>
								<label id="NOData">
									没有数据</label>
							</EmptyDataTemplate>
						</asp:GridView>
						<asp:HiddenField ID="hidTotalCount" runat="server" Value="0" />
					</div>
				</fieldset>
			</td>
			<td style="width: 35%;" valign="top">
				<fieldset>
					<legend>
						<asp:Label ID="lblSucTitle" runat="server">核对成功结果</asp:Label></legend>
					<div>
						<table cellpadding="0" cellspacing="0" border="0" width="100%" style="height: 55px">
							<tr>
								<td>
									<asp:Button ID="btnSucOutExcel" runat="server" Text="导出Excel" OnClick="btnSucOutExcel_Click" />
									<asp:Button ID="btnSucPDF" runat="server" Text="导出PDF" OnClick="btnSucPDF_Click" />
								</td>
							</tr>
						</table>
					</div>
					<div style="overflow: auto; width: 100%; height: 500px;">
						<asp:GridView ID="gvSucData" runat="server" AutoGenerateColumns="false" AllowPaging="false"
							Width="100%" EnableViewState="true">
							<Columns>
								<asp:BoundField DataField="ClientCode" HeaderText="终端编号" />
								<asp:BoundField DataField="OrderForm" HeaderText="订单号" />
								<asp:BoundField DataField="CardNumber" HeaderText="卡号" />
								<asp:BoundField DataField="TradeDate" HeaderText="交易日期" />
								<asp:BoundField DataField="TradeMoney" HeaderText="交易金额" />
								<asp:BoundField DataField="Result" HeaderText="核对结果" />
								<asp:BoundField DataField="ExpressCompanyName" HeaderText="交易站点名" />
							</Columns>
							<EmptyDataTemplate>
								<label id="NOData">
									没有数据</label>
							</EmptyDataTemplate>
						</asp:GridView>
						<asp:HiddenField ID="hidSuccessCount" runat="server" Value="0" />
					</div>
				</fieldset>
			</td>
			<td style="width: 35%;" valign="top">
				<fieldset>
					<legend>
						<asp:Label ID="lblFailTitle" runat="server">核对失败结果</asp:Label></legend>
					<div>
						<table cellpadding="0" cellspacing="0" border="0" width="100%" style="height: 55px">
							<tr>
								<td>
									<asp:Button ID="btnFailExcel" runat="server" Text="导出Excel" OnClick="btnFailExcel_Click" />
									<asp:Button ID="btnFailPDF" runat="server" Text="导出PDF" OnClick="btnFailPDF_Click" />
								</td>
							</tr>
						</table>
					</div>
					<div style="overflow: auto; width: 100%; height: 500px;">
						<asp:GridView ID="gvFailData" runat="server" AutoGenerateColumns="false" AllowPaging="false"
							Width="100%" EnableViewState="true">
							<Columns>
								<asp:BoundField DataField="WaybillNO" HeaderText="订单号" />
								<asp:BoundField DataField="POSCode" HeaderText="终端编号" />
								<asp:BoundField DataField="CreatTime" HeaderText="交易日期" />
								<asp:BoundField DataField="FactAmount" HeaderText="交易金额" />
								<asp:BoundField DataField="Result" HeaderText="核对结果" />
								<asp:BoundField DataField="CompanyName" HeaderText="交易站点名" />
							</Columns>
							<EmptyDataTemplate>
								<label id="NOData">
									没有数据</label>
							</EmptyDataTemplate>
						</asp:GridView>
						<asp:HiddenField ID="hidFailCount" runat="server" Value="0" />
					</div>
				</fieldset>
			</td>
		</tr>
	</table>
</asp:Content>
