//为数组添加contains方法
if (!Array.prototype.contains) Array.prototype.contains = function(item) {
	return RegExp("(^|,)" + item.toString() + "($|,)").test(this);
}
//参数item:必选项，要查找的Array对象中的一子项
//参数i:可选项。该整数值指出在 Array对象内开始查找的索引。如果省略，则从字符串的开始处查找。
if (!Array.prototype.indexOf) Array.prototype.indexOf = function(item, i) {
	//因为"||"或运算符是短路运算，就是当左边为true时不会去执行右边，直接就返回true，只有当左边为false时才会执行，也就是说除非以下情况才会执行i=0;
	//1.没有给indexOf传递参数i，比如array.indexOf(item);		
	//2.i = 0、空字符串（""）、undefined、null、NaN、false
	var ta, rt, d = '\0';
	ta = !i ? this : this.slice(i);
	rt = !i ? 0 : this;
	var str = d + ta.join(d) + d, t = str.indexOf(d + substr + d);
	if (t == -1) return -1;
	rt += str.slice(0, t).replace(/[^\0]/g, '').length;
	return rt;
};
if (!Array.prototype.lastIndexOf) Array.prototype.lastIndexOf = function(item, i) {
	var ta, rt, d = '\0';
	ta = !i ? this : this.slice(i);
	rt = !i ? 0 : this;
	ta = ta.reverse();
	var str = d + ta.join(d) + d, t = str.indexOf(d + substr + d);
	if (t == -1) return -1;
	rt += str.slice(t).replace(/[^\0]/g, '').length - 2;
	return rt;
}
if (!Array.prototype.replace) Array.prototype.replace = function(newVal, oldVal) {
	var ta = this.slice(0), d = '\0';
	var str = ta.join(d); str = str.replace(newVal, oldVal);
	return str.split(d);
}
if (!Array.prototype.search) Array.prototype.search = function(item) {
	var ta = this.slice(0), d = '\0';
	var str = d + ta.join(d) + d;
	var regstr = item.toString();
	item = new RegExp(regstr.replace(/\/((.|\n)+)\/.*/g, '\\0$1\\0'), regstr.slice(regstr.lastIndexOf('/') + 1));
	t = str.search(item);
	if (t == -1) return -1;
	return str.slice(0, t).replace(/[^\0]/g, '').length;
}
if (!Array.prototype.clear) Array.prototype.clear = function() {
	this.length = 0;
}
if (!Array.prototype.insertAt) Array.prototype.insertAt = function(index, obj) {
	this.splice(index, 0, obj);
}
if (!Array.prototype.removeAt) Array.prototype.removeAt = function(index) {
	this.splice(index, 1);
}
if (!Array.prototype.remove) Array.prototype.remove = function(obj) {
	var index = this.indexOf(obj);
	if (index >= 0) {
		this.removeAt(index);
	}
}