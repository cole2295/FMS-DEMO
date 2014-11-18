<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountPeriodTV.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.AccountPeriodTV" %>

<script type="text/javascript" language="javascript">
    function RunAccountOnPageChange(mAccountId, mAccountName, mObjectName, mObjectId, mExpress, mExpressType, mAccountDate, mAccountDateStr, mAccountDateEnd, expressNames) {
        if (window.fnSetAccountMsg)
            window.fnSetAccountMsg(mAccountId, mAccountName, mObjectName, mObjectId, mExpress, mExpressType, mAccountDate, mAccountDateStr, mAccountDateEnd, expressNames);
    }

    function GetAccountPeriodSelect() {
        return $("#<%=HidAccountPeriodObjectID.ClientID %>").val();
    }
</script>

<div style="display: inline" class="openStation">
<table>
<tr>
    <td>
        <div id="divAccountPeriodHidden">
            <input id="txtAccountPeriodName" style="width: 180px; margin-right: 1px; border:1px solid #9999CC;" runat="server"
					type="text" class="text" onkeypress="fnShowAccountPeriod(this);return false;"/>
        
		    <asp:HiddenField ID="HidAccountPeriodID" runat="server" />
            <asp:HiddenField ID="HidAccountPeriodObjectID" runat="server" />
            <asp:HiddenField ID="HidAccountExpressID" runat="server" />
            <asp:HiddenField ID="HidExpressType" runat="server" />
            <asp:HiddenField ID="HidAccountDate" runat="server" />
            <asp:HiddenField ID="HidAccountDateStr" runat="server" />
            <asp:HiddenField ID="HidAccountDateEnd" runat="server" />
        </div>
    </td>
    <td>
        <img alt="选择账期" id="imgMain" src="../images/vie.gif" class="icon" style="cursor: pointer; padding-left: 1px;"
					title="选择账期" onclick="fnShowAccountPeriod(this)" runat="server" />
    </td>
</tr>
</table>
</div>

<script type="text/javascript">

    var periodLoadType = '<%=_periodLoadType %>';
    var periodDataSource = '<%=_periodDataSource %>';

    function fnShowAccountPeriod(E) {
        stationCommon.open(E, '../UserControl/AccountPeriodTreeView.aspx?periodLoadType=' + periodLoadType + '&periodDataSource=' + periodDataSource, 650, 400);
    }
</script>