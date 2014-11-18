<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true" CodeBehind="SortingTransferReporting.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.SortingTransferReporting" %>
<%@ Register Src="~/UserControl/StationCheckBox.ascx" TagName="StationCheckbox" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/MerchantSource.ascx" TagName="MerchantCheckbox" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/CitySelected.ascx" TagName="CitySelectedBox" TagPrefix="uc3" %>
<%@ Register src="~/UserControl/Pager.ascx" tagName="Pager" tagPrefix="uc4" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<title>拣运详细查询</title>
<script src="../Scripts/finance/finance.search.new.js" type="text/javascript"></script>
    <script src="../Scripts/finance/finance.search.page.new.js" type="text/javascript"></script>
    <script type="text/javascript">
        //初始化操作
        function init() {
            var buttons = {
                btnSearch: $.dom("btnSearch", "input"),
                btnSummary: $.dom("btnSummary", "input"),
                btnDetails: $.dom("btnDetails", "input"),
                btnAllDetails: $.dom("btnAllDetails", "input")
            };
            var hiddens = {
                hidTotalCount: $.dom("hidTotalCount", "input"),
                hidDetailsCount: $.dom("hidDetailsCount", "input")
            };
            search.init.ready({
                totalButtons: [buttons.btnSummary, buttons.btnAllDetails],
                detailsButtons: [buttons.btnDetails],
                totalCount: hiddens.hidTotalCount.val(),
                detailsCount: hiddens.hidDetailsCount.val(),
                renderTo: "gvSummary",
                clickable: true
            });
        }

    	
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
 <p>拣运明细查询</p>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="body" runat="server">
<table>
   <tr>
      <td>
      <asp:Label ID="SearchingItem" runat="server">查询项目:</asp:Label>
      </td>
      <td>     
       <asp:DropDownList ID="SearchingItemDDL" runat="server" AutoPostBack="True">
        <asp:ListItem Value="-1">请选择</asp:ListItem>
         <asp:ListItem Value="1">运输运费明细表</asp:ListItem>
          <asp:ListItem Value="2">分拣到城市明细表</asp:ListItem>
           <asp:ListItem Value="3">分拣到站点明细表</asp:ListItem>
            <asp:ListItem Value="4">逆向订单分拣费用明细表</asp:ListItem>
             <asp:ListItem Value="5">提货费用明细表</asp:ListItem>
    </asp:DropDownList>
    </td>
    <td>
    <asp:Label ID="SearchDate" runat="server">查询日期:</asp:Label>
    </td>
    <td>
      <asp:TextBox ID="BeginTime" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d 00:00:00',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"></asp:TextBox> —
      <asp:TextBox ID="EndTime" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d 23:59:59',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})" ></asp:TextBox>
     </td>
    <td>
     运单号：
    </td>
    <td>
     <asp:TextBox ID="WaybillNOtxt" runat="server" MaxLength="20"></asp:TextBox>
    </td>
   </tr>
   <tr>
      <td> 拣运商：</td>
      <td>
      <asp:DropDownList ID="SortingMerchantDDL" runat="server" AutoPostBack="True">
                 <asp:ListItem Value="-1">请选择</asp:ListItem>
            </asp:DropDownList>
        <asp:checkbox ID="SortingMerchantchk" runat="server" />
        全选
     </td>
      <td>分拣中心：</td>
      <td>  
          <uc1:StationCheckbox ID="SortingCenter" runat="server" LoadDataType="RFDSortCenter" />
      </td>
      <td>城市：</td>
      <td>
         <uc3:CitySelectedBox ID="City" runat="server" />
     </td>
   <td> 站点：</td>
   <td>
      <uc1:StationCheckbox ID="StationList" runat="server" LoadDataType="RFDSite" />
   </td>

    </tr>
    <tr>
     <td>配送商：</td>
     <td>
         <uc1:StationCheckbox ID="DistributionList" runat="server" LoadDataType="All" />     
     </td>
    <td>商家：</td>
    <td>
        <uc2:MerchantCheckbox ID="MerchantSource" runat="server" LoadDataType="All" />
    </td>
    <td>
    订单类型：</td>
    <td>
    <asp:DropDownList ID="WaybillTypeDDL" runat="server" AutoPostBack="True">
               <asp:ListItem Value="-1">全部</asp:ListItem>
               <asp:ListItem Value="0">普通订单</asp:ListItem>
               <asp:ListItem Value="1">上门换货</asp:ListItem>
               <asp:ListItem Value="2">上门退货</asp:ListItem>
          </asp:DropDownList>
   </td>
    <td>
    <asp:Button ID="Searchbtn" runat="server" Text="查询" onclick="Searchbtn_Click" />
    <asp:Button ID="Exportbtn" runat="server" Text="导出" onclick="Exportbtn_Click" />
    </td>
   </tr>
  
   
</table>
   
    
<hr width=980 size=3 color="#00ffff" align=center/>
<table>
<tr>
<asp:GridView ID="GridView1" runat="server"  AutoGenerateColumns="False" >
          <Columns>
                
           <asp:BoundField DataField="DetailKid" HeaderText="DetailKid"  Visible="false"/>
           <asp:BoundField DataField="SoringMerchantID" HeaderText="拣运商" />
           <asp:BoundField DataField="SortingCenter" HeaderText="分拣中心" />
           <asp:BoundField DataField="TSortingCenter" HeaderText="二级分拣中心" />
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
           </Columns>
</asp:GridView>
</tr>
<tr>
  <td>
    <uc4:Pager ID="Pager1" runat="server" />
  </td>
</tr>
</table>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PagerContent" runat="server">
</asp:Content>
