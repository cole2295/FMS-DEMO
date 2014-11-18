//封装选择站点逻辑
var station = (function() {
	function clearData(station) {
		$(station).siblings("span").html("").siblings().val("");
	}
	function fillData(station, id, name) {
		$(station).val(name).siblings(":hidden").val(id).siblings("span").html(name);
	}
	function openWindow(url, width, height, index) {
		//打开模式窗口
		window.activedIndex = index == "undefined" ? 0 : index;
		window.showModalDialog(url, window, "dialogWidth=" + width + "px;dialogHeight=" + height + "px;center:yes;resizable:no;scroll:no;help=no;status=no;");
	}
	function open(station, index) {
		openWindow("../UserControl/ExpressListPop.aspx?ID=" + encodeURIComponent(station), 600, 450, index);
	}
	function getStation(station, index) {
		if (station.value != "") {
			BasicSettingWebService.GetStationInfo(encodeURIComponent(station.value), function(Model) {
				var MyStationModel = Model;
				if (MyStationModel == null) {
					//clearData(station);
					open(station.value, index);
				} else {
					fillData(station, MyStationModel.ExpressCompanyID, MyStationModel.CompanyName);
				}
			});
		} else {
			clearData(station);
		}
	}
	function selectStation(obj) {
		var parent = $('.selectStation').has($(obj));
		var index = $('.selectStation').index(parent);
		if (obj.tagName == "input") {
			var evt = (evt) ? evt : ((window.event) ? window.event : "");
			var keyCode = evt.keyCode ? evt.keyCode : (evt.which ? evt.which : evt.charCode);
			if (keyCode == 13) {
				getStation(obj, index);
			}
		} else {
			open(parent.find('.text').val(), index);
		}
	}
	return { select: selectStation };
})();