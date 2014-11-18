<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRuleFee.ascx.cs" Inherits="RFD.FMS.WEB.test.workflow.UCRuleFee" %>
<div align="center">
    <table>
        <tr>
            <td>如果金额</td>
            <td>
                <asp:DropDownList ID="cmbOperatorFee" Width="60px" runat="server">
                    <asp:ListItem Text="大于" Value=">" />
                    <asp:ListItem Text="小于" Value="<" />
                    <asp:ListItem Text="等于" Value="=" />
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="txtFee" Width="60px" runat="server" />
            </td>
            <td>就转到</td>
            <td>
                <asp:DropDownList ID="cmbSelectNode" Width="100px" runat="server" />
            </td>
            <td>
                <asp:Button ID="btnAdd" Width="60px" Text="添加" runat="server" 
                    onclick="btnAdd_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <asp:TextBox ID="txtResultFee" Width="100%" Height="200px" TextMode="MultiLine" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="6" align="right">
                <asp:Button ID="btnSave" Text="保存" runat="server" onclick="btnSave_Click" />
            </td>
        </tr>
    </table>
</div>
