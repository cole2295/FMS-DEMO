<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CitySelected.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.CitySelected" %>
<%@ Register TagPrefix="cc1" Namespace="UNLV.IAP.WebControls" Assembly="DropDownCheckList" %>

<div style="clear:both;">
    <div style="float:left;">
	    <cc1:DropDownCheckList ID="CitySelect" runat="server" 
	        DisplayTextCssStyle="width:180px;"
	        RepeatColumns="4"
	        TruncateString='<span style="font-style: italic; font-size: 8pt;">...</span>'
		    TextWhenNoneChecked="选择城市" 
		    DisplayBoxCssStyle="border: 1px solid #9999CC; cursor: pointer; background-color: #FFFFFF;overflow:auto;"
		    DropImageSrc="../images/expand.gif" DataTextField="CityName" DataValueField="CityID" 
		    ClientCodeLocation="../Scripts/DropDownCheckList.js">
	    </cc1:DropDownCheckList>
    </div>
    <div style="float:left;">
	    <input type="checkbox" id="cityCheckAll"
	     onclick="$('#<%= CitySelect.ClientID %>').find('input:checkbox').attr('checked', this.checked)" />全选
    </div>
   <%-- <script type="text/javascript">
        window.onload = function () {
            alert("1");
            $("#cityCheckAll").attr('checked', true);
            $('#<%= CitySelect.ClientID %>').find('input:checkbox').attr('checked', true);
        }
    </script>--%>
</div>

