<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true" CodeBehind="UpadeSortingFee.aspx.cs" Inherits="RFD.FMS.WEB.SortingManage.UpadeSortingFee" %>
<%@ Register Src="~/UserControl/DistributionSelect.ascx" TagName="DistributionSelect"
    TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/CitySelected.ascx" TagName="CitySelected" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/MerchantSource.ascx" TagName="MerchantSource" TagPrefix="uc3" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    添加拣运商信息
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
    <table>
   <tr>
        <td>拣运商</td>
        <td><asp:DropDownList ID="SortingMerchantDDL" runat="server" AutoPostBack="true">
               <asp:ListItem Value="-1">请选择</asp:ListItem>
            </asp:DropDownList>
        </td>
        <%--<td>
            <uc1:DistributionSelect ID="DistributionSelected" runat="server" />
        </td>--%>
   </tr>
   
   <tr>
     <td>分拣中心</td>
        <td> <asp:DropDownList ID="SortingCenterDDL" runat="server" AutoPostBack="true">
               <asp:ListItem Value="-1">请选择</asp:ListItem>
            </asp:DropDownList>
        </td>
   </tr>
   <tr>
     <td>城市</td>
     <td><uc2:CitySelected ID="CitySelected" runat="server" /></td>
   </tr>

   <tr>
        <td>商家</td>
        <td><uc3:MerchantSource ID="MerchantSource" runat="server" LoadDataType="All" /></td>
   </tr>
   <tr>
    
    
        <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
           <ContentTemplate>--%>
            <td>费用类型</td>
        <td>
        <asp:DropDownList ID="ItemTypeDLL" runat="server" 
                onselectedindexchanged="ItemTypeDLL_SelectedIndexChanged" AutoPostBack="true">
                <asp:ListItem  Value="-1">请选择</asp:ListItem>
                <asp:ListItem Value="1">拣运费用项目</asp:ListItem>
                <asp:ListItem Value="2">分拣到城市费用</asp:ListItem>
                <asp:ListItem Value="3">分拣到站点费用</asp:ListItem>
                <asp:ListItem Value="4">逆向退换货入库费用</asp:ListItem>
                <asp:ListItem Value="5">提货补偿费用</asp:ListItem>
            </asp:DropDownList>
             </td>
       <td>
           <div id="div1" runat="server" >
              单量<asp:TextBox ID="txtBillCount" runat="server" Width="30px" ></asp:TextBox>
             <asp:RadioButton ID="IsAccountWaybill" runat="server" GroupName="AccountWay" />
             按单量结算
             <asp:RadioButton ID="NoAccountWaybill" runat="server" GroupName="AccountWay" />
             按趟次结算
           </div>
       </td>
      
      <%--</ContentTemplate>
      </asp:UpdatePanel>--%>
      
   </tr>
   <tr id="effectdate" runat="server">
     
     <td>生效日期</td>
     <td><asp:TextBox ID="txtEffectDate" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"
     							    runat="server"></asp:TextBox></td>
    	
   </tr>
   <tr>
     <td>结算单价</td>
        <td><asp:TextBox ID="txtAccountFare" runat="server" Width="115px" ></asp:TextBox></td>
   </tr>
    <tr>
    
        <td><asp:Button ID="SubmitBtn" runat="server" Text="提交" onclick="SubmitBtn_Click" /></td>
         <td><asp:Button ID="BackBtn" runat="server" Text="返回" onclick="BackBtn_Click"  /></td>
   </tr>
   
</table>
</asp:Content>

