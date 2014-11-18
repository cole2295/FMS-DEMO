<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountBuild.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.AccountBuild" MasterPageFile="~/Main/main.Master" Theme="default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">

	<script>
	function fnCheckAllList(LE, E) {
		LE.find('input:checkbox').attr('checked', E.checked)
	}

	function ClientConfirm(msg) {
		return !msg ? false : window.confirm(msg);
	}

	function fnShowLayer(url) {
		var k = window.showModalDialog(url, window, 'dialogWidth=330px;dialogHeight=300px;center:yes;resizable:no;scroll:no;help=no;status=no;');
		if (k == 'refreshParent') {
			document.getElementById("reload").click();
		}
	}
</script>


</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="body">
    <a id="reload" href="AccountBuild.aspx?accountNo=<%= AccountNO %>" style="display:none"></a>
<table>
	<tr>
		<td>时间</td>
		<%--<td><asp:TextBox ID="txtBeginTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox></td>--%>
		<td><asp:TextBox ID="txtBeginTime" Width="128px" Visible="true" 
					onFocus="WdatePicker({startDate:'%y-%M-%d 00:00:00',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})" runat="server"></asp:TextBox>
			</td>
		<td>-</td>
		<%--<td><asp:TextBox ID="txtEndTime" runat="server" CssClass="Wdate text" autocomplete="off"></asp:TextBox></td>--%>
		<td>
				<asp:TextBox ID="txtEndTime" Width="128px" Visible="true" 
					onFocus="WdatePicker({startDate:'%y-%M-%d 23:59:59',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})" runat="server"></asp:TextBox>
			</td>
		<td>商家</td>
		<td>
			<asp:DropDownList ID="ddlMerchant" runat="server">
			</asp:DropDownList>
		</td>
		<td>
			<asp:Button ID="btSearch" runat="server" Text="查询" onclick="btSearch_Click" />
		</td>
		<td><asp:Label ID="lbAccountMsg" runat="server" Visible="false">
				结算单号 
				<asp:Label ID="lbAccountNo" runat="server"></asp:Label>
			</asp:Label>
		</td>
	</tr>
</table>
<div>
	<asp:Button ID="btEditAccount" runat="server" Text="修改" 
		onclick="btEditAccount_Click" />
	<asp:Button ID="btSubmitAccount" runat="server" Text="提交结算单" 
		onclick="btSubmitAccount_Click" OnClientClick="return ClientConfirm('确认提交此结算单？');" />
	<asp:Button ID="btDeleteAccount" runat="server" Text="删除结算单号" 
		onclick="btDeleteAccount_Click" OnClientClick="return ClientConfirm('确认删除选中的结算单号？');" />
	<asp:Button ID="btBuildAccount" runat="server" Text="创建并生成结算单号" 
		onclick="btBuildAccount_Click" />
	<asp:Button ID="btAccountAreaSummary" runat="server" Text="区域汇总表" 
		onclick="btAccountAreaSummary_Click" />	
		<asp:Button ID="btExprotDetail" runat="server" Text="导出明细" 
		onclick="btExprotDetail_Click" />
</div>
<div style="text-align:center"><asp:Label ID="noData" runat="server" Visible="false" ForeColor="Red">查询无数据</asp:Label></div>
<asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false" 
		onrowdatabound="gvList_RowDataBound" DataKeyNames="DetailID,ExpressCompanyID,AreaType,DataType">
	<Columns>
		<asp:TemplateField HeaderText="选择" Visible="false">
			<HeaderTemplate>
			</HeaderTemplate>
			<ItemTemplate>
				<asp:CheckBox ID="cbCheck" runat="server" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField HeaderText="分拣中心" DataField="CompanyName" />
		<asp:BoundField HeaderText="区域类型" DataField="AreaType" />
		<asp:BoundField HeaderText="普发数" DataField="DeliveryNum" />
		<asp:BoundField HeaderText="换发数" DataField="DeliveryVNum" />
		<asp:BoundField HeaderText="普拒数" DataField="ReturnsNum" />
		<asp:BoundField HeaderText="换拒数" DataField="ReturnsVNum" />
		<asp:BoundField HeaderText="退数" DataField="VisitReturnsNum" />
		<asp:BoundField HeaderText="退拒数" DataField="VisitReturnsVNum" />
		<asp:BoundField HeaderText="普发结算标准" DataField="DeliveryStandard" />
		<asp:BoundField HeaderText="普发配送费" DataField="DeliveryFare" />
		<asp:BoundField HeaderText="换发结算标准" DataField="DeliveryVStandard" />
		<asp:BoundField HeaderText="换发配送费" DataField="DeliveryVFare" />
		<asp:BoundField HeaderText="普拒结算标准" DataField="RetrunsStandard" />
		<asp:BoundField HeaderText="普拒配送费" DataField="RetrunsFare" />
		<asp:BoundField HeaderText="换拒结算标准" DataField="ReturnsVStandard" />
		<asp:BoundField HeaderText="换拒配送费" DataField="ReturnsVFare" />
		<asp:BoundField HeaderText="退结算标准" DataField="VisitReturnsStandard" />
		<asp:BoundField HeaderText="退配送费" DataField="VisitReturnsFare" />
		<asp:BoundField HeaderText="退拒结算标准" DataField="VisitReturnsVStandard" />
		<asp:BoundField HeaderText="退拒配送费" DataField="VisitReturnsVFare" />
		<asp:BoundField HeaderText="保价费结算标准" DataField="ProtectedStandard" />
		<asp:BoundField HeaderText="保价费" DataField="ProtectedFee" />
		<asp:BoundField HeaderText="代货现金结算标准" DataField="ReceiveStandard" />
		<asp:BoundField HeaderText="代货手续费" DataField="ReceiveFee" />
		<asp:BoundField HeaderText="代货POS结算标准" DataField="ReceivePOSStandard" />
		<asp:BoundField HeaderText="POS机服务费" DataField="ReceivePOSFee" />
		<asp:BoundField HeaderText="超区补助" DataField="OverAreaSubsidy" />
		<asp:BoundField HeaderText="KPI考核" DataField="KPI" />
		<asp:BoundField HeaderText="丢失扣款" DataField="LostDeduction" />
		<asp:BoundField HeaderText="滞留扣款" DataField="ResortDeduction" />
        <asp:BoundField HeaderText="提货费" DataField="DeliveryFee" />
		<asp:BoundField HeaderText="折扣" DataField="DiscountFee" />
		<asp:BoundField HeaderText="其他费用" DataField="OtherFee" />
		<asp:BoundField HeaderText="实际结算费用" DataField="Fare" />
	</Columns>
</asp:GridView>

</asp:Content>