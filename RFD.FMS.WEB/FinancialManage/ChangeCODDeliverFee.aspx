<%@ Page Title="" Language="C#" AutoEventWireup="true"
    Theme="default" EnableEventValidation="false" CodeBehind="ChangeCODDeliverFee.aspx.cs"
    Inherits="RFD.FMS.WEB.FinancialManage.ChangeCODDeliverFee" %>
<%@ Import Namespace="System.Data" %>

<%@ Register Src="~/UserControl/SelectStationCommon.ascx" TagName="SelectStation" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/Pager.ascx" TagName="Pager" TagPrefix="uc2" %>
<%@ Register Src="~/UserControl/MerchantSourceTV.ascx" TagName="UCMerchantSourceTV"
    TagPrefix="uc4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>配送价格编辑</title>
        <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
        <script src="../Scripts/import/common.js" type="text/javascript"></script>
        <script type="text/javascript" src="../Scripts/controls/selectStationCommon.js"></script>
        <script src="../Scripts/controls/OpenCommon.js" type="text/javascript"></script>
        <style type="text/css">
            .topContent
            {
                border:solid 1px #666;width: 220px;padding: 5px;display: none
            }
        </style>
        <script type="text/javascript">
            window.name = "winDeliveryPriceEdit";
            //var mod = $.dom("hidMode", "input");

            $(function () {
                $(".Wdate").bind("focus", function () {
                    WdatePicker({ skin: 'whyGreen', dateFmt: 'yyyy-MM-dd' });
                });
                //alert($("#<%=hidMode.ClientID%>").val());
                if ($("#<%=hidMode.ClientID%>").val() == "1") {
                    //alert(11);
                    $("#BatchChangeDiv").show();
                    $("#SingleChange").hide();
                    $("#ChangeBtn").val("切换单据修改");
                }
                else {
                    //alert(22);
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
                        //                        <% ViewState["mode"] = "1";%> ;
                        //                        alert( <%=ViewState["mode"]%> );
                    }
                    else {
                        $("#BatchChangeDiv").hide();
                        $("#SingleChange").show();
                        $(this).val("切换批量修改");
                        $("#<%=hidMode.ClientID%>").val("0");
                        //                        <% ViewState["mode"] = "0";%> ;
                        //                        alert( <%=ViewState["mode"]%> );
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
                    $(document).progressDialog.showDialog("系统服务从新计算, 请耐心等待1分钟左右...");
                });
                $("#<%=ImportCalcuBtn.ClientID%>").click(function () {
                    $(document).progressDialog.showDialog("计算中...");
                });
            });
           
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
        
    </head>
    <body>
	    <form id="form1" runat="server" target="winDeliveryPriceEdit">
         <asp:HiddenField ID="hidMode" runat="server" Value="0" />
	        <script type="text/javascript">
	            function fnPriceClick(url, txtId) {
	                var p = window.showModalDialog(url, window, 'dialogWidth=500px;dialogHeight=300px;center:yes;resizable:no;scroll:auto;help=no;status=no;');
	                if (p != 'undefined' && p != null) {
	                    $("#" + txtId + "").val(p);
	                }
	            }
            </script>
<%--            <asp:Panel ID="Panel1" runat="server" GroupingText="重新计算配送费">
                
            </asp:Panel>--%>
            <br />
             <input type="button" id="ChangeBtn" value="切换批量修改"/>
             <div id="pop_message" class="topContent">点击切换单据修改和批量修改</div>
             <div id="SingleChange">
            <asp:Panel ID="pSumData" runat="server" GroupingText="单据修改配送费">
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
                                <asp:Button ID="btnSearch" Text="查询" runat="server" onclick="btnSearch_Click"/>
                            </td>
                            <td>
                                <asp:Button ID="btnReEval" Text="重新计算" runat="server" 
                                    onclick="btnReEval_Click"/>
                            </td>
                        </tr>
                    </table>
                    <br/>
                
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
                                <asp:Button ID="btnEvalFee" Text="计算配送费" Width="100px" runat="server" 
                                    onclick="btnEvalFee_Click"/>
                            </td>
                            <td colspan="2" align="left">
                                <asp:Button ID="btnCommitChange" Text="确认修改" Width="100px" runat="server" 
                                    onclick="btnCommitChange_Click"/>
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
				           <a href="../UpFile/配送商配送费修改导入模版.xlsx" style="color: Blue;">下载模板</a>
                           <asp:FileUpload ID="fileUpload" runat="server" Style="width: 180px" ToolTip="请你先下载模板到本地，然后填写相关的信息再进行导入！" />
				           <asp:Button ID="btnBatchImport" runat="server" Text="导入" 
					        title="请你先下载模板到本地，然后填写相关的信息再进行导入！" onclick="btnBatchImport_Click"/>
                        </td> 
                        <td>
                            <asp:Button ID="ExportFailedBtn" runat="server" Text="导出失败" 
                                onclick="ExportFailedBtn_Click"/>
                        </td>
                    </tr>
                    <tr>
                        <td> 
                            <asp:Label ID="TipsLab" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                       <td>
                           <asp:Button ID="SysCalcuBtn" runat="server" Text="按系统重新计算" 
                               onclick="SysCalcuBtn_Click1" />
                           <asp:Button ID="ImportCalcuBtn" runat="server" Text="按导入重新计算" 
                               onclick="ImportCalcuBtn_Click" />
                       </td>
                       <td>
                            <asp:Button ID="BatchSaveBtn" runat="server" Text="确认修改" 
                                onclick="BatchSaveBtn_Click" />
                       </td>
                    </tr>
                 </table> 

                     <asp:gridview ID="gvList" runat="server" AllowPaging="false" AutoGenerateColumns="false">
                       <Columns>
                           <asp:BoundField HeaderText="唯一编号" DataField="InfoID" />
                           <asp:BoundField HeaderText="分配区域" DataField="AreaType" />
                           <asp:BoundField HeaderText="订单号" DataField="WaybillNo" />
                           <asp:BoundField HeaderText="重量" DataField="AccountWeight" />
                           <asp:BoundField HeaderText="配送费公式" DataField="FareFormula" />
                           <asp:BoundField HeaderText="配送公司" DataField="CompanyName"/>
                           <asp:BoundField HeaderText="配送费" DataField="Fare"/>
                           <asp:TemplateField HeaderText="查看操作日志">
			        <ItemTemplate>
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
				            <a href="javascript:fnOpenModalDialog('../BasicSetting/OperateLogView.aspx?PK_NO=<%#((DataRowView)Container.DataItem)["InfoID"] %>&LogType=8',500,300)">查看操作日志</a>
                     </asp:PlaceHolder> 
			        </ItemTemplate>
		        </asp:TemplateField>
                       </Columns>
                     </asp:gridview>

              </asp:Panel>

         </div>
        </form>
    </body>
</html>
