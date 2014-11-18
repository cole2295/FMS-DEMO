<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TypeRelationSet.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.TypeRelationSet" MasterPageFile="~/Main/main.Master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="head">
<script type="text/javascript">
	function fnOpenModalDialog(url, width, height) {
		window.showModalDialog(url, window, "dialogWidth=" + width + "px;dialogHeight=" + height + "px;center:yes;resizable:no;scroll:auto;help=no;location=no;status=no;");
	}
	function ClientConfirm(msg) {
	    return !msg ? false : window.confirm(msg);
	}

	function fnCheckAllList(LE, E) {
	    LE.find('input:checkbox').attr('checked', !E.checked)
	}

</script>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolderTitle">
    商家、配送商分类设置
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="body">
    <!--标题行-->
<span class="title" style="color: #4474BB">
    分类添加          
</span>
<table>
    <tr>
        <td>分类源</td>
        <td>
            <asp:DropDownList ID="ddlTypeSource" runat="server">
            </asp:DropDownList>
        </td>
        <td>
            <asp:Button ID="btSearchType" runat="server" Text="查询分类" 
                onclick="btSearchType_Click" />
            <asp:Button ID="btAddType" runat="server" Text="新增分类" 
                onclick="btAddType_Click" />
            <asp:Button ID="btUpdateType" runat="server" Text="修改分类" 
                onclick="btUpdateType_Click" />
        </td>
    </tr>
</table>
<asp:GridView ID="gvTypeSoureList" runat="server" AutoGenerateColumns="false" DataKeyNames="CodeNo,CodeType">
    <Columns>
		<asp:TemplateField HeaderText="选择">
			<HeaderTemplate>
				<input type="checkbox" id="cbCheckAll" onclick="fnCheckAllList($('#<%= gvTypeSoureList.ClientID %>'),this)" />全选
			</HeaderTemplate>
			<ItemTemplate> 
				<asp:CheckBox ID="cbCheckBox" runat="server" />
			</ItemTemplate>
		</asp:TemplateField>
		<asp:BoundField HeaderText="分类名称" DataField="CodeDesc"/>
        <asp:BoundField HeaderText="是否启用" DataField="Enable"/>
    </Columns>
</asp:GridView>
<span class="title" style="color: #4474BB">
    分类设置          
</span>
<table>
    <tr>
        <td>分类源</td>
        <td>
            <asp:DropDownList ID="ddlTypeSource1" runat="server">
            </asp:DropDownList>
        </td>
        <td>
            <asp:Button ID="btSearchRelation" runat="server" Text="查询设置" 
                onclick="btSearchRelation_Click" />
            <asp:DropDownList ID="ddlType" runat="server"></asp:DropDownList>
            <asp:Button ID="btAddSetType" runat="server" Text="选中设置" 
                onclick="btAddSetType_Click" />
            <span style="color:Red;">已经设置的再次设置遵循最后一次设置的</span>
        </td>
    </tr>
</table>
<asp:GridView ID="gvTypeRelationList" runat="server" AutoGenerateColumns="false" DataKeyNames="TypeRelationKid,RelationID">
    <Columns>
		<asp:TemplateField HeaderText="选择">
			<HeaderTemplate>
				<input type="checkbox" id="cbCheckAll" onclick="fnCheckAllList($('#<%= gvTypeRelationList.ClientID %>'),this)" />全选
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