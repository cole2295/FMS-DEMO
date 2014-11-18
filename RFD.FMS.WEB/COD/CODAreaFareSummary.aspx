<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CODAreaFareSummary.aspx.cs" Inherits="RFD.FMS.WEB.COD.CODAreaFareSummary" Theme="default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<base target="_self" />
    <title>区域运费汇总表</title>
    <script type="text/javascript" src="../Scripts/base.js"></script>
    <script type="text/javascript" src="../Scripts/import/common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="OperatorArea">
		<asp:Button ID="btPrint" runat="server" Text="打印" onclick="btPrint_Click" />
		<asp:Button ID="btExport" runat="server" Text="导出" onclick="btExport_Click" />
    </div>
    <div id="PageHead" style="margin:10px; line-height:25px;">
		<span>结算单号：</span>
		<span><asp:Label ID="lbAccountNO" runat="server"></asp:Label></span>
		<span style="padding-left:20px;">配送商：</span>
		<span><asp:Label ID="lbCompanyName" runat="server"></asp:Label></span>
		<span style="padding-left:20px;">商家：</span>
		<span><asp:Label ID="lbMerchantName" runat="server"></asp:Label></span>
    </div>
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false">
		<Columns>
			<asp:BoundField HeaderText="区域类型" DataField="AreaType" />
			<asp:BoundField HeaderText="普发订单数量" DataField="DeliveryNum" />
			<asp:BoundField HeaderText="换发订单数量" DataField="DeliveryVNum" />
			<asp:BoundField HeaderText="普拒订单数量" DataField="ReturnsNum" />
			<asp:BoundField HeaderText="换拒订单数量" DataField="ReturnsVNum" />
			<asp:BoundField HeaderText="上门退货数量" DataField="VisitReturnsNum" />
			<asp:BoundField HeaderText="结算单量" DataField="AccountNum" />
			<asp:BoundField HeaderText="结算标准" DataField="Formula" />
			<asp:BoundField HeaderText="基准运费" DataField="DatumFare" />
			<asp:BoundField HeaderText="超区补助" DataField="Allowance" />
			<asp:BoundField HeaderText="KPI考核" DataField="KPI" />
			<asp:BoundField HeaderText="POS手续费" DataField="POSPrice" />
			<asp:BoundField HeaderText="滞留扣款" DataField="StrandedPrice" />
			<asp:BoundField HeaderText="城际丢失调整" DataField="IntercityLose" />
            <asp:BoundField HeaderText="代收手续费" DataField="CollectionFee" />
            <asp:BoundField HeaderText="投递费" DataField="DeliveryFee" />
			<asp:BoundField HeaderText="其他费用" DataField="OtherCost" />
			<asp:BoundField HeaderText="实际结算运费" DataField="Fare" />
		</Columns>
    </asp:GridView>
    <div id="PageFooter" style="margin:10px; line-height:25px;">
		<span>发货时间：</span>
		<span style="width:160px; padding-right:20px;">
			<asp:Label ID="lb_D_Date_S" runat="server"></asp:Label>~<asp:Label ID="lb_D_Date_E" runat="server"></asp:Label>
		</span>
		<span>拒收入库时间：</span>
		<span style="width:160px; padding-right:20px;">
			<asp:Label ID="lb_R_Date_S" runat="server"></asp:Label>~<asp:Label ID="lb_R_Date_E" runat="server"></asp:Label>
		</span>
		<span>上门退入库时间：</span>
		<span style="width:160px;">
			<asp:Label ID="lb_V_Date_S" runat="server"></asp:Label>~<asp:Label ID="lb_V_Date_E" runat="server"></asp:Label>
		</span>
    </div>
    </form>
</body>
</html>
