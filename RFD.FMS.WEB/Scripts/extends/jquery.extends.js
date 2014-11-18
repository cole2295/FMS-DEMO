//根据元素标签获取包含容器内(模板页，自定义控件)的元素
if (typeof jQuery.dom == "undefined") jQuery.dom = function (id, tag) {
    if (tag == undefined) {//tag可为元素标签，也可为选择表达式
        return $("#" + id);
    }
    if (tag.toLowerCase() == "drop") {
        return $("select[name$=" + id + "]");
    }
    if (tag.toLowerCase() == "radio") {
        return $("input[name$=" + id + "]");
    }
    if (tag.toLowerCase() == "check") {
        return $("table[id$=" + id + "]");
    }
    if ($(tag + "[id$=" + id + "]").length == 0) {
        return $(tag + "[name$=" + id + "]");
    }
    return $(tag + "[id$=" + id + "]");
}
//获取同一类标签的元素集合
if (typeof jQuery.doms == "undefined") jQuery.doms = function(ids, tag) {
	var arr = ids; //ids可为数组或以逗号分隔的ID列表
	if (!(ids instanceof Array)) {
		arr = ids.split(',');
	}
	var elems = $.dom(arr[0], tag);
	for (var i = 1; i < arr.length; i++) {
		elems = elems.add($.dom(arr[i], tag));
	}
	return elems;
}
