(function ($) {
    $.fn.extend({
        toSuperTable: function (options) {
            var setting = $.extend({
                width: "600px", height: "300px",
                margin: "10px", padding: "0px",
                overflow: "hidden", colWidths: undefined,
                fixedCols: 0, headerRows: 1,
                onStart: function () { },
                onFinish: function () { },
                cssSkin: "sSky"
            }, options);
            return this.each(function () {
                var q = $(this);
                var id = q.attr("id");
                q.removeAttr("style").wrap("<div id='" + id + "_box'></div>");
                var nonCssProps = ["fixedCols", "headerRows", "onStart", "onFinish", "cssSkin", "colWidths"];
                var container = $("#" + id + "_box");
                var height = q.find("tr").height();
                if (setting.count == 0) {
                    setting.height = q.height();
                } else if (q.height() < setting.maxHeight) {
                    setting.height = 3 * height - 8;
                }
                for (var p in setting) {
                    if ($.inArray(p, nonCssProps) == -1) {
                        container.css(p, setting[p]);
                        delete setting[p];
                    }
                }

                var mySt = new superTable(id, setting);
                //alert(container.html());
            });
        }
    });
})(jQuery);