<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true" CodeBehind="SortingTransferDetail.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.SortingTransferDetail" %>
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
        $(function () {
        var buttons = {
            Searchbtn: $.dom("Searchbtn", "input"),
            Exportbtn: $.dom("Exportbtn", "input")
        };

        buttons.Exportbtn.click(function () {
                $(document).progressDialog.showDialog("导出中，请稍后...");
        });
        buttons.Searchbtn.click(function () {
            $(document).progressDialog.showDialog("查询中，请稍后...");
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
    <p>拣运明细查询</p>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="body" runat="server">
<asp:HiddenField ID="download_token_name" runat="server" /> 
<asp:HiddenField ID="download_token_value" runat="server" />
    <table width="%100">
        <tr>
            <td colspan ="2">
             查询日期
              <asp:TextBox ID="BeginTime" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d 00:00:00',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"></asp:TextBox> —
              <asp:TextBox ID="EndTime" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d 23:59:59',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})" ></asp:TextBox>
           </td>
            <td>查询项目
       <asp:DropDownList ID="SearchingItemDDL" runat="server" AutoPostBack="True">
        <asp:ListItem Value="-1">请选择</asp:ListItem>
         <asp:ListItem Value="1">运输运费明细表</asp:ListItem>
          <asp:ListItem Value="2">分拣到城市明细表</asp:ListItem>
           <asp:ListItem Value="3">分拣到站点明细表</asp:ListItem>
            <asp:ListItem Value="4">逆向订单分拣费用明细表</asp:ListItem>
             <asp:ListItem Value="5">提货费用明细表</asp:ListItem>
    </asp:DropDownList>
    </td>
       
    <td colspan ="2">
     运单号
       <asp:TextBox ID="WaybillNOtxt" runat="server" MaxLength="20"></asp:TextBox>
     </td>
   </tr>
   <tr>
      <td colspan="2">分拣中心
          <uc1:StationCheckbox ID="SortingCenter" runat="server" LoadDataType="RFDSortCenter"/>
      </td>
      <td colspan="2">城市
         <uc3:CitySelectedBox ID="City" runat="server" />
     </td>
   <td colspan="2"> 站点
      <uc1:StationCheckbox ID="StationList" runat="server" LoadDataType="RFDSite" />
    </td>
     <td > 拣运商
      <asp:DropDownList ID="SortingMerchantDDL" runat="server" AutoPostBack="True">
                 <asp:ListItem Value="-1">请选择</asp:ListItem>
            </asp:DropDownList>
        <asp:checkbox ID="SortingMerchantchk" runat="server" />
        全选
     </td>
    </tr>
    <tr>
     <td colspan="2">配送商
         <uc1:StationCheckbox ID="DistributionList" runat="server" LoadDataType="All" />     
     </td>
    <td colspan="2">商家
        <uc2:MerchantCheckbox ID="MerchantSource" runat="server" LoadDataType="All" />
    </td>
    <td>
    订单类型
    <asp:DropDownList ID="WaybillTypeDDL" runat="server" AutoPostBack="True">
               <asp:ListItem Value="-1">全部</asp:ListItem>
               <asp:ListItem Value="0">普通订单</asp:ListItem>
               <asp:ListItem Value="1">上门换货</asp:ListItem>
               <asp:ListItem Value="2">上门退货</asp:ListItem>
               <asp:ListItem Value="3">签单返回</asp:ListItem>
          </asp:DropDownList>
   </td>
    <td>
    <asp:Button ID="Searchbtn" runat="server" Text="查询" onclick="Searchbtn_Click" />
     <asp:Button ID="Exportbtn" runat="server" Text="导出" onclick="Exportbtn_Click"  />
    </td>

   </tr>  
</table>
   
<table width="%80">
<tr>
<asp:GridView ID="GridView1" runat="server"  AutoGenerateColumns="False" >
          <Columns>              
           <asp:BoundField DataField="DetailKid" HeaderText="DetailKid"  Visible="false"/>
           <asp:BoundField DataField="SortingMerchantName" HeaderText="拣运商" />
           <asp:BoundField DataField="SortingCenter" HeaderText="分拣中心" />
           <asp:BoundField DataField="OutSortingCenter" HeaderText="出库分拣中心" />
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
</table>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PagerContent" runat="server">
<uc4:Pager ID="Pager1" runat="server" />
</asp:Content>
