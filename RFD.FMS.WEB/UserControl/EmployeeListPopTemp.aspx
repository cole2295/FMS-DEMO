<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmployeeListPopTemp.aspx.cs"
    Inherits="RFD.FMS.WEB.UserControl.EmployeeListPopTemp" %>

<%@ Register Src="Pager.ascx" TagName="Pager" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base  target="_self"/>
    <link href="../StylesNew/main.css" rel="stylesheet" type="text/css" />

    <script language="javascript" type="text/javascript">
        function SelectTRETemp(mKey, mName) 
        {
            window.opener.SetValueETemp(mKey, mName);
            
            window.close();
        }
    </script>

</head>
<body onunload="window.opener.IsPopEmployTemp = null;" class="BodyPop">
    <form id="form1" runat="server">
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td>
                员工编号
            </td>
            <td>
                <asp:TextBox ID="EmployeeCode" Width="100px" runat="server"></asp:TextBox>
            </td>
            <td>
                员工名称
            </td>
            <td>
                <asp:TextBox ID="EmployeeName" Width="100px" runat="server"></asp:TextBox>
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
                            <asp:BoundField DataField="员工编码" HeaderText="员工编码" SortExpression="员工编码" />
                            <asp:BoundField DataField="员工名称" HeaderText="员工名称" SortExpression="部门名称" />
                            <asp:BoundField DataField="所在部门" HeaderText="所在部门" SortExpression="所在部门" />
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
