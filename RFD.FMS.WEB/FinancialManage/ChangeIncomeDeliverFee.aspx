<%@ Page Title="" Language="C#" AutoEventWireup="true" Theme="default" EnableEventValidation="false" MasterPageFile="~/Main/main.Master"
    CodeBehind="ChangeIncomeDeliverFee.aspx.cs" Inherits="RFD.FMS.WEB.FinancialManage.ChangeIncomeDeliverFee" %>
<%@ Import Namespace="System.Data" %>

<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="SelectStation"
    TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV"
    TagPrefix="uc4" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">

        $(function () {
            $(".Wdate").bind("focus", function () {
                WdatePicker({ skin: 'whyGreen', dateFmt: 'yyyy-MM-dd' });
            });

            if ($("#<%=hidMode.ClientID%>").val() == "1") {

                $("#BatchChangeDiv").show();
                $("#SingleChange").hide();
                $("#ChangeBtn").val("切换单据修改");
            }
            else {

                $("#BatchChangeDiv").hide();
                $("#SingleChange").show();
                $("#ChangeBtn").val("切换批量修改");
            }

            $("#ChangeBtn").click(function () {
                if ($(this).val() == "切换批量修改") {
                    $("#BatchChangeDiv").show();
                    $("#SingleChange").hide();
                    $(this).val("切换单据修改");
                    $("#<%=hidMode.ClientID%>").val("1");

                }
                else {
                    $("#BatchChangeDiv").hide();
                    $("#SingleChange").show();
                    $(this).val("切换批量修改");
                    $("#<%=hidMode.ClientID%>").val("0");

                }

            });

            $("#ChangeBtn").hover(function () {
                $("#pop_message").toggle();
            },
                    function () {
                        $("#pop_message").toggle();
                    });
            $("#<%=BatchSaveBtn.ClientID%>").click(function () {

                if (window.confirm("确认批量修改？")) {
                    $(document).progressDialog.showDialog("修改中...");
                };
            });
            $("#<%=btnBatchImport.ClientID%>").click(function () {

                $(document).progressDialog.showDialog("导入中...");
            });
            $("#<%=SysCalcuBtn.ClientID%>").click(function () {
                $(document).progressDialog.showDialog("系统服务从新计算, 请耐心等待..");
            });
            $("#<%=ImportCalcuBtn.ClientID%>").click(function () {
                $(document).progressDialog.showDialog("计算中...");
            });
        });

        function LogSerch() {
            var id = $("#waybillNoTxt").val;
            if (isNaN(id)) {
                alert("输入格式不正确");
                return;
            }
           var url = "../BasicSetting/OperateLogViewByNo.aspx?wayBillNo=" + id + "&LogType=7";
            fnOpenModalDialog(url, 500, 300); 
        }

        function judgeInput() {
            return true;
        }
        function fnOpenModalDialog(url, width, height) {
            window.showModalDialog(url, window, "dialogWidth=" + width + "px;dialogHeight=" + height + "px;center:yes;resizable:no;scroll:auto;help=no;location=no;status=no;");
        }

        function fnOpenModelessDialog(url, width, height) {
            window.showModelessDialog(url, window, "dialogWidth=" + width + "px;dialogHeight=" + height + "px;center:yes;resizable:no;scroll:auto;help=no;location=no;status=no;");
        }

        function fnOpenWindow(url, width, height) {
            window.open(url, 'newwindow', 'height=' + height + ',width=' + width + ',top=0,left=0,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no')
        }
    </script>

</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="ContentPlaceHolderTitle" runat="server">
配送商配送费修改(收入)
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">

    <asp:HiddenField ID="hidMode" runat="server" Value="0" />
    <script type="text/javascript">
        function fnPriceClick(url, txtId) {
            var p = window.showModalDialog(url, window, 'dialogWidth=500px;dialogHeight=300px;center:yes;resizable:no;scroll:auto;help=no;status=no;');
            if (p != 'undefined' && p != null) {
                $("#" + txtId + "").val(p);
            }
        }
    </script>
    <br />
    <input type="button" id="ChangeBtn" value="切换批量修改" />
    <div id="pop_message" class="topContent">
        点击切换单据修改和批量修改</div>
    <div id="SingleChange">
        <asp:Panel ID="pSumData" runat="server" GroupingText="单据修改配送费">
            <br />
            <div style="width: 100%" align="center">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" Text="运单号" runat="server" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtWaybillNo" Width="150px" runat="server" />
                        </td>
                        <td>
                            <asp:Button ID="btnSearch" Text="查询" runat="server" OnClick="btnSearch_Click" />
                        </td>
                        <td>
                            <asp:Button ID="btnReEval" Text="重新计算" runat="server" OnClick="btnReEval_Click" />
                        </td>
                    </tr>
                </table>
                <br />
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" Text="分配区域" runat="server" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtArea" Width="150px" runat="server" />
                        </td>
                        <td>
                            <asp:Label ID="Label3" Text="结算重量" runat="server" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtWeight" Width="150px" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" Text="配送公式" runat="server" />
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txtFormula" Width="100%" ReadOnly="true" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" align="center">
                            <asp:Label ID="lblFee" Text="配送费" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="right">
                            <asp:Button ID="btnEvalFee" Text="计算配送费" Width="100px" runat="server" OnClick="btnEvalFee_Click" />
                        </td>
                        <td colspan="2" align="left">
                            <asp:Button ID="btnCommitChange" Text="确认修改" Width="100px" runat="server" OnClick="btnCommitChange_Click" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </div>
    <div id="BatchChangeDiv" style="width: 100%">
        <asp:Panel ID="bSumData" runat="server" GroupingText="批量修改配送费" Height="16px">
            <table>
                <tr>
                    <td colspan="3">
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                        <a href="../UpFile/商家配送费修改导入模板.xlsx" style="color: Blue;">下载模板</a>
                        <asp:FileUpload ID="fileUpload" runat="server" Style="width: 180px" ToolTip="请你先下载模板到本地，然后填写相关的信息再进行导入！" />
                        <asp:Button ID="btnBatchImport" runat="server" Text="导入" title="请你先下载模板到本地，然后填写相关的信息再进行导入！"
                            OnClick="btnBatchImport_Click" />
                    </td>
                    <td>
                        <asp:Button ID="ExportFailedBtn" runat="server" Text="导出失败" OnClick="ExportFailedBtn_Click" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="SysCalcuBtn" runat="server" Text="按系统重新计算" OnClick="SysCalcuBtn_Click1" />
                        <asp:Button ID="ImportCalcuBtn" runat="server" Text="按导入重新计算" OnClick="ImportCalcuBtn_Click" />
                    </td>
                    <td>
                        <asp:Button ID="BatchSaveBtn" runat="server" Text="确认修改" OnClick="BatchSaveBtn_Click" />
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="TipsLab" runat="server"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="PromptLab" runat="server"></asp:Label>
                    </td>
                </tr>
            
            <asp:GridView ID="gvList" runat="server" AllowPaging="false" AutoGenerateColumns="false">
                <Columns>
                    <asp:TemplateField HeaderText="序号">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="唯一编号" DataField="IncomeFeeID" />
                    <asp:BoundField HeaderText="运单号" DataField="WaybillNo" />
                    <asp:BoundField HeaderText="分配区域" DataField="AreaType" />
                    <asp:BoundField HeaderText="结算重量" DataField="AccountWeight" />
                    <asp:BoundField HeaderText="配送公式" DataField="AccountStandard" />
                    <asp:BoundField HeaderText="配送费" DataField="AccountFare" />
                      <asp:TemplateField HeaderText="查看操作日志">
			        <ItemTemplate>
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
				            <a href="javascript:fnOpenModalDialog('../BasicSetting/OperateLogView.aspx?PK_NO=<%#((DataRowView)Container.DataItem)["WaybillNo"] %>&LogType=7',500,300)">查看操作日志</a>
                     </asp:PlaceHolder> 
			        </ItemTemplate>
		        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            </table>
        </asp:Panel>
    </div>

</asp:Content>



