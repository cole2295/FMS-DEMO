// 表格划过变色
function base_MouseOver() {
	$("table#dataTable tbody tr").hover(function() {
		$(this).addClass("hoverTr");
	},
		    function() {
		    	$(this).removeClass("hoverTr");
		    });
}

function openwindow(url, name, iWidth, iHeight) {
	var url;                            //转向网页的地址;
	var name;                           //网页名称，可为空;
	var iWidth;                          //弹出窗口的宽度;
	var iHeight;                        //弹出窗口的高度;
	var iTop = (window.screen.availHeight - 30 - iHeight) / 2;       //获得窗口的垂直位置;
	var iLeft = (window.screen.availWidth - 10 - iWidth) / 2;           //获得窗口的水平位置;
	window.open(url, name, 'height=' + iHeight + ',,innerHeight=' + iHeight + ',width=' + iWidth + ',innerWidth=' + iWidth + ',top=' + iTop + ',left=' + iLeft + ',toolbar=no,menubar=no,scrollbars=yes,resizeable=yes,location=no,status=no');
}


function base_Menu() {
	//选项卡
	var $menuA = $("div.tabMenu ul li a");
	$menuA.click(function() {
		$(this).addClass("selected")           //当前<a>元素高亮
					.parent().siblings().find("a").removeClass("selected");  //去掉其它<a>元素的高亮
		var index = $menuA.index(this);  // 获取当前点击的<a>元素 在 全部a元素中的索引。
		$("div.tabBox > div")  	//选取子节点。不选取子节点的话，会引起错误。如果里面还有div 
					.eq(index).show()   //显示<a>的父元素<li>对应的<div>元素
					.siblings().hide(); //隐藏其它几个同辈的<div>元素
	}).hover(function() {
		$(this).addClass("hover");
	}, function() {
		$(this).removeClass("hover");
	})
}

// 表格拖动排序
function base_tableMove() {
	$("#promotions tbody").tableDnD({ onDragClass: "trDrag" }); //选择tbody,预防鼠标拖动表thead头和tfoot表底在firefox中出错。
	$('body').mouseup(function() {
		var sRowOrder = "";
		$("#promotions tbody tr").each(function(i, o) {
			rowID = $(this).index("#promotions tbody tr");
			$(this).children().eq(1).text(rowID + 1);
			if (sRowOrder.length) {
				sRowOrder += "," + o.id;
			} else {
				sRowOrder = o.id;
			}
		});
	});
}

// 全选、反选、全清
function base_checkBox() {
	//全选

	$("thead tr :checkbox").click(function() {
		$(this).parents("table").find('tbody tr :checkbox').not(":disabled").
				attr("checked", this.checked);
		$(this).parents("table")
				.find("tbody tr")[this.checked ? "addClass" : "removeClass"]('selected'); //全选全高亮
	});

	// 如果复选框默认情况下是选择的，则高色.
	$('tbody>tr:has(:checked)').addClass('selected');


	//表底全选反选不选
	//全选
	$("tfoot td :nth-child(3n+1)").click(function() {
		$(this).parents("table").find(':checkbox').attr("checked", true);
		$(this).parents("table")
				.find("tbody tr").addClass('selected'); //全选全高亮
	});
	//全不选
	$("tfoot td :nth-child(3n)").click(function() {
		$(this).parents("table").find(':checkbox').attr('checked', false);
		$(this).parents("table")
				.find("tbody tr").removeClass('selected');
	});
	//反选
	$("tfoot td :nth-child(3n+2)").click(function() {
		$(this).parents("table").find('tbody tr :checkbox').each(function() {
			this.checked = !this.checked;
			var hasSelected = $(this).parents("tr").hasClass('selected');
			//如果选中，则移出selected类，否则就加上selected类
			$(this).parents("tr")[hasSelected ? "removeClass" : "addClass"]('selected');
		});
		var $tmp = $(this).parents("table").find('tbody tr :checkbox');
		//用filter方法筛选出选中的复选框。并直接给CheckedAll赋值。
		$(this).parents("table").find('thead tr :checkbox')
				.attr('checked', $tmp.length == $tmp.filter(':checked').length);
	});
}

//GridView的行交替色，鼠标指向背景色，选中背景色
function gridViewColor(gridId, options) {
	var grid = $("#" + gridId);
	if (grid) {
		var defaults = {
			header: "#ccc",
			normal: "#fff",
			alter: "#eee",
			hover: "#FFF8D2",
			selected: "#FFCC99",
			count: 0,
			clickable: false
		};
		var r = $.extend({}, defaults, options);
		//控制所有的数据行
		if (r.count > 0) {
			var orginal = r.normal;
			grid.find("tr:has(th)").css("backgroundColor", r.header).end()
                    .find("tr:has(td)").hover(function() {
                    	orginal = $(this).css("backgroundColor");
                    	$(this).css("backgroundColor", r.hover);
                    }, function() {
                    	$(this).css("backgroundColor", orginal);
                    }).end().find("tr:has(td):odd").css("backgroundColor", r.normal)
                      .end().find("tr:has(td):even").css("backgroundColor", r.alter);
			if (r.clickable && r.count > 0) {
				grid.find("tr:has(td)").css("cursor", "hand").click(function() {
					$(this).css("backgroundColor", $(this).css("backgroundColor") == r.selected ? r.hover : r.selected);
					this.selected = !this.selected;
				});
			}
		}
	}
}
//为数组添加contains方法
Array.prototype.contains = function(item) {
	return RegExp("(^|,)" + item.toString() + "($|,)").test(this);
};
/*
uploadFile--上传控件
btnImport--导入按钮
*/
function checkUploadFileFormat(uploadFile, btnImport) {
	$("#" + uploadFile).change(function() {
		//检查必须上传文件
		var file = $.trim($(this).val());
		if (file == "") return false;
		var exts = ".xls|.xlsx";
		var fileExt = file.substr(file.lastIndexOf(".")).toLowerCase();
		if (fileExt == "" || !exts.split('|').contains(fileExt)) {
			$("#" + btnImport).attr("disabled", true);
			alert("请导入" + exts.replace("|", "或") + "格式的文件！\n当前导入的是: " + fileExt + "!");
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
		$("#" + btnImport).attr("disabled", false);
		return true;
	});
}
function render() {
	if ($(".GridView").length > 0 && $(".GridView").tablegrid != undefined)
		$(".GridView").tablegrid({ oddColor: '#F5FAFC', evenColor: '#FFFFFF', overColor: '#FFF8D2', selColor: '#FFCC99', useClick: false });
	$("input[type=text]").hover(function() {
		$(this).addClass("textHover");
	}, function() {
		$(this).removeClass("textHover");
	});
}
function getUrlParams() {
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
//显示商家来源
function showMerchant(settings) {
	settings = $.extend({ wrap: false }, settings); //对下拉框换行处理
	settings.source.change(function() {
		var options = $(this).find("option");
		var index = options.index(options.filter(":selected"));
		var style = settings.wrap ? "inline" : "block";
		settings.merchant.css("display", index == 3 ? style : "none");
	}).trigger("change");
}
//检查数字是否合法
function checkNumber(txtNum, options) {
	var valid = true;
	var defaults = {
		regex: /^(-)?\d+(\.\d+)?$/, //检测正则表达式
		text: "",
		min: 0,
		max: 100,
		limit: true //验证最大值
	};
	var r = $.extend({}, defaults, options);
	var num = $.trim(txtNum.val());
	if (num == "") {
		alert("请输入" + r.text + "！");
		valid = false;
	} else if (!r.regex.test(num)) {
		alert(r.text + "必须为数字！");
		valid = false;
	} else if (parseFloat(num) <= r.min) {
		alert(r.text + "必须大于" + r.min + "！");
		valid = false;
	} else if (r.limit && num > r.max) {
		alert(r.text + "必须小于" + r.max + "!");
		valid = false;
	}
	if (!valid) {
		txtNum.focus().val("");
	}
	return valid;
}
//为排序加上图标
function addSortIcon(options) {
	var defaults = {
		sortHeaders: ["配送站"],
		sortFields: ["CompanyName"],
		sortAscIcon: $("<label id='asc' style='padding-left:2px;' title='升序'>▲</label>"),
		sortDescIcon: $("<label id='desc' style='padding-left:2px;' title='降序'>▼</label>"),
		defaultSortField: "配送站",
		orderby: "",
		direction: "",
		clickOne: $("input[id$=hidClickOne]")
	};
	var r = $.extend(defaults, options);
	var sortcol = $(".GridView th a").filter(":contains('" + r.defaultSortField + "')");
	//默认给grid加上排序图标
	if (r.clickOne.val() != "1") {
		sortcol.append(r.sortDescIcon);
		r.clickOne.val("1");
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