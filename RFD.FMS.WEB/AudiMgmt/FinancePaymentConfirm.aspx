<%@ Page Title="" Language="C#" MasterPageFile="~/AudiMgmt/FinanceSearch.Master"
    AutoEventWireup="true" CodeBehind="FinancePaymentConfirm.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.FinancePaymentConfirm"
    Theme="default" %>

<%@ MasterType VirtualPath="~/AudiMgmt/FinanceSearch.Master" %>
<%@ Register Src="~/UserControl/SelectStation.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    <title>�����տ�ȷ��</title>

    <script type="text/javascript">
        //��ʼ������
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
            //��ȷ�Ͻ������֤
            $.dom("gvSummary", ".grid").find("tr td:last-child").find("a").click(function() {
                if (!util.numUtil.checkNumber($(this).prev(), { text: "ȷ�Ͻ��", limit: false })) return false;
                hiddens.hidConfirmAmount.val($(this).parent().find("input").filter($.dom("txtConfirmAmount", "input")).val());
                return true;
            });
            //��������������֤
            buttons.btnReload.click(function() {
                if (!search.validate.isSelectStation()) return false;
                if (!util.dateUtil.checkDate(datetimes.reloadTime.val(),
				                             null, {
				                                 isCheckAllEmpty: true,
				                                 emptyText: "��ѡ���������ڣ�"
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
�����տ�ȷ��
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="station" runat="server">
    <table border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td>
                ѡ��վ�㣺
            </td>
            <td>
                <uc:SelectStation ID="station" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="operate" runat="server">
    <div>
        ���ʽ��<asp:DropDownList ID="ddlPayType" runat="server">
        </asp:DropDownList>
    </div>
    <div>
        �������ڣ�
        <asp:TextBox ID="txtBeginTime" CssClass="WSdate text" runat="server" autocomplete="off"></asp:TextBox>
        <span id="to">��</span>
        <asp:TextBox ID="txtEndTime" runat="server" CssClass="WSdate text" autocomplete="off"></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="��  ѯ" OnClick="btnSearch_Click"/>
        <asp:Button ID="btnSummary" runat="server" Text="��������" OnClick="btnSummary_Click"/>
        <asp:Button ID="btnDetails" runat="server" Text="������ϸ" OnClick="btnDetails_Click"/>
        <asp:Button ID="btnAllDetails" runat="server" Text="����������ϸ" OnClick="btnAllDetails_Click"/>
        �������ڣ�
        <asp:TextBox ID="txtReloadDate" CssClass="WSdate text" runat="server" autocomplete="off"></asp:TextBox>
        <asp:Button ID="btnReload" runat="server" Text="��������" OnClick="btnReload_Click" />
        <asp:Button ID="btnCheckMount" runat="server" Text="���У��" OnClientClick="return showWindow();" />
        <asp:Button ID="btnSearchDetails" runat="server" Text="��ѯ��ϸ" OnClick="btnSearchDetails_Click" Style="display: none;" />
    </div>
    <div id="sorttr" style="background: #FFFFFF; border: 1px solid #990000; width: 400px;
        height: 200px; display: none; position: absolute; left: 210px; top: 459px;">
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="summaryContent" runat="server">
    <div class="totalContainer">
    <asp:Literal ID="ltlTotal" runat="server"></asp:Literal>
<%--        <b>�ֽ�ɹ�������</b><asp:Label ID="lblCashCount" runat="server"></asp:Label><b>POS���ɹ�������</b><b>�ɹ�������</b><b>�ɹ���</b><b>�˿������</b><b>�˿��</b><b>����</b>
--%>    </div>
    <asp:GridView ID="gvSummary" runat="server" AutoGenerateColumns="False" Width="100%"
        AllowSorting="true" OnSorting="gvSummary_Sorting" OnRowCommand="gvSummary_RowCommand">
        <Columns>
            <asp:BoundField HeaderText="���" DataField="ID" />
            <asp:BoundField HeaderText="����վ" DataField="CompanyName" SortExpression="CompanyName" />
            <asp:BoundField HeaderText="��Դ" DataField="SourceName" />
            <asp:BoundField HeaderText="�ֽ�ɹ�����" DataField="CashSuccOrderCount" />
            <asp:BoundField HeaderText="POS���ɹ�����" DataField="PosSuccOrderCount" />
            <asp:BoundField HeaderText="�ɹ����" DataField="AcceptAmount" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="�˿����" DataField="DayOutOrderCount" />
            <asp:BoundField HeaderText="�˿���" DataField="CashRealOutSum" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="�����" DataField="SaveAmount" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="�����տ�״̬" DataField="FinanceStatus" />
            <asp:TemplateField HeaderText="��������">
                <ItemTemplate>
                    <asp:Label ID="lblCreateTime" runat="server" Text='<%# Bind("DailyTime", "{0:D}") %>'></asp:Label>
                    <asp:HiddenField ID="hidQueryParams" runat="server" Value='<%# Bind("QueryParams") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ȷ�Ͻ��">
                <ItemTemplate>
                    <asp:Label ID="lblConfirm" runat="server" Text='<%# Bind("RealInCome","{0:#,##0.##}")%>'
                        Visible='<%# Eval("FinanceStatus").ToString()!="δ�տ�"%>'></asp:Label>
                    <asp:TextBox ID="txtConfirmAmount" runat="server" CssClass="text" Width="40px" Visible='<%# Eval("FinanceStatus").ToString()=="δ�տ�"%>'></asp:TextBox>
                    <asp:LinkButton ID="lbtnConfirm" runat="server" Visible='<%# Eval("FinanceStatus").ToString()=="δ�տ�"%>'
                        CommandArgument='<%# Eval("TotalDataID").ToString() %>' CommandName="confirm">ȷ��</asp:LinkButton>
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
            <asp:BoundField HeaderText="���" DataField="ID" />
            <asp:BoundField HeaderText="����վ" DataField="CompanyName" />
            <asp:BoundField HeaderText="��Դ" DataField="SourceName" />
            <asp:BoundField HeaderText="�˵���" DataField="WaybillNO" />
            <asp:BoundField HeaderText="Ӧ�ս��" DataField="NeedPrice" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="Ӧ�˽��" DataField="NeedReturnPrice" DataFormatString="{0:#,##0.##}" />
            <asp:BoundField HeaderText="֧����ʽ" DataField="AcceptType" />
            <asp:BoundField HeaderText="POS�ն˺�" DataField="PosNum" />
            <asp:BoundField HeaderText="��վʱ��" DataField="EnterTime" DataFormatString="{0:D}" />
            <asp:BoundField HeaderText="�ύʱ��" DataField="PostTime" DataFormatString="{0:D}" />
        </Columns>
    </asp:GridView>
    <uc:Pager ID="detailsPager" runat="server" />
    <asp:HiddenField ID="hidDetailsCount" runat="server" Value="0" />
</asp:Content>
