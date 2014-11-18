<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DistributionSelect.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.DistributionSelect" %>
<%@ Register TagPrefix="cc1" Namespace="UNLV.IAP.WebControls" Assembly="DropDownCheckList" %>

<div style="clear:both;">
    <div style="float:left;">
	    <cc1:DropDownCheckList ID="DistributionSelected" runat="server" 
	        DisplayTextCssStyle="width:180px;"
	        RepeatColumns="4"
	        TruncateString='<span style="font-style: italic; font-size: 8pt;">...</span>'
		    TextWhenNoneChecked="选择配送商" 
		    DisplayBoxCssStyle="border: 1px solid #9999CC; cursor: pointer; background-color: #FFFFFF;overflow:auto;"
		    DropImageSrc="../images/expand.gif" DataTextField="DistributionName" DataValueField="DistributionID" 
		    ClientCodeLocation="../Scripts/DropDownCheckList.js">
	    </cc1:DropDownCheckList>
    </div>
    <div style="float:left;">
	    <input type="checkbox" id="cityCheckAll"
	     onclick="$('#<%= DistributionSelected.ClientID %>').find('input:checkbox').attr('checked', this.checked)" />全选
    </div>
   <%-- <script type="text/javascript">
        window.onload = function () {
            alert("1");
            $("#cityCheckAll").attr('checked', true);
            $('#<%= CitySelect.ClientID %>').find('input:checkbox').attr('checked', true);
        }
    </script>--%>
</div>