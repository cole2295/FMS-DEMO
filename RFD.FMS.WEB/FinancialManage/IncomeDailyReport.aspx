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
                $(document).progressDialog.showDialog("�����У����Ժ�...");
            });
            buttons.btnSearch.click(function () {
                $(document).progressDialog.showDialog("��ѯ�У����Ժ�...");
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
            //alert("������ѯ��ɣ��������");
        }  
    </script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    �����ձ���
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <asp:HiddenField ID="download_token_name" runat="server" /> 
    <asp:HiddenField ID="download_token_value" runat="server" />
    <div style="width: 100%">
        <table>
            <tr>
                <td>
                    <asp:Label Text="ͳ��ʱ��" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtBeginTime" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})" Width="150px" runat="server" />               
                </td>
                <td>
                    <asp:Label Text="��" runat="server" />
                </td>
                <td>
                    <asp:TextBox ID="txtEndTime" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})" Width="150px" runat="server" />
                </td>
                <td>
                    <asp:Label Text="�̼�" runat="server" />
                </td>
                <td>
                    <uc4:UCMerchantSourceTV ID="ucMerchant" runat="server" MerchantLoadType="ThirdMerchant"
                    MerchantTypeSource="nk_Merchant" MerchantShowCheckBox="True" />
                </td>
                <td>
                    <asp:Button ID="btnSearch" Text="��ѯ" runat="server" 
                        onclick="btnSearch_Click" />
                </td>
                <td>
                    <asp:Button ID="btnExport" Text="����" runat="server" onclick="btnExport_Click" />
                </td>
            </tr>
        </table>
    </div>
    <div>
        <asp:Panel ID="pSumData" runat="server" GroupingText="������Ϣ">
            <table width="100%" cellpadding="0" cellspacing="5">
                <tr>
                    <td style=" display:none;">
                        <b>ͳ��ʱ��:</b><asp:Label ID="lblStatDate" runat="server"/>
                    </td>
                    <td>
                        <b>����:</b><asp:Label ID="lblWaybillCount" runat="server"/>
                    </td>
                    <td>
                        <b>���ͷ�:</b><asp:Label ID="lblAccountFare" runat="server"/>
                    </td>
                    <td>
                        <b>���۷�:</b><asp:Label ID="lblProtectedFee" runat="server"/>
                    </td>
                    <td>
                        <b>����������:</b><asp:Label ID="lblReceiveFee" runat="server"/>
                    </td>
                    <td>
                        <b>POS�������:</b><asp:Label ID="lblPOSReceiveServiceFee" runat="server"/>
                    </td>
                    <td>
                        <b>Ӧ�պϼ�:</b><asp:Label ID="lblSumFee" runat="server"/>
                    </td>
                    <td>
                        <b>��������:</b><asp:Label ID="lblAvgFee" runat="server"/>
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
                    û������</label>
            </EmptyDataTemplate>
            <HeaderStyle BackColor="#66CCFF" Height="30px" />
        </asp:GridView>
    </div>
</asp:Content>
