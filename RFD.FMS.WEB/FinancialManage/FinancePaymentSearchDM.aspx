<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    CodeBehind="FinancePaymentSearchDM.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.FinancePaymentSearchDM"
    Theme="default" %>
<%@ Register Src="~/UserControl/MerchantSource.ascx" TagName="UCMerchantSource" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/SelectStation.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>财务收款查询(新)</title>
    <script src="../Scripts/finance/finance.search.new.js" type="text/javascript"></script>
    <script src="../Scripts/finance/finance.search.page.new.js" type="text/javascript"></script>
    <script type="text/javascript">
        //初始化操作
        function init() {
            var buttons = {
                btnSearch: $.dom("btnSearch", "input"),
                btnSummary: $.dom("btnSummary", "input"),
                btnDetails: $.dom("btnDetails", "input"),
                btnAllDetails: $.dom("btnAllDetails", "input")
            };
            var hiddens = {
                hidTotalCount: $.dom("hidTotalCount", "input"),
                hidDetailsCount: $.dom("hidDetailsCount", "input")
            };
            search.init.ready({
                totalButtons: [buttons.btnSummary, buttons.btnAllDetails],
                detailsButtons: [buttons.btnDetails],
                totalCount: hiddens.hidTotalCount.val(),
                detailsCount: hiddens.hidDetailsCount.val(),
                renderTo: "gvSummary",
                clickable: true
            });
        }

    	
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                订单来源：
            </td>
            <td>
                <asp:DropDownList ID="ddlOrderSource" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlOrderSource_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td>
                商家来源：
            </td>
            <td>
                <uc1:UCMerchantSource ID="UCMerchantSource" runat="server" LoadDataType="ThirdMerchant" SelectEnable="false" />
            </td>
            <td>
                选择站点：
            </td>
            <td>
                <uc:SelectStation ID="station" runat="server" />
            </td>
            <td>
                导出格式：<asp:RadioButtonList ID="rblExportFormat" runat="server" RepeatDirection="Horizontal"
                    RepeatLayout="Flow">
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                付款方式：
            </td>
            <td>
                <asp:DropDownList ID="ddlPayType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPayType_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td>
                POS机类型：
            </td>
            <td>
                <asp:DropDownList ID="ddlPOSType" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                汇总日期：
            </td>
            <td>
                <asp:TextBox ID="txtBeginTime" CssClass="WSdate text" runat="server" autocomplete="off"></asp:TextBox>至<asp:TextBox
                    ID="txtEndTime" runat="server" CssClass="WSdate text" autocomplete="off"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="查  询" OnClick="btnSearch_Click" />
                <asp:Button ID="btnSummary" runat="server" Text="导出汇总" OnClick="btnSummary_Click" />
                <asp:Button ID="btnDetails" runat="server" Text="导出明细" OnClick="btnDetails_Click" />
                <asp:Button ID="btnAllDetails" runat="server" Text="导出所有明细" OnClick="btnAllDetails_Click" />
                <asp:Button ID="btnSearchDetails" runat="server" Text="查询明细" OnClick="btnSearchDetails_Click"
                    Style="display: none;" />
            </td>
        </tr>
    </table>
    <div>
        <div class="totalContainer">
            <asp:Literal ID="ltlTotal" runat="server"></asp:Literal>
        </div>
        <asp:GridView ID="gvSummary" runat="server" AutoGenerateColumns="False" Width="100%"
            AllowSorting="true" OnSorting="gvSummary_Sorting">
            <Columns>
                <asp:BoundField HeaderText="序号" DataField="ID" />
                <asp:BoundField HeaderText="配送公司" DataField="DistributionName" />
                <asp:BoundField HeaderText="配送站" DataField="CompanyName" SortExpression="CompanyName" />
                <asp:BoundField HeaderText="商家编码" DataField="MerchantCode" />
                <asp:BoundField HeaderText="来源" DataField="SourceName" />
                <asp:BoundField HeaderText="现金成功单量" DataField="CashWaybillCount" />
                <asp:BoundField HeaderText="POS机成功单量" DataField="POSWaybillCount" />
                <asp:BoundField HeaderText="成功金额" DataField="AcceptAmount" DataFormatString="{0:#,##0.##}" />
                <asp:BoundField HeaderText="退款订单量" DataField="BackWaybillCount" />
                <asp:BoundField HeaderText="退款金额" DataField="CashRealOutSum" DataFormatString="{0:#,##0.##}" />
                <asp:BoundField HeaderText="存款金额" DataField="SaveAmount" DataFormatString="{0:#,##0.##}" />
                <asp:TemplateField HeaderText="汇总日期">
                    <ItemTemplate>
                        <asp:Label ID="lblCreateTime" runat="server" Text='<%# Bind("CreateTime", "{0:D}") %>'></asp:Label>
                        <asp:HiddenField ID="hidQueryParams" runat="server" Value='<%# Bind("QueryParams") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <uc:Pager ID="totalPager" runat="server" />
        <asp:HiddenField ID="hidTotalCount" runat="server" Value="0" />
        <asp:HiddenField ID="hidDetailsParams" runat="server" Value="" />
        <asp:HiddenField ID="hidClickOne" runat="server" />
    </div>
    <div>
        <div class="detailsContainer">
            <asp:Literal ID="ltlDetails" runat="server"></asp:Literal>
        </div>
        <asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="False" Width="100%">
            <Columns>
                <asp:BoundField HeaderText="序号" DataField="ID" />
                <asp:BoundField HeaderText="配送站" DataField="CompanyName" />
                <asp:BoundField HeaderText="运单号" DataField="WaybillNO" />
                <asp:BoundField HeaderText="订单号" DataField="CustomerOrder" />
                <asp:BoundField HeaderText="应收金额" DataField="NeedAmount" DataFormatString="{0:#,##0.#}" />
                <asp:BoundField HeaderText="应退金额" DataField="NeedBackAmount" DataFormatString="{0:#,##0.#}" />
                <asp:BoundField HeaderText="支付方式" DataField="AcceptType" />
                <asp:BoundField HeaderText="财务收款状态" DataField="FinancialStatus" />
                <asp:BoundField HeaderText="POS终端号" DataField="POSCode" />
                <asp:BoundField HeaderText="POS机类型" DataField="STATUSNAME" />
                <asp:BoundField HeaderText="入站时间" DataField="IntoTime" DataFormatString="{0:D}" />
                <asp:BoundField HeaderText="提交时间" DataField="CreateTime" DataFormatString="{0:D}" />
            </Columns>
        </asp:GridView>
        <uc:Pager ID="detailsPager" runat="server" />
        <asp:HiddenField ID="hidDetailsCount" runat="server" Value="0" />
    </div>
</asp:Content>
