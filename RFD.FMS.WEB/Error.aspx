<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="RFD.FMS.WEB.Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%">
            <tr>
                <td align="center">
                    <img src="images/error2.jpg" alt="抱歉！系统异常，请重试，或联系信息部（rfd-xxxtb.list@vancl.cn）处理……" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    抱歉！系统异常，请重试，或联系信息部（rfd-xxxtb.list@vancl.cn）处理……
                </td>
            </tr>
            <tr>
                <td align="center">
                    <asp:Label ID="LB_ErrorInfo" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
