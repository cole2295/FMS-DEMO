<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectEmployee.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.SelectEmployee" %>
<table border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <input id="TxtUserControlEmployeeCode" style="width: 80px" runat="server" type="text"
                 onkeydown ="if(event.keyCode==13) event.keyCode=9;" onblur="return getEmployeeInfo();" /><span
                    id="SpanUserControlEmployeeName"></span><asp:HiddenField ID="HidUserControlEmployeeID"
                        runat="server" />
        </td>
        <td>
            <img alt="请选择员工" src="../images/vie.gif" onclick="openWinE();" />
        </td>
    </tr>
</table>
<script src="../Scripts/JsUtil.js" type="text/javascript"></script>

<script language="javascript" type="text/javascript">

    //保存
    function getEmployeeInfo() 
    {
        var simpleSpell = this.d$('<%=TxtUserControlEmployeeCode.ClientID %>').value;

        if (simpleSpell != "") 
        {
            BasicSettingWebService.GetEmployeeInfo(encodeURIComponent(simpleSpell), ShowOKE);
        }
        else 
        {
            this.d$('<%=TxtUserControlEmployeeCode.ClientID %>').value = "";
            this.d$('SpanUserControlEmployeeName').innerHTML = "";
            this.d$('<%=HidUserControlEmployeeID.ClientID%>').value = "";
        }
    }

    //回调
    function ShowOKE(Model) 
    {
        var MyEmployeeModel = Model;

        if (MyEmployeeModel == null) 
        {
            OpenWindowE("../UserControl/EmployeeListPop.aspx?ID=" + encodeURIComponent(d$('<%=TxtUserControlEmployeeCode.ClientID %>').value), 450, 600);
            
            this.d$('<%=TxtUserControlEmployeeCode.ClientID %>').value = "";
            this.d$('SpanUserControlEmployeeName').innerHTML = "";
            this.d$('<%=HidUserControlEmployeeID.ClientID%>').value = "";
        }
        else 
        {
            this.d$('SpanUserControlEmployeeName').innerHTML = MyEmployeeModel.EmployeeName;
            this.d$('<%=TxtUserControlEmployeeCode.ClientID %>').value  = MyEmployeeModel.EmployeeCode;
            this.d$('<%=HidUserControlEmployeeID.ClientID %>').value = MyEmployeeModel.EmployeeID;
        }
    }

    var IsPopEmploy = null;
    
    function OpenWindowE(Url, Width, Height) 
    {
        //打开模式窗口
        var Top = parseInt((screen.availHeight - Height - 30) / 2);
        var Left = parseInt((screen.availWidth - Width - 15) / 2);
        
        if (IsPopEmploy == null) 
        {
            IsPopEmploy = window.open(Url, '', 'toolbar=0,location=0,maximize=1,directories=0,status=1,menubar=0,scrollbars=0,resizable=1,top=' + Top
			    + ',left=' + Left + ',width=' + Width + ',height=' + Height);
        }
        else 
        {
            IsPopEmploy.focus();
        }
    }

    function SetValueE(mKey, mName) 
    {
        this.d$('<%=HidUserControlEmployeeID.ClientID%>').value = mKey;
        this.d$('SpanUserControlEmployeeName').innerHTML ="";
        this.d$('<%=TxtUserControlEmployeeCode.ClientID %>').value = mName;
    }

    function openWinE() 
    {
        OpenWindowE("../UserControl/EmployeeListPop.aspx?ID=" + encodeURIComponent(d$('<%=TxtUserControlEmployeeCode.ClientID %>').value), 600, 450);
    }
    
</script>