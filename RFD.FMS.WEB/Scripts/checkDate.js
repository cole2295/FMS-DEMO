/// <reference path="jquery-1.4.1-vsdoc.js" />

//检验输入日期是否正确
function checkDate_BI(dtsId, dteId) {
    var dts = $("#" + dtsId).val(); //起始日期
    var dte = $("#" + dteId).val();  //结束日期
    var returnValue = true;

    if (dts.toString() == "" && dte.toString() != "") {
        mic.alert("提示", "起始日期不能为空");

        $('.jqmClose').click(function () { $("#" + dtsId).focus(); });
     
        returnValue = false;
    }

    if (dts.toString() != "" && dte.toString() == "") {
        mic.alert("提示", "结束日期不能为空");
        $('.jqmClose').click(function () { $("#" + dteId).focus(); });
       
        returnValue = false;
    }

    if (dte.toString() != "" && dts.toString() != "") {

        var dtsTime = new Date(Date.parse(dts.replace(/-/g, "/")));
        var dteTime = new Date(Date.parse(dte.replace(/-/g, "/")));
        var cdt = (dteTime.getTime() - dtsTime.getTime()) / (24 * 60 * 60 * 1000); //相差天数
        if (cdt < 0) {
            mic.alert("提示", "起始日期不能大于结束日期");
            $('.jqmClose').click(function () { $("#" + dtsId).focus(); });
            returnValue = false;
        }
        //returnValue = dateCompare(dts, dte);
    }
    return returnValue;
}

// 比较两个时间的先后
function dateCompare(dtsDate, dteDate) {
    var resultVal = false;
    var url = "ProductPickDropSta.aspx?type=datecompare&sDate=" + dtsDate + "&eDate=" + dteDate;//需要修改
    $.ajax({
        async: false, // 默认true(异步请求)
        cache: false, // 默认true,设置为 false 将不会从浏览器缓存中加载请求信息。
        type: "GET", // 默认:GET 请求方式:[POST/GET]
        dataType: "html", //默认["xml"/"html"] 返回数据类型:["xml" / "html" / "script" / "json" / "jsonp"]
        url: url, // 默认当前地址,发送请求的地址
        //data: { key: "value" }, // 发送到服务器的数据
        error: function (xml) { alert('Error loading XML document' + xml); }, // 请求失败时调用
        timeout: 1000, // 设置请求超时时间
        success: function (json) { // 请求成功后回调函数 参数：服务器返回数据,数据格式.                  
            result = json; // == "True";
            if (result == "") {
                resultVal = true;
            } else {
                mic.alert("提示", result);
                resultVal = false;
            }
        }
    });
    return resultVal;
}
//日期差(取天数)
function datediff(begin, end) {
    var strSeparator = "-"; //日期分隔符
    var strDateArrayStart;
    var strDateArrayEnd;
    var intDay;
    strDateArrayStart = begin.split(strSeparator);
    strDateArrayEnd = end.split(strSeparator);
    var strDateS = new Date(strDateArrayStart[0] + "/" + strDateArrayStart[1] + "/" + strDateArrayStart[2]);
    var strDateE = new Date(strDateArrayEnd[0] + "/" + strDateArrayEnd[1] + "/" + strDateArrayEnd[2]);
    intDay = (strDateE - strDateS) / (1000 * 3600 * 24);
    return intDay;
}
//判断日期合法性
function isdate(strDate) {
    var r = /^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2})/;
    return r.test(strDate);
}

//日期差(取秒数)
function seconddiff(begin, end) {
    var intSecond;
    var strDateS = new Date(begin.replace("-","/"));
    var strDateE = new Date(end.replace("-", "/"));
    intSecond = (strDateE - strDateS) / 1000;
    return intSecond;
}

//判断年月的合法性
function isyearmonth(strDate) {
    var r = /^\d{4}-\d{1,2}/;
    if (!r.test(strDate)) {
        return false;
    }
    var s = strDate.split("-");
    if (s[1] > 12) {
        return false;
    }
    return true;
}

function monthdiff(beginMonth, endMonth) {
    var sm = beginMonth.split("-");
    var em = endMonth.split("-");
    return parseInt(em[1] - sm[1]);
}

function issameyear(beginMotn, endMonth) {
    var sm = beginMotn.split("-");
    var em = endMonth.split("-");
    if (sm[0] != em[0]) {
        return false;
    }
    return true;
}

function toDate(str)
{
    var sd = str.split("-");
    
    return new Date(sd[0],sd[1],sd[2]);
}

function dateDXCompare(fristDate, secondDate) 
{
    var d1 = toDate(fristDate);
    var d2 = toDate(secondDate);
    
    return (d1 > d2);
}
