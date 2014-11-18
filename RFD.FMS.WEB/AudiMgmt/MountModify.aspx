<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="MountModify.aspx.cs"
    Inherits="RFD.FMS.WEB.AudiMgmt.MountModify" Theme="default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>

    <script src="../Scripts/import/common.js" type="text/javascript"></script>

    <script type="text/javascript">
        window.name = 'aa';
        function CVerification() {
            var result = true;
            var WaybillNO = $.trim($("#WaybillNO").val());
            if (WaybillNO == "") {
                $("#tishi_mail").html("<p>运单号不能为空！</p>");
                result = false;
            }
            if (result == true) {
                var Mount = $.trim($("#Mount").val());
                if (Mount == "") {
                    $("#tishi_mail").html("<p>金额不能为空！</p>");
                    result = false
                }
                else {
                    if (isNaN(Mount)) {
                        $("#tishi_mail").html("<p>金额输入格式错误！</p>");
                        result = false
                    }
                }
            }

            return result;
        };
    </script>

    <title>订单金额调整</title>
</head>
<body>
    <form id="form1" runat="server" target="aa">
    <div>
        &nbsp; &nbsp; &nbsp;
        <table>
            <tr>
                <td>
                    运单号：
                </td>
                <td>
                    <asp:TextBox ID="WaybillNO" runat="server"></asp:TextBox>
                </td>
                <td>
                    运单类型：
                </td>
                <td>
                    <asp:DropDownList ID="ddlOrderType" runat="server">
                        <asp:ListItem Value="1">普通订单</asp:ListItem>
                        <asp:ListItem Value="2">上门换货单</asp:ListItem>
                        <asp:ListItem Value="3">上门退货单</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    金额：
                </td>
                <td colspan="2">
                    <asp:TextBox ID="Mount" runat="server"></asp:TextBox>
                </td>
                <td  align="right">
                    <asp:Button ID="btnGetMount" runat="server" Text="得到金额" 
                        OnClick="btnGetMount_Click" Visible="False" />
                </td>
            </tr>
            <tr>
                <td>
                    校验金额类型：
                </td>
                <td colspan="2">
                    <asp:DropDownList ID="dplType" runat="server">
                        <asp:ListItem Value="0">应收</asp:ListItem>
                        <asp:ListItem Value="1">应退</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td align="right">
                    <asp:Button ID="btnOK" runat="server" Text="校正金额" OnClick="btnOK_Click" OnClientClick="return CVerification();" />
                </td>
            </tr>
            
             <tr>
                <td>
                    订单号列表：
                </td>
                <td colspan="2">
                    <asp:TextBox ID="txtWaybillNOs" runat="server" Height="97px" 
                        TextMode="MultiLine" Width="249px"></asp:TextBox>
                </td>
                <td colspan="2" align="right">
                    <asp:Button ID="btnCreateReport" runat="server" Text="重新生成报表" 
                        onclick="btnCreateReport_Click"/>
                </td>
            </tr>
        </table>
    </div>
    <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    <div id="tishi_mail">
    </form>
</body>
</html>
