function include() {
	//DEBUG版
	//	for (var i = 0; i < arguments.length; i++) {
	//		var file = arguments[i];
	//		if (file.match(/\.js$/i))
	//			document.write('<script type=\"text/javascript\" src=\"' + file + "\?r=" + Math.random() + '\"></sc' + 'ript>');
	//		else
	//			document.write('<style type=\"text/css\">@import \"' + file + "\?r=" + Math.random() + '\" ;</style>');
	//	}
	//RELEASE版
	for (var i = 0; i < arguments.length; i++) {
		var file = arguments[i];
		if (file.match(/\.js$/i))
			document.write('<script type=\"text/javascript\" src=\"' + file + '\"></sc' + 'ript>');
		else
			document.write('<style type=\"text/css\">@import \"' + file + '\" ;</style>');
	}
};