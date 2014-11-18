<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
	CodeBehind="DelayOrderChecked.aspx.cs" Inherits="RFD.FMS.WEB.AudiMgmt.DelayOrderChecked"
	Theme="default" %>

<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>���������˶�</title>

	<script src="../Scripts/import/check.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
// <![CDATA[

        function import_onclick() {

        }

// ]]>
    </script>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
���������˶�
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
	<table cellpadding="0" cellspacing="5" border="0" width="100%">
		<tr>
			<td style="float: left;">
				<a href="../UpFile/������������ģ��.xlsx" style="color: blue;">����ģ��</a>
				<input type="file" style="width: 180px" runat="server" id="txtFile" title="����������ģ�嵽���أ�Ȼ����д��صĶ�����Ϣ�ٽ��е��룡" />
				<input id="import" type="button" value="�˶Ե���" title="����������ģ�嵽���أ�Ȼ����д��صĶ�����Ϣ�ٽ��е��룡"
					disabled="disabled" onclick="return import_onclick()" />
				<asp:Button ID="btnImport" runat="server" Text="�˶Ե���" OnClick="btnImport_Click" Style="display: none;" />
			</td>
			<%--<td colspan="2" style="width:100%;">
				������ʽ��<asp:RadioButtonList ID="rblExportFormat" runat="server" RepeatDirection="Horizontal"
					RepeatLayout="Flow">
				</asp:RadioButtonList>
			</td>--%>
		</tr>
		<tr>
			<td valign="top">
			    <asp:Literal ID="Literal12" runat="server"></asp:Literal>
				<asp:Literal ID="ltlResult" runat="server"></asp:Literal>
			</td>
		</tr>
		<%--<tr>
			<td valign="top" width="30%">
				<fieldset>
					<legend>�����б�<asp:Literal ID="ltlTotalInfo" runat="server"></asp:Literal></legend>
					<div>
						<asp:Button ID="btnCheck" runat="server" Text="�˶�" OnClick="btnCheck_Click" />
					</div>
					<div style="overflow: auto; width: 350px; height: 365px; padding-top: 5px;">
						<table border="0">
							<tr>
								<td>
									<asp:GridView ID="gvTotalData" runat="server" AutoGenerateColumns="false" AllowPaging="false"
										Width="100%">
										<HeaderStyle Wrap="false" />
										<RowStyle Wrap="false" />
										<EmptyDataRowStyle HorizontalAlign="Center" />
										<Columns>
											<asp:BoundField DataField="ID" HeaderText="���" />
											<asp:BoundField DataField="CompanyName" HeaderText="����վ" />
											<asp:BoundField DataField="WaybillNo" HeaderText="������" />
											<asp:BoundField DataField="Amount" HeaderText="�����ܼ�" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="FactAmount" HeaderText="���տ�" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="NeedAmount" HeaderText="Ӧ�տ�" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="WayBillInfoWeight" HeaderText="��������" />
											<asp:BoundField DataField="WayBillBoxNo" HeaderText="��װ���ͺ�" />
											<asp:BoundField DataField="WarehouseName" HeaderText="�����" />
											<asp:BoundField DataField="ReceiveAddress" HeaderText="�ջ��˵�ַ" />
											<asp:BoundField DataField="CreatTime" HeaderText="��������" />
											<asp:BoundField DataField="WaybillStatus" HeaderText="����״̬" />
										</Columns>
									</asp:GridView>
								</td>
							</tr>
							<tr>
								<td>
									<uc2:Pager ID="totalPager" runat="server" />
									<asp:HiddenField ID="hidTotalCount" runat="server" Value="0" />
								</td>
							</tr>
						</table>
					</div>
				</fieldset>
			</td>
			<td valign="top" width="35%">
				<fieldset>
					<legend>��Ͷ/�������/�˻�����ⶩ��<asp:Literal ID="ltlSuccessInfo" runat="server"></asp:Literal></legend>
					<div>
						<asp:Button ID="btnSuccessOrder" runat="server" Text="��������" OnClick="btnSuccessOrder_Click" />
					</div>
					<div style="overflow: auto; width: 370px; height: 365px; padding-top: 5px;">
						<table border="0">
							<tr>
								<td>
									<asp:GridView ID="gvSuccessOrders" runat="server" AutoGenerateColumns="false" AllowPaging="false"
										Width="100%">
										<HeaderStyle Wrap="false" />
										<RowStyle Wrap="false" />
										<EmptyDataRowStyle HorizontalAlign="Center" />
										<Columns>
											<asp:BoundField DataField="ID" HeaderText="���" />
											<asp:BoundField DataField="CompanyName" HeaderText="����վ" />
											<asp:BoundField DataField="WaybillNo" HeaderText="������" />
											<asp:BoundField DataField="Amount" HeaderText="�����ܼ�" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="FactAmount" HeaderText="���տ�" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="NeedAmount" HeaderText="Ӧ�տ�" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="WayBillInfoWeight" HeaderText="��������" />
											<asp:BoundField DataField="WayBillBoxNo" HeaderText="��װ���ͺ�" />
											<asp:BoundField DataField="WarehouseName" HeaderText="�����" />
											<asp:BoundField DataField="ReceiveAddress" HeaderText="�ջ��˵�ַ" />
											<asp:BoundField DataField="CreatTime" HeaderText="��������" />
											<asp:BoundField DataField="WaybillStatus" HeaderText="����״̬" />
										</Columns>
									</asp:GridView>
								</td>
							</tr>
							<tr>
								<td>
									<uc2:Pager ID="successPager" runat="server" />
									<asp:HiddenField ID="hidSuccessCount" runat="server" Value="0" />
								</td>
							</tr>
						</table>
					</div>
				</fieldset>
			</td>
			<td valign="top" width="35%">
				<fieldset>
					<legend>��������<asp:Literal ID="ltlDelayInfo" runat="server"></asp:Literal></legend>
					<div>
						<asp:Button ID="btnDelayOrder" runat="server" Text="��������" OnClick="btnDelayOrder_Click" />
					</div>
					<div style="overflow: auto; width: 370px; height: 365px; padding-top: 5px;">
						<table border="0">
							<tr>
								<td>
									<asp:GridView ID="gvDelayOrders" runat="server" AutoGenerateColumns="false" AllowPaging="false"
										Width="100%">
										<HeaderStyle Wrap="false" />
										<RowStyle Wrap="false" />
										<EmptyDataRowStyle HorizontalAlign="Center" />
										<Columns>
											<asp:BoundField DataField="ID" HeaderText="���" />
											<asp:BoundField DataField="CompanyName" HeaderText="����վ" />
											<asp:BoundField DataField="WaybillNo" HeaderText="������" />
											<asp:BoundField DataField="Amount" HeaderText="�����ܼ�" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="FactAmount" HeaderText="���տ�" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="NeedAmount" HeaderText="Ӧ�տ�" DataFormatString="{0:#,##0.##}" />
											<asp:BoundField DataField="WayBillInfoWeight" HeaderText="��������" />
											<asp:BoundField DataField="WayBillBoxNo" HeaderText="��װ���ͺ�" />
											<asp:BoundField DataField="WarehouseName" HeaderText="�����" />
											<asp:BoundField DataField="ReceiveAddress" HeaderText="�ջ��˵�ַ" />
											<asp:BoundField DataField="CreatTime" HeaderText="��������" />
											<asp:BoundField DataField="WaybillStatus" HeaderText="����״̬" />
										</Columns>
									</asp:GridView>
								</td>
							</tr>
							<tr>
								<td>
									<uc2:Pager ID="delayPager" runat="server" />
									<asp:HiddenField ID="hidDelayCount" runat="server" Value="0" />
								</td>
							</tr>
						</table>
					</div>
				</fieldset>
			</td>
		</tr>--%>
	</table>
</asp:Content>
