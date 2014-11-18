/*FileName: finance.deliverfee.js
**Author:   何名宇
**Date:     2011-09-20
**Usage:    用于定义配送费的相关页面公用js
**/
if (typeof window.Finance == "undefined") window.Finance = {};
if (typeof window.Finance.DeliverFee == "undefined") window.Finance.DeliverFee = {};

var deliver = Finance.DeliverFee.prototype;
deliver = {
	init: (function() {
		function onReadyDocument(settings) {
			util.pageUtil.render(settings);
			util.fileUtil.importFile(settings);
			//修改配送费页面将执行
			if (settings.update) {
//				$.doms(["save", "audit"]).click(function() {
//					//解决多次重复提交的问题
//					var result = deliver.validate.audit();
//					if (result) {
//						$(this).attr("disabled", result);
//						if (this.id == "save") {
//							$(document).progressDialog.showDialog("保存中，请稍后...");
//							$.dom("btnSave", "input").click();
//						} else if (this.id == "audit") {
//							$(document).progressDialog.showDialog("提交审核中，请稍后...");
//							$.dom("btnSubmitAudit", "input").click();
//						}
//					}
//				});
				checkPeriod({ init: true });
				checkPeriod({
					radios: $.dom("DeliverFeePeriod", "radio"),
					period: $.dom("txtDeliverFeePeriodDay", "input"),
					startDate: $.dom("txtDeliverFeePeriodDate", "input"),
					init: true
				});
				//checkFactors({ init: true });
				//checkIsUniform();
				//checkFormula({ init: true });
//				var status = $.dom("hidStatus", "input").val();
//				if (status == "待审核") {
//					//如为待审核状态，则所有项都不能被编辑
//					$.dom("container").find("input").attr("disabled", true)
//					 .filter(".show").attr("disabled", false).parent()
//					 .find(":input:not('.show)'").hide();
//				} else {
//					//维护状态仅"待维护"状态可用
//					$.dom("rblMaintainStatus", "radio").find("input").attr("disabled", true);
//				}
			}
			//基本配送费页面执行
			if (settings.basic) {
				var params = util.urlUtil.getQueryParams();
				//只有"配送费维护"模块中不是"待审核"状态的才可保存基本配送费
				if (params["op"] == "m" && params["status"] != "1") {
					$("input.show").show().attr("disabled", $.dom("hidTotalCount", "input").val() == "0");
					$(".GridView .text").show().next().hide();
				} else {
					$("input.show").hide();
					$(".GridView .text").hide().next().show();
				}
				$.dom("btnQuery", "input").click(function() {
					var beginTime = $.dom("txtBeginTime", "input").val();
					var endTime = $.dom("txtEndTime", "input").val();
					return util.dateUtil.checkDate(beginTime, endTime, {
						isCheckEmpty: false,
						isCheckInteval: false
					});
				});
				$.dom("btnSave", "input").click(function() {
					var regex = /^\d+(\.\d+)?$/;
					var flag = true;
					$(".GridView .text").each(function(i) {
						if (!regex.test($(this).val())) {
							jAlert("第" + (i + 1) + "行基本配送费格式错误!");
							flag = false;
							return false;
						}
					});
					return flag;
				});
			}
		}
		return { ready: onReadyDocument };
	})(),
	validate: {
		query: function(settings) {
			var defaults = {
				statusList: $.dom("cblMaintainStatus", "check"),
				merchant: $.dom("txtMerchantID", "input")
			};
			var r = $.extend({}, defaults, settings);
			var id = $.trim(r.merchant.val());
			if (!util.numUtil.checkNumber(r.merchant, {
				regex: /^\d+$/,
				text: "序号",
				limit: false,
				integer: true
			})) return false;
			var isSelected = util.listUtil.hasSelected(r.statusList, "check");
			if (!isSelected) {
				jAlert("请至少选择一种维护状态再查询！");
				r.statusList.focus();
				return false;
			}
			return true;
		},
		audit: function() {
			//检测货款结算周期
			if (!checkPeriod()) return false;
			//检测配送费结算周期
			if (!checkPeriod({
				radios: $.dom("DeliverFeePeriod", "radio"),
				period: $.dom("txtDeliverFeePeriodDay", "input"),
				startDate: $.dom("txtDeliverFeePeriodDate", "input"),
				errorText: "配送费结算周期",
				errorType: "配送费"
			})) return false;
			//检测计费因素
			//if (!checkFactors()) return false;
			//检测基本配送费
			//if (!checkBasicDeliverFee()) return false;
			//检查计费公式
			//if (!checkFormula()) return false;//wangyongc 2011-10-27
			//检测拒收配送费和代收手续费
			if (!checkFeeRate()) return false;
			return true;
		}
	}
};

