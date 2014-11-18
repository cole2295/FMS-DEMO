<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IncomeAreaExpressLevelAudit.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.AreaExpressLevelIncomeAudit" MasterPageFile="~/Main/main.Master" Theme="default" %>

<%@ Import Namespace="System.Data" %>
<%@ Register Src="../UserControl/PCASerach.ascx" TagName="PCASerach" TagPrefix="uc1" %>
<%@ Register Src="../UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc3" %>
<%@ Register src="../UserControl/SelectStationCommon.ascx" tagname="SelectStationCommon" tagprefix="uc2" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        $(function () {
            $("#checkall").click(function () {
                var all = this;
                var countBox = 0;
                $("#<%=gvAreaExpressIncomeLevel.ClientID %> input:checkbox").each(function () {
                    if (this.id == "checkall") {
                        this.checked = all.checked;
                        countBox++;
                    }
                });
            });
        });

    </script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    商家区域类型审批
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
    <table>
        <tr>
            <td>
                省市区：
            </td>
            <td colspan="2">
                <uc1:PCASerach ID="PCASerach" runat="server" />
            </td>
            <td>
                                商家：
            </td>
            <td>
                <asp:DropDownList ID="drpMerchant" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                区域类型：
            </td>
            <td>
                <asp:DropDownList ID="drpAreaType" runat="server">
                </asp:DropDownList>
            </td>
            <td>配送商：</td>
            <td>
                 <uc2:SelectStationCommon ID="SelectStationCommon" runat="server" LoadDataType="OnlySite"  />
            </td>
        </tr>
        <tr>
            <td>
                审批状态：
            </td>
            <td>
                <asp:RadioButtonList ID="drpStatus" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="未审批" Value="0" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="已审批" Value="1"></asp:ListItem>
                    <asp:ListItem Text="已生效" Value="2"></asp:ListItem>
                    <asp:ListItem Text="已置回" Value="3"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:Button ID="BtnSearch" runat="server" Text="查询" OnClick="BtnSearch_Click" />
            </td>
            <td colspan="7">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="BtnCancel" runat="server" Text="置回" onclick="BtnCancel_Click" />
            </td>
        </tr>
    </table>
    <table style="width: 1200px;">
        <tr>
            <td style="text-align: left; vertical-align: top; width: 60%">
                <asp:GridView ID="gvAreaExpressIncomeLevel" runat="server" AutoGenerateColumns="False"
                    OnRowDataBound="gvAreaExpressIncomeLevel_RowDataBound" OnSelectedIndexChanged="gvAreaExpressIncomeLevel_SelectedIndexChanged">
                    <Columns>
                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                            <HeaderTemplate>
                                <input type="checkbox" id="checkall" name="checkall" /><span style="color: Blue">全选</span>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div style="text-align: center;">
                                    <input name="cbChecked" type="checkbox" value="<%# ((DataRowView)Container.DataItem)["areaid"] %>" /></div>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                        </asp:TemplateField>
                        <asp:BoundField DataField="provincename" HeaderText="省份" />
                        <asp:BoundField DataField="cityname" HeaderText="城市" />
                        <asp:BoundField DataField="areaname" HeaderText="地区" />
                        <asp:BoundField DataField="employeename" HeaderText="审批人" />
                        <asp:BoundField DataField="AuditTime" HeaderText="审批时间" />
                        <asp:BoundField DataField="statusName" HeaderText="状态" />
                        <asp:BoundField DataField="areaid" />
                        <asp:BoundField DataField="auditstatus" HeaderText="auditstatus" />
                         <asp:BoundField DataField="dodate" HeaderText="生效时间" />
                        <asp:CommandField ShowSelectButton="True" SelectText="查看详细信息" />
                    </Columns>
                </asp:GridView>
                <uc3:Pager ID="Pager" runat="server" />
            </td>
            <td style="text-align: left; vertical-align: top; width: 40%">
                <asp:GridView ID="gvAreaExpressIncomeLevelDetail" runat="server" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="areaname" HeaderText="地区" />
                        <asp:BoundField DataField="merchantName" HeaderText="商家" />
                        <asp:BoundField DataField="WareHouseName" HeaderText="分拣中心" />
                        <asp:BoundField DataField="EffectAreaType" HeaderText="设置区域类型" />
                        <asp:BoundField DataField="companyname" HeaderText="配送公司" />
                        <asp:BoundField DataField="AreaType" HeaderText="原区域类型" />
                        <asp:BoundField DataField="EnableStr" HeaderText="生效状态" />
                        <asp:BoundField DataField="updatetime" HeaderText="修改时间" />
                        <asp:BoundField DataField="employeename" HeaderText="操作人" />
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                生效时间：<asp:TextBox ID="txtDoDate" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d 00:00:00',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"></asp:TextBox>
                <asp:Button ID="BtnAudit" runat="server" Text="审批" OnClick="BtnAudit_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
