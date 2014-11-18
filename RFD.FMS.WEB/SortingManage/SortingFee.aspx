<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true" CodeBehind="SortingFee.aspx.cs" Inherits="RFD.FMS.WEB.SortingManage.SortingFee" %>
<%@ Register Src="~/UserControl/StationCheckBox.ascx" TagName="StationCheckbox" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/CitySelected.ascx" TagName="CitySelectedBox" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/DistributionSelect.ascx" TagName="DistributionSelected" TagPrefix="uc3" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="uc4" TagName="Pager" Src="~/UserControl/Pager.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
    function fnCheckAllList(LE, E) {
        LE.find('input:checkbox').attr('checked', E.checked);
    }
    function fnOpenModalDialog(url, width, height) {
        window.showModalDialog(url, window, "dialogWidth=" + width + "px;dialogHeight=" + height + "px;center:yes;resizable:no;scroll:auto;help=no;location=no;status=no;");
    }

    function fnOpenModelessDialog(url, width, height) {
        window.showModelessDialog(url, window, "dialogWidth=" + width + "px;dialogHeight=" + height + "px;center:yes;resizable:no;scroll:auto;help=no;location=no;status=no;");
    }

    function fnOpenWindow(url, width, height) {
        window.open(url, 'newwindow', 'height=' + height + ',width=' + width + ',top=0,left=0,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no')
    }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
    拣运商费用基础信息
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="body" runat="server">
    <table>
   <tr>
       <td>日期:<asp:TextBox ID="txtBeginTime" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"
										    runat="server"></asp:TextBox>至
                                          <asp:TextBox ID="txtEndTime" Width="115px" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})"
											runat="server"></asp:TextBox>
       </td>
       <td>
           拣运商：
       </td>
       <td><uc3:DistributionSelected ID= "SortingMerchantSelected" runat="server" /></td>
       <td>分拣中心:</td>
       <td><uc1:stationcheckbox ID="SortingCenterSelect" runat="server" 
               LoadDataType="RFDSortCenter"/></td>
       <td>城市：</td>
       <td><uc2:cityselectedbox ID="CitySelected" runat="server"/></td>
   </tr>
   <tr>
       <td>状态:
           <asp:DropDownList ID="StatusDDL" runat="server">
           <asp:ListItem Value="-1">全选</asp:ListItem>
           <asp:ListItem Value="0">待审核</asp:ListItem>
           <asp:ListItem Value="1">已审核</asp:ListItem>
           <asp:ListItem Value="3">已生效</asp:ListItem>
           <asp:ListItem Value="2">置回</asp:ListItem>
           <asp:ListItem Value="4">无效</asp:ListItem>
           </asp:DropDownList>
       </td>
       <td>
           费用项目:
           <asp:DropDownList ID="ItemDDL" runat="server">
           <asp:ListItem Value="-1">全选</asp:ListItem>
           <asp:ListItem Value="1">运输费用项目</asp:ListItem>
           <asp:ListItem Value="2">分拣到城市项目</asp:ListItem>
           <asp:ListItem Value="3">分拣到站点项目</asp:ListItem>
           <asp:ListItem Value="4">逆向入库项目</asp:ListItem>
           <asp:ListItem Value="5">提货补偿项目</asp:ListItem>
           </asp:DropDownList>
       </td>
       <td><asp:CheckBox ID="WaitChk" runat="server" 
               oncheckedchanged="WaitChk_CheckedChanged" />待生效</td>
       <td>
       </td>
       <td>
          <asp:Button ID="AddBtn" runat="server" Text="添加拣运商信息" onclick="AddBtn_Click" />
          <asp:Button ID="AddWaitBtn" runat="server" Text="增加拣运商待生效信息" onclick="AddWaitBtn_Click" 
                />
       </td>
       <td></td>
       <td> <asp:Button ID="SearchBtn" runat="server" Text="查询" 
               onclick="SearchBtn_Click" /></td>
   </tr>
</table>
    
 <table>
     <asp:gridview ID="gvSortingFee" runat="server" AutoGenerateColumns="false" 
         DataKeyNames="ID" onrowcommand="gvSortingFee_RowCommand">
      <Columns>
				<asp:TemplateField HeaderText="选择">
					<HeaderTemplate>
						<input type="checkbox" id="cbCheckAll" onclick="fnCheckAllList($('#<%= gvSortingFee.ClientID %>'),this)" />全选
					</HeaderTemplate>
					<ItemTemplate>
						<asp:CheckBox ID="cbCheck" runat="server" />
					</ItemTemplate>
				</asp:TemplateField>
                   
           <asp:BoundField DataField="ID" HeaderText="ID"  Visible="false"/>
           <asp:BoundField DataField="FeeID" HeaderText="FeeID"  Visible="false"/>
           <asp:BoundField DataField="CreateTime" HeaderText="创建时间" />
           <asp:BoundField DataField="SortingMerchant" HeaderText="拣运商" />
           <asp:BoundField DataField="SortingCenter" HeaderText="分拣中心" />
           <asp:BoundField DataField="CityName" HeaderText="城市" />
           <asp:BoundField DataField="MerchantName" HeaderText="商家" />
           <asp:BoundField DataField="FareTypeStr" HeaderText="拣运费用类型" />
           <asp:BoundField DataField="AccountFare" HeaderText="结算单价" />
           <asp:BoundField DataField="Status" HeaderText="状态" />
           <asp:BoundField DataField="EffectDate" HeaderText="生效时间" />
           <asp:BoundField DataField="UpdateTime" HeaderText="更新时间" />
           <asp:BoundField DataField="CreateBy" HeaderText="创建人" />
           <asp:BoundField DataField="AuditBy" HeaderText="审核人" />
           <asp:TemplateField HeaderText="查看操作日志">
			        <ItemTemplate>
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#!string.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "ID").ToString())%>'>
				            <a href="javascript:fnOpenModalDialog('../BasicSetting/OperateLogView.aspx?PK_NO=<%#((DataRowView)Container.DataItem)["ID"] %>&LogType=<%#WaitChk.Checked?10.ToString():9.ToString()%>',500,300)">查看操作日志</a>
                     </asp:PlaceHolder> 
			        </ItemTemplate>
		        </asp:TemplateField>

                <asp:TemplateField HeaderText="修改">
                     <ItemTemplate>
                     <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#!string.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "StatusCode").ToString()) && (Convert.ToInt32(DataBinder.Eval(Container.DataItem, "StatusCode").ToString())==2 
                                                                                                                                                                           ||Convert.ToInt32(DataBinder.Eval(Container.DataItem, "StatusCode").ToString())==0)%>'>
                       <asp:LinkButton ID="ChangeBtn" runat="server" CommandArgument='<%# Eval("ID") %>' CommandName="Change">修改</asp:LinkButton>
                     </asp:PlaceHolder> 
			        </ItemTemplate>
		        </asp:TemplateField>

                 <asp:TemplateField HeaderText="置回">
                     <ItemTemplate>
                     <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#!string.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "StatusCode").ToString()) && Convert.ToInt32(DataBinder.Eval(Container.DataItem, "StatusCode").ToString())==2%>'>
                       <asp:LinkButton ID="BackBtn" runat="server" CommandArgument='<%# Eval("ID") %>' CommandName="Back">置无效</asp:LinkButton>
                     </asp:PlaceHolder> 
			        </ItemTemplate>
		        </asp:TemplateField>
       </Columns> 
          
     </asp:gridview>
 </table>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="PagerContent" runat="server">
    <uc4:Pager ID="Pager1" runat="server" />
</asp:Content>

