/*FileName: finance.deliverfee.validate.js
**Author:   何名宇
**Date:     2011-09-20
**Usage:    用于定义配送费的页面验证js
**/
//检测结算周期
function checkPeriod(settings) {
	var defaults = {
		radios: $.dom("PaymentPeriod", "radio"),
		period: $.dom("txtPaymentPeriodDay", "input"),
		startDate: $.dom("txtPaymentPeriodDate", "input"),
		max: 30,
		errorText: "货款结算周期",
		errorType: "货款",
		init: false
	};
	var r = $.extend(defaults, settings);
	if (r.init) {
		r.radios.change(function() {
			r.period.add(r.startDate).attr("disabled", r.radios.index($(this)) == 1);
		});
		return;
	}
	if (r.radios.index(r.radios.filter(":checked")) == 0) {
		if (!util.numUtil.checkNumber(r.period, {
			regex: /^\d+$/,
			text: r.errorText,
			max: r.max,
			integer: true,
			limit: true
		})) return false;
		return util.dateUtil.selectDate({ date: r.startDate,
			errorText: "请选择" + r.errorType + "结算开始时间！"
		});
	}
	return true;
}
//检测基本配送费
//function checkBasicDeliverFee() {
//	var radios = $.dom("UniformFee", "radio");
//	var index = radios.index(radios.filter(":checked"));
//	if (index == 0) {
//		var fee = $.dom("txtBasicDeliverFee", "input");
//		return util.numUtil.checkNumber(fee, { text: "基本配送费", limit: false });
//	}
//	return true;
//}
//检测拒收配送费和代收手续费
function checkFeeRate() {
	var refuseRate = $.dom("txtRefuseFee", "input");
	var receiveRate = $.dom("txtReceiveFee", "input");
	var receivePosFee = $.dom("txtReceivePosFee", "input");
	var visitReturnsFee = $.dom("txtVisitReturnsFee", "input");
	var visitReturnsVFee = $.dom("txtVisitReturnsVFee", "input");
	var visitChangeFee = $.dom("txtVisitChangeFee", "input");
	if (!util.numUtil.checkNumber(refuseRate, { text: "拒收妥投配送费", max: 1 })) return false;
	if (!util.numUtil.checkNumber(receiveRate, { text: "代收货款现金手续费", max: 1 })) return false;
	if (!util.numUtil.checkNumber(receivePosFee, { text: "代收货款POS手续费", max: 1 })) return false;
	if (!util.numUtil.checkNumber(visitReturnsFee, { text: "上门退配送费", max: 1 })) return false;
	if (!util.numUtil.checkNumber(visitReturnsVFee, { text: "上门退拒收配送费", max: 1 })) return false;
	if (!util.numUtil.checkNumber(visitChangeFee, { text: "上门换配送费", max: 1 })) return false;
	return true;
}
//检测计费因素
//function checkFactors(settings) {
//	var defaults = { init: false };
//	var r = $.extend(defaults, settings);
//	var factors = util.listUtil.getItems({
//		control: "cblFeeFactors",
//		type: "check"
//	});
//	if (!r.init) {
//		if (!util.listUtil.hasSelected(factors, "check")) {
//			jAlert("请选择计费因素！");
//			return false;
//		}
//		return true;
//	}
//	factors.change(function() {
//	    var isSolidFee = factors.index($(this)) == 0;
//	    var formulas = util.listUtil.getItems({
//	        control: "rblFormula1",
//	        type: "radio",
//	        single: true
//	    });
//	    var DeliverFee = $(".DeliverFee");
//	    if (isSolidFee) {
//	        factors.filter(":gt(0)").attr("checked", false);
//	        //如计费因素为"固定费用"所有计费公式均不可用
//	        formulas = $(".rblFormula1");
//	        formulas.attr("disabled", true);	        
//	        DeliverFee.attr("disabled", false);
//	    } else {
//	        factors.eq(0).attr("checked", false);
//	        formulas = $(".rblFormula1");
//	        formulas.attr("disabled", false);
//	        DeliverFee.attr("disabled", true);
//	    }
//	});
//}
//设置配送费是否一致
//function checkIsUniform() {
//	var radios = util.listUtil.getItems({
//		control: "UniformFee",
//		type: "radio",
//		single: true
//	});
//	radios.change(function() {
//		var $this = radios.filter(":checked");
//		var index = radios.index($this);
//		$this.parents("tr").next().find("div:eq(" + index + ")").show().siblings().hide();
//	}).change();
//}
//检测计费公式
//function checkFormula(options) {
//	var defaults = { init: false };
//	options = $.extend(defaults, options);
//	var formulas = util.listUtil.getItems({
//	control: "rblFormula1",
//		type: "radio",
//		single: true
//	});
//	if (!options.init) {
//		if (formulas[0].disabled == false) {
//			if (!util.listUtil.hasSelected(formulas, "radio")) {
//				jAlert("请选择单件计费公式！");
//				return false;
//			}
//		}
//		return true;
//	}
//	var factors = util.listUtil.getItems({ control: "cblFeeFactors", type: "check" });
//	formulas.attr("disabled", factors.index(factors.filter(":checked")) == 0);
//}