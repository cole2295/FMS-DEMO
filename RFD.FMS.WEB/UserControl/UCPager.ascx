<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCPager.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.UCPager" %>

<div>
    <table>
        <tr>
            <td>
                <asp:Button ID="btnFrist" Text="首页" runat="server" onclick="btnFrist_Click"/>
            </td>
            <td>
                <asp:Button ID="btnPre" Text="上一页" runat="server" onclick="btnPre_Click"/>
            </td>
            <td>
                <asp:Button ID="btnNext" Text="下一页" runat="server" onclick="btnNext_Click"/>
            </td>
            <td>
                <asp:Button ID="btnLast" Text="尾页" runat="server" onclick="btnLast_Click"/>
            </td>
        </tr>
    </table>
</div>

