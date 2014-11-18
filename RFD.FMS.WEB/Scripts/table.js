;(function($) {
	$.fn.extend({
		"tableCheck":function(options){
		//设置默认值
		options=$.extend({
			selected:"selected"
			
		},options);

		//全选

		 $("thead tr :checkbox",this).click(function(){
			$(this).parents("table").find('tbody tr :checkbox').
				attr("checked", this.checked );
			$(this).parents("table")
				.find("tbody tr")[this.checked?"addClass":"removeClass"](options.selected);//全选全高亮
		 });
		 //单选
		 $('tbody tr :checkbox',this).click(function(){
			 
			 var hasSelected=$(this).parents("tr").hasClass('selected');
			//如果选中，则移出selected类，否则就加上selected类
			$(this).parents("tr")[hasSelected?"removeClass":"addClass"](options.selected);
			//定义一个临时变量，避免重复使用同一个选择器选择页面中的元素，提升程序效率。
			var $tmp=$(this).parents("table").find('tbody tr :checkbox');
			//用filter方法筛选出选中的复选框。并直接给CheckedAll赋值。
			$(this).parents("table").find('thead tr :checkbox')
				.attr('checked',$tmp.length==$tmp.filter(':checked').length);

		 });

		// 如果复选框默认情况下是选择的，则高色.
		$('tbody>tr:has(:checked)',this).addClass(options.selected);	
		
		
		//表底全选反选不选
		   //全选
		 $("tfoot td :nth-child(3n+1)",this).click(function(){
			 $(this).parents("table").find(':checkbox').attr("checked", true );
			 $(this).parents("table")
				.find("tbody tr").addClass(options.selected);//全选全高亮
		 });
		 //全不选
		 $("tfoot td :nth-child(3n)",this).click(function(){
			$(this).parents("table").find(':checkbox').attr('checked', false);
			$(this).parents("table")
				.find("tbody tr").removeClass(options.selected);
		 });
		 //反选
		 $("tfoot td :nth-child(3n+2)",this).click(function(){
			  $(this).parents("table").find('tbody tr :checkbox').each(function(){
				this.checked=!this.checked;
				var hasSelected=$(this).parents("tr").hasClass(options.selected);
				//如果选中，则移出selected类，否则就加上selected类
				$(this).parents("tr")[hasSelected?"removeClass":"addClass"](options.selected);
			  });
			  var $tmp=$(this).parents("table").find('tbody tr :checkbox');
			//用filter方法筛选出选中的复选框。并直接给CheckedAll赋值。
			$(this).parents("table").find('thead tr :checkbox')
				.attr('checked',$tmp.length==$tmp.filter(':checked').length);
		 });

		}
	});
})(jQuery);




