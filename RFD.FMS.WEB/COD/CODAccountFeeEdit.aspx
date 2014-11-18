<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CODAccountFeeEdit.aspx.cs"
    Inherits="RFD.FMS.WEB.COD.CODAccountFeeEdit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title>COD相关费用编辑</title>
    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
    <script type="text/javascript">
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="line-height: 30px; margin: 30px;">
        <table style="line-height: 30px;">
            <tr>
                <td>
                    超区补助：
                </td>
                <td>
                    <asp:TextBox ID="tbAllowance" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    KPI考核：
                </td>
                <td>
                    <asp:TextBox ID="tbKPI" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    POS机手续费：
                </td>
                <td>
                    <asp:TextBox ID="tbPOSPrice" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    滞留扣款：
                </td>
                <td>
                    <asp:TextBox ID="tbStrandedPrice" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    城际丢失调账：
                </td>
                <td>
                    <asp:TextBox ID="tbIntercityLose" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    代收手续费：
                </td>
                <td>
                    <asp:TextBox ID="tbCollectionFee" runat="server"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td>
                    投递费：
                </td>
                <td>
                    <asp:TextBox ID="tbDeliveryFee" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    其他费用：
                </td>
                <td>
                    <asp:TextBox ID="tbOtherCost" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
        <div style="text-align: right; padding: 30px;">
            <asp:Button ID="btOK" runat="server" Text="确定" OnClick="btOK_Click" />
        </div>
    </div>
    </form>
</body>
</html>
