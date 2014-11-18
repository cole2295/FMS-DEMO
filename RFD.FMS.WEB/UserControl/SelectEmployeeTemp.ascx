<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectEmployeeTemp.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.SelectEmployeeTemp" %>
<table border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <input id="TxtUserControlEmployeeCodeTemp" style="width: 80px" runat="server" type="text"
                 onkeydown ="if(event.keyCode==13) event.keyCode=9;" onblur="return getEmployeeInfoTemp();" />
            <span id="SpanUserControlEmployeeNameTemp"/>
            <asp:HiddenField ID="HidUserControlEmployeeIDTemp" runat="server" />
        </td>
        <td>
            <img alt="请选择员工" src="../images/vie.gif" onclick="openWinETemp();" />
        </td>
    </tr>
</table>
<script src="../Scripts/JsUtil.js" type="text/javascript"></script>

<script language="javascript" type="text/javascript">
// <!CDATA[
    //保存
    function getEmployeeInfoTemp() 
    {
        var simpleSpell = this.d$('<%=TxtUserControlEmployeeCodeTemp.ClientID %>').value;

        if (simpleSpell != "") 
        {
            BasicSettingWebService.getEmployeeInfo(encodeURIComponent(simpleSpell), ShowOKETemp);
        }
        else 
        {
            this.d$('<%=TxtUserControlEmployeeCodeTemp.ClientID %>').value = "";
            this.d$('SpanUserControlEmployeeNameTemp').innerHTML = "";
            this.d$('<%=HidUserControlEmployeeIDTemp.ClientID%>').value = "";
        }
    }

    //回调
    function ShowOKETemp(Model) 
    {
        var MyEmployeeModel = Model;

        if (MyEmployeeModel == null) 
        {
            OpenWindowETemp("../UserControl/EmployeeListPopTemp.aspx?ID=" + encodeURIComponent(d$('<%=TxtUserControlEmployeeCodeTemp.ClientID %>').value), 450, 600);
            
            this.d$('<%=TxtUserControlEmployeeCodeTemp.ClientID %>').value = "";
            this.d$('SpanUserControlEmployeeNameTemp').innerHTML = "";
            this.d$('<%=HidUserControlEmployeeIDTemp.ClientID%>').value = "";
        }
        else 
        {
           this.d$('SpanUserControlEmployeeNameTemp').innerHTML = MyEmployeeModel.EmployeeName;
           this.d$('<%=TxtUserControlEmployeeCodeTemp.ClientID %>').value  = MyEmployeeModel.EmployeeCode;
           this.d$('<%=HidUserControlEmployeeIDTemp.ClientID %>').value = MyEmployeeModel.EmployeeID;
        }
    }

    var IsPopEmployTemp = null;

    function OpenWindowETemp(Url, Width, Height) 
    {
        //打开模式窗口
        var Top = parseInt((screen.availHeight - Height - 30) / 2);
        var Left = parseInt((screen.availWidth - Width - 15) / 2);

        if (IsPopEmployTemp == null) 
        {
            IsPopEmployTemp = window.open(Url, '', 'toolbar=0,location=0,maximize=1,directories=0,status=1,menubar=0,scrollbars=0,resizable=1,top=' + Top
			    + ',left=' + Left + ',width=' + Width + ',height=' + Height);
        }
        else 
        {
            IsPopEmployTemp.focus();
        }
    }

    function SetValueETemp(mKey, mName) 
    {
        this.d$('<%=HidUserControlEmployeeIDTemp.ClientID%>').value = mKey;
        this.d$('SpanUserControlEmployeeNameTemp').innerHTML ="";
        this.d$('<%=TxtUserControlEmployeeCodeTemp.ClientID %>').value = mName;
    }

    function openWinETemp() {
        OpenWindowETemp("../UserControl/EmployeeListPopTemp.aspx?ID=" + encodeURIComponent(d$('<%=TxtUserControlEmployeeCodeTemp.ClientID %>').value), 600,450);
    }
    // ]]>
</script>