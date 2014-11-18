<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="RFD.FMS.WEB.Main.Welcome" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/StylesNew/main.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
    <script src="../Scripts/public.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <table style="width: 96%; margin: 20px">
        <tr>
            <td align="left">
            </td>
            <td align="right" style="padding-right: 10px">
                <asp:Button ID="btnChangePass" Text="修改密码" Width="60px" runat="server" OnClick="btnChangePass_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2" class="notice">
                <%=noticeHtml %>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
