
// 注：2011-02-30 杨来旺 添加

//判断输入字符的长度，调用方法 OnClientClick="TxtLeagth('TextBox1',5);"
function TxtLeagth(ControlID, ControlName, ULeanth) {
    if (typeof (ControlID) != "string") {
        alert("控件名称错误！");
        return false;
    }
    if (isNaN(ULeanth)) {
        alert("参数错误！");
        return false;
    }
    UTrim(ControlID); //特殊字符判断

    var InputContent = document.getElementById(ControlID).value;
    var ContentLeanth = 0;
    for (var i = 0; i < InputContent.toString().length; i++) {
        var leg = InputContent.charCodeAt(i);
        if (leg > 255) {  //如果是汉字加两个字符。
            ContentLeanth += 2;
        }
        else {         //否则按一个字符相加。
            ContentLeanth += 1;
        }
    }

    if (ContentLeanth > parseInt(ULeanth)) {
        alert("输入内容过多，" + ControlName + " 要求不超过" + ULeanth.toString() + "个字符！\n");
        document.getElementById(ControlID).value = "";
        return false;
    }
}

//必填项验证，调用方法OnClientClick="UValidate('TextBox1','输入文本框值！','DropDownList1','输入下拉列表值！');"
function UValidate() {
    var i, msg = "";
    for (var i = 0; i < arguments.length; i += 2) {
        UTrim(arguments[i]);
        if (document.getElementById(arguments[i]).value == "") {
            msg += arguments[i + 1] + "\n";
        }
    }
    if (msg != "") {
        alert("提示： \n\n" + msg.toString());
        return false;
    }
}

//特殊字符过滤！ 调用方法 UTrim(ControlID); 
function UTrim(ControlID) {
    var InputContent = document.getElementById(ControlID).value;
    returnStr = InputContent.toString().replace(" ", "").replace("'", "").replace("%", "").replace("/", "").replace(";", "");
    if (returnStr.length != InputContent.toString().length) {
        alert("请勿输入以下特殊字符（单引号，百分号或空格等）！");
        return false;
    }
}

//必须是数字而且数字在一个范围内！调用方法 OnClientClick="ValidateNumber('TextBox1',null ,null );"
function ValidateNumber(ControlID, ControlName, minNum, maxNum) {
    var msg = "";
    if (isNaN(document.getElementById(ControlID).value)) {  //如果不是数字提示员工！
        msg += ControlName + "只能输入数字！";
    }
    else { //如果是数字不符合规定范围提示员工！
        if (minNum != null && parseFloat(document.getElementById(ControlID).value) < minNum) {
            msg += ControlName + "输入的值不能小于 " + minNum.toString();
        }
        if (maxNum != null && parseFloat(document.getElementById(ControlID).value) > maxNum) {
            msg += ControlName + "输入的值不能大于 " + maxNum.toString();
        }
    }
    if (msg != "") {
        alert(msg);
        return false;
    }
}

//判断年月格式！（例如： 2009-09,200909）调用：OnClientClick="ValidateYM('TextBox1',true);"
function ValidateYM(ControlID, MustFlag) {
    var InputContent = document.getElementById(ControlID).value;
    var YM = InputContent.toString().replace("-", "");
    var flag = true;
    if (MustFlag == true && InputContent == "") {
        alert("请输入日期！");
        return false;
    }
    if (InputContent != "") {
        if (YM.length != 6 && YM.length != 5) {
            flag = false; //去掉-后是6位或5位。
        }
        else {
            var vMonth = 0;
            var vYear = parseInt(YM.substr(0, 4));
            if (YM.length == 6)
                vMonth = parseInt(YM.substr(4, 2).toString().replace("0", ""));
            else if (YM.length == 5)
                vMonth = parseInt(YM.substr(4, 1).toString().replace("0", ""));
            if (isNaN(vYear) || isNaN(vMonth) || vYear < 1 || vMonth > 12 || vMonth < 1)
                flag = false;
        }
        if (flag == false) {
            alert("日期格式不正确！");
            
            return false;
        }
    }
}

//检查年月日格式
function ValidateYMD(ControlID) {
    var date1 = document.getElementById(ControlID).value
    if (date1 != "" && !checkDateYMD(date1)) {
        alert("输入的日期不合法,请输入格式为0000-00-00的日期!");
        
        return false;
    }
}
//日期格式检查
function checkDateYMD(val) {

    var lan = 2;
    var y = 0, m = 0, d = 0;

    if (val.indexOf('(') >= 0) {
        val = val.split('(')[0];
    }
    while (val.indexOf('.') >= 0) {
        val = val.replace('.', '/');
    }
    while (val.indexOf('-') >= 0) {
        val = val.replace('-', '/');
    }
    var ymd = val.split('/');

    if (ymd.length != 3 || isNaN(ymd[0]) || ymd[0] == "" || ymd[0].length != 4) {
        return false;
    }
    if (ymd[0] == "" || ymd[1] == "" || ymd[2] == "") {
        return false;
    }
    var sy = '';
    if (lan == 0 || lan == 2) {
        sy = ymd[0];
        if (isNaN(ymd[1]) || ymd[1].length >= 3) {
            return false;
        }
        m = new Number(ymd[1]);
        if (isNaN(ymd[2]) || ymd[2].length >= 3) {
            return false;
        }
        d = new Number(ymd[2]);

    } else if (lan == 1) {
        m = new Number(ymd[0]);
        d = new Number(ymd[1]);
        sy = ymd[2];
    }
    var year = new Date().getFullYear().toString();
    if (sy.length == 2) {
        sy = year.substring(0, 2) + sy;
    } else if (sy.length == 1) {
        sy = year.substring(0, 3) + sy;
    }
    y = new Number(sy);
    if (m < 1 || m > 12 || isNaN(m) || m == "") {
        return false;
    } else {
        m = m - 1;
    }

    var daysInMonth = new Array(31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31);

    var days = '';
    if (1 == m) {
        days = ((0 == y % 4) && (0 != (y % 100))) || (0 == y % 400) ? 29 : 28;
    } else {
        days = daysInMonth[m];
    }

    if (d > days) {
        return false;
    }
    return true;
}

//判断是否为日期
function isDateString(sDate) {
    var iaMonthDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
    var iaDate = new Array(3)
    var year, month, day
    if (arguments.length != 1) return false
    iaDate = sDate.toString().split("-")
    if (iaDate.length != 3) return false
    if (iaDate[1].length > 2 || iaDate[2].length > 2) return false

    year = parseFloat(iaDate[0])
    month = parseFloat(iaDate[1])
    day = parseFloat(iaDate[2])

    if (year < 1900 || year > 2100) return false
    if (((year % 4 == 0) && (year % 100 != 0)) || (year % 400 == 0)) iaMonthDays[1] = 29;
    if (month < 1 || month > 12) return false
    if (day < 1 || day > iaMonthDays[month - 1]) return false
    return true
}

//全选表格(onclick="CheckAll('<%=GridView.ClientID %>','ChkAll');")
function CheckAll(GridViewControlID, CheckControlID) {
    for (var i = 0; i < document.getElementById(GridViewControlID).rows.length; i++) {
        document.getElementById(GridViewControlID).rows[i].childNodes[0].childNodes[0].checked = document.getElementById(CheckControlID).checked;
    }
}

//判断至少要选择一项
function CheckItems(GridViewControlID) {
    var x = 0;
    for (var i = 1; i < document.getElementById(GridViewControlID).rows.length; i++) {
        if (document.getElementById(GridViewControlID).rows[i].childNodes[0].childNodes[0].checked == true) {
            x++;
        }
    }
    if (x == 0) {
        alert("请至少选择一项！");
        return false;
    }
}


//选中后变色
function ChangeRowColor(GridViewControlID) {
    var x = 0;
    for (var i = 1; i < document.getElementById(GridViewControlID).rows.length; i++) {
        if (document.getElementById(GridViewControlID).rows[i].childNodes[0].childNodes[0].checked == true) {
            document.getElementById(GridViewControlID).rows[i].style.backgroundColor = '#C8F0B4';
        }
    }
}

//清空文本，调用方法ClearTextBox="UValidate('TextBox1','TextBox2'...);"
function ClearTextBox() {
    var i;
    for (var i = 0; i < arguments.length; i += 1) {
        document.getElementById(arguments[i]).value = "";
    }
}

//获取页面所有文本框，判断是否有非法字符
function GetAllTextBox() {
    var boxs = document.getElementsByTagName("input");

    for (var i = 0; i < boxs.length; i++) {
        if (boxs[i].type == "text") {
            var InputContent = boxs[i].value;
            returnStr = InputContent.toString().replace("'", "");
            if (returnStr.length != InputContent.toString().length) {
                alert("请勿输入以下特殊字符(单引号)！");
                return false;
            }
        }

    }

}