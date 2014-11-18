<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IncomeAreaExpressLevelAuditV2.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.IncomeAreaExpressLevelAuditV2" MasterPageFile="~/BasicSetting/IncomeAreaExpressLevel.Master" %>

<%@ MasterType VirtualPath="~/BasicSetting/IncomeAreaExpressLevel.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHead" runat="server">
<script language="javascript" type="text/javascript">
    function fnJudgeInputDoDate() {
        var doDate = $("#<%= txtDoDate.ClientID%>").val();
        if (doDate == "") {
            alert("生效日期不能为空");
            return false;
        }
        var re = /-/g;
        var dd = doDate.replace(re, "").substring(0, 8);
        var dateNow = new Date();
        var m = dateNow.getMonth();
        var d=dateNow.getDate();
        var dn = dateNow.getFullYear().toString() + (m < 10 ? "0" + (m + 1) : m + 1).toString() + (d < 10 ? "0" + d : d).toString();
        //alert(dd + " " + dn);
        if (dd <=dn) {
            alert("生效日期必须大于当天日期");
            return false;
        }

        return JudgeInput();

        return true;
    }
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    商家区域类型审批
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="OperateContent" runat="server">
生效日期：<asp:TextBox ID="txtDoDate" runat="server" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"></asp:TextBox>
<asp:Button ID="btnAudit" runat="server" Text="审批" onclick="btnAudit_Click" OnClientClick="return fnJudgeInputDoDate();ClientConfirm('确定审核选择的区域类型');"/>
<asp:Button ID="btnReset" runat="server" Text="置回" onclick="btnReset_Click" OnClientClick="return ClientConfirm('确定置回选择的区域类型');"/>

</asp:Content>