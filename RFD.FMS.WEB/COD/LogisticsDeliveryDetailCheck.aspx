<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true" CodeBehind="LogisticsDeliveryDetailCheck.aspx.cs" Inherits="RFD.FMS.WEB.COD.LogisticsDeliveryDetailCheck" %>
<%@ Register Src="~/UserControl/ExpressCompanyTV.ascx" TagName="UCExpressCompanyTV"
    TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/MerchantSource.ascx" TagName="MerchantSource"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    $(document).ready(function () {

    });
    $(function () {
        var buttons = {
            btnCheck: $.dom("btnCheck", "input")
        };
        buttons.btnCheck.click(function () {
            $(document).progressDialog.showDialog("核对中，请稍后...");
        });
    });
      
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    发货明细核对
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="QuickSearchContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="SearchContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="body" runat="server">
    <table cellpadding="0" cellspacing="5" border="0" width="100%">
		<tr>
			<td colspan="3">
				<asp:Label ID="lblMessage" runat="server"></asp:Label>
				<a href="../UpFile/配送商发货导入模板.xlsx" style="color: Blue;">下载模板</a>
				<asp:FileUpload ID="fileUpload" runat="server" Style="width: 180px" ToolTip="请你先下载模板到本地，然后填写相关的信息再进行导入！" />
				<asp:Button ID="btnBatchQuery" runat="server" Text="导入" 
					title="请你先下载模板到本地，然后填写相关的信息再进行导入！" onclick="btnBatchQuery_Click" />
			</td>
		</tr>
		<tr>
			<td style="width: 30%;" valign="top">
				<fieldset>
					<legend>
						<asp:Label ID="lblInTitle" runat="server">导入列表</asp:Label></legend>
					<div>
						<table cellpadding="0" cellspacing="0" border="0" width="100%" style="height: 55px">
							<tr>
								<td>
									时间：<asp:TextBox ID="txtBeginTime" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"
										    runat="server"></asp:TextBox>至
                                          <asp:TextBox ID="txtEndTime" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"
											runat="server"></asp:TextBox>
								</td>
							</tr>
							<%--<tr>
								<td>配送公司：  
                                    <asp:DropDownList ID="CompanyDDL" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td>
									<asp:Button ID="btnCheck" runat="server" Text="核对" onclick="btnCheck_Click" />
								</td>
							</tr>--%>
                            <tr>
                             <td>配送商：
                                <uc1:UCExpressCompanyTV ID="DistributionCompany" runat="server" ExpressLoadType="ThirdCompany"
                                     ExpressTypeSource="nk_Express" ExpressCompanyShowCheckBox="False" /> </td>
                                
                                <td>
                                <td>
									<asp:Button ID="btnCheck" runat="server" Text="核对" onclick="btnCheck_Click" />
								</td>
                            </tr>
                            <tr>
                                 <td> 商家：
                                    <uc2:MerchantSource ID="MerchantSrc" runat="server" LoadDataType="All"/>
                                </td>
                            </tr>
						</table>
					</div>
					<div style="overflow: auto; width: 100%; height: 500px;">
					
					</div>
				</fieldset>
			</td>
			<td style="width: 35%;" valign="top">
				<fieldset>
					<legend>
						<asp:Label ID="lblSucTitle" runat="server">核对成功结果</asp:Label></legend>
					<div>
						<table cellpadding="0" cellspacing="0" border="0" width="100%" style="height: 55px">
							<tr>
								<td>
									<asp:Button ID="btnSucOutExcel" runat="server" Text="导出Excel"
                                        onclick="btnSucOutExcel_Click" />
								</td>
							</tr>
						</table>
					</div>
					<div style="overflow: auto; width: 100%; height: 500px;">
						<asp:Label ID="SuccLabelTip" runat="server"></asp:Label>
					</div>
				</fieldset>
			</td>
			<td style="width: 35%;" valign="top">
				<fieldset>
					<legend>
						<asp:Label ID="lblFailTitle" runat="server">核对失败结果</asp:Label></legend>
					<div>
						<table cellpadding="0" cellspacing="0" border="0" width="100%" style="height: 55px">
							<tr>
								<td>
									<asp:Button ID="btnFailExcel" runat="server" Text="导出Excel" 
                                        onclick="btnFailExcel_Click"  />
								</td>
							</tr>
						</table>
					</div>
					<div style="overflow: auto; width: 100%; height: 500px;">
						<asp:Label ID="FailLabelTips" runat="server"></asp:Label>
					</div>
				</fieldset>
			</td>
		</tr>
	</table>
</asp:Content>

