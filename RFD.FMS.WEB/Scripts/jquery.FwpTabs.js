// JavaScript Document
(function($){
	$.fn.extend({
		//Tab开始
		FwpTabs:function(options){
			op=$.extend({
				},options||{});
			var self=this;
			$(this).children('.tabCon').hide();
			$(this).children('ul').find('li').bind('click',function(){
				$(this).addClass('selected').siblings().removeClass('selected');
				on=$(this).attr('title');
				$(self).children('\"#'+on+'\"').show().siblings('.tabCon').hide();
				})
			.each(function(i){
				if($(this).hasClass('selected')) {on=$(this).attr('title');$(self).children('\"#'+on+'\"').show().siblings('.tabCon').hide();}
				})
			.bind('mouseout',function(){$(this).removeClass('hover');})
			.bind('mouseover',function(){$(this).addClass('hover');});
			}//Tab结束
		})
})(jQuery)