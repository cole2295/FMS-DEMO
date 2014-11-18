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
����տ��ѯ
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div style="width: 100%">
        <table cellpadding="3" cellspacing="5">
            <tr>
                <td align="right">
                    ѡ��վ�㣺
                </td>
                <td>
                    <uc:SelectStation ID="UCSelectStation" runat="server" LoadDataType="OnlySite" />
                </td>
                <td>
                    ���ͷѽ��㷽ʽ��
                </td>
                <td>
                    <asp:DropDownList ID="ddlTransferPayType" runat="server">
                        <asp:ListItem Text="---��ѡ��---" Value="0"></asp:ListItem>
                        <asp:ListItem Text="�ֽ�" Value="1"></asp:ListItem>
                        <asp:ListItem Text="����" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlPayType" runat="server" Visible="false">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="right">
                    ѡ���̼ң�
                </td>
                <td>
                    <uc1:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" MerchantLoadType="ThirdMerchant" MerchantTypeSource="cw_Merchant_Inside" MerchantShowCheckBox="True" />
                </td>
                <td>
                    �������ڣ�
                </td>
                <td colspan="3">
                    <asp:TextBox ID="txtBeginTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>��<asp:TextBox
                        ID="txtEndTime" runat="server" CssClass="Wdate text" autocomplete="off"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="5" align="right">
                    <asp:Button ID="btnSearch" runat="server" Text="��  ѯ" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnSummary" runat="server" Text="��������" OnClick="btnSummary_Click" />
                    <asp:Button ID="btnDetails" runat="server" Text="������ϸ" OnClick="btnDetails_Click" />
                    <asp:Button ID="btnAddDetails" runat="server" Text="����������ϸ" OnClick="btnAddDetails_Click" />
                    <asp:Button ID="btnSearchDetails" runat="server" Text="��ѯ��ϸ" OnClick="btnSearchDetails_Click"
                        Style="display: none;" />
                </td>
            </tr>
            <tr>
                <td colspan="5" align="right">
                    <asp:Button ID="btnSearchV2" runat="server" Text="��  ѯV2" 
                        onclick="btnSearchV2_Click"/>
                    <asp:Button ID="btnSummaryV2" runat="server" Text="��������V2" 
                        onclick="btnSummaryV2_Click"/>
                    <asp:Button ID="btnDetailsV2" runat="server" Text="������ϸV2" 
                        onclick="btnDetailsV2_Click"/>
                    <asp:Button ID="btnAddDetailsV2" runat="server" Text="����������ϸV2" 
                        onclick="btnAddDetailsV2_Click"/>
                    <asp:Button ID="btnSearchDetailsV2" runat="server" Text="��ѯ��ϸV2" Style="display: none;" />
                </td>
            </tr>
        </table>
    </div>
    <div>
        <asp:Panel ID="pSumData" runat="server" GroupingText="������Ϣ">
            <table width="100%" cellpadding="0" cellspacing="5">
                <tr>
                    <td style=" display:none;">
                        <b>�ֽ�����</b><asp:Label ID="lblCashCount" runat="server"></asp:Label>
                    </td>
                    <td style=" display:none;">
                        <b>POS������</b><asp:Label ID="lblPOSCount" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>�ܵ�����</b><asp:Label ID="lblSumCount" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>�����ͷѣ�</b><asp:Label ID="lblTransferSum" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>���۷Ѷ�������</b><asp:Label ID="lblProtectedPriceCount" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>�ܱ��۷ѣ�</b><asp:Label ID="lblProtectedPriceSum" runat="server"></asp:Label>
                    </td>
                    <td>
                        <b>�ܴ���</b><asp:Label ID="lblSumPrice" runat="server"></asp:Label>
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
                <asp:BoundField HeaderText="���" DataField="ID" />
                <asp:BoundField HeaderText="���͹�˾" DataField="DistributionName" />
                <asp:BoundField HeaderText="����վ" DataField="CompanyName" />
                <asp:BoundField HeaderText="��Դ" DataField="SourceName" />
                <asp:BoundField HeaderText="�ɹ�����" DataField="CashWaybillCount" />
                <asp:BoundField HeaderText="POS���ɹ�����" DataField="POSWaybillCount" Visible="false"/>
                <asp:BoundField HeaderText="���ͷ�" DataField="TransferFeeSum" DataFormatString="{0:#,##0.##}" />
                <asp:BoundField HeaderText="���۷�" DataField="ProtectedPriceSum" />
                <asp:BoundField HeaderText="�����" DataField="SaveAmount" DataFormatString="{0:#,##0.##}" />
                <asp:TemplateField HeaderText="��������">
                    <ItemTemplate>
                        <asp:Label ID="lblCreateTime" runat="server" Text='<%# Bind("CreateTime", "{0:D}") %>'></asp:Label>
                        <asp:HiddenField ID="hidQueryParams" runat="server" Value='<%# Bind("ExpressCompanyID") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <label id="NOData">
                    û������</label>
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
                <asp:BoundField HeaderText="���" DataField="���" />
                <asp:BoundField HeaderText="����վ" DataField="����վ" />
                <asp:BoundField HeaderText="�˵���" DataField="�˵���" />
                <asp:BoundField HeaderText="������" DataField="������" />
                <asp:BoundField HeaderText="�̼�" DataField="�̼�" />
                <asp:BoundField HeaderText="���ͷ�" DataField="���ͷ�" DataFormatString="{0:#,##0.##}" />
                <asp:BoundField HeaderText="���۷�" DataField="���۷�" />
                <asp:BoundField HeaderText="֧����ʽ" DataField="֧����ʽ" />
                <asp:BoundField HeaderText="POS�ն˺�" DataField="POS�ն˺�" />
                <asp:BoundField HeaderText="ͳ��ʱ��" DataField="ͳ��ʱ��" DataFormatString="{0:D}" />
                <asp:BoundField HeaderText="�ύʱ��" DataField="�ύʱ��" DataFormatString="{0:D}" />
            </Columns>
        </asp:GridView>
        <%--<uc2:Pager ID="detailsPager" runat="server" />
	<asp:HiddenField ID="hidDetailsCount" runat="server" Value="0" />--%>
    </div>
</asp:Content>
