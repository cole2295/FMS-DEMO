<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="RFD.FMS.WEB.test.WebForm2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:TextBox ID="TextBox2" runat="server" TextMode="MultiLine" Height="167px"  Text="BEGIN TRAN

    IF(@@ERROR<>0 OR @@ROWCOUNT<>1)
    BEGIN
       ROLLBACK
       PRINT 'RollBack'
       RETURN
END
PRINT 'Commit Start'
COMMIT
PRINT 'Commit Over'"
        Width="936px"></asp:TextBox><br />
        <asp:Button ID="btnOk" runat="server" Text="OK" onclick="btnOk_Click" />
    </form>
</body>
</html>
