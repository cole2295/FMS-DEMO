<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StationCheckBox.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.StationCheckBox" %>
<%@ Register TagPrefix="cc1" Namespace="UNLV.IAP.WebControls" Assembly="DropDownCheckList" %>

<script language="javascript">
    function GetStationCheckBoxSelectID() {
        return "<%=SelectExpressCompany %>"
    }

    function SetStationCheckBoxEnalbe(v) {
        if (v) {
            $("#divStationCheckBox").show();
        }
        else {
            $("#divStationCheckBox").hide();
        }
    }
</script>

<div id="divStationCheckBox" style="clear:both;">
<div style="float:left;">
	<cc1:DropDownCheckList id="ExpressCompanyCheckBox" runat="server" RepeatColumns="3" 
		TextWhenNoneChecked="请选择" DisplayBoxCssStyle="border: 1px solid #9999CC; cursor: pointer; background-color: #FFFFFF;overflow:auto;"
		DropImageSrc="../images/expand.gif" DataTextField="CompanyName" DataValueField="ExpressCompanyID" 
		ClientCodeLocation="../Scripts/DropDownCheckList.js">
	</cc1:DropDownCheckList>
</div>
<div style="float:left;">
	<%--<input type="checkbox" id="checkAll"
	 onclick="$('#<%= ExpressCompanyCheckBox.ClientID %>').find('input:checkbox').attr('checked', this.checked)" />全选--%>
	 <asp:CheckBox ID="cbCheckAllExpressCompany" runat="server" Text="全选" 
			oncheckedchanged="cbCheckAllExpressCompany_CheckedChanged" AutoPostBack="true" />
</div>
</div>