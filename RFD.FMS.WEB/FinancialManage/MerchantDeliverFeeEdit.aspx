<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MerchantDeliverFeeEdit.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.MerchantDeliverFeeEdit" %>


<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="UCSelectStation" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc2" %>
<%@ Register TagPrefix="uc6" TagName="ucexpresscompanytv" Src="~/UserControl/ExpressCompanyChooseTV.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>配送价格编辑</title>

    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
    <script type="text/javascript" src="../Scripts/controls/selectStationCommon.js"></script>
    <script src="../Scripts/controls/OpenCommon.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.boxy.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.tmpl.js" type="text/javascript"></script>
     <script type="text/javascript" src="~/Scripts/jquery-1.4.1.js"></script> 
    <link href="../css/boxy.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        window.name = "winMerchantPriceEdit";
        function fnPriceClick(url, txtId) {
            var p = window.showModalDialog(url, window, 'dialogWidth=500px;dialogHeight=300px;center:yes;resizable:no;scroll:auto;help=no;status=no;');
            if (p != 'undefined' && p != null) {
                $("#" + txtId + "").val(p);
            }
        }

        $(document).ready(function () {
            fnJudgeIsCod();
            var merchantId = "<%=MerchantID %>";
            if (merchantId > 0) {
                BindMerchantCategory(merchantId);
            }
            JudgeIsExpress();
        });

        function BindMerchantCategory(merchantId) {
            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: "MerchantDeliverFeeEdit.aspx/BindMerchantCategory",
                data: "{merchantId:'" + merchantId + "',disCode:'" + "<%=DisCode %>" + "'}",
                dataType: 'json',
                success: function (result) {
                    if (result.d == null) { dailog = Boxy.alert("服务器无响应", function () { FormCodeHelper.Focus(); }, null); }
                    if (result.d.done) {
                        var obj = result.d.dataModel;
                        var arrH = new Array();
                        $("#<%=IsCategory.ClientID %>").val(1);
                        var checkedList = "," + $("#<%=MerchantCategoryChecked.ClientID %>").val();
                        var isUpdate = $("#<%=IsUpdate.ClientID %>").val();
                        //alert(checkedList);
                        for (var i = 0; i < obj.length; i++) {
                            arrH[arrH.length] = "<input id='" + obj[i].CategoryCode;
                            arrH[arrH.length] = "' type='checkbox' onclick='fnCheckCategory()' value='" + obj[i].CategoryCode + "'";
                            if (checkedList.indexOf("," + obj[i].CategoryCode + ",") >= 0)
                                arrH[arrH.length] = "checked='checked' ";
                            if (isUpdate == 1)
                                arrH[arrH.length] = "disabled='disabled' ";
                            arrH[arrH.length] = "/>";
                            arrH[arrH.length] = obj[i].CategoryName;
                        }
                        $("#MerchantCategory").html(arrH.join(''));
                        SetEnableControl(false, isUpdate);
                    }
                    else {
                        var obj = result.d.dataModel;
                        var code = obj.CategoryCode;
                        var name = obj.CategoryName;
                        $("#MerchantCategory").html("<span style='color:red;'>" + name + "</span>");
                        if (code == "E001") {
                            SetEnableControl(true,0);
                        }
                        else {
                            SetEnableControl(false, $("#<%=IsUpdate.ClientID %>").val());
                        }
                    }
                }
            });
        }

        function SetEnableControl(isEnable,isUpdate) {
            var ddlAreaType = $("#<%=ddlAreaType.ClientID %>");
            var ddlSortCenter =$("#<%=ddlSortCenter.ClientID %>");
            var tbFee = $("#<%=tbFee.ClientID %>");
            var rbYes = $("#<%=rbYes.ClientID %>");
            var rbNo = $("#<%=rbNo.ClientID %>");
            var tbDeliverFee = $("#<%=tbDeliverFee.ClientID %>");
            if (isEnable) {

                ddlSortCenter.attr("disabled", "disabled");
                tbFee.attr("disabled", "disabled");
                rbYes.attr("disabled", "disabled");
                rbNo.attr("disabled", "disabled");
                tbDeliverFee.attr("disabled", "disabled");
                
            }
            else {
                if (isUpdate != 1) {
                    ddlAreaType.removeAttr("disabled");
                    ddlSortCenter.removeAttr("disabled");
                }
                tbFee.removeAttr("disabled");
                rbYes.removeAttr("disabled");
                rbNo.removeAttr("disabled");
                tbDeliverFee.removeAttr("disabled");
                
            }
        }
        function fnCheckCategory() {
            var hiddenField = $("#<%=MerchantCategoryChecked.ClientID %>");
            var arrH = new Array();
            var checkList = $("#MerchantCategory").find('input:checkbox[checked]').each(function () {
                arrH[arrH.length] = this.id+",";
            });
            hiddenField.val(arrH.join(''));
        }

        $(function () {
            fnJudgeIsCod();
        });

        function fnJudgeIsCod() {
            var rbYes = $("#<%=rbYes.ClientID %>");
            var rbNo = $("#<%=rbNo.ClientID %>");
            var lbCodFee = $("#<%=lbCodFee.ClientID %>");
            var tbDeliverFee = $("#<%=tbDeliverFee.ClientID %>");
            var lbFee = $("#<%=lbFee.ClientID %>");
            rbYes.click(function () {
                if (rbYes.attr("checked") == "checked") {
                    tbDeliverFee.show();
                    lbCodFee.html("COD价格公式");
                    lbFee.show();
                }
            });
            rbNo.click(function () {
                if (rbNo.attr("checked") == "checked") {
                    tbDeliverFee.hide();
                    lbCodFee.html("价格公式");
                    lbFee.hide();
                }
            });
        }
        function JudgeIsExpress() {

            $("#cbCheckbox").change(
                function () 
                {
                    if (!$("#cbCheckbox").attr("checked"))
                    {
                     $("#UCExpressCompanyTV").Editable = false;
                    }
                    else if ($("#cbCheckbox").attr("checked"))
                   {
                    $("#UCExpressCompanyTV").Editable = true;
                   }
            });
        }
    </script>
    
   
       
</head>
<body>
	<form id="form1" runat="server" target="winMerchantPriceEdit">
	<div style="width:400px; height:400px; text-align:center; margin:30 10 10 10; padding-top:30px;">
	<asp:Label ID="lbMsg" runat="server" Font-Bold="true"></asp:Label>
	<table style="line-height:25px; padding-top:20px;">
		<tr>
			<td align="right" width="90px;">商家</td>
			<td align="left">
                <uc2:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" MerchantLoadType="All" MerchantShowCheckBox="False" MerchantTypeSource="jc_Merchant_Income" />
                <div id="MerchantCategory"></div>
                <asp:HiddenField ID="MerchantCategoryChecked" runat="server" />
                <asp:HiddenField ID="IsCategory" runat="server" Value="0" />
                <asp:HiddenField ID="IsUpdate" runat="server" Value="0" />
                <asp:Button ID="test" runat="server" onclick="test_Click" Visible="false" Text="测试" />
			</td>
		</tr>
		<tr>
			
			<td align="right" colspan="2">
            <uc6:ucexpresscompanytv ID="UCExpressCompanyTV" runat="server" 
                 ExpressLoadType="ThirdCompany" ExpressTypeSource="nk_Express" 
                 ExpressCompanyShowCheckBox="False"    />
            </td>
		</tr>
		<tr>
			<td align="right">区域类型</td>
			<td align="left">
				<asp:DropDownList ID="ddlAreaType" runat="server">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right">分拣中心</td>
			<td align="left">
				<asp:DropDownList ID="ddlSortCenter" runat="server">
				</asp:DropDownList>
			</td>
		</tr>
        <tr>
            <td align="right"><asp:Label ID="lbIsCod" runat="server">是否区分COD</asp:Label></td>
            <td align="left">
                <asp:RadioButton ID="rbYes" runat="server" Text="是" GroupName="rbIsCOD" />
                <asp:RadioButton ID="rbNo" runat="server" Text="否" GroupName="rbIsCOD" Checked="true" />
            </td>
        </tr>
		<tr>
			<td align="right"><asp:Label ID="lbCodFee" runat="server">价格公式</asp:Label></td>
			<td align="left"><asp:TextBox ID="tbFee" runat="server" ReadOnly="true" ToolTip="点击编辑" Width="260px"></asp:TextBox></td>
		</tr>
        <tr>
			<td align="right"><asp:Label ID="lbFee" runat="server">非COD价格公式</asp:Label></td>
			<td align="left"><asp:TextBox ID="tbDeliverFee" runat="server" ReadOnly="true" ToolTip="点击编辑" Width="260px"></asp:TextBox></td>
		</tr>
		<tr>
			<td colspan="2" align="right">
                <asp:Label ID="lbEffectDate" runat="server">生效时间</asp:Label>
                <asp:TextBox ID="txtEffectDate" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"></asp:TextBox>
				<asp:Button ID="btnOK" runat="server" Text="确定" onclick="btnOK_Click" />
			</td>
		</tr>
	</table>
	</div>
    </form>
</body>
</html>