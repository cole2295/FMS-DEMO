<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryPriceLog.aspx.cs" Inherits="RFD.FMS.WEB.COD.DeliveryPriceLog" Theme="default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>配送价格操作日志</title>
    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>

	<script src="../Scripts/import/common.js" type="text/javascript"></script>
	<script type="text/javascript">
		window.name = "DeliveryPriceLog";
		$(function() {
			$(".Wdate").focus(function() { WdatePicker({ skin: 'whyGreen', dateFmt: 'yyyy-MM-dd HH:mm:ss' }); });
		})
	</script>
</head>
<body>
    <form id="form1" runat="server" target="DeliveryPriceLog">
    <table style="margin:10px;">
		<tr>
			<td>线路编号</td>
			<td><asp:TextBox ID="txtLineNO" runat="server"></asp:TextBox></td>
			<td>操作时间</td>
			<td><asp:TextBox ID="txtBeginTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox></td>
			<td>-</td>
			<td><asp:TextBox ID="txtEndTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox></td>
			<td><asp:Button ID="btSearch" runat="server" Text="查询" onclick="btSearch_Click" /></td>
		</tr>
    </table>
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false">
		<Columns>
			<asp:BoundField DataField="LogID" HeaderText="日志ID" />
			<asp:BoundField DataField="PK_NO" HeaderText="线路编号" />
			<asp:BoundField DataField="CreateBy" HeaderText="操作人" />
			<asp:BoundField DataField="CreateTime" HeaderText="操作时间" />
			<asp:BoundField DataField="LogText" HeaderText="操作详情" />
		</Columns>
    </asp:GridView>
    </form>
</body>
</html>
