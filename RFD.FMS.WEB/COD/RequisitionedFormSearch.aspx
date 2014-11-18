<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RequisitionedFormSearch.aspx.cs" Inherits="RFD.FMS.WEB.COD.RequisitionedFormSearch" MasterPageFile="~/Main/main.Master" Theme="default" %>

<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="UCSelectStationCommon" TagPrefix="uc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
<script type="text/javascript">
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
		$("#<%=gvList.ClientID %>").toSuperTable(
          { width: "1100px", height: "260px", fixedCols: 0,
          	onFinish: function() {
          	}
          });

		$("#<%=gvListStat.ClientID %>").toSuperTable(
          { width: "1100px", height: "160px", fixedCols: 0,
          	onFinish: function() {
          	}
          });
	});
</script>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
领用单查询
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="body">
<table>
		<tr>
			<td>配送公司</td>
			<td><uc1:UCSelectStationCommon ID="UCSelectStationCommon" runat="server" LoadDataType="OnlyThirdCompany" /></td>
			<td>发货时间</td>
			<td><asp:TextBox ID="txtDateStr" runat="server" 
					onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"></asp:TextBox></td>
			<td> ~ </td>
			<td><asp:TextBox ID="txtDateEnd" runat="server" 
					onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="查询" onclick="btnSearch_Click" />
            </td>
            <td>
                <asp:Button ID="btnSearchV2" runat="server" Text="查询V2" onclick="btnSearch_ClickV2" />
            </td>
			<td>
				<asp:Button ID="btnExprot" runat="server" Text="导出" onclick="btnExprot_Click" />
			</td>
		</tr>
    </table>
    
	<%--<table>
		<tr>
			<td valign="top" style="width:60%;">--%>
				<asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false">
					<Columns>
						<asp:BoundField HeaderText="订单号" DataField="WaybillNO" />
						<asp:BoundField HeaderText="配送公司" DataField="CompanyName" />
						<asp:BoundField HeaderText="发货时间" DataField="DeliverTime" />
						<asp:BoundField HeaderText="领用单号" DataField="RequisitionedNo" />
						<asp:BoundField HeaderText="部门" DataField="Dept" />
						<asp:BoundField HeaderText="领用人" DataField="RequisitionedBy" />
						<asp:BoundField HeaderText="制单人" DataField="BuildBy" />
						<asp:BoundField HeaderText="重量" DataField="Weight" />
						<asp:BoundField HeaderText="发货仓库" DataField="WarehouseName" />
						<asp:BoundField HeaderText="运费" DataField="DeliveryFare" />
						<asp:BoundField HeaderText="收货地址" DataField="Address" />
					</Columns>
				</asp:GridView>
			<%--</td>
			<td valign="top" style="width:40%">--%>
				<asp:GridView ID="gvListStat" runat="server" AutoGenerateColumns="false" 
					onrowdatabound="gvListStat_RowDataBound">
					<Columns>
						<asp:BoundField HeaderText="部门" DataField="DeptName" />
						<asp:BoundField HeaderText="配送公司" DataField="CompanyName" />
						<asp:BoundField HeaderText="重量" DataField="WeightSum" />
						<asp:BoundField HeaderText="发货仓库" DataField="WarehouseName" />
						<asp:BoundField HeaderText="订单数" DataField="CountNum" />
						<asp:BoundField HeaderText="运费" DataField="FareSum" />
						
					</Columns>
				</asp:GridView>
			<%--</td>
		</tr>
	</table>--%>
</asp:Content>
