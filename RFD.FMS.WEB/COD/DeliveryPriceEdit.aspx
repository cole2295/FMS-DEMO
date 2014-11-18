<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryPriceEdit.aspx.cs" Inherits="RFD.FMS.WEB.COD.DeliveryPriceEdit" Theme="default" %>

<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="UCSelectStation" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>配送价格编辑</title>
    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/controls/selectStationCommon.js"></script>
    <script src="../Scripts/controls/OpenCommon.js" type="text/javascript"></script>
    <script type="text/javascript">
    	window.name = "winDeliveryPriceEdit";
    	$(function() {
    		$(".Wdate").bind("focus", function() {
    			WdatePicker({ skin: 'whyGreen', dateFmt: 'yyyy-MM-dd' });
    		});
    	})
    	function judgeInput() {
    		return true;
    	}
    </script>
</head>
<body>
	<form id="form1" runat="server" target="winDeliveryPriceEdit">
	<script type="text/javascript">
		function fnPriceClick(url,txtId) {
    		var p = window.showModalDialog(url, window,'dialogWidth=500px;dialogHeight=300px;center:yes;resizable:no;scroll:auto;help=no;status=no;');
    		if (p != 'undefined' && p != null) {
    		    $("#" + txtId + "").val(p);
    		}
    	}
    </script>
    
    <div style="width:400px; height:400px; text-align:center; margin:30 10 10 10; padding-top:30px;">
		<asp:Label ID="Msg" runat="server"></asp:Label>
		<table style="line-height:25px; padding-top:20px;">
			<tr>
				<td align="right"><asp:Label ID="lbucSelectStation" runat="server">配送公司：</asp:Label></td>
				<td align="left"><uc1:UCSelectStation runat="server" ID="ucSelectStation" LoadDataType="OnlySite" /></td>
			</tr>
			
<%--			<tr>
				<td>是否按发货地梯次收费：</td>
				<td align="left">
					<asp:RadioButton GroupName="IsEchelon" ID="IsEchelonTrue" runat="server" Checked="true"
						Text="是" oncheckedchanged="IsEchelonTrue_CheckedChanged" AutoPostBack="true" />
					<asp:RadioButton GroupName="IsEchelon" ID="IsEchelonFalse" runat="server" 
						Text="否" oncheckedchanged="IsEchelonFalse_CheckedChanged" AutoPostBack="true" />
				</td>
			</tr>
			<tr>
				<td align="right">发货仓库：</td>
				<td align="left">
					<asp:DropDownList ID="wareHouse" runat="server">
					</asp:DropDownList>
				</td>
			</tr>--%>
			<tr>
				<td align="right"><asp:Label ID="lbddlMerchant" runat="server">商家：</asp:Label></td>
				<td align="left">
					<uc2:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" MerchantLoadType="All" MerchantShowCheckBox="False" MerchantTypeSource="jc_Merchant_Expense" />
				</td>
			</tr>
			<tr>
				<td align="right"><asp:Label ID="lbAreaType" runat="server">区域类型：</asp:Label></td>
				<td align="left">
					<asp:DropDownList ID="AreaType" runat="server">
			        </asp:DropDownList>
				</td>
			</tr>
            <tr>
                <td><asp:Label ID="lbIsCod" runat="server">是否分COD区：</asp:Label> </td>
                <td align="left">
                    <asp:RadioButton ID="rbIsCodTrue" runat="server" Checked="true" AutoPostBack="true" Text="是" 
                        GroupName="rbIsCod" oncheckedchanged="rbIsCodTrue_CheckedChanged" />
                    <asp:RadioButton ID="rbIsCodFalse" runat="server" Text="否" AutoPostBack="true" GroupName="rbIsCod" 
                        oncheckedchanged="rbIsCodFalse_CheckedChanged" />
                    <asp:DropDownList ID="ddlBatchType" runat="server" Visible="false" AutoPostBack="true" 
                        onselectedindexchanged="ddlBatchType_SelectedIndexChanged">
                        <asp:ListItem Value="0">全部</asp:ListItem>
                        <asp:ListItem Value="1">COD</asp:ListItem>
                        <asp:ListItem Value="2">非COD</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
			<tr>
				<td align="right"><asp:Label ID="lbCodPrice" runat="server"> COD价格：</asp:Label></td>
				<td align="left"><asp:TextBox ID="txtCodPrice" runat="server" ReadOnly="true" ToolTip="点击编辑" Width="260px"></asp:TextBox></td>
			</tr>
            <tr>
				<td align="right"><asp:Label ID="lbPrice" runat="server"> 非COD价格：</asp:Label></td>
				<td align="left"><asp:TextBox ID="txtPrice" runat="server" ReadOnly="true" ToolTip="点击编辑" Width="260px"></asp:TextBox></td>
			</tr>
			<tr>
				<td align="right"><asp:Label ID="lbLineStatus" runat="server">线路状态：</asp:Label></td>
				<td align="left">
                    <asp:DropDownList ID="LineStatus" runat="server">
			        </asp:DropDownList>
                </td>
			</tr>
			<tr>
				<td align="right"><asp:Label ID="labEffectDate" runat="server" Visible="false">生效时间：</asp:Label></td>
				<td><asp:TextBox ID="txtEffectDate" runat="server" ReadOnly="true" Visible="false" CssClass="Wdate text" autocomplete="off"></asp:TextBox></td>
			</tr>
		</table>
		<div style="text-align:right; padding:10 130 10 10; width:300px;">
			<asp:Button ID="btOK" runat="server" Text="确定" onclick="btOK_Click" OnClientClick="return judgeInput();" />
		</div>
    </div>

    </form>
</body>
</html>
