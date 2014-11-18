<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RFD.FMS.WEB.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>财务管理系统</title>
    <link href="css/login.css" rel="stylesheet" type="text/css" />

    <script src="Scripts/lib/jquery-1.6.1.min.js" type="text/javascript"></script>

    <link rel="shortcut icon" href="images/favicon.ico" type="image/x-icon" />

    <script type="text/javascript">
        function Change() {
            var img_ValidateCode = document.getElementById("img_ValidateCode");
            var timeStamp = new Date().getTime();
            img_ValidateCode.src = "UserControl/Validate.aspx?timeStamp=" + timeStamp;
        }
        $(document).ready(function() {
            if (window.top != window.self) {
                window.open('Login.aspx', '_top');
            }
        });

        $.getJSON("http://clouddemo.wuliusys.com/HandlerLogin.ashx?jsoncallback=?", function(json) {
            if ('disabled' == json.LoginButton) {
                $('input').attr('disabled', 'disabled');
            } else {
                $('input').removeAttr('disabled');
            };
            if (json.Tips != "") {
                $('#tip').html(json.Tips);
            }
        });
    </script>

</head>
<body>
    <form id="form1" runat="server" action="Login.aspx">
    <div class="wrapper">
        <div class="logo">
            <img src="images/logo.jpg" alt="财务管理系统" title="财务管理系统" /></div>
        <div class="content">
            <div class="information">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <th scope="row">
                            用户名：
                        </th>
                        <td>
                            <asp:TextBox ID="TxtUserCode" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            密码：
                        </th>
                        <td>
                            <asp:TextBox ID="TxtPassWord" TextMode="Password" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <th scope="row">
                            验证码：
                        </th>
                        <td>
                            <asp:TextBox ID="txtValidate" MaxLength="4" runat="server" Width="60px"></asp:TextBox>
                            <img src="UserControl/Validate.aspx" border="0" width="55" height="20" align="absmiddle"
                                id="img_ValidateCode" style="padding-bottom: 5px;" />
                            <a onclick="Change()" style="cursor: pointer;">刷新</a>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" scope="row">
                            <a href="javascript:void(0);" onclick="javascript:return false;" target="_blank">忘记密码请联系管理员！</a>
                            <input class="bt" type="submit" runat="server" name="button" id="button" value="登  录"
                                onserverclick="btnlogin_Click" />
                        </td>
                    </tr>
                </table>
            </div>
            <!--information-->
            <div style="text-align: center; color: Red;">
                 <span id="tip" >建议: 1024*768以上分别率, IE7以上版本浏览器访问该系统！</span>
            </div>
        </div>
        <!--content-->
    </div>
    <!--wrapper-->
    </form>
</body>
</html>
