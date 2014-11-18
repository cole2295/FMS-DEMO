<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRulePeople.ascx.cs" Inherits="RFD.FMS.WEB.test.workflow.UCRulePeople" %>
<div align="center">
    <table>
        <tr>
            <td>审核人</td>
            <td>
                <asp:DropDownList ID="cmbSelectUser" Width="100px" runat="server" />
            </td>
            <td>
                <asp:DropDownList ID="cmbOperatorPeople" Width="50px" runat="server">
                    <asp:ListItem Text="或者" Value="or" />
                    <asp:ListItem Text="并且" Value="and" />
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btnAdd" Text="添加" Width="60px" runat="server" 
                    onclick="btnAdd_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:TextBox ID="txtResultPeople" Width="100%" Height="200px" TextMode="MultiLine" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div align="left">
                    审批后才能进行下一步审批
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4" align="right">
                <asp:Button ID="btnSave" Text="保存" runat="server" onclick="btnSave_Click" />
            </td>
        </tr>
    </table>
</div>
