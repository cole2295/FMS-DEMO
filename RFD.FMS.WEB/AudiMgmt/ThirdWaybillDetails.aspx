<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ThirdWaybillDetails.aspx.cs" 
Inherits="RFD.FMS.WEB.AudiMgmt.ThirdWaybillDetails" MasterPageFile="~/Main/main.Master"
%>

<%@ Register Src="~/UserControl/StationCheckBox.ascx" TagName="UCStationCheckBox" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/StationCheckBox.ascx" TagName="UCStationCheckBox" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/WareHouseCheckBox.ascx" TagName="UCWareHouseCheckBox" TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/MerchantSource.ascx" TagName="UCMerchantSource" TagPrefix="uc4" %>
<%@ Register TagPrefix="webdiyer" Namespace="Wuqi.Webdiyer" Assembly="AspNetPager, Version=7.0.2.0, Culture=neutral, PublicKeyToken=fb0a0fe055d40fd4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"> 
<link href="../css/superTables_compressed.css" rel="stylesheet" type="text/css" />
<script src="../Scripts/superTables_compressed.js" type="text/javascript"></script>
<script type="text/javascript">
    function OpenWin(url, width, height) {
        //打开模式窗口
        var top = parseInt((screen.availHeight - height - 30) / 2);
        var left = parseInt((screen.availWidth - width - 15) / 2);
        window.open(url, '', 'toolbar=0,location=0,maximize=1,directories=0,status=1,menubar=0,scrollbars=0,resizable=1,top=' + top
			    + ',left=' + left + ',width=' + width + ',height=' + height);
    }

    (function($) {
        $.fn.extend(
            {
                toSuperTable: function(options) {
                    var setting = $.extend(
                    {
                        width: "600px", height: "300px",
                        margin: "10px", padding: "0px",
                        overflow: "hidden", colWidths: undefined,
                        fixedCols: 0, headerRows: 1,
                        onStart: function() { },
                        onFinish: function() { },
                        cssSkin: "sSky"
                    }, options);
                    return this.each(function() {
                        var q = $(this);
                        var id = q.attr("id");
                        q.removeAttr("style").wrap("<div id='" + id + "_box'></div>");

                        var nonCssProps = ["fixedCols", "headerRows", "onStart", "onFinish", "cssSkin", "colWidths"];
                        var container = $("#" + id + "_box");

                        for (var p in setting) {
                            if ($.inArray(p, nonCssProps) == -1) {
                                container.css(p, setting[p]);
                                delete setting[p];
                            }
                        }

                        var mySt = new superTable(id, setting);

                    });
                }
            });
    })(jQuery);
    $(document).ready(function() {
    $("#<%=gridview1.ClientID %>").toSuperTable(
                      { width: "1100px", height: "300px", fixedCols: 0,fontSize:15,
                          onFinish: function() {
                          }
                      });
    });
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
接货明细表
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <table>
		<tr>
			<td>第三方配送商</td>
			<td><uc1:UCStationCheckBox ID="UCStationThirdCompany" runat="server" LoadDataType="ThirdCompany" /></td>
			<td>
				<asp:DropDownList ID="DDLTime" runat="server" Width="80px" Height="22px">
                    <asp:ListItem Selected="True" Value="0">接单时间</asp:ListItem>
                    <asp:ListItem Value="1">归班时间</asp:ListItem>
                    <asp:ListItem Value="2">拒收入库时间</asp:ListItem>
                </asp:DropDownList>
			</td>
			<td>
				<asp:TextBox ID="txtBTime" Width="128px" runat="server"
					onFocus="WdatePicker({startDate:'%y-%M-%d 00:00:00',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})">
				</asp:TextBox>~
				<asp:TextBox ID="txtETime" Width="128px" runat="server"
					onFocus="WdatePicker({startDate:'%y-%M-%d 23:59:59',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})">
				</asp:TextBox>
			</td>
			<td>运单类型</td>
			<td>				
				<asp:DropDownList ID="DDLWaybillType" runat="server" Width="128px">
					<asp:ListItem Value="">全部</asp:ListItem>
					<asp:ListItem Value="0">普通订单</asp:ListItem>
					<asp:ListItem Value="1">上门换货单</asp:ListItem>
					<asp:ListItem Value="2">上门退货单</asp:ListItem>
                    <asp:ListItem Value="3">签单返回单</asp:ListItem>
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td>
				<asp:DropDownList ID="ddlRFDType" runat="server" AutoPostBack="true"
					onselectedindexchanged="ddlRFDType_SelectedIndexChanged">
					<asp:ListItem Value="0">如风达站点</asp:ListItem>
					<asp:ListItem Value="1">如风达分拣</asp:ListItem>
				</asp:DropDownList>
			</td>
			<td><uc2:ucstationcheckbox ID="UCStationRFDSite" runat="server" LoadDataType="RFDSite" /></td>
			<td>
				<asp:DropDownList ID="DDLWaybillNO" runat="server">
					<asp:ListItem Text="运单号" Value="0"></asp:ListItem>
					<asp:ListItem Text="订单号" Value="1"></asp:ListItem>
				</asp:DropDownList>
			</td>
			<td>
				<asp:TextBox Width="128px" ID="txtWaybillNO" runat="server"></asp:TextBox>
			</td>
		    <td>支付类型</td>
			<td>
				<asp:DropDownList ID="DDLAcceptType" runat="server" Width="128px">
					<asp:ListItem Selected="True" Value="">全部</asp:ListItem>
					<asp:ListItem>POS机</asp:ListItem>
					<asp:ListItem>现金</asp:ListItem>
				</asp:DropDownList>
			</td>
		</tr>
		<tr>	
			<td>发货分拣中心</td>
			<td>
				<uc3:UCWareHouseCheckBox ID="UCWareHouseCheckBox" runat="server" LoadDataType="SortCenter" HouseCheckType="all" />
			</td>
			<td>订单状态</td>
			<td>
				<asp:DropDownList ID="DDLWaybillStatus" runat="server" Width="128px">
				</asp:DropDownList>
				<asp:DropDownList ID="DDLWaybillBackStatus" runat="server" Width="128px">
				</asp:DropDownList>
			</td>
			<td>区域类型</td>
			<td>				
                <asp:DropDownList ID="ddlAreaExpressLevel" runat="server" Width="128px">
                </asp:DropDownList>
            </td>
		</tr>
		<tr>
			<td>商家</td>
			<td>
				<uc4:UCMerchantSource ID="UCMerchantSource" runat="server" LoadDataType="ThirdMerchant" />
			</td>
			<td>无效状态</td>
			<td>
                <asp:DropDownList ID="ddlInefficacyStatus" runat="server">
                </asp:DropDownList>
            </td>
			<td>
                <asp:Button ID="BtnQuickQuery" runat="server" Text="查询V2" 
                    onclick="BtnQuickQuery_Click" />
                <asp:Button ID="BtnQuickExport"
                    runat="server" Text="导出V2" onclick="BtnQuickExport_Click" />
            </td>
			<td align="right">
				<asp:Button ID="BtnQuery" runat="server" Text="查询(Q)" OnClick="BtnQuery_Click" />
				<asp:Button ID="BtnExport" OnClientClick="return checkPage();" runat="server" OnClick="BtnExport_Click" Text="导出(O)" AccessKey="o" />
			</td>
		</tr>
    </table>

    <table style="font-weight:bold; font-size:13px;">
		<tr>
			<td>订单量合计：</td>
			<td><asp:Label ID="lbWaybillStat" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;">应收订单量合计：</td>
			<td><asp:Label ID="lbNeedStat" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;">应收款合计：</td>
			<td><asp:Label ID="lbNeedPayStat" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;">应退订单量合计：</td>
			<td><asp:Label ID="lbNeedBackStat" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;">应退款合计：</td>
			<td><asp:Label ID="lbNeedBackPayStat" runat="server">0.00</asp:Label></td>
			<td style="padding-left:10px;">结算重量合计：</td>
			<td><asp:Label ID="lbWeightStat" runat="server">0.000</asp:Label></td>
			<td style="padding-left:10px;">保价费合计：</td>
			<td><asp:Label ID="lbProtectFare" runat="server">0.000</asp:Label></td>
		</tr>
	</table>

    <asp:gridview ID="gridview1" runat="server" Width="100%" Font-Size="Large" Wrap="False"></asp:gridview>
    
	<webdiyer:AspNetPager runat="server" ID="Pager1" runat="server" ShowPageIndexBox="Always"
        CssClass="aspnetpager" PageIndexBoxType="DropDownList" FirstPageText="首页" LastPageText="尾页"
        NextPageText="后页" PrevPageText="前页" HorizontalAlign="Right" ShowNavigationToolTip="True"
        TextBeforePageIndexBox="转到第" pagesizedisplay="true" TextAfterPageIndexBox="页"
        AlwaysShow="True" NumericButtonTextFormatString="{0}" NumericButtonCount="4"
        CustomInfoHTML="第%CurrentPageIndex%页，共%PageCount%页，共%RecordCount%条记录，每页显示%PageSize%条记录"
        Width="100%" CustomInfoTextAlign="left" ShowCustomInfoSection="left" SubmitButtonStyle="Hight: 10px;float:right;"
        PageIndexBoxStyle="Hight: 10px;float:right;" submitbuttonimageurl="" urlpagesizename=""
        CurrentPageButtonTextFormatString="[{0}]" CustomInfoClass="aspnetpager" PageSize="50"
        layouttype="Div" CenterCurrentPageButton="True" RecordCount="0" Visible="True">
    </webdiyer:AspNetPager>
</asp:Content>
