<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    CodeBehind="FinancePaymentSearch.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.FinancePaymentSearch"
    Theme="default" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/AccountPeriodTV.ascx" TagName="UCAccountPeriodTV" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>�����տ��ѯ</title>
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
                //�жϱ���ѡ������
                if (!JudgeSearchAccountPeriod()) return false;
                $(document).progressDialog.showDialog("��ѯ�У����Ժ�...");
            });
            buttons.btnSearchDetails.click(function () {
                $(document).progressDialog.showDialog("��ѯ�У����Ժ�...");
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
            arrH[arrH.length] = '�������ƣ�' + mAccountName + "<br>";
            arrH[arrH.length] = '�������ڣ�' + mAccountDate + "  ";
            arrH[arrH.length] = '��ʼ���ڣ�' + mAccountDateStr + "  ";
            arrH[arrH.length] = '�������ڣ�' + mAccountDateEnd + "<br>";
            arrH[arrH.length] = '�������͹�˾��' + (mExpressType == "0" ? "ȫ��" : mExpressType == "1" ? "����" : "������") + "  ";
            arrH[arrH.length] = (mExpressType == "1" || mExpressType == "2" ? '���͹�˾��' + expressNames : "") + "<br>";
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
                alert("û��ѡ������");
                return false;
            }
            return true;
        }
    </script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
�����տ��ѯ
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <div>
        <input type="radio" id="rbIsAccountYes" name="rbIsAccount" onclick="fnCheckSearchCondition(this)" /> ���ڲ�ѯ
        <input type="radio" id="rbIsAccountNo" name="rbIsAccount" checked="checked" onclick="fnCheckSearchCondition(this)" /> �����ڲ�ѯ
    </div>
    <table border="0" cellpadding="0" cellspacing="0" id="tbAccountNo">
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
                <uc1:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" Visible="false" MerchantLoadType="ThirdMerchant" MerchantTypeSource="cw_Merchant_Inside" MerchantShowCheckBox="True" />
                <asp:DropDownList ID="ddlVanclSource" runat="server" Visible="false">
                    <asp:ListItem Value="0" Text="ȫ��"></asp:ListItem>
                    <asp:ListItem Value="1" Text="��ͨ����"></asp:ListItem>
                    <asp:ListItem Value="2" Text="���ն���"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                ѡ��վ�㣺
            </td>
            <td>
                <uc:SelectStation ID="station" runat="server" LoadDataType="CompanySite"  />
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
            <td colspan="2">
                <asp:TextBox ID="txtBeginTime" CssClass="WSdate text" runat="server" autocomplete="off"></asp:TextBox>��<asp:TextBox
                    ID="txtEndTime" runat="server" CssClass="WSdate text" autocomplete="off"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="7">
                <asp:Button ID="btnSearch" runat="server" Text="��  ѯ" OnClick="btnSearch_Click" />
                <asp:Button ID="btnSummary" runat="server" Text="��������" OnClick="btnSummary_Click" />
                <asp:Button ID="btnDetails" runat="server" Text="������ϸ" OnClick="btnDetails_Click" />
                <asp:Button ID="btnAllDetails" runat="server" Text="����������ϸ" OnClick="btnAllDetails_Click" />
                <asp:Button ID="btnSearchDetails" runat="server" Text="��ѯ��ϸ" OnClick="btnSearchDetails_Click"
                    Style="display: none;" />
            </td>
        </tr>
        <tr>
            <td colspan="7">
                <asp:Button ID="btnSearchV2" runat="server" Text="��  ѯV2" 
                    onclick="btnSearchV2_Click"/>
                <asp:Button ID="btnSummaryV2" runat="server" Text="��������V2" 
                    onclick="btnSummaryV2_Click"/>
                <asp:Button ID="btnDetailsV2" runat="server" Text="������ϸV2" 
                    onclick="btnDetailsV2_Click"/>
                <asp:Button ID="btnAllDetailsV2" runat="server" Text="����������ϸV2" 
                    onclick="btnAllDetailsV2_Click"/>
                <asp:Button ID="btnSearchDetailsV2" runat="server" Text="��ѯ��ϸV2" Style="display: none;" />
            </td>
        </tr>
    </table>
    <table id="tbAccountYes" border="0" style="display:none;">
        <tr>
            <td style="width:60px;">����ѡ��:</td>
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
                <asp:Button ID="btnAccountSearch" runat="server" Text="��ѯ" onclick="btnAccountSearch_Click" />
                <asp:Button ID="btnAccountExprotSum" runat="server" Text="��������" onclick="btnAccountExprotSum_Click" OnClientClick="return JudgeSearchAccountPeriod();" />
                <asp:Button ID="btnAccountExprotDetail" runat="server" Text="������ϸ" onclick="btnAccountExprotDetail_Click" OnClientClick="return JudgeSearchAccountPeriod();" />
                <asp:Button ID="btnAccountExprotAllDetail" runat="server" Text="����������ϸ" onclick="btnAccountExprotAllDetail_Click" OnClientClick="return JudgeSearchAccountPeriod();" />
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
                <asp:BoundField HeaderText="��վʱ��" DataField="IntoTime" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField HeaderText="�ύʱ��" DataField="CreateTime" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField HeaderText="���ʱ��" DataField="BackStationTime" DataFormatString="{0:yyyy-MM-dd}" />
            </Columns>
        </asp:GridView>
        <uc:Pager ID="detailsPager" runat="server" />
        <asp:HiddenField ID="hidDetailsCount" runat="server" Value="0" />
    </div>
</asp:Content>
