<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IncomeAreaExpressLevelSet.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.AreaExpressLevelSetIncome" MasterPageFile="~/Main/main.Master" Theme="default" %>

<%@ Register Src="~/UserControl/PCASerach.ascx" TagName="UCPCASerach" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc5" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    商家区域类型设置
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
	<script language="javascript" type="text/javascript" >
	    var dbl_click = false;

	    function JudgeInput() {
	        var areaType = $('#<%=this.ddlAreaType.ClientID %>').val();
	        if (areaType == "") {
	            alert("未选择区域类型");
	            return false;
	        }

	        return true;
	    }

	    function ShowCenterRight(display) {
	        $("#tdCenter").css("display", display);
	        $("#tdRight").css("display", display);
	    }

	    function fnColumnOnClick(rowIndex) {
	        var index = parseInt(rowIndex) + 2
	        if (parseInt(index) < 10)
	            index = "0" + index;
	        setTimeout("if(dbl_click){{dbl_click=false;}}else{{{__doPostBack('ctl00$body$gvArea$ctl" + index + "$sc','')}}};", 1000 * 0.01);
	    }
	    function fnCheckAllList(LE, E) {
	        LE.find('input:checkbox').attr('checked', E.checked)
	    }
	</script>
	<table cellpadding="0" cellspacing="0" border="0">
		<tr>
			<td style="width:400px;" valign="top">
				<table id="Search" style=" margin:5px;">
					<tr>
						<td>省市区：</td>
						<td><uc1:UCPCASerach runat="server" ID="ucPCASerach" /></td>
						<td></td>
					</tr>
					<tr>
						<td>商家：</td>
						<td>
							<asp:DropDownList ID="ddlMerchant" runat="server">
							</asp:DropDownList>
						</td>
						<td>
							<asp:Button ID="btnSearch" runat="server" Text="查询" onclick="btnSearch_Click" />
						</td>
					</tr>
					<tr>
						<td colspan="3">
							<asp:Button ID="btnExprotDownLoad" runat="server" Text="导入模板下载" 
								onclick="btnExprotDownLoad_Click" />
							<asp:FileUpload ID="fuExprot" runat="server" />
							<asp:Button ID="btnExprot" runat="server" Text="导入" onclick="btnExprot_Click" />
						</td>
					</tr>
				</table>
				<asp:GridView ID="gvArea" runat="server" AutoGenerateColumns="false" DataKeyNames="AreaID"
					>
					<Columns>
						<asp:TemplateField HeaderText="选择">
							<HeaderTemplate>
								<input type="checkbox" id="cbCheckAll" onclick="fnCheckAllList($('#<%= gvArea.ClientID %>'),this)" />全选
							</HeaderTemplate>
							<ItemTemplate>
								<asp:CheckBox ID="cbCheckBox" runat="server" />
							</ItemTemplate>
						</asp:TemplateField>
						<asp:BoundField HeaderText="ProvinceID" DataField="ProvinceID" Visible="false"/>
						<asp:BoundField DataField="ProvinceName" HeaderText="省份" />
						<asp:BoundField HeaderText="CityID" DataField="CityID" Visible="false"/>
						<asp:BoundField DataField="CityName" HeaderText="城市" />
						<asp:BoundField HeaderText="AreaID" DataField="AreaID" Visible="false"/>
						<asp:BoundField DataField="AreaName" HeaderText="区县" />
						<asp:BoundField DataField="IsAreaType" HeaderText="有无维护" />
						<asp:TemplateField Visible="false">
							<HeaderTemplate>查看</HeaderTemplate>
							<ItemTemplate>
								<asp:LinkButton runat="server" ID="sc" OnCommand="command" Text="查看" 
								CommandArgument='<%#Container.DataItemIndex%>' CommandName='<%#Container.DataItemIndex%>'></asp:LinkButton>
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
				</asp:GridView>
			</td>
			<td style="width:250px; display:block; text-align:center;" valign="top" id="tdCenter">
				<div style="margin:50px 5px 5px 5px;">
					<asp:Label ID="AreaDisplay" runat="server" Font-Bold="true" Font-Size="Large" ForeColor="Red"></asp:Label>
					<table style="text-align:center; margin-left:20px;">
						<tr>
							<td align="right">配送公司：</td>
							<td align="left"><asp:TextBox ID="txtExpressCompany" ReadOnly="true" runat="server" Text="" Width="250px"></asp:TextBox></td>
						</tr>
						<tr>
							<td align="right">区域类型：</td>
							<td align="left"><asp:DropDownList ID="ddlAreaType" runat="server"></asp:DropDownList></td>
						</tr>
						<tr>
							<td align="right">商家：</td>
							<td align="left"><asp:DropDownList ID="ddlMerchant1" runat="server">
							</asp:DropDownList></td>
						</tr>
						<tr>
							<td align="right">分拣中心：</td>
							<td align="left">
								<asp:DropDownList ID="ddlWareHouse" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
					</table>

					<div style="padding-top:5px;">
						<asp:Button ID="btAddAreaType" OnClientClick="return JudgeInput()" runat="server" Text="添加区域类型" 
							  onclick="btAddAreaType_Click" />
						<asp:Button ID="btUpdateAreaType" OnClientClick="return JudgeInput()" 
							runat="server" Text="更新区域类型" onclick="btUpdateAreaType_Click" />
					</div>
				</div>
			</td>
			<td style="width:400px; display:block;" valign="top" id="tdRight">
				<asp:Button ID="btDelete" runat="server" Text="删除" onclick="btDelete_Click" />
				<asp:GridView ID="gvAreaType" runat="server" AutoGenerateColumns="false" DataKeyNames="AutoID">
					<Columns>
						<asp:TemplateField HeaderText="选择">
							<HeaderTemplate>
								<input type="checkbox" id="cbCheckAll" onclick="fnCheckAllList($('#<%= gvAreaType.ClientID %>'),this)" />全选
							</HeaderTemplate>
							<ItemTemplate>
								<asp:CheckBox ID="cbTypeCheckBox" runat="server" />
							</ItemTemplate>
						</asp:TemplateField>
						<asp:BoundField DataField="AutoID" HeaderText="AutoID" Visible="false"/>
						<asp:BoundField DataField="MerchantID" HeaderText="MerchantID" Visible="false"/>
						<asp:BoundField DataField="MerchantName" HeaderText="商家"/>
						<asp:BoundField DataField="ExpressCompanyName" HeaderText="配送公司" />
						<asp:BoundField DataField="ExpressCompanyID" HeaderText="WareHouseID" Visible="false"/>
						<asp:BoundField DataField="CompanyName" HeaderText="分拣中心" />
						<asp:BoundField DataField="AreaType" HeaderText="区域类型"/>
						<asp:BoundField DataField="EffectAreaType" HeaderText="待生效区域类型"/>
						<asp:BoundField DataField="EnableStr" HeaderText="生效状态"/>
						<asp:BoundField DataField="AuditStatusStr" HeaderText="审核状态"/>
					</Columns>
				</asp:GridView>
			</td>
		</tr>
        <tr>
            <td colspan="2">
                <div>
                    <uc5:UCPager ID="UCPager" runat="server" PageSize="50" />
                </div>
            </td>
        </tr>
	</table>
</asp:Content>