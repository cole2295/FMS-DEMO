<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AreaExpressLevelSet.aspx.cs"
    Inherits="RFD.FMS.WEB.BasicSetting.AreaExpressLevelSet" MasterPageFile="~/Main/main.Master"
    Theme="default" %>

<%@ Register Src="~/UserControl/PCASerach.ascx" TagName="UCPCASerach" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="UCSelectStationTmp"
    TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="UCSelectStation"
    TagPrefix="uc3" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTVTmp"
    TagPrefix="uc4" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV"
    TagPrefix="uc5" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPager" TagPrefix="uc5" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="UCPagerType" TagPrefix="uc6" %>
<%@ Register TagPrefix="webdiyer" Namespace="Wuqi.Webdiyer" Assembly="AspNetPager, Version=7.0.2.0, Culture=neutral, PublicKeyToken=fb0a0fe055d40fd4" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    配送商区域类型设置
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
    <script language="javascript" type="text/javascript">
        var dbl_click = false;

        function JudgeInput() {
            var areaType = $('#<%=this.ddlAreaType.ClientID %>').val();
            if (areaType == "") {
                alert("区域类型必选");
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
            setTimeout("if(dbl_click){{dbl_click=false;}}else{{{__doPostBack('ctl00$body$gvArea$ctl" + index + "$sc','')}}};", 1000 * 0.1);
        }

        function fnCheckAllList(LE, E) {
            LE.find('input:checkbox').attr('checked', E.checked)
        }
    </script>
    <table cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td style="width: 400px;" valign="top">
                <table id="Search" style="margin: 5px;">
                    <tr>
                        <td>
                            省市区：
                        </td>
                        <td colspan="3">
                            <uc1:UCPCASerach runat="server" ID="ucPCASerach" />
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            配送商：
                        </td>
                        <td>
                            <uc2:UCSelectStationTmp runat="server" ID="ucSelectStationTmp" LoadDataType="OnlySite" />
                        </td>
                        <td>
                            商家：
                        </td>
                        <td>
                            <uc4:UCMerchantSourceTVTmp ID="UCMerchantSourceTVTmp" runat="server" MerchantLoadType="All"
                                MerchantShowCheckBox="False" MerchantTypeSource="jc_Merchant_Expense" />
                        </td>
                        <td>
                            <asp:Button ID="btnSearch" runat="server" Text="查询" OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <asp:Button ID="btnExprotDownLoad" runat="server" Text="导入模板下载" OnClick="btnExprotDownLoad_Click" />
                            <asp:FileUpload ID="fuExprot" runat="server" />
                            <asp:Button ID="btnExprot" runat="server" Text="导入" OnClick="btnExprot_Click" />
                        </td>
                    </tr>
                </table>
                <asp:GridView ID="gvArea" runat="server" AutoGenerateColumns="false" DataKeyNames="AreaID">
                    <Columns>
                        <asp:TemplateField HeaderText="选择">
                            <HeaderTemplate>
                                <input type="checkbox" id="cbCheckAll" onclick="fnCheckAllList($('#<%= gvArea.ClientID %>'),this)" />全选
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="cbCheckBox" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="ProvinceID" DataField="ProvinceID" Visible="false" />
                        <asp:BoundField DataField="ProvinceName" HeaderText="省份" />
                        <asp:BoundField HeaderText="CityID" DataField="CityID" Visible="false" />
                        <asp:BoundField DataField="CityName" HeaderText="城市" />
                        <asp:BoundField HeaderText="AreaID" DataField="AreaID" Visible="false" />
                        <asp:BoundField DataField="AreaName" HeaderText="区县" />
                        <asp:BoundField DataField="IsAreaType" HeaderText="有无维护" />
                        <asp:TemplateField Visible="false">
                            <HeaderTemplate>
                                查看</HeaderTemplate>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="sc" OnCommand="command" Text="查看" CommandArgument='<%#Container.DataItemIndex%>'
                                    CommandName='<%#Container.DataItemIndex%>'></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <uc5:UCPager ID="UCPager1" runat="server" PageSize="20" />
            </td>

            <td style="width: 250px; display: block; text-align: center;" valign="top" id="tdCenter">
                <div style="margin: 50px 5px 5px 5px;">
                    <asp:Label ID="AreaDisplay" runat="server" Font-Bold="true" Font-Size="Large" ForeColor="Red"></asp:Label>
                    <table style="text-align: center;">
                        <tr>
                            <td align="right">
                                区域类型：
                            </td>
                            <td align="left">
                                <asp:DropDownList ID="ddlAreaType" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                配送商：
                            </td>
                            <td>
                                <uc3:UCSelectStation runat="server" ID="ucSelectStation" LoadDataType="OnlySite" />
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                商家：
                            </td>
                            <td align="left">
                                <uc5:UCMerchantSourceTV ID="UCMerchantSourceTV" runat="server" MerchantLoadType="All"
                                    MerchantShowCheckBox="False" MerchantTypeSource="jc_Merchant_Expense" />
                            </td>
                        </tr>
                        <%--<tr>
							<td colspan="2" align="left" style="color:#cccccc;">以下两项可为更新使用</td>
						</tr>
						<tr>
							<td align="right">仓库/分拣：</td>
							<td>
								<asp:DropDownList ID="ddlWareHouse" runat="server">
								</asp:DropDownList>
							</td>
						</tr>
						<tr>
							<td align="right">产品：</td>
							<td align="left"><asp:DropDownList ID="ddlProduct" runat="server"></asp:DropDownList></td>
						</tr>--%>
                    </table>
                    <div style="padding-top: 5px;">
                        <asp:Button ID="btAddAreaType" OnClientClick="return JudgeInput()" runat="server"
                            Text="添加区域类型" OnClick="btAddAreaType_Click" />
                        <asp:Button ID="btUpdateAreaType" OnClientClick="return JudgeInput()" runat="server"
                            Text="更新区域类型" OnClick="btUpdateAreaType_Click" />
                         <asp:Button ID="btnSecondSerch" runat="server" Text="查询" OnClick="btnSecondSerch_Click" OnClientClick="return JudgeInput() " />
                    </div>
                </div>
            </td>
            <td style="width: 600px; display: block;" valign="top" id="tdRight">
                <asp:Button ID="btDelete" runat="server" Text="删除" OnClick="btDelete_Click" />
                <asp:GridView ID="gvAreaType" runat="server" AutoGenerateColumns="false" 
                    DataKeyNames="AutoID" AllowPaging="False" PageSize="20">
                    <Columns>
                        <asp:TemplateField HeaderText="选择">
                            <HeaderTemplate>
                                <input type="checkbox" id="cbCheckAll" onclick="fnCheckAllList($('#<%= gvAreaType.ClientID %>'),this)" />全选
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="cbTypeCheckBox" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="AutoID" HeaderText="AutoID" Visible="false" />
                        <asp:BoundField DataField="ExpressCompanyID" HeaderText="ExpressCompanyID" Visible="false" />
                        <asp:BoundField DataField="CompanyName" HeaderText="配送公司" />
                        <asp:BoundField DataField="MerchantName" HeaderText="商家" />
                        <asp:BoundField DataField="AreaType" HeaderText="区域类型" />
                        <asp:BoundField DataField="EffectAreaType" HeaderText="首次新增区域类型" />
                        <asp:BoundField DataField="EnableStr" HeaderText="生效状态" />
                        <asp:BoundField DataField="AuditStatusStr" HeaderText="审核状态" />
                    </Columns>
                </asp:GridView>
                <uc6:UCPagerType ID="UCPagerType" runat="server" PageSize="20" />
                </td>
                    
        </tr>
    </table>
</asp:Content>
