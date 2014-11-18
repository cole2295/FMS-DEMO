/*FileName: finance.search.js
**Author:   何名宇
**Date:     2011-08-12
**Usage:    用于定义财务查询的相关页面公用js
**/
if (typeof window.Finance == "undefined") window.Finance = {};
if (typeof window.Finance.Search == "undefined") window.Finance.Search = {};

var search = Finance.Search.prototype;
search = {
	init: (function() {
		//根据查询结果显示按钮是否可用
		function enableButtons(settings) {
			var defaults = {
				totalButtons: [],
				detailsButtons: [],
				totalCount: 0,
				detailsCount: 0
			};
			var r = $.extend({}, defaults, settings);
			disabledButtons({ buttons: r.totalButtons, disabled: r.totalCount == 0 });
			disabledButtons({ buttons: r.detailsButtons, disabled: r.totalCount == 0 || r.detailsCount == 0 });
		}
		function disabledButtons(settings) {
			var defaults = {
				buttons: [],
				disabled: false
			};
			var r = $.extend({}, defaults, settings);
			for (var i = 0; i < r.buttons.length; i++) {
				r.buttons[i].attr("disabled", r.disabled);
			}
		}
		function onReadyDocument(settings) {
			enableButtons(settings);
			util.dateUtil.rebindDate({ type: 'S' });
			util.pageUtil.render(settings);
			util.fileUtil.importFile(settings);
			util.listUtil.setListItemPadding(settings);
		}
		return { ready: onReadyDocument, disable: disabledButtons };
	})(),
	/*显示商家来源
	**source: 订单来源下拉框
	**merchant: 商家来源下拉框父元素
	**wrap: 是否处理元素换行
	**/
	showMerchant: function(settings) {
		settings = $.extend({ wrap: false }, settings);
		settings.source.change(function() {
			var options = $(this).find("option");
			var index = options.index(options.filter(":selected"));
			var style = settings.wrap ? "inline" : "block"; //对下拉框换行处理
			if (index == 3) {
				settings.merchant.css("display", style);
			} else {
				settings.merchant.hide().find("select")[0].selectedIndex = 0;
			}
		}).trigger("change");
	},
	//为页面验证
	validate: {
		//查询验证
		query: function(settings) {
			return util.dateUtil.checkDate(settings.begin, settings.end, settings);
		},
		selected: function(listControl) {
			return listControl.index(listControl.filter(":selected")) != 0;
		},
		//判断必须选择站点
		isSelectStation: function(settings) {
			var defaults = {
				stationID: $.dom("TxtUserControlSatationSpell", "input").val(),
				stationName: $.dom("HidUserControlStationID", "input").val(),
				errorText: "请选择重新生成的站点！"
			};
			var r = $.extend({}, defaults, settings);
			if ($.trim(r.stationID) == "" || $.trim(r.stationName) == "station") {
				jAlert(r.errorText);
				$.dom("TxtUserControlSatationSpell", "input").focus();
				return false;
			}
			return true;
		}
	},
	//为排序加上图标
	addSortIcon: function(options) {
		var defaults = {
			sortHeaders: ["配送站"],
			sortFields: ["CompanyName"],
			sortAscIcon: $("<label id='asc' style='padding-left:2px;cursor:hand;' title='升序'>▲</label>"),
			sortDescIcon: $("<label id='desc' style='padding-left:2px;cursor:hand;' title='降序'>▼</label>"),
			defaultSortField: "配送站",
			defaultSortDirection: "asc",
			orderby: "",
			direction: "",
			clickOne: "hidClickOne"
		};
		var r = $.extend(defaults, options);
		var sortcol = $(".GridView th a").filter(":contains('" + r.defaultSortField + "')");
		//默认给grid加上排序图标
		if ($.dom(r.clickOne, "input").val() != "1") {
			sortcol.append(r.defaultSortDirection.toLowerCase() == "desc" ? r.sortDescIcon : r.sortAscIcon);
			$.dom(r.clickOne, "input").val("1");
			return;
		}
		var header = r.sortHeaders[Array.indexOf(r.sortFields, r.orderby)];
		var th = $(".GridView th a").filter(":contains('" + header + "')");
		if (r.direction.toLowerCase() == "desc") {
			th.append(r.sortDescIcon);
		} else {
			th.append(r.sortAscIcon);
		}
	}
};
