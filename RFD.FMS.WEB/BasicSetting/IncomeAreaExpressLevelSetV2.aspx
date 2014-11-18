<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IncomeAreaExpressLevelSetV2.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.IncomeAreaExpressLevelSetV2" MasterPageFile="~/BasicSetting/IncomeAreaExpressLevel.Master" Theme="default" %>

<%@ MasterType VirtualPath="~/BasicSetting/IncomeAreaExpressLevel.Master" %>
<%@ Register Src="~/UserControl/GoodsCategoryCheckBox.ascx" TagName="UCGoodsCategoryCheckBox" TagPrefix="uc1" %>
<%@ Register TagPrefix="uc2" TagName="ucexpresscompanytv" Src="~/UserControl/ExpressCompanyTV.ascx" %>
<%@ Register TagPrefix="uc3"  TagName="ucexpresscompanytv" Src="~/UserControl/ExpressCompanyChooseTV.ascx"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHead" runat="server">
    <script language="javascript" type="text/javascript" >

    function JudgeInputEdit() {
        var areaType = $('#<%=this.ddlSetAreaType.ClientID %>').val();
        if (areaType == "") {
            alert("未选择区域类型");
            return false;
        }

        return JudgeInput();

        return true;
    }
   
</script>
 </asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    商家区域类型设置
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="OperateContent" runat="server">
  
    <div>
    <a href="../UpFile/收入区域类型导入模板.xls">导入模板下载</a>
	<asp:FileUpload ID="fuExprot" runat="server" />
	<asp:Button ID="btnImport" runat="server" Text="导入" onclick="btnImport_Click" />
    <asp:Button ID="btnDeleteAreaType" runat="server" Text="删除" 
        onclick="btnDeleteAreaType_Click" />
</div>
<table>
<tr>
    <td>区域类型：</td>
    <td><asp:DropDownList ID="ddlSetAreaType" runat="server"></asp:DropDownList></td>
    <td><asp:Label ID="lbCategory" runat="server" Visible="false">货物品类：</asp:Label></td>
    <td><uc1:UCGoodsCategoryCheckBox ID="UCGoodsCategoryCheckBoxSet" runat="server" Visible="false" /></td>
     <td><uc3:ucexpresscompanytv ID="UCExpressCompanyTV" runat="server" 
                 ExpressLoadType="ThirdCompany" ExpressTypeSource="nk_Express" 
                 ExpressCompanyShowCheckBox="False"  />
         </td>
    <td>
        <asp:Button ID="btnAddAreaType" runat="server" Text="选中设置区域类型配送商" 
                onclick="btnAddAreaType_Click" OnClientClick="return JudgeInputEdit()" />
        <asp:Button ID="btnEditAreaType" runat="server" Text="选中更新区域类型" 
        onclick="btnEditAreaType_Click" OnClientClick="return JudgeInput()" ToolTip="更新时与货物品类无关" />
         <asp:Button ID="btnEditExpress" runat="server" Text="选中更新配送商" 
        onclick="btnEditExpress_Click" OnClientClick="return JudgeInput()" ToolTip="更新时与货物品类、区域无关" />
        <asp:Label ID="lbSetMsg" runat="server" ForeColor="Red"></asp:Label>
    </td>
</tr>
</table>
</asp:Content>