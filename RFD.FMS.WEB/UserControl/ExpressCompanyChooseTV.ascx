<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpressCompanyChooseTV.ascx.cs" Inherits="RFD.FMS.WEB.UserControl.ExpressCompanyChoose.ExpressCompanyChooseTV" %>
  <style type="text/css">  
      .input{  
       background-color:#f4f4f4;  
      } 
  </style> 
<script language="javascript" type="text/javascript">
    function GetExpressImageID() {
        return $("#<%=imgMain.ClientID %>");
    }

    function GetUseExpressID() {
        return $("#<%=this.HidUserControlStationID.ClientID %>").val();
    }

    $(function () {
        $("#<%=cbCheckBox.ClientID %>").click(function () {
            if ($(this).attr("checked")) {
                $("#<%=TxtUserControlSatationSpell.ClientID %>").removeAttr("disabled");
                $("#<%=TxtUserControlSatationSpell.ClientID %>").attr("class", "text");
                $("#<%=imgMain.ClientID %>").removeAttr("disabled");
                $("#<%=imgMain.ClientID %>").attr("class", "icon");
            } else {
                $("#<%=TxtUserControlSatationSpell.ClientID %>").attr("disabled", "disabled");
                $("#<%=imgMain.ClientID %>").attr("disabled", "disabled");
                $("#<%=TxtUserControlSatationSpell.ClientID %>").attr("class", "input");
                $("#<%=imgMain.ClientID %>").attr("class", "input");
                
            }
        }
        )
            ;
    });

</script>

<div style="display: inline" class="openStation">
<table>
<tr>
    <td style="font:large"> <input id="cbCheckBox" type="checkbox" runat="server"/>配送商</td>
    <td>
        <input id="TxtUserControlSatationSpell" style="width: 180px; margin-right: 1px; border:1px solid #9999CC;" runat="server"
					type="text" class="input" onkeypress="fnShowExpressPage(this);return false;"   disabled="disabled"  />
				<span id="spanMustInput" style="color:Red" runat="server"></span>
				<span id="SpanUserControlStationName"></span>
				<asp:HiddenField ID="HidUserControlStationID" runat="server" />
    </td>
    <td>
        <img alt="选择配送公司" id="imgMain" src="../images/vie.gif" class="input" style="cursor: pointer; padding-left: 1px;"
					title="选择配送公司" onclick="fnShowExpressPage(this)" runat="server" disabled="disabled"/>
    </td>
</tr>
</table>
</div>

<script type="text/javascript">

    var expressLoadType = '<%=_expressLoadType %>';
    var expressTypeSource = '<%=_expressTypeSource %>';
    var showCheckBox = '<%=_expressCompanyShowCheckBox %>';

    function fnShowExpressPage(E) {
        stationCommon.open(E, '../UserControl/ExpressCompanyTreeView.aspx?loadType=' + expressLoadType + '&typeSource=' + expressTypeSource + '&showCheckBox=' + showCheckBox, 400, 400);
    }
</script>
