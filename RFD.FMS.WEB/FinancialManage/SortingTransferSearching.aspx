<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true" CodeBehind="SortingTransferSearching.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.SortingTransferSearching" %>
<%@ Register Src="~/UserControl/StationCheckBox.ascx" TagName="StationCheckbox" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/MerchantSource.ascx" TagName="MerchantCheckbox" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/CitySelected.ascx" TagName="CitySelectedBox" TagPrefix="uc3" %>
<%@ Register src="~/UserControl/Pager.ascx" tagName="Pager" tagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    $(function () {
        var buttons = {
            Search: $.dom("Search","input"),
            MerchantExport: $.dom("MerchantExport","input"),
            DailyExport: $.dom("DailyExport", "input"),
            DetailExport: $.dom("DetailExport", "input"),
            Exportbtn: $.dom("Export", "input")
            
        };
        buttons.Search.click(function () {
            $(document).progressDialog.showDialog("查询中，请稍后...");
        });
        buttons.MerchantExport.click(function () {
            $(document).progressDialog.showDialog("查询中，请稍后...");
        });
        buttons.DailyExport.click(function () {
            $(document).progressDialog.showDialog("查询中，请稍后...");
        });
        buttons.DetailExport.click(function () {
            $(document).progressDialog.showDialog("查询中，请稍后...");
        });

        buttons.Exportbtn.click(function () {
            $(document).progressDialog.showDialog("导出中，请稍后...");
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
        
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    拣运报表查询
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="body" runat="server">
<asp:HiddenField ID="download_token_name" runat="server" /> 
<asp:HiddenField ID="download_token_value" runat="server" />
<table width="%100">
    <tr>
         <td>日期</td> 
          <td>
           <asp:TextBox ID="BeginTime" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d 00:00:00',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"></asp:TextBox> —
           <asp:TextBox ID="EndTime" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d 23:59:59',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})" ></asp:TextBox>
         </td>
          <td>分拣中心
              <uc1:StationCheckbox ID="SortingCenter" runat="server" LoadDataType="RFDSortCenter" />
          </td>
          <td>城市
              <uc3:CitySelectedBox ID="CitySelected" runat="server" />
          </td>
          <td>
          拣运商
              <asp:DropDownList ID="SortingMerchantDDL" runat="server" />
              <asp:CheckBox ID="SortingMerchantChk" runat="server" Checked="true" />
               全选
          </td>
    </tr>
    <tr>
        <td>报表查询</td>
        <td>
         <asp:DropDownList ID="SearchingItemDDL" runat="server" AutoPostBack="True">
         <asp:ListItem Value="-1">请选择</asp:ListItem>
         <asp:ListItem Value="1">运输运费报表</asp:ListItem>
         <asp:ListItem Value="2">分拣到城市报表</asp:ListItem>
         <asp:ListItem Value="3">分拣到站点报表</asp:ListItem>
         <asp:ListItem Value="4">逆向订单分拣费用报表</asp:ListItem>
         <asp:ListItem Value="5">提货费用报表</asp:ListItem>
         </asp:DropDownList>
        </td>
         <td> 商家
            <uc2:MerchantCheckbox ID="MerchantSource" runat="server" LoadDataType="All" />
         </td>
        <td colspan="5">
           <asp:Button ID="Search" runat="server" Text="汇总查询" onclick="Search_Click" />
           <asp:Button ID="DailyExport" runat="server" Text="日统计汇总查询" 
                onclick="DailyExport_Click" />
           <asp:Button ID="MerchantExport" runat="server" Text="商家汇总查询" 
                onclick="MerchantExport_Click" />
           <asp:Button ID="DetailExport" runat="server" Text="明细查询" 
                onclick="DetailExport_Click" />
           <asp:Button ID="Export" runat="server" Text="导出" onclick="Export_Click" />
        </td>
    </tr>
</table>

<table>
   <tr>
      <asp:GridView ID="GridView1" runat="server"  AutoGenerateColumns="False" >
          <Columns>
                
           <asp:BoundField DataField="DetailKid" HeaderText="DetailKid"  Visible="false"/>
           <asp:BoundField DataField="StatisticsType" HeaderText="统计类型"/>
           <asp:BoundField DataField="SDate" HeaderText="日期" />
           <asp:BoundField DataField="SortingMerchantName" HeaderText="拣运商" />
           <asp:BoundField DataField="SortingCenterAll" HeaderText="分拣中心" />
           <asp:BoundField DataField="SortingCenter" HeaderText="分拣中心" />
           <asp:BoundField DataField="OutSortingCenter" HeaderText="出库分拣中心" />
           <asp:BoundField DataField="TSortingCenter" HeaderText="分拣中心" />
           <asp:BoundField DataField="CreateCity" HeaderText="接单城市" />
           <asp:BoundField DataField="SortCity" HeaderText="城市" />
           <asp:BoundField DataField="Distribution" HeaderText="配送商" />
           <asp:BoundField DataField="DeliverStation" HeaderText="配送站" />
           <asp:BoundField DataField="InSortingTime" HeaderText="入库时间" />
           <asp:BoundField DataField="OutBoundTime" HeaderText="出库时间" />
           <asp:BoundField DataField="ToStationTime" HeaderText="入站时间" />
           <asp:BoundField DataField="ReturnTime" HeaderText="返库时间" />
           <asp:BoundField DataField="WaybillNO" HeaderText="运单号" />
           <asp:BoundField DataField="WaybillType" HeaderText="订单类型" />
           <asp:BoundField DataField="MerchantName" HeaderText="商家" />
           <asp:BoundField DataField="City" HeaderText="城市" />
           <asp:BoundField DataField="WaybillSum" HeaderText="单量" />
           <asp:BoundField DataField="Price" HeaderText="结算单价" />
           <asp:BoundField DataField="Fee" HeaderText="结算费用" />
           </Columns>
</asp:GridView>
   </tr>
   </table>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PagerContent" runat="server">
  <uc4:Pager ID="Pager1" runat="server" />
</asp:Content>
