<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatusCodeEdit.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.StatusCodeEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>状态编辑</title>
    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
    <script type="text/javascript">
    	window.name = "winStatusCodeEdit";
    </script>
</head>
<body>
    <form id="form1" runat="server" target="winStatusCodeEdit">
    <div style="margin:10px; text-align:center;">
    <asp:Label ID="TitleShow" runat="server"></asp:Label>
    <table>
        <tr>
            <td align="right">名称</td>
            <td><asp:TextBox ID="txtCodeDesc" runat="server" Width="100px" MaxLength="10"></asp:TextBox></td>
        </tr>
        <tr>
            <td align="right">是否启用</td>
            <td>
                <asp:RadioButton ID="rbTrue" runat="server" GroupName="radioUser" Checked="true" Text="启用" />
                <asp:RadioButton ID="rbFalse" runat="server" GroupName="radioUser" Text="停用" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="right">
                <asp:Button ID="btOK" runat="server" Text="确定" onclick="btOK_Click" />
            </td>
        </tr>
    </table>
    </div>
    </form>
</body>
</html>
