//1.js验证只能输入数字.
function validateInt(value) {
    var flag = true;
    for (var i = 0; i < value.length; i++) {
        code = value.charCodeAt(i); //获取当前字符的unicode编码
        if (code >= 48 && code <= 57) {
        }
        else {
            flag = false;
        }
    }
    return flag;

}
//2.js验证只能输入字母.数字和下划线. 
function check_validateIntAndStr(value) {
    var reg = /^\w+$/;
    if (value.constructor === String) {
        var re = value.match(reg);
        return true;
    }
    return false;

}
//3.js验证固定电话：只能是数字.并且有相应的格式//028-67519441 或者 0839-8777222或者 028-6545124
function validatePhone(value) {
    var reg = /^(\d{3,4})-(\d{7,8})/;
    if (value.constructor === String) {
        var re = value.match(reg);
        return true;
    }
    return false;
}
//验证输入数量只能为10位以内的整数
function checkInQuantity(value) {
    var reg = /^[+]?\d{1,10}$/;
    if (reg.test(value)) {
        return true;
    }
    else {
        return false;
    }
}

function validateDEmail(value) {

    //    var reg = /^([#\*\w-]+(#\*\.[\w-]+)*@[\w-]+(\.[\w-]+)+)$/

    var reg = /^([#\*\w-]+(#\*\.[\w-]+)*@[\w-]+(\.[\w-]+)+)(,[#\*\w-]+(#\*\.[\w-]+)*@[\w-]+(\.[\w-]+)+)*$/
    if (reg.test(value) || (value == "")) {
        return true;
    }
    else {
        return false;
    }
}

function validateDPhone(value) {
    var reg = /^\d{11}(,\d{11})*$/;
    if (reg.test(value) || (value == "")) {
        return true;
    }
    else {
        return false;
    }
}

// 验证0-100的数值
function TextValidate(source, arguments) {

    var regNum = new RegExp("^[0-9]{0,3}$");

    if (!(regNum.test(document.getElementById("ctl00_ContentPlaceHolder1_txt_saveStock").value)
    && (parseInt(document.getElementById("ctl00_ContentPlaceHolder1_txt_saveStock").value, 10) < 100)
    && (parseInt(document.getElementById("ctl00_ContentPlaceHolder1_txt_saveStock").value, 10) > 0))) {
        arguments.IsValid = false;
        document.getElementById("ctl00_ContentPlaceHolder1_txt_saveStock").focus();
        return false;
    }

    arguments.IsValid = true;
}