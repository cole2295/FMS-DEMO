<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CODDetailView.aspx.cs" Inherits="RFD.FMS.WEB.COD.CODDetailView" Theme="default" MasterPageFile="~/Main/main.Master" %>

<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    $(document).ready(function() {
        $('#<%=btSearch_D_1.ClientID %>').attr("onclick", "MarkShowEventButton(this)");
        $('#<%=btSearch_D_2.ClientID %>').attr("onclick", "MarkShowEventButton()");
    });

    function MarkShowEventButton(E) {
        $('#<%=pOperator.ClientID %>').find('input:submit').each(
            function (i) {
                $(this).removeAttr("style");
            }
        );
        if(E)
            E.style.color = "red";
    }
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
<asp:Panel ID="pOperator" runat="server">
	<asp:Button ID="btSearch_D_1" runat="server" Text="查询普通发货" 
	onclick="btSearch_D_1_Click" />
	<asp:Button ID="btSearch_D_2" runat="server" Text="查询上门换发货" 
	onclick="btSearch_D_2_Click" />
	<asp:Button ID="btSearch_R_1" runat="server" Text="查询普通拒收" 
	onclick="btSearch_R_1_Click" />
	<asp:Button ID="btSearch_R_2" runat="server" Text="查询上门换拒收" 
	onclick="btSearch_R_2_Click" />
	<asp:Button ID="btSearch_V" runat="server" Text="查询上门退货" 
	onclick="btSearch_V_Click" />

<div>
	<asp:Button ID="btExport_D_1" runat="server" Text="导出普通发货" 
		onclick="btExport_D_1_Click" />
	<asp:Button ID="btExport_D_2" runat="server" Text="导出上门换发货" 
		onclick="btExport_D_2_Click" />
	<asp:Button ID="btExport_R_1" runat="server" Text="导出普通拒收" 
		onclick="btExport_R_1_Click" />
	<asp:Button ID="btExport_R_2" runat="server" Text="导出上门换拒收" 
		onclick="btExport_R_2_Click" />
	<asp:Button ID="btExport_V" runat="server" Text="导出上门退货" 
		onclick="btExport_V_Click" />
</div>
</asp:Panel>
	<uc1:UCPager ID="pager" runat="server" />
	<asp:Label ID="noData" runat="server" ForeColor="Red"></asp:Label>
	<asp:GridView ID="gvList" runat="server" AutoGenerateColumns="false" DataKeyNames="订单号">
		<Columns>
			<asp:BoundField HeaderText="序号" DataField="序号" />
			<asp:BoundField HeaderText="类型" DataField="类型" />
			<asp:BoundField HeaderText="订单号" DataField="订单号" />
			<asp:BoundField HeaderText="发货仓库" DataField="发货仓库" />
			<asp:BoundField HeaderText="发货时间" DataField="发货时间" />
			<asp:BoundField HeaderText="入库仓库" DataField="入库仓库" />
			<asp:BoundField HeaderText="入库时间" DataField="入库时间" />
			<asp:BoundField HeaderText="配送商" DataField="配送商" />
			<asp:BoundField HeaderText="商家" DataField="商家" />
			<asp:BoundField HeaderText="区域类型" DataField="区域类型" />
			<asp:BoundField HeaderText="重量" DataField="重量" />
			<asp:BoundField HeaderText="运费" DataField="运费" />
			<asp:BoundField HeaderText="计算公式" DataField="计算公式" />
			<asp:BoundField HeaderText="地址" DataField="地址" />
		</Columns>
	</asp:GridView>
</asp:Content>