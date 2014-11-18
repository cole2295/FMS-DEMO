<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IncomeAreaExpressLevelViewLog.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.AreaExpressLevelIncomeViewLog" Theme="default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title>收入区域类型操作日志</title>
    <script src="../Scripts/base.js" type="text/javascript"></script>

	<script src="../Scripts/import/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false">
		<Columns>
			<asp:BoundField HeaderText="日志编号" DataField="LogID" />
			<asp:BoundField HeaderText="省" DataField="ProvinceName" />
			<asp:BoundField HeaderText="市" DataField="CityName" />
			<asp:BoundField HeaderText="区" DataField="AreaName" />
			<asp:BoundField HeaderText="配送公司" DataField="CompanyName" />
			<asp:BoundField HeaderText="分拣中心" DataField="SortCenterName" />
			<asp:BoundField HeaderText="商家" DataField="MerchantName" />
			<asp:BoundField HeaderText="操作人" DataField="EmployeeName" />
			<asp:BoundField HeaderText="操作时间" DataField="CreateTime" />
			<asp:BoundField HeaderText="操作内容" DataField="LogText" />
		</Columns>
    </asp:GridView>
    </form>
</body>
</html>
