<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CODAccountDetailView.aspx.cs" Inherits="RFD.FMS.WEB.COD.CODAccountDetailView" Theme="default" %>

<%@ Register Src="~/UserControl/WareHouseCheckBox.ascx" TagName="UCWareHouseCheckBox" TagPrefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<base target="_self" />
    <title>结算单明细</title>
    <script type="text/javascript" src="../Scripts/base.js"></script>
    <script type="text/javascript" src="../Scripts/import/common.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="OperatorArea">
		<asp:Button ID="btPrint" runat="server" Text="打印" onclick="btPrint_Click" />
		<asp:Button ID="btExport" runat="server" Text="导出" onclick="btExport_Click" />
    </div>
    <table style="line-height:25px; margin:5px;">
		<tr>
			<td>结算单号：</td>
			<td><asp:Label ID="lbAccountNO" runat="server"></asp:Label></td>
			<td align="right">配送商：</td>
			<td><asp:Label ID="lbCompanyName" runat="server"></asp:Label></td>
			<td align="right">商家：</td>
			<td><asp:Label ID="lbMerchantName" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td>发货时间：</td>
			<td><asp:Label ID="lb_D_Date_S" runat="server"></asp:Label>~<asp:Label ID="lb_D_Date_E" runat="server"></asp:Label></td>
			<td>拒收入库时间：</td>
			<td><asp:Label ID="lb_R_Date_S" runat="server"></asp:Label>~<asp:Label ID="lb_R_Date_E" runat="server"></asp:Label></td>
			<td>上门退入库时间：</td>
			<td><asp:Label ID="lb_V_Date_S" runat="server"></asp:Label>~<asp:Label ID="lb_V_Date_E" runat="server"></asp:Label></td>
		</tr>
		<tr>
			<td>发货仓库：</td>
			<td><uc2:UCWareHouseCheckBox id="ucDeliveryHouse" LoadDataType="All" HouseCheckType="all" runat="server"></uc2:UCWareHouseCheckBox></td>
			<td>拒收入库仓库：</td>
			<td><uc2:UCWareHouseCheckBox id="ucReturnHouse" LoadDataType="All" HouseCheckType="all" runat="server"></uc2:UCWareHouseCheckBox></td>
			<td>上门退入库仓库：</td>
			<td><uc2:UCWareHouseCheckBox id="ucVisitReturnHouse" LoadDataType="All" HouseCheckType="all" runat="server"></uc2:UCWareHouseCheckBox></td>
		</tr>
    </table>
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false" 
		 DataKeyNames="AccountDetailID,AreaType,Formula,DataType" 
		onrowdatabound="gvList_RowDataBound">
		<Columns>
			<asp:BoundField DataField="MerchantName" HeaderText="商家" />
			<asp:BoundField DataField="CompanyName" HeaderText="配送站" />
			<asp:BoundField DataField="AreaType" HeaderText="区域类型" />
			<asp:BoundField DataField="DeliveryNum" HeaderText="普通发货数" />
			<asp:BoundField DataField="DeliveryVNum" HeaderText="上门换发货数" />
			<asp:BoundField DataField="ReturnsNum" HeaderText="普通拒收数" />
			<asp:BoundField DataField="ReturnsVNum" HeaderText="上门换拒收数" />
			<asp:BoundField DataField="VisitReturnsNum" HeaderText="上门退货数" />
			<asp:BoundField DataField="AccountNum" HeaderText="结算单量" />
			<asp:BoundField DataField="Formula" HeaderText="结算标准" />
			<asp:BoundField DataField="DatumFare" HeaderText="基准运费" />
			<asp:BoundField DataField="Allowance" HeaderText="超区补助" />
			<asp:BoundField DataField="KPI" HeaderText="KPI考核" />
			<asp:BoundField DataField="POSPrice" HeaderText="POS机手续费" />
			<asp:BoundField DataField="StrandedPrice" HeaderText="滞留扣款" />
			<asp:BoundField DataField="IntercityLose" HeaderText="城际丢失调账" />
            <asp:BoundField DataField="CollectionFee" HeaderText="代收手续费" />
            <asp:BoundField DataField="DeliveryFee" HeaderText="投递费" />
			<asp:BoundField DataField="OtherCost" HeaderText="其他费用" />
			<asp:BoundField DataField="Fare" HeaderText="实际结算运费" />
			<asp:BoundField DataField="DataType" HeaderText="DataType" Visible="false" />
			<asp:BoundField DataField="AccountDetailID" HeaderText="AccountDetailID" Visible="false" />
		</Columns>
    </asp:GridView>
    </form>
</body>
</html>
