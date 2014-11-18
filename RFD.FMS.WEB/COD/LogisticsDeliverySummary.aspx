<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogisticsDeliverySummary.aspx.cs" Inherits="RFD.FMS.WEB.COD.LogisticsDeliverySummary" MasterPageFile="~/Main/main.Master" %>

<%@ Register Src="~/UserControl/ExpressCompanyTV.ascx" TagName="UCExpressCompanyTV" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/StationCheckBox.ascx" TagName="UCStationCheckBox" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/WareHouseCheckBox.ascx" TagName="UCWareHouseCheckBox" TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc4" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc5" %>

<asp:Content ContentPlaceHolderID="head" ID="Content1" runat="server">
<style type="text/css">
    .style5
    {
        width: 96px;
    }
</style>
<link href="../css/superTables_compressed.css" rel="stylesheet" type="text/css" />
<script src="../Scripts/superTables_compressed.js" type="text/javascript"></script>
<script src="../Scripts/plugins/table/jquery.supertable.js" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function() {
		$("#<%=gvList.ClientID %>").toSuperTable(
            { width: "1120px", height: "300px", fixedCols: 0,
                onFinish: function() {
                }
            });
        });
        $(function () {
            var buttons = {
                btSearch: $.dom("btSearch", "input"),
                btExprot: $.dom("btExprot", "input")
            };
            buttons.btExprot.click(function () {
                $(document).progressDialog.showDialog("导出中，请稍后...");
            });
            buttons.btSearch.click(function () {
                $(document).progressDialog.showDialog("查询中，请稍后...");
            });
        });

        $(document).ready(function () {
            $('#form1').submit(function () {
                blockUIForDownload($('#<% =download_token_value.ClientID%>'), $('#<% =download_token_name.ClientID%>'));
            });
        });
        var fileDownloadCheckTimer;
        function blockUIForDownload(value, name) {
            var token = new Date().getTime(); //use the current timestamp as the token value
            value.val(token);
            name.val(token + "token");
            fileDownloadCheckTimer = window.setInterval(function () {
                var cookieValue = $.cookie(name.val());
                if (cookieValue == token)
                    finishDownload(name);
            }, 1000);
        }

        function finishDownload(name) {
            window.clearInterval(fileDownloadCheckTimer);
            $.cookie(name.val(), null);
            $(document).progressDialog.hideDialog();
            alert("导出查询完成，点击保存");
        }  
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
发货汇总表
</asp:Content>
<asp:Content ContentPlaceHolderID="body" ID="Content2" runat="server">
    <asp:HiddenField ID="download_token_name" runat="server" /> 
    <asp:HiddenField ID="download_token_value" runat="server" />
    	<table border="0">
		<tr>
			<td>第三方配送商</td>
			<td><uc1:UCExpressCompanyTV ID="UCExpressCompanyTV" runat="server" ExpressLoadType="ThirdCompany" ExpressTypeSource="nk_Express" ExpressCompanyShowCheckBox="True" /></td>
			<td>报表类型</td>
			<td>
				<asp:DropDownList ID="ddlReportType" runat="server" AutoPostBack="true"
					onselectedindexchanged="ddlReportType_SelectedIndexChanged">
					<asp:ListItem Value="">发货明细表</asp:ListItem>
					<asp:ListItem Value="7">拒收明细表</asp:ListItem>
					<asp:ListItem Value="6">上门退换货明细表</asp:ListItem>
				</asp:DropDownList>
			</td>
			<td></td>
			<td>
				<asp:DropDownList ID="ddlShipmentType" runat="server" Visible="false">
					<asp:ListItem Value="">全部</asp:ListItem>
					<asp:ListItem Value="0">普通发货</asp:ListItem>
					<asp:ListItem Value="1">上门换发货</asp:ListItem>
                    <asp:ListItem Value="3">签单返回发货</asp:ListItem>
				</asp:DropDownList>
			</td>
			<td>分配区域</td>
			<td colspan="2">
				<asp:DropDownList ID="ddlAreaType" runat="server">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td>
				<asp:DropDownList ID="ddlRFDType" runat="server" AutoPostBack="true"
					onselectedindexchanged="ddlRFDType_SelectedIndexChanged">
					<asp:ListItem Value="0">站点</asp:ListItem>
					<asp:ListItem Value="1">分拣</asp:ListItem>
				</asp:DropDownList>
			</td>
			<td><uc2:UCStationCheckBox ID="UCStationRFDSite" runat="server" LoadDataType="RFDSite" /></td>
			<td>
				<asp:DropDownList ID="ddlDateType" runat="server" AutoPostBack="true" 
					onselectedindexchanged="ddlDateType_SelectedIndexChanged">
                    <asp:ListItem Value="1">最终发日期</asp:ListItem>
					<asp:ListItem Value="0">初始发日期</asp:ListItem>
				</asp:DropDownList>
			</td>
			<td>
				<asp:TextBox ID="txtBTime" Width="128px" Visible="true" 
					onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})" runat="server"></asp:TextBox>
			</td>
			<td>~</td>
			<td>
				 <asp:TextBox ID="txtETime" Width="128px" Visible="true" 
					onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})" runat="server"></asp:TextBox>
			</td>
			<td>运单号</td>
			<td colspan="2"><asp:TextBox ID="txtWaybillNO" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td>商家</td>
			<td><uc4:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" MerchantLoadType="All" MerchantTypeSource="nk_Merchant_Expense" MerchantShowCheckBox="True" /></td>
			<td>
				<asp:Label ID="lbHouseType" runat="server">最终发货仓</asp:Label>
				<asp:HiddenField ID="hfHouseType" runat="server" Value="1" />
			</td>
			<td colspan="3">
				<uc3:UCWareHouseCheckBox ID="UCWareHouseCheckBox" runat="server" HouseCheckType="all" />
			</td>
			<td></td>
            <td></td>
		</tr>
        <tr>
            <td></td>
            <td></td>
            <td></td>
            <td><asp:CheckBox ID="cbIsAreaType" runat="server" Text="区域汇总" /></td>
            <td></td>
             <td>
                <asp:CheckBox ID="cbIsCod" runat="server" Text="业务汇总" />
            </td>
            <td colspan="2">
				<asp:Button ID="btSearch" runat="server" Text="查询" AccessKey="Q" 
					onclick="btSearch_Click" />
				<asp:Button ID="btSearchV2" runat="server" Text="查询V2" AccessKey="Q" 
					onclick="btSearch_ClickV2" Visible="false" />
				<asp:Button ID="btExprot" runat="server" Text="导出" AccessKey="O" 
					onclick="btExprot_Click" />
				<asp:Button ID="btExprotV2" runat="server" Text="导出V2" AccessKey="O" 
					onclick="btExprot_ClickV2" Visible="false" />
			</td>
        </tr>
	</table>
    <div style="color:Red;">注意：多选的需要全部查询，则不用选择即表示为全部包含查询。如：商家需要全部包含查询，则不用选择商家即为全部查询。</div>
	<table style="font-weight:bold; font-size:13px;">
		<tr>
			<td>订单量合计：</td>
			<td><asp:Label ID="lbWaybillStat" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;">总价合计：</td>
			<td><asp:Label ID="lbTotalAmount" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;"><asp:Label ID="lbPaidName" runat="server">已收款合计</asp:Label>：</td>
			<td><asp:Label ID="lbPaidStat" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;"><asp:Label ID="lbNeedName" runat="server">应收订单量合计</asp:Label>：</td>
			<td><asp:Label ID="lbNeedStat" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;"><asp:Label ID="lbNeedPayName" runat="server">应收款合计</asp:Label>：</td>
			<td><asp:Label ID="lbNeedPayStat" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;">结算重量合计：</td>
			<td><asp:Label ID="lbWeightStat" runat="server">0.000</asp:Label></td>
			<td style="padding-left:10px;">配送费合计：</td>
			<td><asp:Label ID="lbFareStat" runat="server">0.000</asp:Label></td>
			<td style="padding-left:10px;">保价金额合计：</td>
			<td><asp:Label ID="lbProtectedPriceStat" runat="server">0.000</asp:Label></td>
		</tr>
	</table>
	<div style="text-align:center"><asp:Label ID="noData" runat="server" ForeColor="Red" Visible="false"></asp:Label></div>
	<asp:GridView ID="gvList" runat="server">
		<HeaderStyle CssClass="noWrap" Wrap="false"></HeaderStyle>
		<RowStyle CssClass="noWrap" Wrap="false"></RowStyle>
	</asp:GridView>
    <div>
		<uc5:UCPager ID="UCPager" runat="server" PageSize="50" />
	</div>
</asp:Content>
