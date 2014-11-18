<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    CodeBehind="WebForm11.aspx.cs" Inherits="RFD.FMS.WEB.test.WebForm11" %>

<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/AccountPeriodTV.ascx" TagName="UCAccountPeriodTV" TagPrefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
<div style="overflow: hidden; padding-top: 10px;" id="total">
<asp:Button ID="btnCheck" runat="server" Text="查询" onclick="btnCheck_Click" />
    <asp:GridView ID="gvSummary" runat="server" Width="100%">
        <HeaderStyle HorizontalAlign="Center" />
        <Columns>
        </Columns>
    </asp:GridView>
    </div>

    <%--tree测试--%>
    <div>
    
        <uc1:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" MerchantLoadType="All" MerchantTypeSource="cw_Merchant_Foreign" MerchantShowCheckBox="false" />
        <asp:Button ID="btShow" runat="server" Text="显示ID" onclick="btShow_Click" />
        <asp:Label ID="lbShow" runat="server"></asp:Label>
    </div>

    <div>
        <asp:Button ID="btnMailTest" runat="server" Text="邮件发送测试" 
            onclick="btnMailTest_Click" />
    </div>
    <div>
        <uc2:UCAccountPeriodTV ID="UCAccountPeriodTV" runat="server" PeriodDataSource="Merchant_Third" PeriodLoadType="zq_cw_Merchant_In" />
        <asp:Button ID="Button1" runat="server" Text="显示ID" onclick="Button1_Click" />
        <asp:Label ID="Label1" runat="server"></asp:Label>
    </div>
</asp:Content>
