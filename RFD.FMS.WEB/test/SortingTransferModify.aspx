<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true" CodeBehind="SortingTransferModify.aspx.cs" Inherits="RFD.FMS.WEB.test.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
           
        });
        $(function () {
            var buttons = {
                GoBtn: $.dom("GoBtn", "input"),
                GotoBtn: $.dom("GotoBtn", "input")
            };
            buttons.GoBtn.click(function () {
                $(document).progressDialog.showDialog("执行中，请稍后...");
            });
            buttons.GotoBtn.click(function () {
                $(document).progressDialog.showDialog("执行中，请稍后...");
            });
        });
      
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="body" runat="server">
    <div>
       时间：<asp:TextBox ID="txtBeginTime" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d 00:00:00',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"
										runat="server"></asp:TextBox>至<asp:TextBox ID="txtEndTime" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d 23:59:59',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"
								runat="server"> </asp:TextBox>
       <asp:Button ID="GotoBtn" runat="server" Text="拣运服务推送数据" onclick="GotoBtn_Click"> </asp:Button>
                        <br/>
       最小ID <asp:TextBox ID="minIDtxt" Width="200px" 
										runat="server"></asp:TextBox>
       最大ID <asp:TextBox ID="maxIDtxt" Width="200px" 
										runat="server"></asp:TextBox>
    <asp:Button ID="GoBtn" runat="server" Text="删除数据" onclick="GoBtn_Click"> </asp:Button>
    <br/>
    运单号（多个运单用,隔开）<asp:TextBox ID="WaybillNotxt" Width="200px" 
										runat="server"></asp:TextBox>
    <asp:Button ID="ModifyBtnByNo" runat="server" Text="运单数据推送" 
           onclick="ModifyBtnByNo_Click" > </asp:Button>
 </div>
 <div>
     <asp:UpdatePanel ID="UpdatePanel1" runat="server">
         <ContentTemplate>
             <asp:Label ID="LabelTips" runat="server">
             </asp:Label>
         </ContentTemplate> 
     </asp:UpdatePanel>
  </div>  
  <div>
       时间：<asp:TextBox ID="Time1" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d 00:00:00',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"
										runat="server"></asp:TextBox>至<asp:TextBox ID="Time2" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d 23:59:59',dateFmt:'yyyy-MM-dd HH:mm:ss',alwaysUseStartDate:true})"
								runat="server"> </asp:TextBox>
       站点：<asp:TextBox ID="Station" runat="server"></asp:TextBox>
                                <asp:Button ID="VanclCODBtn" runat="server" 
           Text="vanclCOD查询" onclick="VanclCODBtn_Click" 
            > </asp:Button>
       <div>
          RowCount:<asp:TextBox ID="txtRowCount" runat="server"></asp:TextBox>
           <asp:Button ID="MerchantDeliverFeeBtn" runat="server" 
           Text="配送商基础信息更新服务" onclick="MerchantDeliverFeeBtn_Click" 
            > </asp:Button>
       </div>

       <div>
          RowCount:<asp:TextBox ID="txtExcuteCount" runat="server"></asp:TextBox>
           <asp:Button ID="MerchantDeliverFeeModBtn" runat="server" 
           Text="商家配送费修改服务" onclick="MerchantDeliverFeeModBtn_Click" 
            > </asp:Button>
       </div>

       <div>
          RowCount:<asp:TextBox ID="txtSortingFee" runat="server"></asp:TextBox>
           <asp:Button ID="SortingFeeBtn" runat="server" 
           Text="拣运生效服务" onclick="SortingFeeBtn_Click"  
            > </asp:Button>
       </div>
       <div>
         
           <asp:Button ID="TimerTestBtn" runat="server" 
           Text="Timer测试" onclick="TimerTestBtn_Click" 
            > </asp:Button>
            <asp:Label ID="Timerlabel" runat="server" ></asp:Label>
       </div>
  </div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="PagerContent" runat="server">
</asp:Content>
