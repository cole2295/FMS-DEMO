/*FileName: finance.search.page.js
**Author:   何名宇
**Date:     2011-08-12
**Usage:    用于定义财务查询的相关页面应用JS
**/

$(function() {
	//声明页面所需的变量
	var buttons = {
		btnSearch: $.dom("btnSearch", "input"),
		btnAllDetails: $.dom("btnAllDetails", "input")
	};
	var datetimes = {
		beginTime: $.dom("txtBeginTime", "input"),
		endTime: $.dom("txtEndTime", "input")
    };
    var droplists = {
    ddlOrderSource: $.dom("ddlOrderSource", "select")
    };
	buttons.btnSearch.click(function() {
		//判断必须选择日期
		if (!search.validate.query({
			begin: datetimes.beginTime.val(),
			end: datetimes.endTime.val()
		})) return false;
		$(document).progressDialog.showDialog("查询中，请稍后...");
	});
	//对导出所有明细做限制
	buttons.btnAllDetails.click(function() {
		//选择订单来源
		var options = droplists.ddlOrderSource.find("option");
		if (!search.validate.selected(options)) {
			jAlert("请选择订单来源！");
			return false;
		}
		//判断必须选择日期
		return search.validate.query({
			begin: datetimes.beginTime.val(),
			end: datetimes.endTime.val(),
			diffdays: 7,
			intevalText: "只能导出7天以内的明细！"
		});
	});
});
