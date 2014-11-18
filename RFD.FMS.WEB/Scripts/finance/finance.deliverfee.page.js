$(function() {
	var buttons = {
		btnQuery: $.dom("btnQuery", "input")
	};
	buttons.btnQuery.click(function() {
		return deliver.validate.query();
	});
	deliver.init.ready({ update: false });
})