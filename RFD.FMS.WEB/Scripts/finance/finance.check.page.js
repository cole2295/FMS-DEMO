/*FileName: finance.check.page.js
**Author:   何名宇
**Date:     2011-09-12
**Usage:    用于定义订单核对的相关页面应用JS
**/

//初始化操作
function init() {
	var buttons = [
				$.dom("btnCheck", "input"),
				$.dom("btnSuccessOrder", "input"),
				$.dom("btnDelayOrder", "input")
			];
	var hiddens = [
				$.dom("hidTotalCount", "input"),
				$.dom("hidSuccessCount", "input"),
				$.dom("hidDelayCount", "input")
			];
	for (var i = 0; i < buttons.length; i++) {
		search.init.disable({
			buttons: [buttons[i]],
			disabled: i == 0 ? hiddens[i].val() == 0 :
					           hiddens[0].val() == 0 || hiddens[i].val() == 0
		});
	}
	search.init.ready();
}