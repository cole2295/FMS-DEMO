<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WareHouseCheckBox.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.WareHouseCheckBox" %>

<%@ Register TagPrefix="cc1" Namespace="UNLV.IAP.WebControls" Assembly="DropDownCheckList" %>

<div style="clear:both;">
	<div style="float:left;">
		<cc1:DropDownCheckList id="WareHouseCheckBoxs" runat="server"
			TextWhenNoneChecked="选择" 
			DisplayTextCssStyle="width:180px;"
			RepeatColumns="4"
			TruncateString='<span style="font-style: italic; font-size: 8pt;">...</span>'
			DisplayBoxCssStyle="border: 1px solid #9999CC; cursor: pointer; background-color: #FFFFFF;overflow:auto;"
			DropImageSrc="../images/expand.gif" DataTextField="WareHouseName" DataValueField="WareHouseID" 
			ClientCodeLocation="../Scripts/DropDownCheckList.js">
		</cc1:DropDownCheckList>
	</div>
	<div style="float:left;">
		<%--<input type="checkbox" id="checkAllWareHouse"
		 onclick="$('#<%= WareHouseCheckBoxs.ClientID %>').find('input:checkbox').attr('checked', this.checked)" />全选--%>
		 <asp:CheckBox ID="cbCheckAllWareHouse" runat="server" Text="全选" 
			oncheckedchanged="cbCheckAllWareHouse_CheckedChanged" AutoPostBack="true" />
	</div>
</div>
