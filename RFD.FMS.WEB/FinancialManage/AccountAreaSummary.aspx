<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountAreaSummary.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.AccountAreaSummary" MasterPageFile="~/Main/main.Master" Theme="default" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">

</asp:Content>


<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="body">
    <div>
		<asp:Button ID="btPrint" runat="server" Text="打印" onclick="btPrint_Click" />
		<asp:Button ID="btExprot" runat="server" Text="导出" onclick="btExprot_Click" />
    </div>
	<table style="line-height:25px;">
		<tr>
			<td>结算单号:</td>
			<td><asp:Label ID="lbAccountNO" runat="server"></asp:Label></td>
			<td style=" padding-left:10px;">商家:</td>
			<td><asp:Label ID="lbMerchant" runat="server"></asp:Label></td>
			<td style=" padding-left:10px;">结算日期:</td>
			<td><asp:Label ID="lbDateStr" runat="server"></asp:Label></td>
			<td> ~ </td>
			<td><asp:Label ID="lbDateEnd" runat="server"></asp:Label></td>
		</tr>
	</table>
	<asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false">
		<Columns>
			<asp:BoundField HeaderText="区域类型" DataField="AreaType" />
			<asp:BoundField HeaderText="妥投单量" DataField="DeliveryNum" />
			<asp:BoundField HeaderText="妥投配送费" DataField="DeliveryFare" />
			<asp:BoundField HeaderText="拒收单量" DataField="ReturnsVNum" />
			<asp:BoundField HeaderText="拒收配送费" DataField="DeliveryVFare" />
			<asp:BoundField HeaderText="保价费用" DataField="ProtectedFee" />
			<asp:BoundField HeaderText="pos机手续费" DataField="ReceiveFee" />
			<asp:BoundField HeaderText="现金手续费" DataField="ReceivePOSFee" />
            <asp:BoundField HeaderText="提货费" DataField="DeliveryFee" />
			<asp:BoundField HeaderText="折扣" DataField="DiscountFee" />
			<asp:BoundField HeaderText="其他" DataField="OtherFee" />
			<asp:BoundField HeaderText="合计" DataField="Total" />
		</Columns>
	</asp:GridView>
</asp:Content>

