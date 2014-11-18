;(function($) {
	$.fn.extend({
	"trHilight":function(options){
		options=$.extend({
			hoverTr:"hoverTr"
		},options);

		$('tbody tr',this).live('mouseover',function(){
		  $(this).addClass(options.hoverTr);
		});
		$('tbody tr',this).live('mouseout',function(){
		  $(this).removeClass(options.hoverTr);
		});
		
		}
	});
})(jQuery);

