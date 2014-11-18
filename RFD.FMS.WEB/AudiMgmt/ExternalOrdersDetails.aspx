<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalOrdersDetails.aspx.cs"
    Inherits="RFD.FMS.WEB.AudiMgmt.ExternalOrdersDetails" MasterPageFile="~/Main/main.Master" %>

<%@ Register Src="~/UserControl/ExpressCompanyTV.ascx"  TagName="UCExpressCompanyTV"  TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/StationCheckBox.ascx" TagName="UCStationCheckBox"
    TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/WareHouseCheckBox.ascx" TagName="UCWareHouseCheckBox"
    TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV"
    TagPrefix="uc4" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc5" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
        $(document).ready(function () {
            $("#<%=gridview1.ClientID %>").toSuperTable(
              { width: "1120px", height: "280px", fixedCols: 0,
                  onFinish: function () {
                  }
              });
        });

        $(function () {
            var buttons = {
                BtnQuery: $.dom("BtnQuery", "input"),
                btExport: $.dom("btnExport", "input"),
                btExportV3: $.dom("btExportV3", "input")
            };
            buttons.btExportV3.click(function () {
                $(document).progressDialog.showDialog("导出中，请稍后...");
            });
            buttons.btExport.click(function () {
                $(document).progressDialog.showDialog("导出中，请稍后...");
            });
            buttons.BtnQuery.click(function () {
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
    接货明细表
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">   
    <asp:HiddenField ID="download_token_name" runat="server" /> 
    <asp:HiddenField ID="download_token_value" runat="server" />
    <table>
        <tr>
            <td>
                第三方配送商
            </td>
            <td>
                <uc1:UCExpressCompanyTV ID="UCExpressCompanyTV" runat="server" ExpressLoadType="ThirdCompany" ExpressTypeSource="nk_Express" ExpressCompanyShowCheckBox="True" />
            </td>
            <td>
                <asp:DropDownList ID="DDLTime" runat="server" Width="80px" Height="22px">
                    <asp:ListItem Selected="True" Value="0">接单时间</asp:ListItem>
                    <asp:ListItem Value="1">归班时间</asp:ListItem>
                    <asp:ListItem Value="2">拒收入库时间</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="txtBTime" Width="128px" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})">
                </asp:TextBox>~
                <asp:TextBox ID="txtETime" Width="128px" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})">
                </asp:TextBox>
            </td>
            <td>
                运单类型
            </td>
            <td>
                <asp:DropDownList ID="DDLWaybillType" runat="server" Width="128px">
                    <asp:ListItem Value="">全部</asp:ListItem>
                    <asp:ListItem Value="0">普通订单</asp:ListItem>
                    <asp:ListItem Value="1">上门换货单</asp:ListItem>
                    <asp:ListItem Value="2">上门退货单</asp:ListItem>
                    <asp:ListItem Value="3">签单返回</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:DropDownList ID="ddlRFDType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRFDType_SelectedIndexChanged">
                    <asp:ListItem Value="0">站点</asp:ListItem>
                    <asp:ListItem Value="1">分拣</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <uc2:UCStationCheckBox ID="UCStationRFDSite" runat="server" LoadDataType="RFDSite" />
            </td>
            <td>
                <asp:DropDownList ID="DDLWaybillNO" runat="server">
                    <asp:ListItem Text="运单号" Value="0"></asp:ListItem>
                    <asp:ListItem Text="订单号" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox Width="128px" ID="txtWaybillNO" runat="server"></asp:TextBox>
            </td>
            <td>
                支付类型
            </td>
            <td>
                <asp:DropDownList ID="DDLAcceptType" runat="server" Width="128px">
                    <asp:ListItem Selected="True" Value="">全部</asp:ListItem>
                    <asp:ListItem>POS机</asp:ListItem>
                    <asp:ListItem>现金</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                发货分拣中心
            </td>
            <td>
                <uc3:UCWareHouseCheckBox ID="UCWareHouseCheckBox" runat="server" LoadDataType="SortCenter"
                    HouseCheckType="all" />
            </td>
            <td>
                订单状态
            </td>
            <td>
                <asp:DropDownList ID="DDLWaybillStatus" runat="server" Width="128px">
                    <asp:ListItem Text="全部" Value=""></asp:ListItem>
                    <asp:ListItem Text="妥投" Value="3"></asp:ListItem>
                    <asp:ListItem Text="拒收" Value="5"></asp:ListItem>
                    <asp:ListItem Text="无效" Value="-9"></asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="DDLWaybillBackStatus" runat="server" Width="128px">
                    <asp:ListItem Text="全部" Value=""></asp:ListItem>
                    <asp:ListItem Text="返货入库" Value="1"></asp:ListItem>
                    <asp:ListItem Text="退换货入库" Value="6"></asp:ListItem>
                    <asp:ListItem Text="拒收入库" Value="7"></asp:ListItem>
                    <asp:ListItem Text="签单返回入库" Value="13"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                区域类型
            </td>
            <td>
                <asp:DropDownList ID="ddlAreaExpressLevel" runat="server" Width="128px">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                商家
            </td>
            <td>
                <uc4:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" MerchantLoadType="ThirdMerchant"
                    MerchantTypeSource="nk_Merchant" MerchantShowCheckBox="True" />
            </td>
            <td>
                无效状态
            </td>
            <td>
                <asp:DropDownList ID="ddlInefficacyStatus" runat="server">
                </asp:DropDownList>
            </td>
            <td align="right" colspan="2">
                <asp:Button ID="BtnQuery" runat="server" Text="查询(Q)" OnClick="BtnQuery_Click" />
                <asp:Button ID="btExportV3" runat="server" Text="导出" AccessKey="O" OnClick="btnExport_ClickV3"
                    Style="width: 80px" />
                <asp:Button ID="btnExport" runat="server" Visible="false" OnClick="btnExport_Click" Text="导出(O)"
                    AccessKey="o" />
                <asp:Button ID="BtnQueryV2" runat="server" Visible="false" Text="查询V2(Q)" OnClick="BtnQuery_ClickV2" />
                <asp:Button ID="Button1" runat="server" Visible="false" OnClick="btnExport_ClickV2" Text="导出V2(O)"
                    AccessKey="o" />
                
            </td>
        </tr>
    </table>
    <div style="color:Red;">注意：多选的需要全部查询，则不用选择即表示为全部包含查询。如：商家需要全部包含查询，则不用选择商家即为全部查询。</div>
    <table style="font-weight: bold; font-size: 13px;">
        <tr>
            <td>
                订单量合计：
            </td>
            <td>
                <asp:Label ID="lbWaybillStat" runat="server">0.00</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                应收订单量合计：
            </td>
            <td>
                <asp:Label ID="lbNeedStat" runat="server">0.00</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                应收款合计：
            </td>
            <td>
                <asp:Label ID="lbNeedPayStat" runat="server">0.00</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                应退订单量合计：
            </td>
            <td>
                <asp:Label ID="lbNeedBackStat" runat="server">0.00</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                应退款合计：
            </td>
            <td>
                <asp:Label ID="lbNeedBackPayStat" runat="server">0.00</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                结算重量合计：
            </td>
            <td>
                <asp:Label ID="lbWeightStat" runat="server">0.000</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                保价金额合计：
            </td>
            <td>
                <asp:Label ID="lbProtectFare" runat="server">0.000</asp:Label>
            </td>
            <td style="padding-left: 10px;">
                配送费合计：
            </td>
            <td>
                <asp:Label ID="lbDeliverFare" runat="server">0.000</asp:Label>
            </td>
            <td style="padding-left:10px;">pos服务费合计：</td>
			<td><asp:Label ID="lbposFare" runat="server">0.000</asp:Label></td>
            <td style="padding-left:10px;">代收手续费合计：</td>
			<td><asp:Label ID="lbCashFare" runat="server">0.000</asp:Label></td>
        </tr>
    </table>
    <div style="text-align: center">
        <asp:Label ID="noData" runat="server" ForeColor="Red" Visible="false"></asp:Label></div>
    <asp:GridView ID="gridview1" runat="server" Width="100%" Font-Size="Large" Wrap="False" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="序号" HeaderText="序号" />
            <asp:BoundField DataField="FEE唯一编号" HeaderText="FEE唯一编号" />
            <asp:BoundField DataField="运单号" HeaderText="运单号" />
            <asp:TemplateField HeaderText="明细">
                <ItemTemplate>
                    <a href='javascript:void(0)' onclick="fnOpenModalDialog('../BasicSetting/OperateLogView.aspx?PK_NO=<%# DataBinder.Eval(Container.DataItem, "运单号") %>&LogType=7',500,300);">
                        日志</a>
                    <asp:HiddenField ID="WayBillNO" Value='<%# DataBinder.Eval(Container.DataItem, "运单号") %>'
                        runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="订单号" HeaderText="订单号" />
            <asp:BoundField DataField="配送单号" HeaderText="配送单号" />
            <asp:BoundField DataField="运单类型" HeaderText="运单类型" />
            <asp:BoundField DataField="月结商家名称" HeaderText="月结商家名称" />
            <asp:BoundField DataField="商家" HeaderText="商家" />
            <asp:BoundField DataField="子公司" HeaderText="子公司" />
            <asp:BoundField DataField="子公司编码" HeaderText="子公司编码" />
            <asp:BoundField DataField="配送商" HeaderText="配送商" />
            <asp:BoundField DataField="配送站" HeaderText="配送站" />
            <asp:BoundField DataField="接单时间" HeaderText="接单时间" />
            <asp:BoundField DataField="发货分拣中心" HeaderText="发货分拣中心" />
            <asp:BoundField DataField="归班时间" HeaderText="归班时间" />
            <asp:BoundField DataField="运单状态" HeaderText="运单状态" />
            <asp:BoundField DataField="拒收入库时间" HeaderText="拒收入库时间" />
            <asp:BoundField DataField="返货状态" HeaderText="返货状态" />
            <asp:BoundField DataField="结算重量" HeaderText="结算重量" />
            <asp:BoundField DataField="货物品类" HeaderText="货物品类" />
            <asp:BoundField DataField="配送费" HeaderText="配送费" />
            <asp:BoundField DataField="配送费计算公式" HeaderText="配送费计算公式" />
            <asp:BoundField DataField="应收款" HeaderText="应收款" />
            <asp:BoundField DataField="应退金额" HeaderText="应退金额" />
            <asp:BoundField DataField="保价金额" HeaderText="保价金额" />
            <asp:BoundField DataField="无效状态" HeaderText="无效状态" />
            <asp:BoundField DataField="支付方式" HeaderText="支付方式" />
            <asp:BoundField DataField="收入区域类型" HeaderText="收入区域类型" />
            <asp:BoundField DataField="省" HeaderText="省" />
            <asp:BoundField DataField="市" HeaderText="市" />
            <asp:BoundField DataField="区" HeaderText="区" />
            <asp:BoundField DataField="地址" HeaderText="地址" />
            <asp:BoundField DataField="保价费" HeaderText="保价费" />
            <asp:BoundField DataField="现金服务费" HeaderText="现金服务费" />
            <asp:BoundField DataField="现金手续费" HeaderText="现金手续费" />
            <asp:BoundField DataField="POS服务费" HeaderText="POS服务费" />
            <asp:BoundField DataField="POS手续费" HeaderText="POS手续费" />
        </Columns>
    </asp:GridView>
    <div>
        <uc5:UCPager ID="UCPager" runat="server" />
    </div>
</asp:Content>
