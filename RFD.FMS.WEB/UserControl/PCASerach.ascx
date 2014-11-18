<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PCASerach.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.PCASerach" %>

<table  border="0" cellpadding="0" cellspacing="0" style="display:inline">
    <tr>
        <td>
            <asp:DropDownList ID="DrpProvince" AutoPostBack="true" runat="server" OnSelectedIndexChanged="DrpProvince_SelectedIndexChanged">
                <asp:ListItem Value="01">北京</asp:ListItem>
            </asp:DropDownList>
        </td>
        <td>
            <asp:UpdatePanel ID="UdpCity" runat="server">
                <ContentTemplate>
                    <asp:DropDownList ID="DrpCity" AutoPostBack="true" runat="server" OnSelectedIndexChanged="DrpCity_SelectedIndexChanged">
                    </asp:DropDownList>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DrpProvince" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
        <td>
            <asp:UpdatePanel ID="UdpArea" runat="server">
                <ContentTemplate>
                    <asp:DropDownList ID="DrpArea" runat="server">
                    </asp:DropDownList>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DrpCity" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </td>
    </tr>
</table>