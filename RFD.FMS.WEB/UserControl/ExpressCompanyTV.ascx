<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpressCompanyTV.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.ExpressCompanyTV" %>

<script language="javascript">
    function GetExpressImageID() {
        return $("#<%=imgMain.ClientID %>");
    }

    function GetUseExpressID() {
        return $("#<%=this.HidUserControlStationID.ClientID %>").val();
    }
</script>

<div style="display: inline" class="openStation">
<table>
<tr>
    <td>
        <input id="TxtUserControlSatationSpell" style="width: 180px; margin-right: 1px; border:1px solid #9999CC;" runat="server"
					type="text" class="text" onkeypress="fnShowExpressPage(this);return false;"/>
				<span id="spanMustInput" style="color:Red" runat="server"></span>
				<span id="SpanUserControlStationName"></span>
				<asp:HiddenField ID="HidUserControlStationID" runat="server" />
    </td>
    <td>
        <img alt="选择配送公司" id="imgMain" src="../images/vie.gif" class="icon" style="cursor: pointer; padding-left: 1px;"
					title="选择配送公司" onclick="fnShowExpressPage(this)" runat="server" />
    </td>
</tr>
</table>
</div>

<script type="text/javascript">

    var expressLoadType = '<%=_expressLoadType %>';
    var expressTypeSource = '<%=_expressTypeSource %>';
    var showCheckBox = '<%=_expressCompanyShowCheckBox %>';
    
    function fnShowExpressPage(E) {
        stationCommon.open(E, '../UserControl/ExpressCompanyTreeView.aspx?loadType=' + expressLoadType + '&typeSource=' + expressTypeSource + '&showCheckBox=' + showCheckBox, 400, 400);
    }
</script>