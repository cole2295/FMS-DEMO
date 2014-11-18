include("../Scripts/plugins/tab/jquery-tab.css");
include("../Scripts/lib/jquery-1.6.1.min.js");
include("../Scripts/plugins/tab/jquery.tab.js");
function init() {
	window['MyTabContainter'] = new TabView({
		containerId: 'tab_menu',
		pageid: 'page',
		cid: 'tab_po',
		position: "top"
	});
	window['MyTabContainter'].add({
		id: 'id_1000',
		title: "办公桌面",
		url: "Welcome.aspx",
		isClosed: false
	});
	if (!window['SetPage']) {
	    window['SetPage'] = function(id, title, url, isclosed) {
	        window['MyTabContainter'].add({
	            id: id,
	            title: title,
	            url: url,
	            isClosed: isclosed
	        });
	    };
	}
}