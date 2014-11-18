/*FileName: menu.js
**Author:   何名宇
**Date:     2011-10-09
**Usage:    用于定义配送项目LMS的导航菜单js
**/
if (typeof window.Menu == "undefined") window.Menu = {};
var menu = Menu.prototype;
var lastIndex = 0;
menu = {
    load: function() {
        var contents = document.getElementsByClassName('content');
        var toggles = document.getElementsByClassName('type');
        var myAccordion = new fx.Accordion(
			        toggles, contents, { opacity: true, duration: 400 });
        myAccordion.showThisHideOpen(contents[0]);
        jQuery(function() {
            jQuery('#container li a').bind('click', function(e) {
                var main = window.top.frames["main"];
                if (main && main.SetPage) {
                    main.SetPage(this.id, this.innerHTML, this.href, true);
                    //main.SetPage(jQuery(this).attr('id'), jQuery(this).text(), jQuery(this).attr('href'), true);
                    e.preventDefault();
                }
            });
        });
    },
    show: function() {
        jQuery.noConflict();
        (function($) {
            var parent = window.top.document.getElementById("frame");
            var imgObj = $("#imgBar");
            if (parent && imgObj) {
                imgObj.toggle(function() {
                    parent.cols = "0,11,*";
                    this.src = "../images/menu_left.gif";
                }, function() {
                    parent.cols = "182,11,*";
                    this.src = "../images/menu_right.gif";
                })
            }
        })(jQuery);
    },
    navigate: function(options) {
        var main = window.top.frames["main"];
        if (main && main.SetPage) {
            main.SetPage(this.id, options.text, options.url, true);
            options.event.preventDefault();
        }
    },
    toggle: function(index) {
        (function($) {
            var contents = $(".content");
            var actived = contents.eq(index);
            if (lastIndex == index) {
                $(".type").eq(index).unbind("click");
                if (actived.css("display") == "block") {
                    actived.hide("400");
                } else {
                    actived.show("400");
                }
            } else {
                actived.show("400");
            }
            lastIndex = index;
            $(".type").eq(index).bind("click");
        })(jQuery);
    },
    exit: function() {
        if (!confirm("确认要离开本系统吗?")) return false;
        window.opener = null;
        window.parent.close();
        window.location.href = "";
        return true;
    }
}