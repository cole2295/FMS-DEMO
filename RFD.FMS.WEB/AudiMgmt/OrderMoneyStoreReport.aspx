<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
	Theme="default" CodeBehind="OrderMoneyStoreReport.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.OrderMoneyStoreReport" %>

<%@ Register Src="~/UserControl/SelectStation.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

	<script type="text/javascript">
		$(function() {
			//�ж��Ƿ���ʾ�̼���Դ
			showMerchant({
				source: $("#<%=ddlOrderSource.ClientID %>"),
				merchant: $("#<%=ddlMerchantList.ClientID %>").parent()
			});
			render();
		})
		function check() {
			var stationTime = $.trim($("#<%=txtIntoStationTime.ClientID %>").val());
			var signTime = $.trim($("#<%=txtSignTime.ClientID %>").val());
			var beginTime = $.trim($("#<%=txtBegTime.ClientID %>").val());
			var endTime = $.trim($("#<%=txtEndTime.ClientID %>").val());
			if (stationTime != "" && !isdate(stationTime)) {
				alert("������Ϸ�����վʱ�䣡");
				return false;
			}
			if (signTime != "" && !isdate(signTime)) {
				alert("������Ϸ���ǩ��ʱ�䣡");
				return false;
			}
			if (beginTime != "" && !isdate(beginTime)) {
				alert("������Ϸ��ķ�����ʼʱ�䣡");
				return false;
			}
			if (endTime != "" && !isdate(endTime)) {
				alert("������Ϸ��ķ�������ʱ�䣡");
				return false;
			}
			if (beginTime == "" || endTime == "") {
				return true;
			}
			var seconds = seconddiff(beginTime, endTime);
			if (seconds < 0) {
				alert("��������ʱ�������ڵ��ڷ�����ʼʱ�䣡");
				return false;
			}
			return true;
		}
	</script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
���״̬��ѯ
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table cellpadding="0" cellspacing="5" border="0" width="100%">
		<tr>
			<td align="right">
				�˵��ţ�
			</td>
			<td>
				<asp:TextBox ID="txtWayBillNO" runat="server"></asp:TextBox>
			</td>
			<td align="right">
				ѡ��վ�㣺
			</td>
			<td>
				<uc:SelectStation ID="station" runat="server" />
			</td>
			<td align="right">
				���״̬��
			</td>
			<td>
				<asp:DropDownList ID="ddlInBoundStatus" runat="server">
					<asp:ListItem Value="">��ѡ��</asp:ListItem>
					<asp:ListItem Value="0">δ���</asp:ListItem>
					<asp:ListItem Value="1">�����</asp:ListItem>
				</asp:DropDownList>
			</td>
			<td align="right">
				�տ�״̬��
			</td>
			<td>
				<asp:DropDownList ID="ddlMoneyStatus" runat="server">
					<asp:ListItem Value="">��ѡ��</asp:ListItem>
					<asp:ListItem Value="0">δ����</asp:ListItem>
					<asp:ListItem Value="1">�Ѹ���</asp:ListItem>
				</asp:DropDownList>
			</td>
			<td>
				������Դ��
			</td>
			<td>
				<asp:DropDownList ID="ddlOrderSource" runat="server">
				</asp:DropDownList>
			</td>
			<td>
				�̼���Դ��
				<asp:DropDownList ID="ddlMerchantList" runat="server">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td align="right">
				��վʱ�䣺
			</td>
			<td>
				<asp:TextBox ID="txtIntoStationTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>
			</td>
			<td align="right">
				ǩ��ʱ�䣺
			</td>
			<td>
				<asp:TextBox ID="txtSignTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>
			</td>
			<td align="right">
				����ʱ�䣺
			</td>
			<td colspan="6">
				<asp:TextBox ID="txtBegTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>
				��<asp:TextBox ID="txtEndTime" runat="server" CssClass="Wdate text" autocomplete="off"></asp:TextBox></td>
		</tr>
		<tr>
			<td colspan="11" align="right">
                <asp:Button ID="btnQuery" runat="server" Text="��ѯ" OnClick="btnQuery_Click" OnClientClick="return check();" />&nbsp;&nbsp;
				<asp:Label ID="lblMessage" runat="server"></asp:Label>
				<a href="../UpFile/������ѯģ��.xls">����ģ��</a>
				<input type="file" style="width: 180px" runat="server" id="txtFile" title="����������ģ�嵽���أ�Ȼ����д��صĶ�����Ϣ�ٽ��е��룡" />
                <asp:RadioButton ID="rbWaybillno" runat="server" GroupName="rbSearchType" Text="�˵���" Checked="true" />
                <asp:RadioButton ID="rbOrderId" runat="server" GroupName="rbSearchType" Text="������" />
				<asp:Button ID="btnBatchQuery" runat="server" Text="������ѯ" OnClick="btnBatchQuery_Click"
					OnClientClick="return check();" Visible="false" />&nbsp;&nbsp;
                <asp:Button ID="btnBatchQueryV2" runat="server" Text="������ѯV2"
					OnClientClick="return check();" onclick="btnBatchQueryV2_Click" />&nbsp;&nbsp;
				
				<asp:Button ID="btnReportData" runat="server" Text="����" OnClick="btnReportData_Click" />
			</td>
		</tr>
		<tr>
			<td colspan="11">
				<fieldset>
					<legend>����վ��������</legend>
					<table style="width: 100%; border-width: 0px;">
						<tr>
							<td>
								<label for="">
									���տ����
								</label>
								<asp:Label ID="lblMoneyOrderCount" runat="server"></asp:Label>
							</td>
							<td>
								<label for="">
									δ�տ����
								</label>
								<asp:Label ID="lblNoMoneyOrderCount" runat="server"></asp:Label>
							</td>
							<td>
								<label for="">
									����ⵥ����
								</label>
								<asp:Label ID="lblInBoundCount" runat="server"></asp:Label>
							</td>
							<td>
								<label for="">
									δ��ⵥ����
								</label>
								<asp:Label ID="lblNotInBoundCount" runat="server"></asp:Label>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
	</table>
	<asp:GridView ID="gv" runat="server" Width="100%" AutoGenerateColumns="false" AllowPaging="false"
		EnableViewState="true" OnRowEditing="gv_RowEditing" DataKeyNames="WaybillSignInfoID">
		<HeaderStyle Wrap="false" />
		<RowStyle Wrap="false" />
		<Columns>
			<asp:BoundField DataField="RowNumber" HeaderText="���" />
			<asp:BoundField DataField="SendTime" HeaderText="����ʱ��" />
			<asp:BoundField DataField="IntoTime" HeaderText="��վʱ��" />
			<asp:BoundField DataField="BackStationTime" HeaderText="������ʱ��" />
			<asp:BoundField DataField="WaybillNO" HeaderText="�˵���" />
            <asp:BoundField DataField="CustomerOrder" HeaderText="������" />
			<asp:BoundField DataField="WaybillTypeName" HeaderText="��������" />
			<asp:BoundField DataField="StatusName" HeaderText="״̬" />
			<asp:BoundField DataField="NeedAmount" HeaderText="Ӧ�ս��" />
			<asp:BoundField DataField="NeedBackAmount" HeaderText="Ӧ�˽��" />
			<asp:BoundField DataField="WayBillInfoWeight" HeaderText="��������" />
			<asp:BoundField DataField="AcceptType" HeaderText="���ʽ" />
			<asp:BoundField DataField="EmployeeName" HeaderText="����Ա" />
			<asp:BoundField DataField="FinancialStatus" HeaderText="�����տ�״̬" />
			<asp:BoundField DataField="FinancialTime" HeaderText="�տ�ȷ��ʱ��" />
			<asp:BoundField DataField="POSCode" HeaderText="POS���ն˺�" />
			<asp:BoundField DataField="BackStatusName" HeaderText="���״̬" />
			<asp:BoundField DataField="ISBackBound" HeaderText="�Ƿ����" />
			<asp:BoundField DataField="CompanyName" HeaderText="վ��" />
			<asp:BoundField DataField="MerchantName" HeaderText="�̼�����" />
			<asp:CommandField EditText="ȷ��" HeaderText="ȷ��" ShowEditButton="true" />
		</Columns>
		<EmptyDataTemplate>
			<label id="NOData">
				û������</label>
		</EmptyDataTemplate>
	</asp:GridView>
	<uc2:Pager ID="pager" runat="server" />
	<asp:HiddenField ID="hidBatchData" runat="server" Value="0"/>

</asp:Content>
