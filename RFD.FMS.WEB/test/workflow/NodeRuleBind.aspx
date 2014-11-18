<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true" CodeBehind="NodeRuleBind.aspx.cs" Inherits="RFD.FMS.WEB.test.workflow.NodeRuleBind" %>

<%@ Register src="UCRulePannel.ascx" tagname="UCRulePannel" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <div>
        流程节点规则绑定
    </div>
    <div>
        <uc1:UCRulePannel ID="UCRulePannel1" runat="server" />
    </div>
    <div>
        <div>
            <table>
            <tr>
                <td>
                    <asp:Button ID=""                
                </td>
            </tr>
        </table>
        </div>
        <table>
            <tr>
                <td>
                                
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
