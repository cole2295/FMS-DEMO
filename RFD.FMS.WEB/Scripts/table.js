;(function($) {
	$.fn.extend({
		"tableCheck":function(options){
		//����Ĭ��ֵ
		options=$.extend({
			selected:"selected"
			
		},options);

		//ȫѡ

		 $("thead tr :checkbox",this).click(function(){
			$(this).parents("table").find('tbody tr :checkbox').
				attr("checked", this.checked );
			$(this).parents("table")
				.find("tbody tr")[this.checked?"addClass":"removeClass"](options.selected);//ȫѡȫ����
		 });
		 //��ѡ
		 $('tbody tr :checkbox',this).click(function(){
			 
			 var hasSelected=$(this).parents("tr").hasClass('selected');
			//���ѡ�У����Ƴ�selected�࣬����ͼ���selected��
			$(this).parents("tr")[hasSelected?"removeClass":"addClass"](options.selected);
			//����һ����ʱ�����������ظ�ʹ��ͬһ��ѡ����ѡ��ҳ���е�Ԫ�أ���������Ч�ʡ�
			var $tmp=$(this).parents("table").find('tbody tr :checkbox');
			//��filter����ɸѡ��ѡ�еĸ�ѡ�򡣲�ֱ�Ӹ�CheckedAll��ֵ��
			$(this).parents("table").find('thead tr :checkbox')
				.attr('checked',$tmp.length==$tmp.filter(':checked').length);

		 });

		// �����ѡ��Ĭ���������ѡ��ģ����ɫ.
		$('tbody>tr:has(:checked)',this).addClass(options.selected);	
		
		
		//���ȫѡ��ѡ��ѡ
		   //ȫѡ
		 $("tfoot td :nth-child(3n+1)",this).click(function(){
			 $(this).parents("table").find(':checkbox').attr("checked", true );
			 $(this).parents("table")
				.find("tbody tr").addClass(options.selected);//ȫѡȫ����
		 });
		 //ȫ��ѡ
		 $("tfoot td :nth-child(3n)",this).click(function(){
			$(this).parents("table").find(':checkbox').attr('checked', false);
			$(this).parents("table")
				.find("tbody tr").removeClass(options.selected);
		 });
		 //��ѡ
		 $("tfoot td :nth-child(3n+2)",this).click(function(){
			  $(this).parents("table").find('tbody tr :checkbox').each(function(){
				this.checked=!this.checked;
				var hasSelected=$(this).parents("tr").hasClass(options.selected);
				//���ѡ�У����Ƴ�selected�࣬����ͼ���selected��
				$(this).parents("tr")[hasSelected?"removeClass":"addClass"](options.selected);
			  });
			  var $tmp=$(this).parents("table").find('tbody tr :checkbox');
			//��filter����ɸѡ��ѡ�еĸ�ѡ�򡣲�ֱ�Ӹ�CheckedAll��ֵ��
			$(this).parents("table").find('thead tr :checkbox')
				.attr('checked',$tmp.length==$tmp.filter(':checked').length);
		 });

		}
	});
})(jQuery);




