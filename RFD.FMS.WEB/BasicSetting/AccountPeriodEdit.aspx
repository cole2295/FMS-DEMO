<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccountPeriodEdit.aspx.cs" Inherits="RFD.FMS.WEB.BasicSetting.AccountPeriodEdit" Theme="default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register Src="~/UserControl/ExpressCompanyTV.ascx" TagName="UCExpressCompanyTV" TagPrefix="uc1" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>账期编辑</title>
    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
    <script src="../Scripts/My97DatePicker/WdatePicker.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/controls/OpenCommon.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        window.name = "AccountPeriodEdit";

        $(document).ready(function () {
            fnChangeExpress(true);
            CreateSelectMonthDay();

            //初始化编辑
            fnInitEdit();
            if ("<%=IsImitate %>" == 1) {
                $("#btnImitatePeriod").attr("disabled", "disabled");
                $("#btnOk").attr("disabled", "disabled");
                ImitatePeriod();
            }
        });
        //窗体控件
        function GetControl() {
            var c = {
                hidPeriodType: $.dom("hidPeriodType", "input"),
                hidIsMonthPeriod: $.dom("hidIsMonthPeriod", "input"),
                hidPeriodTypeChild: $.dom("hidPeriodTypeChild", "input"),
                hidIsDeleted: $.dom("hidIsDeleted", "input"),
                hidPeriodStart: $.dom("hidPeriodStart", "input"),
                hidIsExpress: $.dom("hidIsExpress", "input"),
                hidIntervalNum: $.dom("hidIntervalNum", "input"),
                rbPeriodType1: $.dom("rbPeriodType1", "input"),
                rbPeriodType2: $.dom("rbPeriodType2", "input"),
                rbPeriodType3: $.dom("rbPeriodType3", "input"),
                cbIsMonthPeriod: $.dom("cbIsMonthPeriod", "input"),
                spanIsMonthPeriod: $.dom("spanIsMonthPeriod", "span"),
                divWeek: $.dom("divWeek", "div"),
                divMonth: $.dom("divMonth", "div"),
                divCustom: $.dom("divCustom", "div"),
                divPublic: $.dom("divPublic", "div"),
                divWeek1: $.dom("divWeek1", "div"),
                divWeek2: $.dom("divWeek2", "div"),
                selectExpress: $.dom("selectExpress", "select"),
                selectMonthDay: $.dom("selectMonthDay", "select"),
                PeriodTypeChild1: $.dom("PeriodTypeChild1", "input"),
                PeriodTypeChild2: $.dom("PeriodTypeChild2", "input"),
                rbEnableYes: $.dom("rbEnableYes", "input"),
                rbEnableNo: $.dom("rbEnableNo", "input"),
                txtPeriodName: $.dom("txtPeriodName", "input"),
                txtStartDate: $.dom("txtStartDate", "input"),
                txtIntervalNum: $.dom("txtIntervalNum", "input"),
                txtWeekIntervalnum: $.dom("txtWeekIntervalnum", "input"),
                divWeekIntervalnum: $.dom("divWeekIntervalnum", "div")
            };
            return c;
        }

        //加载编辑
        function fnInitEdit() {
            if ("<%=Kid %>" == "")
                return;

            $("#divPeriodType").attr("disabled", "disabled");
            $("#tdPeriod").attr("disabled", "disabled");

            var c = GetControl();
            //账期类型
            if (c.hidPeriodType.val() != undefined && c.hidPeriodType.val()>0) {
                $("#rbPeriodType" + c.hidPeriodType.val()).attr("checked", "checked");
                fnChangePeriodType();
            }
            //子账期类型
            if (c.hidPeriodTypeChild.val() > 0) {
                document.getElementById("PeriodTypeChild" + c.hidPeriodTypeChild.val()).checked = "checked";
                fnChangePeriodTypeChild(document.getElementById("PeriodTypeChild" + c.hidPeriodTypeChild.val()));
                fnSetWeekPeriod();
                if (c.hidPeriodType.val() == "1" && c.hidPeriodTypeChild.val() == "1") {
                    c.txtWeekIntervalnum.val(c.hidIntervalNum.val());
                }
            }
            else {
                //月的直接在创建初始化时写值
                //自定义
                if (c.hidPeriodType.val() == "3") {
                    c.txtIntervalNum.val(c.hidIntervalNum.val());
                    c.txtStartDate.val(c.hidPeriodStart.val());
                }
            }
            //是否启用
            c.hidIsDeleted.val() == "0" ? c.rbEnableYes.attr("checked", "checked") : c.rbEnableNo.attr("checked", "checked");
            //只结当月费用，未结部分流入下一账期
            c.hidIsMonthPeriod.val() == "1" ? c.cbIsMonthPeriod.attr("checked", "checked") : c.cbIsMonthPeriod.removeAttr("checked");
            //配送公司
            c.selectExpress.val(c.hidIsExpress.val());
            fnChangeExpress(false);
        }

        //创建月下拉
        function CreateSelectMonthDay() {
            var c = GetControl();
            c.selectMonthDay.append("<option value=''>请选择</option>"); 
            for (var i = 1; i < 29; i++) {
                c.selectMonthDay.append("<option value='" + i + "'>" + i + "</option>");
            }
            if (c.hidPeriodType.val() == "2") {
                if(c.hidPeriodStart.val()>0)
                    c.selectMonthDay.val(c.hidPeriodStart.val());
            }  
        }

        //变更账期
        function fnChangePeriodType() {
            var c = GetControl();
            if (!!c.rbPeriodType1.attr("checked")) {
                c.divWeek.show();
                c.divMonth.hide();
                c.divCustom.hide();
                c.divPublic.show();
                c.cbIsMonthPeriod.show();
                c.spanIsMonthPeriod.show();
                c.hidPeriodType.val(c.rbPeriodType1.val());
            }
            if (!!c.rbPeriodType2.attr("checked")) {
                c.divWeek.hide();
                c.divMonth.show();
                c.divCustom.hide();
                c.divPublic.show();
                c.cbIsMonthPeriod.hide();
                c.spanIsMonthPeriod.hide();
                c.hidPeriodType.val(c.rbPeriodType2.val());
            }
            if (!!c.rbPeriodType3.attr("checked")) {
                c.divWeek.hide();
                c.divMonth.hide();
                c.divCustom.show();
                c.divPublic.show();
                c.cbIsMonthPeriod.show();
                c.spanIsMonthPeriod.show();
                c.hidPeriodType.val(c.rbPeriodType3.val());
            }
        }

        $(function () {
            var c = GetControl();
            c.cbIsMonthPeriod.click(function () {
                if (!!c.cbIsMonthPeriod.attr("checked")) {
                    c.hidIsMonthPeriod.val("1");
                } else {
                    c.hidIsMonthPeriod.val("0");
                }
                //alert(c.hidIsMonthPeriod.val());
            });
            c.selectExpress.change(function () {
                c.hidIsExpress.val(c.selectExpress.val());
                fnChangeExpress(true);
                //alert(c.hidIsExpress.val());
            });
            c.selectMonthDay.change(function () {
                c.hidPeriodStart.val(c.selectMonthDay.val());
                //alert(c.hidPeriodStart.val());
            });
        });

        function fnChangeExpress(isShow) {
            var c = GetControl();
            var expressImg = GetExpressImageID();
            if (c.hidIsExpress.val() == "0") {
                expressImg.attr("disabled", "disabled");
                expressImg.attr("title", "全部包含时不可选");
            } else {
                expressImg.removeAttr("disabled");
                expressImg.attr("title", "选择配送公司");
            }
            if (!isShow) {
                expressImg.attr("disabled", "disabled");
                expressImg.attr("title", "不可选择");
            }
        }

        function fnChangePeriodTypeChild(E) {
            var c = GetControl();
            c.hidPeriodTypeChild.val(E.value);
            c.divWeek1.attr("disabled", "disabled");
            c.divWeek2.attr("disabled", "disabled");
            $("#divWeek" + E.value).removeAttr("disabled");
            if (E.value == "1") {
                c.divWeekIntervalnum.removeAttr("disabled");
            }
            else {
                c.divWeekIntervalnum.attr("disabled", "disabled");
            }
            //alert(c.hidPeriodTypeChild.val())
        }

        function fnCheckWeekPeriod(E, type) {
            var c = GetControl();
            var arrH = new Array();
            var checkList = $("#" + E.parentNode.id).find('input:' + type + '[checked]').each(function () {
                arrH[arrH.length] = this.value + ",";
            });
            c.hidPeriodStart.val(arrH.join('').substring(0, arrH.join('').length - 1));
            //alert(c.hidPeriodStart.val());
        }

        function fnSetWeekPeriod() {
            var c = GetControl();
            var hidPeriodStart = c.hidPeriodStart.val().split(',');
            var type = c.hidPeriodTypeChild.val() == "1" ? "checkbox" : "radio";
            var divWeeks = c.hidPeriodTypeChild.val() == "1" ? c.divWeek1 : c.divWeek2;
            divWeeks.find('input:' + type).each(function () {
                for (var i = 0; i < hidPeriodStart.length; i++) {
                    if ($(this).val() == hidPeriodStart[i]) {
                        $(this).attr("checked", "checked");
                    }
                }
            });
        }
        //设置按周间隔天数
        function fnSetWeekIntervalNum() {
            var c = GetControl();
            if (!fnJudgeNumber(c.txtWeekIntervalnum)) return false;
            c.hidIntervalNum.val(c.txtWeekIntervalnum.val());
            //alert(c.hidIntervalNum.val());
        }
        //设置自定义间隔天数
        function fnSetIntervalNum() {
            var c = GetControl();
            if (!fnJudgeNumber(c.txtIntervalNum)) return false;
            c.hidIntervalNum.val(c.txtIntervalNum.val());
            //alert(c.hidIntervalNum.val())
        }

        function fnSetStartDate() {
            var c = GetControl();
            c.hidPeriodStart.val(c.txtStartDate.val());
            //alert(c.hidPeriodStart.val())
        }

        function fnJudgeNumber(E) {
            if (E.val().length <= 0) {
                alert("必须输入数字");
                E.select();
                return false;
            }
            if (isNaN(E.val())) {
                alert("必须输入数字");
                E.select();
                return false;
            }
            return true;
        }

        function fnJudgeInput(n) {
            var c = GetControl();
            if (c.hidPeriodType.val() == "0") {
                alert("请选择账期类型");
                return false;
            }
            if (c.hidPeriodType.val() == "1" && c.hidPeriodTypeChild.val() == "0") {
                alert("请选择周子账期类型");
                return false;
            }
            if (c.hidPeriodStart.val() == "0" || c.hidPeriodStart.val() == "") {
                alert("请设定账期");
                return false;
            }
            if (c.hidPeriodType.val() == "3" && (c.hidIntervalNum.val() == "0" || c.hidIntervalNum.val() == "")) {
                alert("周期起始日期不能为空");
                return false;
            }
            if (c.hidPeriodType.val() == "1" && c.hidPeriodTypeChild.val()=="1" && c.hidIntervalNum.val() == "" && n==0) {
                alert("按周设置间隔天数不能为空");
                return false;
            }
            if (c.txtPeriodName.val() == "" && n==0) {
                alert("账期名称不能为空");
                return false;
            }
            return true;
        }

        function fnGetPeriodData() {
            var c = GetControl();
            var isEnable = c.rbEnableYes.attr("checked") ? 0 : 1;
            var expressId = GetUseExpressID();
            return "{userid:'" + "<%=UserId %>"
                        + "',distributionCode:'" + "<%=DisCode %>"
                        + "',hidPeriodType:'" + c.hidPeriodType.val()
                        + "',hidPeriodTypeChild:'" + c.hidPeriodTypeChild.val()
                        + "',expressId:'" + expressId
                        + "',hidPeriodStart:'" + c.hidPeriodStart.val()
                        + "',hidIntervalNum:'" + c.hidIntervalNum.val()
                        + "',txtPeriodName:'" + c.txtPeriodName.val()
                        + "',hidIsExpress:'" + c.hidIsExpress.val()
                        + "',hidIsMonthPeriod:'" + c.hidIsMonthPeriod.val()
                        + "',isEnable:'" + isEnable
                        + "',PeriodSource:'" + "<%=PeriodSource %>"
                        + "',Kid:'" + "<%=Kid %>"
                        + "'}";
        }

        function SubmitData() {
            if (!fnJudgeInput(0)) return false;
            var isEnable = $("#rbEnableYes").attr("checked") ? 0 : 1;
            var expressId = GetUseExpressID();
            //alert(expressId)
            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: "AccountPeriodEdit.aspx/Submit",
                data: fnGetPeriodData(),
                dataType: 'json',
                success: function (result) {
                    if (result.d == null) { alert("服务器无响应"); }
                    var obj = result.d.dataModel;
                    var errorMsg = obj.ErrorName;
                    if (result.d.done) {
                        alert(errorMsg);
                        window.close();
                    }
                    else {
                        alert(errorMsg);
                    }
                }
            });
        }

        function ImitatePeriod() {
            if (!fnJudgeInput(1)) return false;
            $.ajax({
                type: "POST",
                contentType: "application/json",
                url: "AccountPeriodEdit.aspx/LoadImitatePeriod",
                data: fnGetPeriodData(),
                dataType: 'json',
                success: function (result) {
                    if (result.d == null) { alert("服务器无响应"); }
                    var obj = result.d.dataModel;
                    var errorMsg = obj.ErrorName;
                    if (result.d.done) {
                        $("#divImitatePeriod").html(errorMsg);
                    }
                    else {
                        alert(errorMsg);
                    }
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" target="AccountPeriodEdit">
    <div style="overflow:auto; border:1px solid #ccc; width:618px; height:418px;">
        <table border="0" cellpadding="0" cellspacing="0" width="608px" style="height:260px; margin:5px; border:1px solid #ccc;">
            <tr>
                <td align="left" valign="top" style="border-right:1px solid #ccc; width:150px; padding:5px;">
                <div id="divPeriodType" style="line-height:30px; font-size:14px;border:0px solid red;">
                    <div>账期类型：</div>
                    <div style="margin-left:20px;">
                        <input type="radio" id="rbPeriodType1" name="PeriodType" value="1" onclick="fnChangePeriodType();" />按周设置<br />
                        <input type="radio" id="rbPeriodType2" name="PeriodType" value="2" onclick="fnChangePeriodType();" />按月设置<br />
                        <input type="radio" id="rbPeriodType3" name="PeriodType" value="3" onclick="fnChangePeriodType();" />自定义设置
                        <asp:HiddenField ID="hidPeriodType" runat="server" Value="0" />
                    </div>
                </div>
                <div style="margin-top:20px;line-height:30px; border:0px solid red;">
                    <div>账期名称<asp:TextBox ID="txtPeriodName" runat="server" Width="120px" MaxLength="50"></asp:TextBox></div>
                    <div>是否启用
                        <input type="radio" id="rbEnableYes" name="rbEnable" value="0" checked="checked" />是
                        <input type="radio" id="rbEnableNo" name="rbEnable" value="1" />否<br />
                    </div>
                    <div style="text-align:right;">
                        <input type="button" id="btnImitatePeriod" value="模拟账期" onclick="ImitatePeriod();" />
                        <input type="button" id="btnOk" value="确定" onclick="SubmitData();" />
                    </div>
                </div>
            </td>
                                                                                                                                                                                                                                                                <td id="tdPeriod" align="left" valign="top"  style="width:453px; padding:5px;">
                <%--周--%>
                <div id="divWeek" style="display:none; line-height:30px;">
                    <input type="radio" id="PeriodTypeChild1" name="PeriodTypeChild" value="1" onclick="fnChangePeriodTypeChild(this);" />一周N次(选择一周的哪几天结)<br />
                    <%--一周N次--%>
                    <div id="divWeek1" style=" margin-left:20px; line-height:30px;" disabled="disabled">
                        <input type="checkbox" id="cbWeek1" name="cbweeks" value="1" onclick="fnCheckWeekPeriod(this,'checkbox');" />周一
                        <input type="checkbox" id="cbWeek2" name="cbweeks" value="2" onclick="fnCheckWeekPeriod(this,'checkbox');" />周二
                        <input type="checkbox" id="cbWeek3" name="cbweeks" value="3" onclick="fnCheckWeekPeriod(this,'checkbox');" />周三
                        <input type="checkbox" id="cbWeek4" name="cbweeks" value="4" onclick="fnCheckWeekPeriod(this,'checkbox');" />周四
                        <input type="checkbox" id="cbWeek5" name="cbweeks" value="5" onclick="fnCheckWeekPeriod(this,'checkbox');" />周五
                        <input type="checkbox" id="cbWeek6" name="cbweeks" value="6" onclick="fnCheckWeekPeriod(this,'checkbox');" />周六
                        <input type="checkbox" id="cbWeek7" name="cbweeks" value="7" onclick="fnCheckWeekPeriod(this,'checkbox');" />周日
                    </div>
                    <input type="radio" id="PeriodTypeChild2" name="PeriodTypeChild" value="2" onclick="fnChangePeriodTypeChild(this);" />本周结上周(选择周几结上一周)<br />
                    <%--本周结上周--%>
                    <div id="divWeek2" style=" margin-left:20px; line-height:30px;" disabled="disabled">
                        <input type="radio" id="rbWeek1" name="rbweeks" value="1" onclick="fnCheckWeekPeriod(this,'radio');" />周一
                        <input type="radio" id="rbWeek2" name="rbweeks" value="2" onclick="fnCheckWeekPeriod(this,'radio');" />周二
                        <input type="radio" id="rbWeek3" name="rbweeks" value="3" onclick="fnCheckWeekPeriod(this,'radio');" />周三
                        <input type="radio" id="rbWeek4" name="rbweeks" value="4" onclick="fnCheckWeekPeriod(this,'radio');" />周四
                        <input type="radio" id="rbWeek5" name="rbweeks" value="5" onclick="fnCheckWeekPeriod(this,'radio');" />周五
                        <input type="radio" id="rbWeek6" name="rbweeks" value="6" onclick="fnCheckWeekPeriod(this,'radio');" />周六
                        <input type="radio" id="rbWeek7" name="rbweeks" value="7" onclick="fnCheckWeekPeriod(this,'radio');" />周日
                    </div>
                    <div id="divWeekIntervalnum">
                        账期日向前间隔<input id="txtWeekIntervalnum" type="text" size="5" onblur="fnSetWeekIntervalNum();" />天
                    </div>
                </div>
                <%--月--%>
                <div id="divMonth" style="display:none; line-height:30px;">
                    每月<select id="selectMonthDay">
                    </select>日结上月整月
                </div>
                <%--自定义--%>
                <div id="divCustom" style="display:none; line-height:30px;">
                    <div>间隔天<input type="text" id="txtIntervalNum" size="10" onblur="fnSetIntervalNum();" />天</div>
                    <div>周期起始日期
                        <input type="text" id="txtStartDate" size="20" onFocus="WdatePicker({startDate:'%y-%M-%d',dateFmt:'yyyy-MM-dd',alwaysUseStartDate:true})" onblur="fnSetStartDate();" />
                    </div>
                </div>
                <div id="divPublic" style="display:none; line-height:30px;">
                    <input type="checkbox" id="cbIsMonthPeriod" name="IsMonthPeriod" /><span id="spanIsMonthPeriod">只结当月费用，未结部分流入下一账期</span><br />
                    <div style=" clear:both;">
                        <div style="float:left;"><select id="selectExpress">
                            <option value="0">全部配送公司</option>
                            <option value="1">包含哪些配送公司</option>
                            <option value="2">不包含哪些配送公司</option>
                        </select>
                        </div>
                        <div style="float:left; line-height:0px;">
                            <uc1:UCExpressCompanyTV ID="UCExpressCompanyTV" runat="server" ExpressCompanyShowCheckBox="True" ExpressLoadType="ThirdCompany" ExpressTypeSource="other_Express" />
                        </div>
                    </div>
                </div>

                <asp:HiddenField ID="hidPeriodTypeChild" runat="server" Value="0" /><%--子账期--%>
                <asp:HiddenField ID="hidPeriodStart" runat="server" Value="0" /><%--账期开始值：周值（,分割），月（日数），自定义（起始日期）--%>
                <asp:HiddenField ID="hidIsMonthPeriod" runat="server" Value="0" /><%--是否只结当月费用，未结部分流入下一账期，0否，1是--%>
                <asp:HiddenField ID="hidIsExpress" runat="server" Value="0" /><%--是否按配送商，0、全部，1、不包含，2包含--%>
                <asp:HiddenField ID="hidExpressId" runat="server" Value="0" /><%--配送公司ID--%>
                <asp:HiddenField ID="hidIntervalNum" runat="server" Value="0" /><%--间隔数--%>
                <asp:HiddenField ID="hidIsDeleted" runat="server" Value="0" /><%--是否启用--%>
            </td>
            </tr>
        </table>
        <div  style="overflow:auto; width:606px; height:141px;border:1px solid #ccc;margin:5px;">
            <div id="divImitatePeriod" style="padding:5px; line-height:20px;"></div> 
        </div>
    </div>
    </form>
</body>
</html>
