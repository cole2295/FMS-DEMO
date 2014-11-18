<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogisticsDeliveryDaily.aspx.cs" Inherits="RFD.FMS.WEB.COD.LogisticsDeliveryDaily" MasterPageFile="~/Main/main.Master" %>

<%@ Register Src="~/UserControl/ExpressCompanyTV.ascx" TagName="UCExpressCompanyTV" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/WareHouseCheckBox.ascx" TagName="UCWareHouseCheckBox" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/StationCheckBox.ascx" TagName="UCStationCheckBox" TagPrefix="uc4" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc4" %>



<asp:Content ContentPlaceHolderID="head" ID="Content1" runat="server">
    <style type="text/css">
    .style5
    {
        width: 96px;
    }
</style>
<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=gvList.ClientID %>").toSuperTable(
                      { width: "1120px", height: "330px", fixedCols: 0,
                          onFinish: function () {
                          }
                      });
                  });
    $(function () {
        var buttons = {
                          btSearch: $.dom("btSearch", "input"),
                          btExprot: $.dom("btExprot", "input")
                      };
        buttons.btSearch.click(function() {
           $(document).progressDialog.showDialog("查询中，请稍后...");
        });
        buttons.btExprot.click(function() {
           $(document).progressDialog.showDialog("导出中，请稍后...");
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

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    配送费发货日报
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
    <asp:HiddenField ID="download_token_name" runat="server" /> 
    <asp:HiddenField ID="download_token_value" runat="server" />
    <table style="line-height:25px; margin:5px;" border="0">
		<tr>
			<td align="right">配送商：</td>
			<td>
                <uc1:UCExpressCompanyTV ID="UCExpressCompanyTV" runat="server" ExpressLoadType="ThirdCompany" ExpressTypeSource="nk_Express" ExpressCompanyShowCheckBox="True" />
            </td>
            <td align="right">仓库/分拣：</td>
			<td><uc2:UCWareHouseCheckBox id="ucDeliveryHouse" LoadDataType="All" HouseCheckType="all" runat="server"></uc2:UCWareHouseCheckBox></td>
			<td align="right">时间范围：</td>
			<td>
            <asp:TextBox ID="tbDate_D_S" Width="128px" Visible="true" 
					onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})" runat="server"></asp:TextBox>
            </td>
			<td>-</td>
			<td><asp:TextBox ID="tbDate_D_E" Width="128px" Visible="true" 
					onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})" runat="server"></asp:TextBox>
            </td>
		</tr>
		<tr>
			<td align="right">站点：</td>
			<td>
				<uc4:UCStationCheckBox ID="UCStationCheckBoxSite" runat="server" LoadDataType="RFDSite" />
			</td>
            <td align="right">商家：</td>
			<td>
				<uc3:UCMerchantSourceTV runat="server" ID="UCMerchantSourceTV" MerchantLoadType="All" MerchantTypeSource="nk_Merchant_Expense" MerchantShowCheckBox="True" />
			</td>
            <td colspan="2">
                <asp:CheckBox ID="IsAreaType" runat="server" Text="区域汇总" />
                <asp:CheckBox ID="cbIsCod" runat="server" Text="业务汇总" />
            </td>
            <td colspan="2" align="right">
                <asp:Button ID="btSearch" runat="server" Text="查询" onclick="btSearch_Click" />
				<asp:Button ID="btExprot" runat="server" Text="导出" onclick="btExprot_Click" />
                <asp:Button ID="btSearchV2" runat="server" Text="查询V2" onclick="btSearch_ClickV2" Visible="false" />
                <asp:Button ID="btExprotV2" runat="server" Text="导出V2" onclick="btExprot_ClickV2" Visible="false" />
            </td>
		</tr>
    </table>
    <div style="color:Red;">注意：多选的需要全部查询，则不用选择即表示为全部包含查询。如：商家需要全部包含查询，则不用选择商家即为全部查询。</div>
    <table style="font-weight:bold; font-size:13px;">
		<tr>
			<td>普通发货数：</td>
			<td><asp:Label ID="lb_D_Sum" runat="server">0</asp:Label></td>
			<td style="padding-left:10px;">普通拒收数：</td>
			<td><asp:Label ID="lb_D_R_Sum" runat="server">0</asp:Label></td>
			<td style="padding-left:10px;">上门换发数：</td>
			<td><asp:Label ID="lb_DV_Sum" runat="server">0</asp:Label></td>
			<td style="padding-left:10px;">上门换拒数：</td>
			<td><asp:Label ID="lb_DV_R_Sum" runat="server">0</asp:Label></td>
			<td style="padding-left:10px;">上门退单数：</td>
			<td><asp:Label ID="lb_V_Sum" runat="server">0</asp:Label></td>
            <td style="padding-left:10px;">签单返回发数：</td>
			<td><asp:Label ID="lb_SR_D_Sum" runat="server">0</asp:Label></td>
            <td style="padding-left:10px;">签单返回据数：</td>
			<td><asp:Label ID="lb_SR_R_Sum" runat="server">0</asp:Label></td>
			<td style="padding-left:10px;">实际支付订单量：</td>
			<td><asp:Label ID="lb_PaySum" runat="server">0</asp:Label></td>
			<td style="padding-left:10px;">实际支付配送费：</td>
			<td><asp:Label ID="lb_PayFee" runat="server">0.00</asp:Label></td>
		</tr>
        <tr>
           <td colspan="18">
               <asp:Label ID="TimeLabel" runat="server"></asp:Label>
           </td>
        </tr>
	</table>
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="true">
		<HeaderStyle CssClass="noWrap" Wrap="false"></HeaderStyle>
		<RowStyle CssClass="noWrap" Wrap="false"></RowStyle>
	</asp:GridView>
    <uc4:UCPager ID="pager" runat="server" />
</asp:Content>