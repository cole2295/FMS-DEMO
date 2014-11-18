<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MerchantSourceTV.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.MerchantSourceTV" %>

<script type="text/javascript">

    function GetUseMerchantID() {
        return $("#<%=this.HidUserControlStationID.ClientID %>").val();
    }
</script>

<script type="text/javascript">

    function RunOnPageChange(merchantId) {
        if(window.BindMerchantCategory)
            window.BindMerchantCategory(merchantId);
    }
</script>

<div style="display: inline" class="openStation">
<table>
<tr>
    <td>
        <input id="TxtUserControlSatationSpell" style="width: 180px; margin-right: 1px; border:1px solid #9999CC;" runat="server"
					type="text" class="text" onkeypress="fnShowMerchantPage(this);return false;"/>
				<span id="spanMustInput" style="color:Red" runat="server"></span>
				<span id="SpanUserControlStationName"></span>
				<asp:HiddenField ID="HidUserControlStationID" runat="server" />
    </td>
    <td>
        <img alt="选择商家" id="imgMain" src="../images/vie.gif" class="icon" style="cursor: pointer; padding-left: 1px;"
					title="选择商家" onclick="fnShowMerchantPage(this)" runat="server" />
    </td>
</tr>
</table>
</div>

<script type="text/javascript">

    var merchantLoadType = '<%=_merchantLoadType %>';
    var merchantTypeSource = '<%=_merchantTypeSource %>';
    var showCheckBox = '<%=_merchantShowCheckBox %>';

    function fnShowMerchantPage(E) {
        var url = '../UserControl/MerchantTreeView.aspx?loadType='  + merchantLoadType
                                                     + '&typeSource=' + merchantTypeSource
                                                     + '&showCheckBox=' + showCheckBox
        stationCommon.open(E, url, 400, 400);
    }
</script>