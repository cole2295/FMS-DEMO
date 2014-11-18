<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OperrateLogViewByNo.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.OperrateLogViewByNo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>操作日志查询</title>
    <script src="../Scripts/base.js" type="text/javascript"></script>

	<script src="../Scripts/import/common.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField HeaderText="日志编号" DataField="LogID" />
            <asp:BoundField HeaderText="查询编号" DataField="PK_NO" />
            <asp:BoundField HeaderText="操作人" DataField="EmployeeName" />
            <asp:BoundField HeaderText="创建时间" DataField="CreateTime" />
            <asp:BoundField HeaderText="操作信息" DataField="LogText" />
        </Columns>
    </asp:GridView>
    </form>
</body>
</html>
