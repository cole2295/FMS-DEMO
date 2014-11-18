<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoodsCategoryCheckBox.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.GoodsCategoryCheckBox" %>
<%@ Register TagPrefix="cc1" Namespace="UNLV.IAP.WebControls" Assembly="DropDownCheckList" %>

<div style="clear:both;">
<div style="float:left;">
	<cc1:DropDownCheckList id="GoodsCategoryCheckBoxs" runat="server" RepeatColumns="3" 
		TextWhenNoneChecked="请选择" DisplayBoxCssStyle="border: 1px solid #9999CC; cursor: pointer; background-color: #FFFFFF;overflow:auto;"
		DropImageSrc="../images/expand.gif" DataTextField="name" DataValueField="code" 
		ClientCodeLocation="../Scripts/DropDownCheckList.js">
	</cc1:DropDownCheckList>
</div>
<div style="float:left;">
	<input type="checkbox" id="checkAll"
	 onclick="$('#<%= GoodsCategoryCheckBoxs.ClientID %>').find('input:checkbox').attr('checked', this.checked)" />全选
	 <%--<asp:CheckBox ID="cbCheckAllGoodsCategory" runat="server" Text="全选" 
			oncheckedchanged="cbCheckAllExpressCompany_CheckedChanged" AutoPostBack="true" />--%>
</div>
</div>