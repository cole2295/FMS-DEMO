<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UCRulePannel.ascx.cs" Inherits="RFD.FMS.WEB.test.workflow.UCRulePannel" %>
<div align="center">
   <table>
       <tr>
            <td>
                <div>
                    <asp:Label ID="lblRuleType" Text="规则类型" runat="server" />     
                    <asp:DropDownList ID="cmbRuleType" Width="200px" runat="server" AutoPostBack = "true" />
                </div>
            </td>
       </tr>
       <tr>
            <td>
                <div>
                    <asp:Panel ID="pnlCtrl" Height="260px" Width="450px" runat="server" />
                </div>
            </td>
       </tr>
   </table> 
</div>
