<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    CodeBehind="FinancePaymentSearchDM.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.FinancePaymentSearchDM"
    Theme="default" %>
<%@ Register Src="~/UserControl/MerchantSource.ascx" TagName="UCMerchantSource" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/SelectStation.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>�����տ��ѯ(��)</title>
    <script src="../Scripts/finance/finance.search.new.js" type="text/javascript"></script>
    <script src="../Scripts/finance/finance.search.page.new.js" type="text/javascript"></script>
    <script type="text/javascript">
        //��ʼ������
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
                ������Դ��
            </td>
            <td>
                <asp:DropDownList ID="ddlOrderSource" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlOrderSource_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td>
                �̼���Դ��
            </td>
            <td>
                <uc1:UCMerchantSource ID="UCMerchantSource" runat="server" LoadDataType="ThirdMerchant" SelectEnable="false" />
            </td>
            <td>
                ѡ��վ�㣺
            </td>
            <td>
                <uc:SelectStation ID="station" runat="server" />
            </td>
            <td>
                ������ʽ��<asp:RadioButtonList ID="rblExportFormat" runat="server" RepeatDirection="Horizontal"
                    RepeatLayout="Flow">
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                ���ʽ��
            </td>
            <td>
                <asp:DropDownList ID="ddlPayType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPayType_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td>
                POS�����ͣ�
            </td>
            <td>
                <asp:DropDownList ID="ddlPOSType" runat="server">
                </asp:DropDownList>
            </td>
            <td>
                �������ڣ�
            </td>
            <td>
                <asp:TextBox ID="txtBeginTime" CssClass="WSdate text" runat="server" autocomplete="off"></asp:TextBox>��<asp:TextBox
                    ID="txtEndTime" runat="server" CssClass="WSdate text" autocomplete="off"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="��  ѯ" OnClick="btnSearch_Click" />
                <asp:Button ID="btnSummary" runat="server" Text="��������" OnClick="btnSummary_Click" />
                <asp:Button ID="btnDetails" runat="server" Text="������ϸ" OnClick="btnDetails_Click" />
                <asp:Button ID="btnAllDetails" runat="server" Text="����������ϸ" OnClick="btnAllDetails_Click" />
                <asp:Button ID="btnSearchDetails" runat="server" Text="��ѯ��ϸ" OnClick="btnSearchDetails_Click"
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
                <asp:BoundField HeaderText="���" DataField="ID" />
                <asp:BoundField HeaderText="���͹�˾" DataField="DistributionName" />
                <asp:BoundField HeaderText="����վ" DataField="CompanyName" SortExpression="CompanyName" />
                <asp:BoundField HeaderText="�̼ұ���" DataField="MerchantCode" />
                <asp:BoundField HeaderText="��Դ" DataField="SourceName" />
                <asp:BoundField HeaderText="�ֽ�ɹ�����" DataField="CashWaybillCount" />
                <asp:BoundField HeaderText="POS���ɹ�����" DataField="POSWaybillCount" />
                <asp:BoundField HeaderText="�ɹ����" DataField="AcceptAmount" DataFormatString="{0:#,##0.##}" />
                <asp:BoundField HeaderText="�˿����" DataField="BackWaybillCount" />
                <asp:BoundField HeaderText="�˿���" DataField="CashRealOutSum" DataFormatString="{0:#,##0.##}" />
                <asp:BoundField HeaderText="�����" DataField="SaveAmount" DataFormatString="{0:#,##0.##}" />
                <asp:TemplateField HeaderText="��������">
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
                <asp:BoundField HeaderText="���" DataField="ID" />
                <asp:BoundField HeaderText="����վ" DataField="CompanyName" />
                <asp:BoundField HeaderText="�˵���" DataField="WaybillNO" />
                <asp:BoundField HeaderText="������" DataField="CustomerOrder" />
                <asp:BoundField HeaderText="Ӧ�ս��" DataField="NeedAmount" DataFormatString="{0:#,##0.#}" />
                <asp:BoundField HeaderText="Ӧ�˽��" DataField="NeedBackAmount" DataFormatString="{0:#,##0.#}" />
                <asp:BoundField HeaderText="֧����ʽ" DataField="AcceptType" />
                <asp:BoundField HeaderText="�����տ�״̬" DataField="FinancialStatus" />
                <asp:BoundField HeaderText="POS�ն˺�" DataField="POSCode" />
                <asp:BoundField HeaderText="POS������" DataField="STATUSNAME" />
                <asp:BoundField HeaderText="��վʱ��" DataField="IntoTime" DataFormatString="{0:D}" />
                <asp:BoundField HeaderText="�ύʱ��" DataField="CreateTime" DataFormatString="{0:D}" />
            </Columns>
        </asp:GridView>
        <uc:Pager ID="detailsPager" runat="server" />
        <asp:HiddenField ID="hidDetailsCount" runat="server" Value="0" />
    </div>
</asp:Content>
