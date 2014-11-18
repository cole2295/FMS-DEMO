<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    CodeBehind="ExpressPriceManage.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.ExpressPriceManage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table cellpadding="0" cellspacing="0">
        <tr>
            <td>
                <asp:Label ID="lblNO" runat="server">城市编号：</asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtCityID" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="lblName" runat="server">城市名称：</asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtCityName" runat="server"></asp:TextBox>
            </td>
            <td>
            <asp:Button ID="btnQuery" runat="server" Text="查询" onclick="btnQuery_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <div style="overflow: auto; width: 500px; height: 500px;">
                    <asp:GridView ID="gvData" runat="server" AutoGenerateColumns="false" AllowPaging="false"
                        Width="100%" EnableViewState="true" onrowediting="gvData_RowEditing">
                        <Columns>
                            <asp:BoundField DataField="CityID" HeaderText="城市ID" />
                            <asp:BoundField DataField="cityname" HeaderText="城市" />
                            <asp:BoundField DataField="AreaName" HeaderText="区域" />
                            <asp:BoundField DataField="TransferFee" HeaderText="运费" />
                            <asp:CommandField EditText="修改" HeaderText="修改" ShowEditButton="true" />
                        </Columns>
                        <EmptyDataTemplate>
                            <label id="NOData">
                                没有数据</label>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
