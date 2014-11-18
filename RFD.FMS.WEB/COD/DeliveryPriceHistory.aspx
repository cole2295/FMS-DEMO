<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryPriceHistory.aspx.cs" Inherits="RFD.FMS.WEB.COD.DeliveryPriceHistory" Theme="default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>配送价格历史查询</title>
    <script type="text/javascript" src="../Scripts/base.js"></script>
    <script type="text/javascript" src="../Scripts/import/common.js"></script>
    <script src="../Scripts/jquery-gridhover.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin:10px; overflow-f:auto;">
    <asp:GridView ID="gvList" runat="server">
    </asp:GridView>
    </div>

    </form>
</body>
</html>
