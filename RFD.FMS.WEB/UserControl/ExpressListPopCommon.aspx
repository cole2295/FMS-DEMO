﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpressListPopCommon.aspx.cs"
    Inherits="RFD.FMS.WEB.UserControl.ExpressListPopCommon" Theme="default" %>

<%@ Register Src="Pager.ascx" TagName="Pager" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base  target="_self"/>
    <link href="../StylesNew/main.css" rel="stylesheet" type="text/css" />

    <script src="../Scripts/lib/jquery-1.6.1.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        function SelectTR(mKey, mName) {
            var index = window.dialogArguments.activedIndex;
            var parent = window.dialogArguments.document;
            $(parent).find(".selectStation .text").eq(index).val(mName).siblings(":hidden").val(mKey);
            window.close();
        }
    </script>

</head>
<body class="BodyPop">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                部门编号
            </td>
            <td>
                <asp:TextBox ID="ExpressCompanyCode" Width="100px" runat="server"></asp:TextBox>
            </td>
            <td>
                部门名称
            </td>
            <td>
                <asp:TextBox ID="CompanyName" Width="100px" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="BtnQuery" AccessKey="q" OnClick="BtnQuery_Click" runat="server" Text="查询(Q)" />
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <div id="Div1" class="tableContainer">
                    <asp:GridView ID="GridView1" Width="100%" BorderWidth="0px" PageSize="15" AllowPaging="true"
                        AllowSorting="true" runat="server" OnSorting="GridView1_Sorting" AutoGenerateColumns="false"
                        OnPageIndexChanging="GridView1_PageIndexChanging">
                        <Columns>
                            <asp:BoundField DataField="序号" HeaderText="序号" SortExpression="序号" />
                            <asp:BoundField DataField="部门编码" HeaderText="部门编码" SortExpression="部门编码" />
                            <asp:BoundField DataField="部门名称" HeaderText="部门名称" SortExpression="部门名称" />
                            <asp:BoundField DataField="简拼" HeaderText="简拼" SortExpression="简拼" />
                            <asp:BoundField DataField="部门类型" HeaderText="部门类型" SortExpression="部门类型" />
                            <asp:BoundField DataField="上级部门" HeaderText="上级部门" SortExpression="上级部门" />
                            <asp:BoundField DataField="订单峰值" HeaderText="订单峰值" SortExpression="订单峰值" />
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
