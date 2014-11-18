<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FmsPager.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.FmsPager" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<webdiyer:AspNetPager ID="pager" runat="server" OnPageChanged="pager_PageChanged"
	ShowPageIndexBox="Always" PageIndexBoxType="DropDownList" FirstPageText="首页"
	LastPageText="尾页" NextPageText="后页" PrevPageText="前页" HorizontalAlign="Left"
	ShowNavigationToolTip="True" TextBeforePageIndexBox="转到第" TextAfterPageIndexBox="页"
	AlwaysShow="True" NumericButtonTextFormatString="{0}" NumericButtonCount="4"
	CurrentPageButtonTextFormatString="[{0}]" CustomInfoClass="aspnetpager" PageSize="10"   CustomInfoHTML="第%CurrentPageIndex%页，共%PageCount%页，共%RecordCount%条记录，每页显示%PageSize%条记录"
RecordCount="0"  ShowCustomInfoSection="left"  > 
</webdiyer:AspNetPager>
