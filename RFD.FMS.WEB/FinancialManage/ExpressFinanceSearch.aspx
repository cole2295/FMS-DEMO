<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    Theme="default" EnableEventValidation="false" CodeBehind="ExpressFinanceSearch.aspx.cs"
    Inherits="RFD.FMS.WEB.FinancialManage.ExpressFinanceSearch" %>

<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(function() {
            render();
        })
    </script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
快递收款查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="width: 100%">
        <table cellpadding="3" cellspacing="5">
            <tr>
                <td align="right">
                    选择站点：
                </td>
                <td>
                    <uc:SelectStation ID="UCSelectStation" runat="server" LoadDataType="OnlySite" />
                </td>
                <td>
                    配送费结算方式：
                </td>
                <td>
                    <asp:DropDownList ID="ddlTransferPayType" runat="server">
                        <asp:ListItem Text="---请选择---" Value="0"></asp:ListItem>
                        <asp:ListItem Text="现结" Value="1"></asp:ListItem>
                        <asp:ListItem Text="到付" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlPayType" runat="server" Visible="false">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right">
                    选择商家：
                </td>
                <td>
                    <uc1:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" MerchantLoadType="ThirdMerchant" MerchantTypeSource="cw_Merchant_Inside" MerchantShowCheckBox="True" />
                </td>
                <td>
                    汇总日期：
                </td>
                <td colspan="3">
                    <asp:TextBox ID="txtBeginTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>至<asp:TextBox
                        ID="txtEndTime" runat="server" CssClass="Wdate text" autocomplete="off"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="5" align="right">
                    <asp:Button ID="btnSearch" runat="server" Text="查  询" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnSummary" runat="server" Text="导出汇总" OnClick="btnSummary_Click" />
                    <asp:Button ID="btnDetails" runat="server" Text="导出明细" OnClick="btnDetails_Click" />
                    <asp:Button ID="btnAddDetails" runat="server" Text="导出所有明细" OnClick="btnAddDetails_Click" />
                    <asp:Button ID="btnSearchDetails" runat="server" Text="查询明细" OnClick="btnSearchDetails_Click"
                        Style="display: none;" />
                </td>
            </tr>
            <tr>
                <td colspan="5" align="right">
                    <asp:Button ID="btnSearchV2" runat="server" Text="查  询V2" 
                        onclick="btnSearchV2_Click"/>
                    <asp:Button ID="btnSummaryV2" runat="server" Text="导出汇总V2" 
                        onclick="btnSummaryV2_Click"/>
                    <asp:Button ID="btnDetailsV2" runat="server" Text="导出明细V2" 
                        onclick="btnDetailsV2_Click"/>
                    <asp:Button ID="btnAddDetailsV2" runat="server" Text="导出所有明细V2" 
                        onclick="btnAddDetailsV2_Click"/>
                    <asp:Button ID="btnSearchDetailsV2" runat="server" Text="查询明细V2" Style="display: none;" />
                </td>
            </tr>
        </table>
    </div>
    <div>
        <asp:Panel ID="pSumData" runat="server" GroupingText="汇总信息">
            <table width="100%" cellpadding="0" cellspacing="5">
                <tr>
                    <td style=" display:none;">
                        <b>现金单量：</b><asp:Label ID="lblCashCount" runat="server"></asp:Label>
                    </td>
                    <td style=" display:none;">
                        <b>POS单量：</b><asp:Label ID="lblPOSCount" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>总单量：</b><asp:Label ID="lblSumCount" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>总配送费：</b><asp:Label ID="lblTransferSum" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>保价费订单量：</b><asp:Label ID="lblProtectedPriceCount" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>总保价费：</b><asp:Label ID="lblProtectedPriceSum" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>总存款金额：</b><asp:Label ID="lblSumPrice" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <div style="overflow-y: auto; overflow-x: hidden; height: 230px; width: 100%">
        <asp:GridView ID="gvSummary" runat="server" Width="100%" AutoGenerateColumns="false"
            AllowPaging="false" EnableViewState="true" OnSelectedIndexChanged="gvSummary_SelectedIndexChanged"
            OnSelectedIndexChanging="gvSummary_SelectedIndexChanging" OnRowDataBound="gvSummary_RowDataBound">
            <HeaderStyle Wrap="false" />
            <RowStyle Wrap="false" />
            <Columns>
                <asp:BoundField HeaderText="序号" DataField="ID" />
                <asp:BoundField HeaderText="配送公司" DataField="DistributionName" />
                <asp:BoundField HeaderText="配送站" DataField="CompanyName" />
                <asp:BoundField HeaderText="来源" DataField="SourceName" />
                <asp:BoundField HeaderText="成功单量" DataField="CashWaybillCount" />
                <asp:BoundField HeaderText="POS机成功单量" DataField="POSWaybillCount" Visible="false"/>
                <asp:BoundField HeaderText="配送费" DataField="TransferFeeSum" DataFormatString="{0:#,##0.##}" />
                <asp:BoundField HeaderText="保价费" DataField="ProtectedPriceSum" />
                <asp:BoundField HeaderText="存款金额" DataField="SaveAmount" DataFormatString="{0:#,##0.##}" />
                <asp:TemplateField HeaderText="汇总日期">
                    <ItemTemplate>
                        <asp:Label ID="lblCreateTime" runat="server" Text='<%# Bind("CreateTime", "{0:D}") %>'></asp:Label>
                        <asp:HiddenField ID="hidQueryParams" runat="server" Value='<%# Bind("ExpressCompanyID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <label id="NOData">
                    没有数据</label>
            </EmptyDataTemplate>
        </asp:GridView>
        <%--<uc2:Pager ID="totalPager" runat="server" />
--%>
        <asp:HiddenField ID="hidTotalCount" runat="server" Value="0" />
        <asp:HiddenField ID="hidDetailsParams" runat="server" Value="" />
        <asp:HiddenField ID="hidClickOne" runat="server" />
    </div>
    <br />
    <div style="overflow-y: auto; overflow-x: hidden; height: 230px; width: 100%">
        <asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="False" Width="100%">
            <Columns>
                <asp:BoundField HeaderText="序号" DataField="序号" />
                <asp:BoundField HeaderText="配送站" DataField="配送站" />
                <asp:BoundField HeaderText="运单号" DataField="运单号" />
                <asp:BoundField HeaderText="订单号" DataField="订单号" />
                <asp:BoundField HeaderText="商家" DataField="商家" />
                <asp:BoundField HeaderText="配送费" DataField="配送费" DataFormatString="{0:#,##0.##}" />
                <asp:BoundField HeaderText="保价费" DataField="保价费" />
                <asp:BoundField HeaderText="支付方式" DataField="支付方式" />
                <asp:BoundField HeaderText="POS终端号" DataField="POS终端号" />
                <asp:BoundField HeaderText="统计时间" DataField="统计时间" DataFormatString="{0:D}" />
                <asp:BoundField HeaderText="提交时间" DataField="提交时间" DataFormatString="{0:D}" />
            </Columns>
        </asp:GridView>
        <%--<uc2:Pager ID="detailsPager" runat="server" />
	<asp:HiddenField ID="hidDetailsCount" runat="server" Value="0" />--%>
    </div>
</asp:Content>
