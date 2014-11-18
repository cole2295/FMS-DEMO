<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MerchantSource.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.MerchantSource" %>
<%@ Register TagPrefix="cc1" Namespace="UNLV.IAP.WebControls" Assembly="DropDownCheckList" %>

<div style="clear:both;">
    <div style="float:left;">
	    <cc1:DropDownCheckList id="MerchantSources" runat="server" 
	        DisplayTextCssStyle="width:180px;"
	        RepeatColumns="4"
	        TruncateString='<span style="font-style: italic; font-size: 8pt;">...</span>'
		    TextWhenNoneChecked="选择商家" 
		    DisplayBoxCssStyle="border: 1px solid #9999CC; cursor: pointer; background-color: #FFFFFF;overflow:auto;"
		    DropImageSrc="../images/expand.gif" DataTextField="MerchantName" DataValueField="ID" 
		    ClientCodeLocation="../Scripts/DropDownCheckList.js">
	    </cc1:DropDownCheckList>
    </div>
    <div style="float:left;">
	    <input type="checkbox" id="merchantCheckAll"
	     onclick="$('#<%= MerchantSources.ClientID %>').find('input:checkbox').attr('checked', this.checked)" />全选
    </div>
   <%-- <script type="text/javascript">
        window.onload = function () {
            $("#merchantCheckAll").attr('checked', true);
            $('#<%= MerchantSources.ClientID %>').find('input:checkbox').attr('checked', true);
        }
    </script>--%>
</div>