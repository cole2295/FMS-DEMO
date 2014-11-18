<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountFeeEdit.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.AccountFeeEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<base target="_self" />
    <title>收入结算费用修改</title>
     <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <table style="margin:20px;">
    <tr>
			<td>代收货款手续费：</td>
			<td><asp:TextBox ID="txtReceiveFee" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
		<tr>
			<td>POS机服务费：</td>
			<td><asp:TextBox ID="txtReceivePOSFee" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
		<tr>
			<td>保价费：</td>
			<td><asp:TextBox ID="txtProtectedFee" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
		<tr>
			<td>超区补助：</td>
			<td><asp:TextBox ID="txtOverAreaSubsidy" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
		<tr>
			<td>KPI考核：</td>
			<td><asp:TextBox ID="txtKPI" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
		<tr>
			<td>丢失扣款：</td>
			<td><asp:TextBox ID="txtLostDeduction" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
		<tr>
			<td>滞留扣款：</td>
			<td><asp:TextBox ID="txtResortDeduction" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
        <tr>
			<td>提货费：</td>
			<td><asp:TextBox ID="txtDeliveryFee" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
        <tr>
			<td>折扣：</td>
			<td><asp:TextBox ID="txtDiscountFee" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
		<tr>
			<td>其他费用：</td>
			<td><asp:TextBox ID="txtOtherFee" runat="server" Width="100px"></asp:TextBox></td>
		</tr>
		
		<tr>
			<td colspan="2" style="padding-left:50px;">
				<asp:Button ID="btOK" runat="server" Text="确定" onclick="btOK_Click" />
			</td>
		</tr>
    </table>
    </form>
</body>
</html>
