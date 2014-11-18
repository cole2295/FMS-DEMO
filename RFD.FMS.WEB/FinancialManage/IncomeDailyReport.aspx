<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    Theme="default" EnableEventValidation="false" CodeBehind="IncomeDailyReport.aspx.cs"
    Inherits="RFD.FMS.WEB.FinancialManage.IncomeDailyReport" %>

<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(function() {
            render();
        })

        $(function () {
            var buttons = {
                btnSearch: $.dom("btnSearch", "input"),
                btnExport: $.dom("btnExport", "input")
            };
            buttons.btnExport.click(function () {
                $(document).progressDialog.showDialog("导出中，请稍后...");
            });
            buttons.btnSearch.click(function () {
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
            //alert("导出查询完成，点击保存");
        }  
    </script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    收入日报表
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <asp:HiddenField ID="download_token_name" runat="server" /> 
    <asp:HiddenField ID="download_token_value" runat="server" />
    <div style="width: 100%">
        <table>
            <tr>
                <td>
                    <asp:Label Text="统计时间" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtBeginTime" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})" Width="150px" runat="server" />               
                </td>
                <td>
                    <asp:Label Text="至" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtEndTime" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})" Width="150px" runat="server" />
                </td>
                <td>
                    <asp:Label Text="商家" runat="server" />
                </td>
                <td>
                    <uc4:UCMerchantSourceTV ID="ucMerchant" runat="server" MerchantLoadType="ThirdMerchant"
                    MerchantTypeSource="nk_Merchant" MerchantShowCheckBox="True" />
                </td>
                <td>
                    <asp:Button ID="btnSearch" Text="查询" runat="server" 
                        onclick="btnSearch_Click" />
                </td>
                <td>
                    <asp:Button ID="btnExport" Text="导出" runat="server" onclick="btnExport_Click" />
                </td>
            </tr>
        </table>
    </div>
    <div>
        <asp:Panel ID="pSumData" runat="server" GroupingText="汇总信息">
            <table width="100%" cellpadding="0" cellspacing="5">
                <tr>
                    <td style=" display:none;">
                        <b>统计时间:</b><asp:Label ID="lblStatDate" runat="server"/>
                    </td>
                    <td>
                        <b>单量:</b><asp:Label ID="lblWaybillCount" runat="server"/>
                    </td>
                    <td>
                        <b>配送费:</b><asp:Label ID="lblAccountFare" runat="server"/>
                    </td>
                    <td>
                        <b>保价费:</b><asp:Label ID="lblProtectedFee" runat="server"/>
                    </td>
                    <td>
                        <b>代收手续费:</b><asp:Label ID="lblReceiveFee" runat="server"/>
                    </td>
                    <td>
                        <b>POS机服务费:</b><asp:Label ID="lblPOSReceiveServiceFee" runat="server"/>
                    </td>
                    <td>
                        <b>应收合计:</b><asp:Label ID="lblSumFee" runat="server"/>
                    </td>
                    <td>
                        <b>单均收入:</b><asp:Label ID="lblAvgFee" runat="server"/>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <div style="overflow-y: auto; overflow-x: hidden; height: 100%; width: 100%">
        <uc2:Pager ID="UCPager" runat="server" PageSize="10" />
        <asp:GridView ID="gv" runat="server" Width="100%" AllowPaging="false" EnableViewState="true">
            <EmptyDataTemplate>
                <label id="NOData">
                    没有数据</label>
            </EmptyDataTemplate>
            <HeaderStyle BackColor="#66CCFF" Height="30px" />
        </asp:GridView>
    </div>
</asp:Content>
