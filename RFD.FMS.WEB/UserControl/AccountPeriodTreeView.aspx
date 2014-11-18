<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountPeriodTreeView.aspx.cs" Inherits="RFD.FMS.WEB.UserControl.AccountPeriodTreeView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>账期选择</title>
    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        window.name = "winAccountPeriodTree";
        function fnSelectChecked(mAccountId, mAccountName, mObjectName, mObjectId, mExpress, mExpressType, mAccountDate, mAccountDateStr, mAccountDateEnd, expressNames) {
            var index = window.dialogArguments.activedIndex;
            var parent = window.dialogArguments.document;
            $(parent).find("#divAccountPeriodHidden .text").val(mAccountName + ":" + mObjectName)
            //$(parent).find(".openStation .text").eq(index).val(mAccountName + ":" + mObjectName).siblings(":hidden").val(mAccountId).siblings(":hidden").val(mObjectId).siblings(":hidden").val(mExpress).siblings(":hidden").val(mExpressType);
            $(parent).find("#divAccountPeriodHidden input[type='hidden']").each(function () {
                if ($(this).attr("id").indexOf("HidAccountPeriodID") > -1)
                    $(this).val(mAccountId);
                if ($(this).attr("id").indexOf("HidAccountPeriodObjectID") > -1)
                    $(this).val(mObjectId);
                if ($(this).attr("id").indexOf("HidAccountExpressID") > -1)
                    $(this).val(mExpress);
                if ($(this).attr("id").indexOf("HidExpressType") > -1)
                    $(this).val(mExpressType);
                if ($(this).attr("id").indexOf("HidAccountDate") > -1)
                    $(this).val(mAccountDate);
                if ($(this).attr("id").indexOf("HidAccountDateStr") > -1)
                    $(this).val(mAccountDateStr);
                if ($(this).attr("id").indexOf("HidAccountDateEnd") > -1)
                    $(this).val(mAccountDateEnd);
            });
            $(parent).find("#divAccountPeriodHidden .text").eq(index).attr("title", mAccountName + ":" + mObjectName);
            window.dialogArguments.RunAccountOnPageChange(mAccountId, mAccountName, mObjectName, mObjectId, mExpress, mExpressType, mAccountDate, mAccountDateStr, mAccountDateEnd, expressNames);
            window.close();
        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server" target="winAccountPeriodTree">
    <div id="Treeview" style="overflow-x:auto;overflow-y:auto; width:610px; height:360px; padding:20px; font-size:14px;">
        <div>
            <asp:TextBox ID="txtKeyWord" runat="server" Width="180px"></asp:TextBox>
            <asp:Button ID="btSearch" runat="server" Text="查询" onclick="btSearch_Click" />
            <asp:Button ID="btReset" runat="server" Text="还原初始" onclick="btReset_Click" />
        </div>
        <div id="divAccountPeriod" style="overflow-x:auto; width:600px; height:340px; font-size:14px;">
            <asp:TreeView ID="AccountPeriodTree" runat="server" ShowLines="true" 
                onselectednodechanged="AccountPeriodTree_SelectedNodeChanged">
            </asp:TreeView>
        </div>
    </div>
    </form>
</body>
</html>

