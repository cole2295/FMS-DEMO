<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectStation.ascx.cs"
	Inherits="RFD.FMS.WEB.UserControl.SelectStation" %>

<script type="text/javascript" src="../Scripts/controls/selectStation.js"/>	
<script type="text/javascript">
    
 function GetStationID()
 {
     return $("#<%=this.HidUserControlStationID.ClientID %>").val();    
 }   
    
</script>
			
<div style="display: inline" class="selectStation">
	<table border="0" cellpadding="0" cellspacing="0">
		<tr>
			<td>
				<input id="TxtUserControlSatationSpell" style="width: 80px; margin-right: 1px;" runat="server"
					type="text" class="text" onkeypress="station.select(this);return false;"/>
				<span id="spanMustInput" style="color:Red" runat="server">*</span>
				<span id="SpanUserControlStationName"></span>
				<asp:HiddenField ID="HidUserControlStationID" runat="server" />
			</td>
			<td>
				<img id="imgMain" src="../images/vie.gif" class="icon" style="cursor: pointer; padding-left: 1px;"
					title="选择站点" onclick="station.select(this);" runat="server" />
			</td>
		</tr>
	</table>
</div>
