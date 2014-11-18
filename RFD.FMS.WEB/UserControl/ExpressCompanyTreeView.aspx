<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpressCompanyTreeView.aspx.cs" Inherits="RFD.FMS.WEB.UserControl.ExpressCompanyTreeView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>树显示</title>
    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
        window.name = "winExpressTree";
        $(document).ready(function () {
            //为了不至于和页面上其他元素混乱，所以把TreeView控件放在一个id为Treeview的div中，然后
            //再查找checkbox
            $("#Treeview table tr td  input[type=checkbox]").click(
             function () {
                 if ($(this).attr("checked") == "checked")
                     $("#Treeview div[id=" + $(this).attr("id").toString().replace(/CheckBox/, "Nodes") + "] table tr td  input[type=checkbox]").attr("checked", $(this).attr("checked"));
                 else
                     $("#Treeview div[id=" + $(this).attr("id").toString().replace(/CheckBox/, "Nodes") + "] table tr td  input[type=checkbox]").removeAttr("checked");
             }
            );
        });

        function fnSelectChecked(mKey, mName) {
            var index = window.dialogArguments.activedIndex;
            var parent = window.dialogArguments.document;
            $(parent).find(".openStation .text").eq(index).val(mName).siblings(":hidden").val(mKey);
            $(parent).find(".openStation .text").eq(index).attr("title", mName);
            window.close();
        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server" target="winExpressTree">
    <div id="Treeview" style=" width:360px; height:360px; padding:20px; font-size:14px;">
        <div>
            <asp:TextBox ID="txtKeyWord" runat="server" Width="180px"></asp:TextBox>
            <asp:Button ID="btSearch" runat="server" Text="查询" onclick="btSearch_Click" />
            <asp:Button ID="btReset" runat="server" Text="还原初始" onclick="btReset_Click" />
            
        </div>
        <asp:Panel ID="pOk" runat="server">
            <div style=" width:300px; height:25px; text-align:right; padding-right:20px; clear:both; border:0px solid red;">
                <div style="float:left"><asp:Button ID="Button1" runat="server" Text="确定" onclick="btOK_Click" /></div>
                <div style="float:right; color:Red;">一个都不选表示全选</div>
            </div>
        </asp:Panel>
        <div id="divExpressTreeView" style="overflow-x:auto; width:350px; height:320px; font-size:14px;">
        <asp:TreeView ID="tree" runat="server" ShowCheckBoxes="All" ShowLines="true" 
            onselectednodechanged="tree_SelectedNodeChanged">
            
        </asp:TreeView>
        </div>
    </div>
    </form>
</body>
</html>
