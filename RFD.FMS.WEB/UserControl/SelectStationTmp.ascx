<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectStationTmp.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.SelectStationTmp" %>

<script src="../Scripts/JsUtil.js" type="text/javascript"></script>

<script language="javascript" type="text/javascript">

    function GetTempStationID() 
    {
        return $("#<%=this.HidUserControlStationIDTmp.ClientID %>").val();
    }

    //保存
    function getStationInfoTmp() 
    {
        var simpleSpell = encodeURIComponent(this.d$('<%=TxtUserControlSatationSpellTmp.ClientID %>').value);

        if (simpleSpell != "") 
        {
            BasicSettingWebService.GetStationInfo(simpleSpell, function(Model) 
            {
                var MyStationModel = Model;
                
                if (MyStationModel == null) 
                {
                    OpenWindowTmp("../UserControl/ExpressListPopTmp.aspx?ID=" + encodeURIComponent(d$('<%=TxtUserControlSatationSpellTmp.ClientID %>').value), 450, 600);
                    
                    this.d$('<%=TxtUserControlSatationSpellTmp.ClientID %>').value = "";
                    this.d$('SpanUserControlStationNameTmp').innerHTML = "";
                    this.d$('<%=HidUserControlStationIDTmp.ClientID%>').value = "";
                }
                else 
                {
                    this.d$('SpanUserControlStationNameTmp').innerHTML = MyStationModel.CompanyName;
                    this.d$('<%=HidUserControlStationIDTmp.ClientID %>').value = MyStationModel.ExpressCompanyID;
                }
            });
        }
        else 
        {
            this.d$('<%=TxtUserControlSatationSpellTmp.ClientID %>').value = "";
            this.d$('SpanUserControlStationNameTmp').innerHTML = "";
            this.d$('<%=HidUserControlStationIDTmp.ClientID%>').value = "";
        }
    }

    var IsPopTmp = null;

    function OpenWindowTmp(Url, Width, Height) 
    {
        //打开模式窗口
        var Top = parseInt((screen.availHeight - Height - 30) / 2);
        var Left = parseInt((screen.availWidth - Width - 15) / 2);
        
        if (IsPopTmp == null) 
        {
            IsPopTmp = window.open(Url, '', 'toolbar=0,location=0,maximize=1,directories=0,status=1,menubar=0,scrollbars=0,resizable=1,top=' + Top
			    + ',left=' + Left + ',width=' + Width + ',height=' + Height);
        }
        else 
        {
            IsPopTmp.focus();
        }
    }

    function SetValueTmp(mKey, mName) 
    {
        this.d$('<%=TxtUserControlSatationSpellTmp.ClientID %>').value = mName;
        this.d$('<%=HidUserControlStationIDTmp.ClientID%>').value = mKey;
    }

    function openWinTmp() 
    {
        OpenWindowTmp("../UserControl/ExpressListPopTmp.aspx?ID=" + encodeURIComponent(d$('<%=TxtUserControlSatationSpellTmp.ClientID %>').value), 450, 600);
    }

    function onKeyPressEvent(event) 
    {
        if (event.keyCode == 13) 
        {
            getStationInfoTmp();

            event.keyCode = 9;
        }
    }

    function CheckHasValueTmp() {
        var simpleSpell = this.d$('<%=TxtUserControlSatationSpellTmp.ClientID %>').value;

        if (simpleSpell != "") return true;

        return false;
    }
    
</script>

<table border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <input id="TxtUserControlSatationSpellTmp" style="width: 80px" runat="server" type="text"
                 onkeypress="onKeyPressEvent(event)" />
            <span id="spanMustInput" style="color:Red" runat="server">*</span>
            <span id="SpanUserControlStationNameTmp"></span>
            <asp:HiddenField ID="HidUserControlStationIDTmp" runat="server" />
        </td>
        <td>
            <img id="imgTemp" alt="请选择部门" src="../images/vie.gif" onclick="openWinTmp();" runat="server"/>
        </td>
        <td>
        </td>
    </tr>
</table>
