<%@ Page Title="" Language="C#" MasterPageFile="~/AudiMgmt/FinanceSearch.Master"
    AutoEventWireup="true" CodeBehind="FinancePaymentConfirm.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.FinancePaymentConfirm"
    Theme="default" %>

<%@ MasterType VirtualPath="~/AudiMgmt/FinanceSearch.Master" %>
<%@ Register Src="~/UserControl/SelectStation.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>财务收款确认</title>

    <script type="text/javascript">
        //初始化操作
        function init() {
            var buttons = {
                btnSearch: $.dom("btnSearch", "input"),
                btnSummary: $.dom("btnSummary", "input"),
                btnDetails: $.dom("btnDetails", "input"),
                btnAllDetails: $.dom("btnAllDetails", "input"),
                btnReload: $.dom("btnReload", "input")
            };
            var hiddens = {
                hidTotalCount: $.dom("hidTotalCount", "input"),
                hidDetailsCount: $.dom("hidDetailsCount", "input"),
                hidConfirmAmount: $.dom("hidConfirmAmount", "input")
            };
            var datetimes = {
                beginTime: $.dom("txtBeginTime", "input"),
                endTime: $.dom("txtEndTime", "input"),
                reloadTime: $.dom("txtReloadDate", "input")
            };
            search.init.ready({
                totalButtons: [buttons.btnSummary, buttons.btnAllDetails],
                detailsButtons: [buttons.btnDetails],
                totalCount: hiddens.hidTotalCount.val(),
                detailsCount: hiddens.hidDetailsCount.val(),
                renderTo: "gvSummary",
                clickable: true,
                selected: "tr td:not(:last-child)"
            });
            //对确认金额做验证
            $.dom("gvSummary", ".grid").find("tr td:last-child").find("a").click(function() {
                if (!util.numUtil.checkNumber($(this).prev(), { text: "确认金额", limit: false })) return false;
                hiddens.hidConfirmAmount.val($(this).parent().find("input").filter($.dom("txtConfirmAmount", "input")).val());
                return true;
            });
            //对重新生成做验证
            buttons.btnReload.click(function() {
                if (!search.validate.isSelectStation()) return false;
                if (!util.dateUtil.checkDate(datetimes.reloadTime.val(),
				                             null, {
				                                 isCheckAllEmpty: true,
				                                 emptyText: "请选择生成日期！"
				                             })) {
                    datetimes.reloadTime.focus();
                    return false;
                }
                return true;
            });

        }
        function showWindow() {
            window.showModalDialog("MountModify.aspx", 'dialogHeight=100px;dialogWidth=500px;center=yes;resizable=no;help=no;scroll:no;location=no;status=no');
            return false;
        }
    </script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
财务收款确认
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="station" runat="server">
    <table border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                选择站点：
            </td>
            <td>
                <uc:SelectStation ID="station" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="operate" runat="server">
    <div>
        付款方式：<asp:DropDownList ID="ddlPayType" runat="server">
        </asp:DropDownList>
    </div>
    <div>
        汇总日期：
        <asp:TextBox ID="txtBeginTime" CssClass="WSdate text" runat="server" autocomplete="off"></asp:TextBox>
        <span id="to">至</span>
        <asp:TextBox ID="txtEndTime" runat="server" CssClass="WSdate text" autocomplete="off"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="查  询" OnClick="btnSearch_Click"/>
        <asp:Button ID="btnSummary" runat="server" Text="导出汇总" OnClick="btnSummary_Click"/>
        <asp:Button ID="btnDetails" runat="server" Text="导出明细" OnClick="btnDetails_Click"/>
        <asp:Button ID="btnAllDetails" runat="server" Text="导出所有明细" OnClick="btnAllDetails_Click"/>
        生成日期：
        <asp:TextBox ID="txtReloadDate" CssClass="WSdate text" runat="server" autocomplete="off"></asp:TextBox>
        <asp:Button ID="btnReload" runat="server" Text="重新生成" OnClick="btnReload_Click" />
        <asp:Button ID="btnCheckMount" runat="server" Text="金额校验" OnClientClick="return showWindow();" />
        <asp:Button ID="btnSearchDetails" runat="server" Text="查询明细" OnClick="btnSearchDetails_Click" Style="display: none;" />
    </div>
    <div id="sorttr" style="background: #FFFFFF; border: 1px solid #990000; width: 400px;
        height: 200px; display: none; position: absolute; left: 210px; top: 459px;">
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="summaryContent" runat="server">
    <div class="totalContainer">
    <asp:Literal ID="ltlTotal" runat="server"></asp:Literal>
<%--        <b>现金成功单量：</b><asp:Label ID="lblCashCount" runat="server"></asp:Label><b>POS机成功单量：</b><b>成功单量：</b><b>成功金额：</b><b>退款订单量：</b><b>退款金额：</b><b>存款金额：</b>
--%>    </div>
    <asp:GridView ID="gvSummary" runat="server" AutoGenerateColumns="False" Width="100%"
        AllowSorting="true" OnSorting="gvSummary_Sorting" OnRowCommand="gvSummary_RowCommand">
        <Columns>
            <asp:BoundField HeaderText="序号" DataField="ID" />
            <asp:BoundField HeaderText="配送站" DataField="CompanyName" SortExpression="CompanyName" />
            <asp:BoundField HeaderText="来源" DataField="SourceName" />
            <asp:BoundField HeaderText="现金成功单量" DataField="CashSuccOrderCount" />
            <asp:BoundField HeaderText="POS机成功单量" DataField="PosSuccOrderCount" />
            <asp:BoundField HeaderText="成功金额" DataField="AcceptAmount" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="退款订单量" DataField="DayOutOrderCount" />
            <asp:BoundField HeaderText="退款金额" DataField="CashRealOutSum" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="存款金额" DataField="SaveAmount" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="财务收款状态" DataField="FinanceStatus" />
            <asp:TemplateField HeaderText="汇总日期">
                <ItemTemplate>
                    <asp:Label ID="lblCreateTime" runat="server" Text='<%# Bind("DailyTime", "{0:D}") %>'></asp:Label>
                    <asp:HiddenField ID="hidQueryParams" runat="server" Value='<%# Bind("QueryParams") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="确认金额">
                <ItemTemplate>
                    <asp:Label ID="lblConfirm" runat="server" Text='<%# Bind("RealInCome","{0:#,##0.##}")%>'
                        Visible='<%# Eval("FinanceStatus").ToString()!="未收款"%>'></asp:Label>
                    <asp:TextBox ID="txtConfirmAmount" runat="server" CssClass="text" Width="40px" Visible='<%# Eval("FinanceStatus").ToString()=="未收款"%>'></asp:TextBox>
                    <asp:LinkButton ID="lbtnConfirm" runat="server" Visible='<%# Eval("FinanceStatus").ToString()=="未收款"%>'
                        CommandArgument='<%# Eval("TotalDataID").ToString() %>' CommandName="confirm">确认</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <uc:Pager ID="totalPager" runat="server" />
    <asp:HiddenField ID="hidTotalCount" runat="server" Value="0" />
    <asp:HiddenField ID="hidDetailsParams" runat="server" Value="" />
    <asp:HiddenField ID="hidClickOne" runat="server" />
    <asp:HiddenField ID="hidConfirmAmount" runat="server" Value="" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="detailsContent" runat="server">
    <div class="detailsContainer">
        <asp:Literal ID="ltlDetails" runat="server"></asp:Literal>
    </div>
    <asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="False" Width="100%">
        <Columns>
            <asp:BoundField HeaderText="序号" DataField="ID" />
            <asp:BoundField HeaderText="配送站" DataField="CompanyName" />
            <asp:BoundField HeaderText="来源" DataField="SourceName" />
            <asp:BoundField HeaderText="运单号" DataField="WaybillNO" />
            <asp:BoundField HeaderText="应收金额" DataField="NeedPrice" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="应退金额" DataField="NeedReturnPrice" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="支付方式" DataField="AcceptType" />
            <asp:BoundField HeaderText="POS终端号" DataField="PosNum" />
            <asp:BoundField HeaderText="入站时间" DataField="EnterTime" DataFormatString="{0:D}" />
            <asp:BoundField HeaderText="提交时间" DataField="PostTime" DataFormatString="{0:D}" />
        </Columns>
    </asp:GridView>
    <uc:Pager ID="detailsPager" runat="server" />
    <asp:HiddenField ID="hidDetailsCount" runat="server" Value="0" />
</asp:Content>
