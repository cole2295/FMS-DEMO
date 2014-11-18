<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TabPage.aspx.cs" Inherits="RFD.FMS.WEB.Main.TabPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <script src="../Scripts/base.js" type="text/javascript"></script>

    <script src="../Scripts/import/container.js" type="text/javascript"></script>

    <style type="text/css">
        html, body
        {
            height: 100%;
            padding: 0px;
        }
        body
        {
            margin: 7px 0px 0px 10px;
            overflow-y: hidden;
        }
        #container
        {
            height: 100%;
            width: 100%;
        }
    </style>
</head>
<body>
    <div id="container">
        <div id="tab_menu">
        </div>
        <div id="page">
        </div>
    </div>
</body>
</html>

<script type="text/javascript">
    init();
    window.onload = window.onresize = function() {
        var pageCon = document.getElementById("page");
        var bodyHeight = document.body.clientHeight;
        pageCon.style.height = (bodyHeight - 43) + "px";
    }
</script>

