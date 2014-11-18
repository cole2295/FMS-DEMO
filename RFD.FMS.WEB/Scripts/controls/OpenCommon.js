//封装选择站点逻辑
var stationCommon = (function () {
    function clearData(stationCommon) {
        $(stationCommon).siblings("span").html("").siblings().val("");
    }
    function fillData(stationCommon, id, name) {
        $(stationCommon).val(name).siblings(":hidden").val(id).siblings("span").html(name);
    }
    function openWindow(url, width, height, index) {
        //打开模式窗口
        window.activedIndex = index == "undefined" ? 0 : index;
        window.showModalDialog(url, window, "dialogWidth=" + width + "px;dialogHeight=" + height + "px;center:yes;resizable:no;scroll:no;help=no;status=no;");
    }
    function open(stationCommon, index, url, width, height) {
        openWindow(url, width, height, index);
    }
    function getStation(stationCommon, index) {
        if (stationCommon.value != "") {
            BasicSettingWebService.GetStationInfo(encodeURIComponent(stationCommon.value), function (Model) {
                var MyStationModel = Model;
                if (MyStationModel == null) {
                    //clearData(station);
                    open(station.value, index);
                } else {
                    fillData(stationCommon, MyStationModel.ExpressCompanyID, MyStationModel.CompanyName);
                }
            });
        } else {
            clearData(stationCommon);
        }
    }
    function openWin(obj, url, width, height) {
        var parent = $('.openStation').has($(obj));
        var index = $('.openStation').index(parent);
        if (obj.tagName == "input") {
            var evt = (evt) ? evt : ((window.event) ? window.event : "");
            var keyCode = evt.keyCode ? evt.keyCode : (evt.which ? evt.which : evt.charCode);
            if (keyCode == 13) {
                getStation(obj, index);
            }
        } else {
            //open(parent.find('.text').val(), index, url, width, height);
            //alert(url)
            openWindow(  url, width, height,index);
        }
    }
    return { open: openWin };
})();