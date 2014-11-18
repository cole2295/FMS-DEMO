<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pager.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.Pager" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

<div style="padding-top: 2px; height: 23px; padding-left: 12px; padding-right: 10px;">
    <webdiyer:AspNetPager ID="AspNetPager" runat="server" ShowPageIndexBox="Always"
        PageIndexBoxType="DropDownList" FirstPageText="首页" LastPageText="尾页" NextPageText="后页"
        PrevPageText="前页" HorizontalAlign="Right" ShowNavigationToolTip="True" TextBeforePageIndexBox="转到第"
        PageSizeDisplay="true" TextAfterPageIndexBox="页" OnPageChanged="AspNetPager_PageChanged"
        AlwaysShow="True" NumericButtonTextFormatString="{0}" NumericButtonCount="4"
        CustomInfoHTML="第%CurrentPageIndex%页，共%PageCount%页，共%RecordCount%条，每页%PageSize%条"
        Width="100%" CustomInfoTextAlign="left" ShowCustomInfoSection="left" SubmitButtonStyle="Hight: 10px;float:right;"
        PageIndexBoxStyle="Hight: 10px;float:right;" SubmitButtonImageUrl="" UrlPageSizeName=""
        CurrentPageButtonTextFormatString="[{0}]" CustomInfoClass="aspnetpager" PageSize="10"
        LayoutType="Div"  CenterCurrentPageButton="True"
        RecordCount="0">
    </webdiyer:AspNetPager>
</div>

<%--<webdiyer:AspNetPager ID="AspNetPager" runat="server" PageSize="1" HorizontalAlign="Right" ShowPageIndexBox="Always"
    OnPageChanged="AspNetPager_PageChanged" ShowCustomInfoSection="Left" Width="95%"
    meta:resourceKey="AspNetPager1" Style="font-size: 12px" CustomInfoHTML="共<b><font color='red'>%RecordCount%</font></b>条记录　当前页：<font color='red'><b>%CurrentPageIndex%/%PageCount%</b></font>　每页：%PageSize%"
    AlwaysShow="True" FirstPageText="首页" LastPageText="尾页" NextPageText="下一页" PrevPageText="上一页"
    SubmitButtonText="确定" SubmitButtonClass="submitBtn" CustomInfoStyle="FONT-SIZE: 12px"
    InputBoxStyle="width:30px; border:1px solid #999999; text-align:center; " TextBeforeInputBox="转到第"
    TextAfterInputBox="页 ">
</webdiyer:AspNetPager>--%>

