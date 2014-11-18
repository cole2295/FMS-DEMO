<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountDetailView.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.AccountDetailView" MasterPageFile="~/Main/main.Master" Theme="default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">

</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="body">
<table style="margin:20px; line-height:30px;">
	<tr>
		<td>结算单号：</td>
		<td><asp:Label ID="lbAccountNO" runat="server"></asp:Label></td>
		<td style=" padding-left:10px;">商家：</td>
		<td><asp:Label ID="lbMerchant" runat="server"></asp:Label></td>
		<td style=" padding-left:10px;">时间：</td>
		<td><asp:Label ID="lbDateStr" runat="server"></asp:Label></td>
		<td> ~ </td>
		<td><asp:Label ID="lbDateEnd" runat="server"></asp:Label></td>
	</tr>
</table>
<asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false" 
		onrowdatabound="gvList_RowDataBound" DataKeyNames="DetailID,ExpressCompanyID,AreaType,DataType">
	<Columns>
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
		<asp:BoundField HeaderText="代货现金手续费" DataField="ReceiveFee" />
		<asp:BoundField HeaderText="代货POS结算标准" DataField="ReceivePOSStandard" />
		<asp:BoundField HeaderText="代货POS机手续费" DataField="ReceivePOSFee" />
        <asp:BoundField HeaderText="提货费" DataField="DeliveryFeeFee" />
		<asp:BoundField HeaderText="折扣" DataField="DiscountFee" />
		<asp:BoundField HeaderText="其他费用" DataField="OtherFee" />
		<asp:BoundField HeaderText="实际结算费用" DataField="Fare" />
	</Columns>
</asp:GridView>
</asp:Content>