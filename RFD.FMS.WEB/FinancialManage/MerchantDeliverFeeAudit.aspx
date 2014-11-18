<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MerchantDeliverFeeAudit.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.MerchantDeliverFeeAudit" MasterPageFile="~/FinancialManage/MerchantDeliverFee.Master" Theme="default" %>


<%@ MasterType VirtualPath="~/FinancialManage/MerchantDeliverFee.Master" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
商家配送费审核
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="OperatorArea" runat="server">
	<asp:Button ID="btnAuditFee" runat="server" Text="审核" 
		onclick="btnAuditFee_Click" OnClientClick="return ClientConfirm('确认审核选中的记录？')" />
	<asp:Button ID="btnResetFee" runat="server" Text="置回" 
		onclick="btnResetFee_Click" OnClientClick="return ClientConfirm('确认置回选中的记录？')" />

    <asp:Button ID="btnAuditFeeWait" runat="server" Text="审核待生效" 
        onclick="btnAuditFeeWait_Click" OnClientClick="return ClientConfirm('确认审核选中的记录？')" />
    <asp:Button ID="btnResetFeeWait" runat="server" Text="置回待生效" 
        onclick="btnResetFeeWait_Click" OnClientClick="return ClientConfirm('确认置回选中的记录？')" />
</asp:Content>