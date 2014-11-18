<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CODAccountEdit.aspx.cs" Inherits="RFD.FMS.WEB.COD.CODAccountEdit" Theme="default" MasterPageFile="~/Main/main.Master" %>

<%@ Register Src="~/UserControl/ExpressCompanyTV.ascx" TagName="UCExpressCompanyTV" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/WareHouseCheckBox.ascx" TagName="UCWareHouseCheckBox" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV" TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="body" runat="server">
    <script type="text/javascript" src="../Scripts/controls/selectStationCommon.js"></script>
    <script type="text/javascript">
        var isShowProgressDialog = true;
        $(function () {
            var buttons = {
                SaveAccount: $.dom("SaveAccount", "input"),
                btSearch: $.dom("btSearch", "input")
            };
            buttons.SaveAccount.click(function () {
                if(isShowProgressDialog)
                    $(document).progressDialog.showDialog("保存中，请稍后...");
            });
            buttons.btSearch.click(function () {
                if(isShowProgressDialog)
                    $(document).progressDialog.showDialog("查询中，请稍后...");
            });
        });
        
    	function fnShowLayer(url) {
    		var k = window.showModalDialog(url, window, 'dialogWidth=330px;dialogHeight=420px;center:yes;resizable:no;scroll:no;help=no;status=no;');
    		if (k == 'refreshParent') {
    			document.getElementById("reload").click();
    		}
    	}

    	function fnViewDetail() {
    		fnOpenNewPage('AccountDetail', 'COD结算明细', '../COD/CODDetailView.aspx?accountNo=<%= AccountNO %>', true);
    	}

    	function fnAreaFareMsg() {
    		fnOpenNewPage('AccountAreaFareMenu', 'COD区域运费汇总表', '../COD/CODAreaFareSummary.aspx?accountNo=<%= AccountNO %>', true);
          }
        function ClientConfirm(msg) {
              return !msg ? false : window.confirm(msg);
          }

          function JudgeInputSearch() {
              var expressId = GetUseExpressID();
              if (expressId == "" && $("#cbRfd").attr("checked") != "checked") {
                  alert("配送公司必选");
                  isShowProgressDialog = false;
                  return false;
              }
              var dateStr = $("#<%=tbDate_D_S.ClientID %>").val();
              var dateEnd = $("#<%=tbDate_D_E.ClientID %>").val();
              if (!fnJudgeDate(dateStr, dateEnd, "发货")) { isShowProgressDialog = false; return false; }

              dateStr = $("#<%=tbDate_R_S.ClientID %>").val();
              dateEnd = $("#<%=tbDate_R_E.ClientID %>").val();
              if (!fnJudgeDate(dateStr, dateEnd, "拒收")) { isShowProgressDialog = false; return false; }

              dateStr = $("#<%=tbDate_V_S.ClientID %>").val();
              dateEnd = $("#<%=tbDate_V_E.ClientID %>").val();
              if (!fnJudgeDate(dateStr, dateEnd, "上门退")) { isShowProgressDialog = false; return false; }

              var merchantId = GetUseMerchantID();
              if (merchantId == "") {
                  alert("商家必选");
                  isShowProgressDialog = false;
                  return false;
              }
              var merchantIdStr = "," + merchantId + ",";
              if (merchantIdStr.indexOf(',8,') > -1 || merchantIdStr.indexOf(',9,') > -1) {
                  var merchantIds = merchantId.split(',');
                  if (merchantIds.length > 1) {
                      alert("vancl、vjia需要单独创建");
                      isShowProgressDialog = false;
                      return false;
                  }
              }
              isShowProgressDialog = true;
              return true;
          }

          function fnJudgeDate(dateStr, dateEnd, name) {
              if (dateStr == "" || dateEnd == "") {
                  alert(name + "查询日期不能为空");
                  return false;
              }

              var dtsTime = new Date(Date.parse(dateStr.replace(/-/g, "/")));
              var dteTime = new Date(Date.parse(dateEnd.replace(/-/g, "/")));
              var cdt = (dteTime.getTime() - dtsTime.getTime()) / (24 * 60 * 60 * 1000); //相差天数
              if (cdt < 0) {
                  alert(name + "开始日期不能大于结束日期");
                  return false;
              }

              if (cdt > 31) {
                  alert(name + "日期范围不能大于31天");
                  return false;
              }

              return true;
          }

          function fnCheckRfd(){
              var isRfd = $("#cbRfd");
              var expressId = GetExpressImageID();
              var hid = $("#<%=hidRfdChecked.ClientID %>");
              if (isRfd.attr("checked") == "checked") {
                  hid.val(isRfd.val());
                  expressId.attr("disabled", "disabled");
                  expressId.attr("title","如风达不选，即可选择其他配送公司");
              }
              else {
                  hid.val("");
                  expressId.removeAttr("disabled");
                  expressId.attr("title", "选择配送公司");
              }
          };

          $(document).ready(function () {
              if ($("#<%=hidRfdChecked.ClientID %>").val() != "") {
                  $("#cbRfd").attr("checked", "checked");
                  fnCheckRfd();
              }
          });
    </script>

    <a id="reload" href="CODAccountEdit.aspx?accountNo=<%= AccountNO %>" style="display:none"></a>
    <table style="line-height:25px; margin:5px;">
		<tr>
			<td align="right">发货仓库：</td>
			<td><uc2:UCWareHouseCheckBox id="ucDeliveryHouse" LoadDataType="All" HouseCheckType="Distinguish" runat="server"></uc2:UCWareHouseCheckBox></td>
			<td align="right">发货时间：</td>
			<td><asp:TextBox ID="tbDate_D_S" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>-</td>
			<td><asp:TextBox ID="tbDate_D_E" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox></td>
			<td></td>
		</tr>
		<tr>
			<td align="right">拒收入库仓库：</td>
			<td><uc2:UCWareHouseCheckBox id="ucReturnHouse" LoadDataType="All" HouseCheckType="Distinguish" runat="server"></uc2:UCWareHouseCheckBox></td>
			<td align="right">拒收入库时间：</td>
			<td><asp:TextBox ID="tbDate_R_S" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>-</td>
			<td><asp:TextBox ID="tbDate_R_E" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox></td>
			<td colspan="2"></td>
		</tr>
		<tr>
			<td align="right">上门退货入库仓库：</td>
			<td><uc2:UCWareHouseCheckBox id="ucVisitReturnHouse" LoadDataType="All" HouseCheckType="Distinguish" runat="server"></uc2:UCWareHouseCheckBox></td>
			<td align="right">上门退货入库时间：</td>
			<td><asp:TextBox ID="tbDate_V_S" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>-</td>
			<td><asp:TextBox ID="tbDate_V_E" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox></td>
			<td colspan="2"></td>
		</tr>
        <tr>
            <td align="right">商家：</td>
			<td>
				<uc3:ucmerchantsourcetv ID="UCMerchantSourceTV" runat="server" MerchantLoadType="All" MerchantTypeSource="nk_Merchant_Expense" MerchantShowCheckBox="True" />
			</td>
            <td align="right">配送商：</td>
			<td colspan="2">
                <div style="clear:both;">
                    <div style="float:left;">
                        <uc1:UCExpressCompanyTV ID="UCExpressCompanyTV" runat="server" ExpressCompanyShowCheckBox="True" ExpressLoadType="ThirdCompany" ExpressTypeSource="nk_Express" />
                    </div>
                    <div style="float:left;">
                        <input type="checkbox" value="11" id="cbRfd" onclick="fnCheckRfd()" />如风达
                        <asp:HiddenField ID="hidRfdChecked" runat="server" />
                    </div>
                </div>
            </td>
            <td align="right">结算方式：</td>
			<td><asp:RadioButton ID="rbGeneral" runat="server" GroupName="rbAccountType" Text="普通" Checked="true" />
				<asp:RadioButton ID="rbEMS" runat="server" GroupName="rbAccountType" Text="邮政" />
				<asp:RadioButton ID="rbZJS" runat="server" GroupName="rbAccountType" Text="宅急送" />
			</td>
            <td><asp:Button ID="btSearch" runat="server" Text="查询" onclick="btSearch_Click" OnClientClick="return JudgeInputSearch();" />
				
			</td>
        </tr>
    </table>
    <div id="operatorBt">

		<input type="button" id="ViewDetail" value="查看明细" runat="server"
			onclick="fnViewDetail()" />
		<asp:Button ID="ExportMsg" runat="server" Text="导出汇总表信息" 
			onclick="ExportMsg_Click" />
		<asp:Button ID="PrintMsg" runat="server" Text="打印汇总表信息" 
			onclick="PrintMsg_Click" />
		<asp:Button ID="UpdateMsg" runat="server" Text="修改" onclick="UpdateMsg_Click" />
		<asp:Button ID="SubmitAccount" runat="server" Text="提交结算" 
			onclick="SubmitAccount_Click" OnClientClick="return ClientConfirm('确定提交此结算？');" />
		<asp:Button ID="DeleteAccountNO" runat="server" Text="删除结算单号" 
			onclick="DeleteAccountNO_Click" OnClientClick="return ClientConfirm('确定删除此结算单号？');" />
		<asp:Button ID="SaveAccount" runat="server" Text="保存并生成结算单号" 
			onclick="SaveAccount_Click" />
		<input type="button" id="AreaFareMsg" value="区域运费汇总表" runat="server"
			onclick="fnAreaFareMsg()" />
		结算单号：<asp:TextBox ID="tbAccountNO" runat="server" Enabled="false"></asp:TextBox>
		<asp:Button ID="ExportDifference" runat="server" Text="导出差异数据" 
            BorderColor="Red" ForeColor="Red" Font-Bold="true" Visible="false" onclick="ExportDifference_Click" />   
    </div>

    <asp:Label ID="noData" runat="server" ForeColor="Red"></asp:Label>
    <uc4:UCPager ID="UCPager" runat="server" PageSize="10" />
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false" 
		onrowdatabound="gvList_RowDataBound" DataKeyNames="AccountDetailID,AreaType,Formula,DataType">
		<Columns>
			<asp:BoundField DataField="MERCHANTNAME" HeaderText="商家" />
			<asp:BoundField DataField="CompanyName" HeaderText="配送站" />
			<asp:BoundField DataField="AreaType" HeaderText="区域类型" />
			<asp:BoundField DataField="DeliveryNum" HeaderText="普通发货数" />
			<asp:BoundField DataField="DeliveryVNum" HeaderText="上门换发货数" />
			<asp:BoundField DataField="ReturnsNum" HeaderText="普通拒收数" />
			<asp:BoundField DataField="ReturnsVNum" HeaderText="上门换拒收数" />
			<asp:BoundField DataField="VisitReturnsNum" HeaderText="上门退货数" />
			<asp:BoundField DataField="AccountNum" HeaderText="结算单量" />
			<asp:BoundField DataField="Formula" HeaderText="结算标准" />
			<asp:BoundField DataField="DatumFare" HeaderText="基准运费" />
			<asp:BoundField DataField="Allowance" HeaderText="超区补助" />
			<asp:BoundField DataField="KPI" HeaderText="KPI考核" />
			<asp:BoundField DataField="POSPrice" HeaderText="POS机手续费" />
			<asp:BoundField DataField="StrandedPrice" HeaderText="滞留扣款" />
			<asp:BoundField DataField="IntercityLose" HeaderText="城际丢失调账" />
            <asp:BoundField DataField="CollectionFee" HeaderText="代收手续费" />
            <asp:BoundField DataField="DeliveryFee" HeaderText="投递费" />
			<asp:BoundField DataField="OtherCost" HeaderText="其他费用" />
			<asp:BoundField DataField="Fare" HeaderText="实际结算运费" />
			<asp:BoundField DataField="DataType" HeaderText="DataType" Visible="false" />
			<asp:BoundField DataField="AccountDetailID" HeaderText="AccountDetailID" Visible="false" />
		</Columns>
    </asp:GridView>
    <div style="margin:5px; padding:5px;"><asp:Panel ID="pColumns" runat="server"></asp:Panel></div>
</asp:Content>
