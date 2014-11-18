<%@ Page Title="" Language="C#" MasterPageFile="~/Main/main.Master" AutoEventWireup="true"
    CodeBehind="DeliverFeeStatReport.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.DeliverFeeStatReport"
    Theme="default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        $(function() {
            //判断是否显示商家来源
            showMerchant({
                source: $("#<%=ddlOrderSource.ClientID %>"),
                merchant: $("#<%=ddlMerchantList.ClientID %>").parent()
            });
            render();
        })
        function check() {

            var beginTime = $.trim($("#<%=txtBeginTime.ClientID %>").val());
            var endTime = $.trim($("#<%=txtEndTime.ClientID %>").val());
            var accept = $("#<%=ddlOrderSource.ClientID%>  option:selected").val();

            if (accept == "") {
                alert("请选择商家来源");
                return false;
            }

            if (beginTime != "" && !isdate(beginTime)) {
                alert("请输入合法的开始时间！");
                return false;
            }
            if (endTime != "" && !isdate(endTime)) {
                alert("请输入合法的结束时间！");
                return false;
            }
            if (beginTime == "" || endTime == "") {
                return true;
            }
            var seconds = seconddiff(beginTime, endTime);
            if (seconds < 0) {
                alert("结束时间必须大于等于开始时间！");
                return false;
            }
            return true;
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table cellpadding="5" cellspacing="5" border="0" width="100%">
        <tr>
            <td colspan="">
                <table cellpadding="5" cellspacing="5" border="0">
                    <tr>
                        <td align="right">
                            商家来源：
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlOrderSource" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="ddlMerchantList" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="right" colspan="2">
                            <asp:RadioButtonList ID="rbtnReportType" runat="server" RepeatColumns="2">
                                <asp:ListItem Value="0" Text="周期报表" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="1" Text="日报表"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td colspan="2">
                            <asp:TextBox ID="txtBeginTime" CssClass="Wdate text" runat="server" autocomplete="off"></asp:TextBox>至<asp:TextBox
                                ID="txtEndTime" runat="server" CssClass="Wdate text" autocomplete="off"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td align="right">
                            省市：
                        </td>
                        <td colspan="2">
                            <asp:DropDownList runat="server" ID="ddlProvince" AutoPostBack="true" AppendDataBoundItems="true"
                                OnSelectedIndexChanged="DrpProvince_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:DropDownList runat="server" ID="ddlCity" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged"
                                AutoPostBack="True">
                                <asp:ListItem Value="">请选择</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td align="right">
                            站点：
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlStation" AppendDataBoundItems="true">
                                <asp:ListItem Value="">请选择</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td colspan="2">
                            <asp:Label ID="lblText" runat="server"></asp:Label><asp:Button ID="btnSearch" runat="server"
                                Text="查  询" OnClick="btnSearch_Click" OnClientClick=" return check();" />&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnSearchDetails" runat="server" Text="导  出" OnClick="btnSearchDetails_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        
        <tr>
            <td>
                <div style="overflow-y: auto; overflow-x: hidden; height: 300px; width: 100%">
                    <asp:GridView ID="gv" runat="server" Width="95%" AutoGenerateColumns="False" OnRowEditing="gv_RowEditing"
                        DataKeyNames="MerchantID,ExpressCompanyID,Sources">
                        <HeaderStyle Wrap="false" />
                        <RowStyle Wrap="false" />
                        <Columns>
                            <asp:TemplateField HeaderText="序号">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="MerchantName" HeaderText="商家" />
                            <asp:BoundField DataField="CreateTime" HeaderText="报表周期" />
                            <asp:BoundField DataField="ProvinceName" HeaderText="省份" />
                            <asp:BoundField DataField="CityName" HeaderText="城市" />
                            <asp:BoundField DataField="CompanyName" HeaderText="站点" />
                            <asp:BoundField DataField="success" HeaderText="妥投量" />
                            <asp:BoundField DataField="SuccessDeliverFee" HeaderText="妥投配送费" DataFormatString="{0:N2}">
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Refuse" HeaderText="拒收量" />
                            <asp:BoundField DataField="RefuseDeliverFee" HeaderText="拒收配送费" DataFormatString="{0:N2}">
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Protectedprice" HeaderText="保价费" DataFormatString="{0:N2}">
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:BoundField DataField="SumDeliverFee" HeaderText="总配送费" DataFormatString="{0:N2}">
                                <ItemStyle HorizontalAlign="Right" />
                            </asp:BoundField>
                            <asp:TemplateField HeaderText="导出明细" ShowHeader="False">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnReportDetail" runat="server" CausesValidation="False" CommandName="Edit"
                                        Text="导出明细"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <label id="NOData">
                                没有数据</label>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="8">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="8">
                <fieldset>
                    <legend>报表汇总</legend>
                    <table style="width: 100%; border-width: 0px;">
                        <tr>
                            <td>
                                报表周期：
                            </td>
                            <td>
                                <asp:Label ID="lblReportCycle" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                妥投量：
                            </td>
                            <td>
                                <asp:Label ID="lblSuccessCount" runat="server"></asp:Label>
                            </td>
                            <td>
                                妥投配送费：
                            </td>
                            <td>
                                <asp:Label ID="lblSuccessFeeSum" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                拒收量：
                            </td>
                            <td>
                                <asp:Label ID="lblFailCount" runat="server"></asp:Label>
                            </td>
                            <td>
                                拒收配送费：
                            </td>
                            <td>
                                <asp:Label ID="lblFailFeeSum" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                            
                            </td>
                             <td>
                                保价费：
                            </td>
                            <td>
                            <asp:Label ID="lblProtectedprice" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                单量合计：
                            </td>
                            <td>
                                <asp:Label ID="lblSumCount" runat="server"></asp:Label>
                            </td>
                            <td>
                                配送费合计：
                            </td>
                            <td>
                                <asp:Label ID="lblFeeSum" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
    </table>
</asp:Content>
