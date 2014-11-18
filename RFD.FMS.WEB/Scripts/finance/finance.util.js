/*FileName: finance.util.js
**Author:   何名宇
**Date:     2011-08-15
**Usage:    用于定义FMS的工具JS
**/
if (typeof window.Finance == "undefined") window.Finance = {};
if (typeof window.Finance.Util == "undefined") window.Finance.Util = {};

var util = Finance.Util.prototype;
util = {
	dateUtil: (function() {
		function check(beginTime, endTime, options) {
			var defaults = {
				begin: $.trim(beginTime),
				end: $.trim(endTime),
				beginText: "请选择开始时间或格式不合法！",
				endText: "请选择结束时间或格式不合法！",
				diffdays: 30,
				compareText: "结束时间必须大于开始时间！",
				intevalText: "查询时间不能超过一个月！",
				isCompared: true, //是否比较大小
				isCheckAllEmpty: false, //是否检查两者均为空
				isCheckEmpty: true, //是否检查空值
				isCheckInteval: true, //是否检查间隔
				emptyText: "请选择查询时间！"
			};
			var r = $.extend({}, defaults, options);
			//首先处理单个日期
			if (!endTime) {
				var isValidDate = r.begin != "" && isdate(r.begin);
				if (!isValidDate) {
					jAlert(r.emptyText);
					return false;
				}
				return true;
			}
			if (r.isCheckEmpty) {
				var isValidBeginTime = r.begin != "" && isdate(r.begin);
				var isValidEndTime = r.end != "" && isdate(r.end);
				if (r.isCheckAllEmpty) {
					if (!isValidBeginTime && !isValidEndTime) {
						jAlert(r.emptyText);
						return false;
					}
				} else {
					//判断必须选择时间段
					if (!isValidBeginTime) {
						jAlert(r.beginText);
						return false;
					}
					if (!isValidEndTime) {
						jAlert(r.endText);
						return false;
					}
					var days = datediff(r.begin, r.end);
					if (r.isCompared && days < 0) {
						jAlert(r.compareText);
						return false;
					}
					if (r.isCheckInteval && days > r.diffdays) {
						jAlert(r.intevalText);
						return false;
					}
				}
			}
			return true;
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
		//重新绑定时间焦点事件
		function rebind(options) {
			var r = $.extend({
				date: '', //传入要绑定的日期控件,不传则绑定所有日期控件
				dateCss: '.Wdate',
				type: 'L', //选择长日期(L)或短日期(S)或月份(M)
				skin: 'whyGreen',
				format: 'yyyy-MM-dd HH:mm:ss'
			}, options);
			switch (r.type.toUpperCase()) {
				case 'S':
					r.dateCss = '.WSdate';
					r.format = 'yyyy-MM-dd';
					break;
				case 'M':
					r.dateCss = '.WMdate';
					r.format = 'yyyy-MM';
					break;
			}
			var wdate = (r.date == '') ? $(r.dateCss) : $(r.dateCss).filter(r.date);
			wdate.bind("focus", function() {
				WdatePicker({ skin: r.skin, dateFmt: r.format });
			});
		}
		//判断选择单个日期
		function select(options) {
			var r = $.extend({ date: '', errorText: '请选择报表日期！' }, options);
			var date = typeof (r.date) == "string" ? $.dom(r.date, "input") : r.date;
			date.unbind("focus");
			rebind({ date: r.date, type: 'S' });
			return check(date.val(), null, {
				beginText: r.errorText
			});
		}
		return { checkDate: check, rebindDate: rebind, selectDate: select };
	})(),
	numUtil: (function() {
		//检查数字是否合法
		function check(txtNum, options) {
			var valid = true;
			var defaults = {
				regex: /^(-)?\d+(\.\d+)?$/, //检测正则表达式
				text: "",
				min: 0,
				max: 100,
				limit: true, //验证最大值
				integer: false //验证整数
			};
			var r = $.extend({}, defaults, options);
			var num = $.trim(txtNum.val());
			if (num == "") {
				jAlert("请输入" + r.text + "!");
				valid = false;
			} else if (!r.regex.test(num)) {
				jAlert(r.text + (r.integer ? "必须为正整数!" : "必须为数字!"));
				valid = false;
			} else if (parseFloat(num) <= r.min) {
				jAlert(r.text + "必须大于" + r.min + "!");
				valid = false;
			} else if (r.limit && num > r.max) {
				jAlert(r.text + "必须小于" + r.max + "!");
				valid = false;
			}
			if (!valid) {
				txtNum.focus().val("");
			}
			return valid;
		}
		return { checkNumber: check };
	})(),
	fileUtil: (function() {
		/*
		uploadFile--上传控件
		importBtn--导入按钮
		*/
		function check(settings) {
			var defaults = {
				file: "txtFile",
				imports: "import",
				trigger: "btnImport",
				exts: ".xls|.xlsx"
			};
			var r = $.extend({}, defaults, settings);
			var result = true;
			$.dom(r.file, "input").change(function() {
				//检查必须上传文件
				var file = $.trim($(this).val());
				if (file == "") {
					$.dom(r.imports).attr("disabled", true);
					return false;
				}
				var fileExt = file.substr(file.lastIndexOf(".")).toLowerCase();
				if (fileExt == "" || !r.exts.split('|').contains(fileExt)) {
					$.dom(r.imports).attr("disabled", true);
					jAlert("请导入" + r.exts.replace("|", "或") + "格式的文件！\n当前导入的是: " + fileExt + "!");
					//清空file
					if (!!document.all) {
						$(this).select();
						document.execCommand("delete");
					} else {
						$(this).val("");
					}
					return false;
				}
				//检测大小...
				$.dom(r.imports).attr("disabled", false);
				result = true;
			});
			if (result) {
				$.dom(r.imports).click(function() {
					$(this).attr("disabled", "disabled");
					$.dom(r.trigger, "input").click();
					$(document).progressDialog.showDialog("导入中，请稍后...");
				});
			}
		}
		return { importFile: check };
	})(),
	listUtil: (function() {
		//设置列表控件(如:checkboxlist,radiobuttonlist)项间距
		function padding(options) {
			var defaults = {
				controls: ["rblExportFormat"],
				type: "radio", //可为check,radio
				padding: "10"
			};
			var r = $.extend({}, defaults, options);
			for (var i = 0; i < r.controls.length; i++) {
				$.dom(r.controls[i], r.type).filter("input:not(:first)").css("padding-left", r.padding + "px");
			}
		}
		//获取列表控件的子项
		function items(options) {
			var defaults = {
				control: "", //列表控件ID或者控件本身
				type: "radio", //可为check,radio,drop,
				single: false//对单一radiobutton,checkbox处理
			};
			var r = $.extend({}, defaults, options);
			//如果传入ID，则转化为对应的控件，否则直接使用该对象
			var parents = typeof (r.control) == "string" ? $.dom(r.control, r.type) : r.control;
			if (r.single) return parents;
			if (r.type == "drop") {
				return parents.find("option");
			} else {
				return parents.find("input");
			}
		}
		//获取列表选中项的索引
		function index(c, t) {
			var parents = items({ control: c, type: t });
			if (t == "drop") {
				return parents.index(parents.filter(":selected"));
			} else {
				return parents.index(parents.filter(":checked"));
			}
		}
		//判断列表控件是否选中
		function selectedItem(c, t) {
			if (t.type == "drop") {
				return c.filter(":selected").length > 0;
			} else {
				return c.filter(":checked").length > 0;
			}
		}
		return { setListItemPadding: padding,
			hasSelected: selectedItem,
			getItems: items,
			getSelectedIndex: index
		};
	})(), //页面渲染
	pageUtil: (function() {
		function gridRender(settings) {
			var defaults = {
				renderTo: "", //当前需要点击的grid
				tooltip: "点击当前行查看该站点的明细数据",
				dataCount: 0,
				oddRowCss: "oddColor",
				evenRowCss: "evenColor",
				selectedRowCss: "selectedColor",
				hoverRowCss: "hoverColor",
				superable: false
			};
			var r = $.extend({}, defaults, settings);
			if ($(".GridView").length > 0 && $(".GridView").tablegrid != "undefined") {
				var trs = $(".GridView").find("tr:has(td)");
				trs.filter(":odd").find("td").addClass(r.oddRowCss);
				trs.filter(":even").find("td").addClass(r.evenRowCss);
				trs.hover(function() {
					$(this).find("td").addClass(r.hoverRowCss);
				}, function() {
					$(this).find("td").removeClass(r.hoverRowCss).removeClass(r.selectedRowCss);
				}).click(function() {
					$(this).find("td").addClass(r.selectedRowCss);
				});
				$(".GridView").each(function() {
					var grid = $.dom(r.renderTo, ".GridView")[0];
					if (grid && $(this)[0].id == grid.id && r.totalCount > 0) {
						$(this).find("tr td").css("cursor", "pointer").attr("title", r.tooltip);
						showDetails(settings);
					}
				});
				if (r.superable) {
					$(".GridView").toSuperTable({
						width: ($(document).width() - 100) + "px",
						height: "300px",
						fixedCols: 1,
						count: $.dom("hidTotalCount", "input").val(),
						onfinish: function() {
							//var height = grid.find("tr").height();
							//$.dom("content").css("height", this.count == 0 ? 2 * height : "auto");
							//$.dom("totalinfo").css("display", this.count == 0 ? "none" : "block");
						}
					});
				}
			};
			//对textbox加渲染效果
			$("input[type=text]").hover(function() {
				$(this).addClass("textHover");
			}, function() {
				$(this).removeClass("textHover");
			}).keypress(function(e) {
				if (e.keyCode == 13) return false;
			});
		}
		//单击某一行显示明细数据
		function showDetails(options) {
			var defaults = {
				totalGrid: "gvSummary", //汇总GridView
				selected: "tr td", //要点击筛选行
				params: "hidQueryParams", //明细查询参数
				detailsParams: "hidDetailsParams", //明细参数
				btnSearchDetails: "btnSearchDetails"//明细查询按钮
			};
			var r = $.extend({}, defaults, options);
			$.dom(r.totalGrid, ".GridView").find(r.selected).click(function() {
				var inputs = $(this).parent().find("input");
				var params = inputs.filter($.dom(r.params, "input")).val();
				//				for (param in params) {
				//					if ($.trim(param) == "") {
				//						alert("当前明细数据不存在！");
				//						return false;
				//					}
				//				}
				$.dom(r.detailsParams, "input").val(params);
				$(document).progressDialog.showDialog("查询中，请稍后...");
				$.dom(r.btnSearchDetails, "input").click();
			});
		}
		return { render: gridRender };
	})(),
	//URL处理
	urlUtil: (function() {
		//获取url的查询参数列表
		function params() {
			var args = new Object();
			var query = location.search.substring(1); //获取查询串   
			var pairs = query.split("&"); //在逗号处断开   
			for (var i = 0; i < pairs.length; i++) {
				var pos = pairs[i].indexOf('='); //查找name=val   
				if (pos == -1) continue; //如果没有找到就跳过   
				var argname = pairs[i].substring(0, pos); //提取name   
				var val = pairs[i].substring(pos + 1); //提取val   
				args[argname] = unescape(val); //存为属性(解码) 
			}
			return args;
		}
		return { getQueryParams: params };
	})()
};
