<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountPeriodSet.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.AccountPeriodSet" MasterPageFile="~/Main/main.Master" Theme="default" %>

<%@ Import Namespace="RFD.FMS.MODEL.BasicSetting" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
    <script language="javascript" type="text/javascript">
    function fnJudgeSetPeriodInput() {
        var ddlPeriodSource = $("#<%=ddlPeriodSource.ClientID %>").val();
        if (ddlPeriodSource == "") {
            alert("账期源必选");
            return false;
        }
        return true;
    }

    function fnOpenWindow(kid,isImitate) {
        if(!fnJudgeSetPeriodInput()) return false;
        fnOpenModalDialog('AccountPeriodEdit.aspx?periodSource=' + $("#<%=ddlPeriodSource.ClientID %>").val() + '&kid=' + kid + '&isImitate=' + isImitate, 620, 420);
    }
</script>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolderTitle">
    账期管理
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="body">
    <!--标题行-->
<span class="title" style="color: #4474BB">
    账期设置          
</span>
<table>
    <tr>
        <td>账期源</td>
        <td>
            <asp:DropDownList ID="ddlPeriodSource" runat="server"></asp:DropDownList>
        </td>
        <td>
            <asp:Button ID="btSearchPeriod" runat="server" Text="查询账期" 
                onclick="btSearchPeriod_Click"  />
            <input type="button" id="btAddPeriod" value="新增账期" onclick="fnOpenWindow('',0)" />
        </td>
    </tr>
</table>
<asp:GridView ID="gvPeriodSoureList" runat="server" AutoGenerateColumns="false" DataKeyNames="AccountPeriodKid">
    <Columns>
		<asp:TemplateField HeaderText="操作">
			<ItemTemplate>
				<a href="javascript:fnOpenWindow('<%#((AccountPeriod)Container.DataItem).AccountPeriodKid %>',0);">修改账期</a> /
                <a href="javascript:fnOpenWindow('<%#((AccountPeriod)Container.DataItem).AccountPeriodKid %>',1);">模拟账期</a>
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField HeaderText="账期名称" DataField="PeriodName"/>
        <asp:BoundField HeaderText="是否启用" DataField="IsDeletedStr"/>
    </Columns>
</asp:GridView>
<span class="title" style="color: #4474BB">
    账期关联          
</span>
<table>
    <tr>
        <td>账期源</td>
        <td>
            <asp:DropDownList ID="ddlPeriodSource1" runat="server">
            </asp:DropDownList>
            <asp:TextBox ID="merchantName" runat="server"></asp:TextBox>
        </td>
        <td>
            <asp:Button ID="btSearchRelation" runat="server" Text="查询设置" onclick="btSearchRelation_Click" />
            <asp:DropDownList ID="ddlType" runat="server"></asp:DropDownList>
            <asp:Button ID="btAddSetType" runat="server" Text="选中设置" onclick="btAddSetType_Click" />
            <asp:Button ID="btDeletePeriod" runat="server" Text="选中删除" 
                onclick="btDeletePeriod_Click" />
            <span style="color:Red;">已经设置的再选中设置会多次关联账期</span>
        </td>
    </tr>
</table>
<uc1:UCPager ID="UCPager" runat="server" PageSize="20" />
<asp:GridView ID="gvPeriodRelationList" runat="server" AutoGenerateColumns="false" DataKeyNames="TypeRelationKid,RelationID">
    <Columns>
		<asp:TemplateField HeaderText="选择">
			<HeaderTemplate>
				<input type="checkbox" id="cbCheckAll" onclick="fnCheckAllList($('#<%= gvPeriodRelationList.ClientID %>'),this)" />全选
			</HeaderTemplate>
			<ItemTemplate> 
				<asp:CheckBox ID="cbCheckBox" runat="server" />
			</ItemTemplate>
		</asp:TemplateField>
        <asp:BoundField HeaderText="分类名称" DataField="CodeDesc"/>
        <asp:BoundField HeaderText="源名称" DataField="RelationName"/>

    </Columns>
</asp:GridView>
</asp:Content>