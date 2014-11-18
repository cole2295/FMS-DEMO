<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    CodeBehind="UpdateDeliverFee.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.UpdateDeliverFee"
    Theme="default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <base target="_self" />
    <title>配送费修改</title>
	
    <script src="../Scripts/import/deliverfee.update.js" type="text/javascript"></script>
   <script type="text/javascript">
       $(function () {
           if ($("#<%=this.rbtweightfive.ClientID %>").attr("checked")) {
               setrbtweighValue(true);
           } else {
               setrbtweighValue(false);
           }
       })
       function setrbtweighValue(value) {
        if (value != true) {
            $("#rbtweighValue").removeAttr("style");
        }
        else{
            $("#rbtweighValue").attr("style", "display:none");
        }
    }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table cellpadding="0" cellspacing="5" border="0" width="100%" id="container" style=" line-height:25px;">
        <tr>
            <td colspan="3">
                <asp:Label ID="lblMerchantName" runat="server" Text="" CssClass="title"></asp:Label>
                <hr />
            </td>
        </tr>
        <tr>
         <td>
             
         结算重量：
         </td>
         <td>
            <asp:RadioButton ID="rbtweightone" runat="server" Text="商家重量"  GroupName="weight" onclick="setrbtweighValue(false)"/>
            <asp:RadioButton ID="rbtweighttwo" runat="server" Text="称重重量"  GroupName="weight"  onclick="setrbtweighValue(false)" />
            <asp:RadioButton ID="rbtweightThree" runat="server" Text="商家重量与体积重量较大者"  GroupName="weight"  onclick="setrbtweighValue(false)" />
            <asp:RadioButton ID="rbtweightfour" runat="server" Text="取件重量"  GroupName="weight"  onclick="setrbtweighValue(false)" />
            <asp:RadioButton ID="rbtweightfive" runat="server" Text="单量结算"  GroupName="weight"  onclick="setrbtweighValue(true)" />
         </td>
        </tr>
        
        <tr style="display:none" id="rbtweighValue">
         <td > 重量数值规则：</td>
         <td>
            <asp:RadioButton ID="rbtweighValuetzore" runat="server" Text="原重量" Checked="true" GroupName="weightValue" />
            <asp:RadioButton ID="rbtweighValuetone" runat="server" Text="四舍五入" GroupName="weightValue" />
            <asp:RadioButton ID="rbtweighValuettwo" runat="server" Text="2下3上、7下8上"  GroupName="weightValue" />
           
         </td>
        </tr>
        
        <tr>
            <td>
                体积重量计算公式
            </td>
            <td>
                体积重量=体积/<asp:TextBox ID="txtVolumeParmer" runat="server" Width="50"></asp:TextBox></td>
        </tr>
         <tr>
            <td>
                保价费计算公式
            </td>
            <td>
                保价费=保价金额×<asp:TextBox ID="txtProtectedParmer" runat="server" Width="50"></asp:TextBox>+<asp:TextBox ID="txtExtraProtected" runat="server" Width="50"/> </td>
        </tr>
        <tr>
            <td>
                拒收订单配送费计算公式
            </td>
            <td>
                拒收订单配送费=<asp:TextBox ID="txtRefuseFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>×妥投配送费+<asp:TextBox ID="txtExtraRefuseFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                上门退订单配送费计算公式
            </td>
            <td>
                上门退订单配送费=<asp:TextBox ID="txtVisitReturnsFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>×妥投配送费
                + <asp:TextBox ID="txtExtraVisitReturnsFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                上门退拒收订单配送费计算公式
            </td>
            <td>
                上门退拒收订单配送费=<asp:TextBox ID="txtVisitReturnsVFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>×妥投配送费
                + <asp:TextBox ID="txtExtraVisitReturnsVFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                上门换订单配送费计算公式
            </td>
            <td>
                上门换订单配送费=<asp:TextBox ID="txtVisitChangeFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>×妥投配送费
                +<asp:TextBox ID="txtExtraVisitChangeFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                代收货款现金<span style="color:red">手续费</span>计算公式
            </td>
            <td>
				<asp:RadioButton ID="rbCashPeriodRate" runat="server" Text="周期结" GroupName="CashRate" Checked="true" />
				<asp:RadioButton ID="rbCashMonthRate" runat="server" Text="月结" GroupName="CashRate" />
                代收货款现金手续费=<asp:TextBox ID="txtReceiveFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>×代收货款现金
                + <asp:TextBox ID="txtExtraReceiveFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                代收货款POS<span style="color:red">手续费</span>计算公式
            </td>
            <td>
				<asp:RadioButton ID="rbPosPeriodRate" runat="server" Text="周期结" GroupName="PosRate" Checked="true" />
				<asp:RadioButton ID="rbPosMonthRate" runat="server" Text="月结" GroupName="PosRate" />
                代收货款POS手续费=<asp:TextBox ID="txtReceivePosFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>×代收货款POS机刷卡金额
                +<asp:TextBox ID="txtExtraReceivePosFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                代收货款现金<span style="color:red">服务费</span>手续费计算公式
            </td>
            <td>
				<asp:RadioButton ID="rbCashPeriodService" runat="server" Text="周期结" GroupName="CashService" Checked="true" />
				<asp:RadioButton ID="rbCashMonthService" runat="server" Text="月结" GroupName="CashService" />
                代收货款现金服务费=<asp:TextBox ID="txtCashServiceFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>×代收货款现金
                +<asp:TextBox ID="txtExtraCashServiceFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                代收货款POS<span style="color:red">服务费</span>手续费计算公式
            </td>
            <td>
				<asp:RadioButton ID="rbPosPeriodService" runat="server" Text="周期结" GroupName="PosService" Checked="true" />
				<asp:RadioButton ID="rbPosMonthService" runat="server" Text="月结" GroupName="PosService" />
                代收货款POS服务费=<asp:TextBox ID="txtPOSServiceFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>×代收货款POS机刷卡金额
                + <asp:TextBox ID="txtExtraPOSServiceFee" runat="server" Width="40px" CssClass="text"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="right" style="padding-right:60px;">
               <asp:RadioButton ID="RBIsCategory" runat="server" Text="按货物品类结算" GroupName="Category"  />
				<asp:RadioButton ID="RBNoCategory" runat="server" Text="非货物品类结算" GroupName="Category" />
                &nbsp;&nbsp;
                <asp:Label ID="lbEffect" runat="server">生效时间</asp:Label>
                <asp:TextBox ID="txtEffectDate" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"></asp:TextBox>
                <asp:Button ID="btnSave" runat="server" Text="保存" OnClick="btnSave_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
