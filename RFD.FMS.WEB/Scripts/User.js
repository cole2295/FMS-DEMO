

var UserCreate = {
    moveRight: function () {
        var s1 = $("#select1 option:selected");
        $.each(s1, function (i, n) {
            if (!UserCreate.s2Has(n)) {
                $("#select2").append($(n).clone());
            }
        });
    },
    s2Has: function (n1) {
        var lst = $("#select2 option");
        var f = false;
        $.each(lst, function (i, n) {
            var a = $(n1).html();
            var b = $(n).html();
            if (a == b) {
                f = true;
            }
        });
        return f;
    },
    allmoveRight: function () {
        var a = $("#select1 option");
        $.each(a, function (i, n) {
            if (!UserCreate.s2Has(n)) {
                $("#select2").append($(n).clone());
            }
        });
    },
    allmoveLeft: function () {
        $("#select2 option").remove();
    },
    moveLeft: function () {
        $("#select2 option").remove(":selected");
    },
    setData: function () {
        var data = "";         
        if ($("#select2 option").length == 0) {
            $('#subbutton').attr("disabled", "disabled")
            mic.alert("提示", "必须选择至少一个角色");
            $(".jqmClose").click(function () { $('#subbutton').removeAttr("disabled"); });
            return false;
        }
        else {
            var d = $("#select2 option").each(
            function () {
                data = data + "," + $(this).attr("value");
            });
            $("#roleData").val(data);
            return true;
        }
    },
    loadRole: function () {
        var data = $("#roleData").val();

        if (data == "[]" || data == "") { return }
        try {
            var roles = eval("(" + data + ")");

            $(roles).each(function (i, d) {
                $("#select2").append("<option value=" + d.RoleId + ">" + d.RoleName + "</option>");
            })
        } catch (e) {

        }
    }

}

function checkCharset(value, element) {
    var flag = true;
    if (/[^\x00-\xff]/g.test(value)) {

        flag = false;
    }
    return this.optional(element) || flag;
}
function DBC2SBC(value, element) {
    var flag = true;
    for (var i = 0; i < value.length; i++) {
        code = value.charCodeAt(i); //获取当前字符的unicode编码
        if (code >= 65281 && code <= 65373)//在这个unicode编码范围中的是所有的英文字母已经各种字符
        {
            flag = false;
        }
        else if (code == 12288)//空格
        {
            flag = false;
        }
    }
    if (value.search(/([\(\)\[\]\{\}\^\$\+\,\*\?\#\%\!\'\&\'\:\;\<\>！\、\‘\”\：\；\？\=\~\`\￥\……\ \（\）\【\】\——\\\|\/])/g, '') >= 0) {
        flag = false;
    }
    return this.optional(element) || flag;
}

function showResponse(data) {
    var message;
    //    if (1 == data.msg) 
    $('#newdialog').jqmHide();
    mic.alert("提示", data.msgStr);
    $(".jqmClose").click(function () { location.href = location.href; });

}



var User = {
    imgPath: mic.global.imgpath,
    useridval: "",
    CkTxt: function (sender) {

        sender.value = sender.value.replace(/([\(\)\[\]\{\}\^\$\+\,\*\?\#\%\!\'\&\'\:\;\<\>！\、\‘\”\：\；\？\=\~\`\￥\……\（\）\【\】\——\\\|\/])/g, '');
    },
    ChangeState: function (sender, userName) {
        $.ajax({ type: "GET",
            cache: false,
            url: "/User/ChangeEnable",
            data: "userName=" + userName,
            success: function (msg) { User.ChangeResult(sender, msg); },
            Error: function (msg) { }
        });

    },
    ChangeResult: function (sender, msg) {
        var imgurl = msg.newState != 1 ? "start.gif" : "stop.gif";
        imgurl = User.imgPath + imgurl;
        $(sender).attr("src", imgurl);
        $(sender).parent().prev().html(msg.stateStr);
        if (msg.newState == 1) {
            $(sender).parent().prev().addClass("correct").removeClass("error");
        } else {

            $(sender).parent().prev().addClass("error").removeClass("correct");
        }
    },
    resetPassword: function (val) {
        User.useridval = val;
        mic.confirm("重置密码提示", "你确认重置此员工密码么？", User.resetPasswordadd);
    },
    resetPasswordadd: function () {
        $.ajax({ type: "GET",
            cache: false,
            url: "/User/ResetPassword",
            data: "userName=" + User.useridval,
            success: function (msg) { mic.alert("密码重置", msg.result); },
            Error: function (msg) { }
        });
    },
    calendar: function () {
        $("#dateset").calendar([' ']);
        $("#dateend").calendar([' ']);
    }
}

function EditUser() {
    $('#ex2').jqm({ ajax: "/User/EditUser", trigger: 'a.ex2trigger' });
}
function checkDate() {

    var ed = $("#dateend").val();
    var bd = $("#dateset").val();
    if ((bd == "") || (ed == "")) return true;
    var b = new Date(Date.parse(bd.replace(/-/g, "/")));
    var e = new Date(Date.parse(ed.replace(/-/g, "/")));
    if (e < b) {
        mic.alert("提示", "搜索条件中的结束时间不能小于开始时间");
        return false;
    } else {
        return true;
    }
}
