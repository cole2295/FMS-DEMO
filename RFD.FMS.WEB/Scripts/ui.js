$(document).ready(function(){
  //搜索切换

  $(".btn_on a").click(function(event){
    $(".search").addClass("search_on");
    $(".search_c").slideDown(300);
  });

  $(".btn_off a").click(function(event){
    $(".search").removeClass("search_on");
    $(".search_c").slideUp(600);
  });

  $('.del').live('click', function(){
    $(this).parent().parent().parent().remove();
    $('.add').show();
    return false;
  });
  
  $('#add_new').live('click', function(){
    $('#add_new_gift').hide();
    $('#add_new_tbl').show();
    return false;
  });
  
  $('#add_gift').live('click', function(){
    $('#add_new_tbl').hide();
    $('#add_new_gift').show();
    return false;
  });

  $('input.datepicker').live('click', function() {
      $(this).datepicker({showOn:'focus',changeMonth: true,changeYear: true}).focus();
  });

  $('.add').click(function() {
      var html = $("#time_select").html();
      $("#time_select").before(html);
      //$('.add').hide();
      return false;
  });

  //表格隔行变色
	$(".table1 tr:nth-child(even)").addClass("td_bg");

  
  //订单状态 展开 收起	
//	$(".order_all_on").toggle(
//		function () {
//			$(this).text("[全部收起]");
//			$("#order_status .off").text("收起");
//			$("#order_status .off").parent().parent().next().fadeIn();
//			$("#order_status .off").removeClass("off").addClass("on");
//			
//			 return false;
//		},
//		function () {
//			$(this).text("[全部展开]");
//			$("#order_status .on").text("展开");
//			$("#order_status .on").parent().parent().next().fadeOut();
//			$("#order_status .on").removeClass("on").addClass("off");
//			
//		 	return false;
//		}
//	); 
	
	$(".order_all_on").toggle(
		function () {
			$(this).children(".blue").children("a").text("[全部收起]");
			$("#order_status .off").text("收起");
			$("#order_status .off").parent().parent().next().css("display","");
			$("#order_status .off").removeClass("off").addClass("on");
			
			 return false;
		},
		function () {
			$(this).children(".blue").children("a").text("[全部展开]");
			$("#order_status .on").text("展开");
			$("#order_status .on").parent().parent().next().css("display","none");
			$("#order_status .on").removeClass("on").addClass("off");
			
		 	return false;
		}
	); 



	$(".table3 td:nth-child(2)").toggle(
			function () {
				$(this).children(".off").text("收起");
				$(this).parent().next().fadeIn();
				$(this).children(".off").removeClass("off").addClass("on");
				 return false;
			},
			function () {
				$(this).children(".on").text("展开");
				$(this).parent().next().fadeOut();
				$(this).children(".on").removeClass("on").addClass("off");
				 return false;
			}
		);
	
	//修改收货地址
	$(".address_toggle").toggle(
		function () {
			$(this).children(".blue").children("a").text("[取消修改]");
			$(".address").fadeIn();
			 return false;
		},
		function () {
			$(this).children(".blue").children("a").text("[修改]");
			$(".address").fadeOut();		
		 	return false;
		}
	);


	//支付信息切换
	$(".pay_table_toggle").toggle(
		function () {
			$(this).children(".blue").children("a").text("[收起]");
			$(".pay_table").fadeIn();
			 return false;
		},
		function () {
			$(this).children(".blue").children("a").text("[展开]");
			$(".pay_table").fadeOut();		
		 	return false;
		}
	);

	setInterval(function(){
	  $(".pop_up_bg").each(function(){
	    var div_h = $(this).prev().height();
	    $(this).height(div_h+16);
		  $(this).css("top",$(window).scrollTop()+400);
		  $(this).prev().css("top",$(window).scrollTop()+400)
	  });
	}, 1);
	
	//添加
	$(".add_btn").click(function(event){
   		$("#pop_up_add").fadeIn("500");
		 return false;

	});
	$("#pop_up_add .btn_save").click(function(event){
   		$("#pop_up_add").fadeOut("500");
		 return false;
	});
	$("#pop_up_add .btn_cancel_2").click(function(event){
   		$("#pop_up_add").fadeOut("500");
		 return false;
	});
	//换货
	$(".ch_btn").click(function(event){
   		$("#pop_up_ch").fadeIn("500");
		 return false;
	});
	$("#pop_up_ch .btn_save").click(function(event){
   		$("#pop_up_ch").fadeOut("500");
		 return false;
	});
	$("#pop_up_ch .btn_cancel_2").click(function(event){
   		$("#pop_up_ch").fadeOut("500");
		 return false;
	});
	//退货
	$(".re_btn").click(function(event){
   		$("#pop_up_re").fadeIn("500");
		 return false;
	});
	$("#pop_up_re .btn_save").click(function(event){
   		$("#pop_up_re").fadeOut("500");
		 return false;
	});
	$("#pop_up_re .btn_cancel_2").click(function(event){
   		$("#pop_up_re").fadeOut("500");
		 return false;
	});
	//删除
	$(".del_btn").click(function(event){
   		$("#pop_up_del").fadeIn("500");
		 return false;
	});
	$("#pop_up_del .btn_save_2").click(function(event){
   		$("#pop_up_del").fadeOut("500");
		 return false;
	});
	$("#pop_up_del .btn_cancel_2").click(function(event){
   		$("#pop_up_del").fadeOut("500");
		 return false;
	});
	//编辑
	$(".edit_btn").click(function(event){
   		$("#pop_up_edit").fadeIn("500");
		 return false;
	});
	$("#pop_up_edit .btn_save").click(function(event){
   		$("#pop_up_edit").fadeOut("500");
		 return false;
	});
	$("#pop_up_edit .btn_cancel_2").click(function(event){
   		$("#pop_up_edit").fadeOut("500");
		 return false;
	});
	//客服确认
	$(".btn_6").click(function(event){
   		$("#pop_up_6").fadeIn("500");
		 return false;
	});
	$("#pop_up_6 .btn_save").click(function(event){
   		$("#pop_up_6").fadeOut("500");
		 return false;
	});
	$("#pop_up_6 .btn_cancel_2").click(function(event){
   		$("#pop_up_6").fadeOut("500");
		 return false;
	});
	

	
	//浮层阴影效果
	var wrap_h =$(document).height(); 
	$(".wrap_bg").height(wrap_h);
	
	//输入框提示
	$(".text").each(function(){
		$(this).wrap("<span class='text_wrap'></span>");
		$(this).click(function(event){
			$(".text_wrap").children("ul").fadeOut();
			$(this).parent(".text_wrap").children("ul").fadeIn();					
		});
	});

	$(document).mouseup(function(event){
		$(".text_wrap ul").fadeOut(200);
	});

	$(".text_wrap").append("<ul><li>内容1</li><li>内容2</li><li>内容3</li><li>内容4</li><li>内容5</li><li>内容6</li><li>内容7</li><li>内容8</li><li>内容9</li><li>内容10</li><li>内容11</li><li>内容12</li></ul>"); 
	
	$(".text_wrap ul li").click(function(event){
		var text = $(this).text()
		$(this).parents().parents().children(".text").val(text);
		$(this).parent().fadeOut();	
		return false;
	});
	
	$(".text_wrap ul li").hover(
	function () {
		$(this).css({color:"#FFF", background:"#36C"}); 
	},
	function () {
		$(this).css({color:"#666", background:"#FFF"});
	}); 
	
	//商品信息列表
	$(".selected_tr tr").hover(
	function () {
		$(this).addClass("hover"); 
	},
	function () {
		$(this).removeClass("hover");
	});

	$(".selected_tr tr td").click(function(event){
		$(this).parent().addClass("selected")
		$(this).parent().siblings().removeClass("selected");
		return false;
	});
	

});