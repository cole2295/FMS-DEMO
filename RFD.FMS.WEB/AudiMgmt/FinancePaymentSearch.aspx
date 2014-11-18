<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    CodeBehind="FinancePaymentSearch.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.FinancePaymentSearch"
    Theme="default" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/AccountPeriodTV.ascx" TagName="UCAccountPeriodTV" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>财务收款查询</title>
    <script src="../Scripts/finance/finance.search.new.js" type="text/javascript"></script>
    <script src="../Scripts/finance/finance.search.page.new.js" type="text/javascript"></script>
    <script type="text/javascript">
        
        function init() 
        {
            var buttons = {
                btnSearch: $.dom("btnSearch", "input"),
                btnSummary: $.dom("btnSummary", "input"),
                btnDetails: $.dom("btnDetails", "input"),
                btnAllDetails: $.dom("btnAllDetails", "input"),
                btnSearchV2: $.dom("btnSearchV2", "input"),
                btnSummaryV2: $.dom("btnSummaryV2", "input"),
                btnDetailsV2: $.dom("btnDetailsV2", "input"),
                btnAllDetailsV2: $.dom("btnAllDetailsV2", "input"),
                btnAccountSearch: $.dom("btnAccountSearch", "input"),
                btnSearchDetails: $.dom("btnSearchDetails", "input")
            };

            var hiddens = {
                hidTotalCount: $.dom("hidTotalCount", "input"),
                hidDetailsCount: $.dom("hidDetailsCount", "input")
            };

            buttons.btnAccountSearch.click(function () {
                //判断必须选择日期
                if (!JudgeSearchAccountPeriod()) return false;
                $(document).progressDialog.showDialog("查询中，请稍后...");
            });
            buttons.btnSearchDetails.click(function () {
                $(document).progressDialog.showDialog("查询中，请稍后...");
            });

            search.init.ready({
                totalButtons: [buttons.btnSummary, buttons.btnAllDetails],
                detailsButtons: [buttons.btnDetails],
                totalCount: hiddens.hidTotalCount.val(),
                detailsCount: hiddens.hidDetailsCount.val(),
                renderTo: "gvSummary",
                clickable: true
            });
        }

        function fnCheckSearchCondition(E) {
            if (E.id == "rbIsAccountYes" && E.checked) {
                $("#tbAccountYes").show();
                $("#tbAccountNo").hide();
                $("#<%=hidIsAccountPeriod.ClientID %>").val("1");
            } else {
                $("#tbAccountYes").hide();
                $("#tbAccountNo").show();
                $("#<%=hidIsAccountPeriod.ClientID %>").val("0");
            }
        }

        function fnSetAccountMsg(mAccountId, mAccountName, mObjectName, mObjectId, mExpress, mExpressType, mAccountDate, mAccountDateStr, mAccountDateEnd, expressNames) {
            var tdAccountMsg = $("#tdAccountMsg");
            var arrH = new Array();
            arrH[arrH.length] = '账期名称：' + mAccountName + "<br>";
            arrH[arrH.length] = '账期日期：' + mAccountDate + "  ";
            arrH[arrH.length] = '开始日期：' + mAccountDateStr + "  ";
            arrH[arrH.length] = '结束日期：' + mAccountDateEnd + "<br>";
            arrH[arrH.length] = '包含配送公司：' + (mExpressType == "0" ? "全部" : mExpressType == "1" ? "包含" : "不包含") + "  ";
            arrH[arrH.length] = (mExpressType == "1" || mExpressType == "2" ? '配送公司：' + expressNames : "") + "<br>";
            tdAccountMsg.html(arrH.join(''));
            $("#<%=hidAccountPeriodMsg.ClientID %>").val(arrH.join(''));
        }

        $(document).ready(function () {
            if ($("#<%=hidIsAccountPeriod.ClientID %>").val() == 1) {
                var rbYes = document.getElementById("rbIsAccountYes");
                rbYes.checked = true;
                fnCheckSearchCondition(rbYes);
            }
            else {
                fnCheckSearchCondition(document.getElementById("rbIsAccountNo"));
            }
            $("#tdAccountMsg").html($("#<%=hidAccountPeriodMsg.ClientID %>").val());
        });

        function JudgeSearchAccountPeriod() {
            if (GetAccountPeriodSelect() == "") {
                alert("没有选择账期");
                return false;
            }
            return true;
        }
    </script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
财务收款查询
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div>
        <input type="radio" id="rbIsAccountYes" name="rbIsAccount" onclick="fnCheckSearchCondition(this)" /> 账期查询
        <input type="radio" id="rbIsAccountNo" name="rbIsAccount" checked="checked" onclick="fnCheckSearchCondition(this)" /> 非账期查询
    </div>
    <table border="0" cellpadding="0" cellspacing="0" id="tbAccountNo">
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
                <uc1:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" Visible="false" MerchantLoadType="ThirdMerchant" MerchantTypeSource="cw_Merchant_Inside" MerchantShowCheckBox="True" />
                <asp:DropDownList ID="ddlVanclSource" runat="server" Visible="false">
                    <asp:ListItem Value="0" Text="全部"></asp:ListItem>
                    <asp:ListItem Value="1" Text="普通订单"></asp:ListItem>
                    <asp:ListItem Value="2" Text="对日订单"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                选择站点：
            </td>
            <td>
                <uc:SelectStation ID="station" runat="server" LoadDataType="CompanySite"  />
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
            <td colspan="2">
                <asp:TextBox ID="txtBeginTime" CssClass="WSdate text" runat="server" autocomplete="off"></asp:TextBox>至<asp:TextBox
                    ID="txtEndTime" runat="server" CssClass="WSdate text" autocomplete="off"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="7">
                <asp:Button ID="btnSearch" runat="server" Text="查  询" OnClick="btnSearch_Click" />
                <asp:Button ID="btnSummary" runat="server" Text="导出汇总" OnClick="btnSummary_Click" />
                <asp:Button ID="btnDetails" runat="server" Text="导出明细" OnClick="btnDetails_Click" />
                <asp:Button ID="btnAllDetails" runat="server" Text="导出所有明细" OnClick="btnAllDetails_Click" />
                <asp:Button ID="btnSearchDetails" runat="server" Text="查询明细" OnClick="btnSearchDetails_Click"
                    Style="display: none;" />
            </td>
        </tr>
        <tr>
            <td colspan="7">
                <asp:Button ID="btnSearchV2" runat="server" Text="查  询V2" 
                    onclick="btnSearchV2_Click"/>
                <asp:Button ID="btnSummaryV2" runat="server" Text="导出汇总V2" 
                    onclick="btnSummaryV2_Click"/>
                <asp:Button ID="btnDetailsV2" runat="server" Text="导出明细V2" 
                    onclick="btnDetailsV2_Click"/>
                <asp:Button ID="btnAllDetailsV2" runat="server" Text="导出所有明细V2" 
                    onclick="btnAllDetailsV2_Click"/>
                <asp:Button ID="btnSearchDetailsV2" runat="server" Text="查询明细V2" Style="display: none;" />
            </td>
        </tr>
    </table>
    <table id="tbAccountYes" border="0" style="display:none;">
        <tr>
            <td style="width:60px;">账期选择:</td>
            <td style="width:160px;">
                <uc4:UCAccountPeriodTV ID="UCAccountPeriodTV" runat="server" PeriodDataSource="Merchant_Third" PeriodLoadType="zq_cw_Merchant_In" />
            </td>
            <td></td>
        </tr>
        <tr>
            <td colspan="3" id="tdAccountMsg"></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Button ID="btnAccountSearch" runat="server" Text="查询" onclick="btnAccountSearch_Click" />
                <asp:Button ID="btnAccountExprotSum" runat="server" Text="导出汇总" onclick="btnAccountExprotSum_Click" OnClientClick="return JudgeSearchAccountPeriod();" />
                <asp:Button ID="btnAccountExprotDetail" runat="server" Text="导出明细" onclick="btnAccountExprotDetail_Click" OnClientClick="return JudgeSearchAccountPeriod();" />
                <asp:Button ID="btnAccountExprotAllDetail" runat="server" Text="导出所有明细" onclick="btnAccountExprotAllDetail_Click" OnClientClick="return JudgeSearchAccountPeriod();" />
                <asp:HiddenField ID="hidIsAccountPeriod" runat="server" Value="0" />
                <asp:HiddenField ID="hidAccountPeriodMsg" runat="server" Value="" />
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
                <asp:BoundField HeaderText="入站时间" DataField="IntoTime" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField HeaderText="提交时间" DataField="CreateTime" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField HeaderText="归班时间" DataField="BackStationTime" DataFormatString="{0:yyyy-MM-dd}" />
            </Columns>
        </asp:GridView>
        <uc:Pager ID="detailsPager" runat="server" />
        <asp:HiddenField ID="hidDetailsCount" runat="server" Value="0" />
    </div>
</asp:Content>
