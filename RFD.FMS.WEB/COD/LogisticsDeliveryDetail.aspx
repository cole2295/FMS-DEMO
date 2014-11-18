<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogisticsDeliveryDetail.aspx.cs"
    Inherits="RFD.FMS.WEB.COD.LogisticsDeliveryDetail" MasterPageFile="~/Main/main.Master" %>

<%@ Register Src="~/UserControl/ExpressCompanyTV.ascx" TagName="UCExpressCompanyTV"
    TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/StationCheckBox.ascx" TagName="UCStationCheckBox"
    TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/WareHouseCheckBox.ascx" TagName="UCWareHouseCheckBox"
    TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV"
    TagPrefix="uc4" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .style5
        {
            width: 96px;
        }
    </style>
    <script type="text/javascript">
        var isShowDialog = false;
        $(document).ready(function () {
            $("#<%=gvList.ClientID %>").toSuperTable(
                { width: "1000px", height: "280px", fixedCols: 0,
                    onFinish: function () {
                    }
                });
        });
        $(function () {
            var buttons = {
                btSearch: $.dom("btSearch", "input"),
                btExprot: $.dom("btExprot", "input"),
                btExportV3: $.dom("btExportV3", "input")
            };
            buttons.btExportV3.click(function () {
                if(isShowDialog)
                    $(document).progressDialog.showDialog("导出中，请稍后...");
            });
            buttons.btSearch.click(function () {
                if(isShowDialog)
                    $(document).progressDialog.showDialog("查询中，请稍后...");
            });
        });

        $(document).ready(function () {
            $('#form1').submit(function () {
                blockUIForDownload($('#<% =download_token_value.ClientID%>'), $('#<% =download_token_name.ClientID%>'));
            });
        });
        var fileDownloadCheckTimer;
        function blockUIForDownload(value, name) {
            var token = new Date().getTime(); //use the current timestamp as the token value
            value.val(token);
            name.val(token + "token");
            fileDownloadCheckTimer = window.setInterval(function () {
                var cookieValue = $.cookie(name.val());
                if (cookieValue == token)
                    finishDownload(name);
            }, 1000);
        }

        function finishDownload(name) {
            window.clearInterval(fileDownloadCheckTimer);
            $.cookie(name.val(), null);
            $(document).progressDialog.hideDialog();
            alert("导出查询完成，点击保存");
        }

        function JudgeInputSearch() {
            //金额输入
            var amountStr = $("#<%=txtAmountStr.ClientID %>").val();
            var amountEnd = $("#<%=txtAmountEnd.ClientID %>").val();
            if (!validateInt(amountStr) || !validateInt(amountEnd)) {
                alert("金额输入必须为数字");
                isShowDialog = false;
                return false;
            }
            var dateStr = $("#<%=txtBTime.ClientID %>").val();
            var dateEnd = $("#<%=txtETime.ClientID %>").val();
            if (dateStr == "" || dateEnd == "") {
                alert("查询日期不能为空");
                isShowDialog = false;
                return false;
            }

            var dtsTime = new Date(Date.parse(dateStr.replace(/-/g, "/")));
            var dteTime = new Date(Date.parse(dateEnd.replace(/-/g, "/")));
            var cdt = (dteTime.getTime() - dtsTime.getTime()) / (24 * 60 * 60 * 1000); //相差天数
            if (cdt < 0) {
                alert("开始日期不能大于结束日期");
                isShowDialog = false;
                return false;
            }

            if (cdt > 31) {
                alert("日期范围不能大于31天");
                isShowDialog = false;
                return false;
            }

            isShowDialog = true;
            return true;
        }
    </script>
</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    发货明细表
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">   
    <asp:HiddenField ID="download_token_name" runat="server" /> 
    <asp:HiddenField ID="download_token_value" runat="server" />
    <table border="0">
        <tr>
            <td>
                第三方配送商
            </td>
            <td>
                <uc1:UCExpressCompanyTV ID="UCExpressCompanyTV" runat="server" ExpressLoadType="ThirdCompany"
                    ExpressTypeSource="nk_Express" ExpressCompanyShowCheckBox="True" />
            </td>
            <td>
                报表类型
            </td>
            <td>
                <asp:DropDownList ID="ddlReportType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlReportType_SelectedIndexChanged">
                    <asp:ListItem Value="">发货明细表</asp:ListItem>
                    <asp:ListItem Value="7">拒收明细表</asp:ListItem>
                    <asp:ListItem Value="6">上门退换货明细表</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                -
            </td>
            <td>
                <asp:DropDownList ID="ddlShipmentType" runat="server">
                    <asp:ListItem Value="">全部</asp:ListItem>
                    <asp:ListItem Value="0">普通发货</asp:ListItem>
                    <asp:ListItem Value="1">上门换发货</asp:ListItem>
                    <asp:ListItem Value="3">签单返回发货</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                分配区域
            </td>
            <td>
                <asp:DropDownList ID="ddlAreaType" runat="server">
                </asp:DropDownList>
            </td>
            <td>订单总价</td>
            <td>
                <asp:TextBox ID="txtAmountStr" runat="server" Width="50px"></asp:TextBox>~<asp:TextBox ID="txtAmountEnd" runat="server" Width="50px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:DropDownList ID="ddlRFDType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRFDType_SelectedIndexChanged">
                    <asp:ListItem Value="0">站点</asp:ListItem>
                    <asp:ListItem Value="1">分拣</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <uc2:UCStationCheckBox ID="UCStationRFDSite" runat="server" LoadDataType="RFDSite" />
            </td>
            <td>
                <asp:DropDownList ID="ddlDateType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDateType_SelectedIndexChanged">
                    <asp:ListItem Value="1">最终发日期</asp:ListItem>
                    <asp:ListItem Value="0">初始发日期</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="txtBTime" Width="128px" Visible="true" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"
                    runat="server"></asp:TextBox>
            </td>
            <td>
                ~
            </td>
            <td colspan="1">
                <asp:TextBox ID="txtETime" Width="128px" Visible="true" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"
                    runat="server"></asp:TextBox>
            </td>
            <td>
                运单号
            </td>
            <td>
               <%-- <asp:TextBox ID="txtWaybillNO" runat="server"></asp:TextBox>--%>
               <asp:TextBox ID="txtWaybillNO" runat="server" Width="200px" Height="45px" 
                    TextMode="MultiLine"></asp:TextBox>
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td>
                商家
            </td>
            <td>
                <uc4:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" MerchantLoadType="All"
                    MerchantTypeSource="nk_Merchant_Expense" MerchantShowCheckBox="True" />
            </td>
            <td>
                <asp:Label ID="lbHouseType" runat="server">最终发货仓</asp:Label>
                <asp:HiddenField ID="hfHouseType" runat="server" Value="0" />
            </td>
            <td colspan="3">
                <uc3:UCWareHouseCheckBox ID="UCWareHouseCheckBox" runat="server" HouseCheckType="all" />
            </td>
            <td>
                业务类型
            </td>
            <td>
                <asp:DropDownList ID="ddlIsCod" runat="server">
                    <asp:ListItem Value="-1">全部</asp:ListItem>
                    <asp:ListItem Value="0">非COD</asp:ListItem>
                    <asp:ListItem Value="1">COD</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td></td>
            <td></td>
        </tr>
        <tr>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td></td>
            <td colspan="4" align="left">
                <asp:Button ID="btSearch" runat="server" Text="查询" AccessKey="Q" OnClick="btSearch_Click" OnClientClick="return JudgeInputSearch();" />
                <asp:Button ID="btExportV3" runat="server" Text="导出" AccessKey="O" OnClick="btExprot_ClickV3"
                    Style="width: 80px" OnClientClick="return JudgeInputSearch();" ToolTip="原导出（改进）"  />
                <asp:Button ID="btExprot" runat="server" Text="导出" AccessKey="O" OnClick="btExprot_Click"
                    Style="width: 40px" OnClientClick="return JudgeInputSearch();" Visible="false" />
                    <asp:Button ID="btSearchV2" runat="server" Text="查询V2" Visible="false" AccessKey="Q" OnClick="btSearch_ClickV2" />
                <asp:Button ID="btExprotV2" runat="server" Text="导出V2" Visible="false" AccessKey="O" OnClick="btExprot_ClickV2"
                    Style="width: 60px" />
            </td>
        </tr>
    </table>
    <div style="color:Red;">注意：多选的需要全部查询，则不用选择即表示为全部包含查询。如：商家需要全部包含查询，则不用选择商家即为全部查询。</div>
    <table style="font-weight: bold; font-size: 13px;">
        <tr>
            <td>
                订单量合计：
            </td>
            <td>
                <asp:Label ID="lbWaybillStat" runat="server">0</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                总价合计：
            </td>
            <td>
                <asp:Label ID="lbTotalAmount" runat="server">0.00</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                <asp:Label ID="lbPaidName" runat="server">已收款合计</asp:Label>：
            </td>
            <td>
                <asp:Label ID="lbPaidStat" runat="server">0.00</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                <asp:Label ID="lbNeedName" runat="server">应收订单量合计</asp:Label>：
            </td>
            <td>
                <asp:Label ID="lbNeedStat" runat="server">0</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                <asp:Label ID="lbNeedPayName" runat="server">应收款合计</asp:Label>：
            </td>
            <td>
                <asp:Label ID="lbNeedPayStat" runat="server">0.00</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                结算重量合计：
            </td>
            <td>
                <asp:Label ID="lbWeightStat" runat="server">0.000</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                配送费合计：
            </td>
            <td>
                <asp:Label ID="lbFareStat" runat="server">0.000</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                保价金额合计：
            </td>
            <td>
                <asp:Label ID="lbProtectedPriceStat" runat="server">0.000</asp:Label>
            </td>
        </tr>
    </table>
    <div style="text-align: center">
        <asp:Label ID="noData" runat="server" ForeColor="Red" Visible="false"></asp:Label></div>
    <asp:GridView ID="gvList" runat="server" Wrap="False" AutoGenerateColumns="false">
        <HeaderStyle CssClass="noWrap" Wrap="false"></HeaderStyle>
        <RowStyle CssClass="noWrap" Wrap="false"></RowStyle>
    </asp:GridView>
    <div>
        <uc5:UCPager ID="UCPager" runat="server" />
    </div>
    <asp:Panel ID="pColumns" runat="server">
    </asp:Panel>
</asp:Content>
