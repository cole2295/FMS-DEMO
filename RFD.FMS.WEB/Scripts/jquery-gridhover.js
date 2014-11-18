/* 
Create by FreezeSoul
Blog:http://hi.baidu.com/freezesoul
Time:2011-07-22
*/
$.fn.tablegrid = function(params) {
    var options = {
        oddColor: '#F5FAFC',
        evenColor: '#FFFFFF',
        overColor: '#FFF8D2',
        selColor: '#FFCC99',
        maxRow: 250,
        useClick: false
    };
    $.extend(options, params);
    $(this).each(function() {
        $(this).find('tr:odd > td').css('backgroundColor', options.oddColor);
        $(this).find('tr:even > td').css('backgroundColor', options.evenColor);
        if ($(this).attr('disableRowSeleced') && $(this).attr('disableRowSeleced') == 'true')
            return;
        if ($(this).find('tr').size() > options.maxRow)
            return;
        var tableThis = this;
        if (options.useClick) {
            var $checkBox = $(this).find('th :checkbox');
            if ($checkBox.length == 1) {
                $checkBox.unbind('click').removeAttr('onclick').click(function(event) {
                    event.preventDefault();
                    event.stopPropagation();
                }).mousedown(function(event) {
                    $checkBox.attr('checked', $checkBox.attr('checked') ? false : true);
                    $(tableThis).find('tr').each(function() {
                        var $checkBoxTd = $(this).find('td :checkbox');
                        $checkBoxTd.click(function() {
                        $checkBox.attr("checked", this.checked ? $checkBoxTd.parents(".GridView").find("td :checkbox:not(:checked)").length == 0 : false);
                        });
                        if ($checkBoxTd.length == 1) {
                            if (event.which == 1) {
                                $checkBoxTd.attr('checked', $checkBox.attr('checked') ? true : false);
                                if ($checkBoxTd.attr('checked')) {
                                    $(this).find('td').css('backgroundColor', options.selColor);
                                } else {
                                    $(this).find('td').css('backgroundColor', this.origColor);
                                }
                            }
                            else {
                                $checkBoxTd.attr('checked', $checkBoxTd.attr('checked') ? false : true);
                                $(this).find('td').css('backgroundColor', $checkBoxTd.attr('checked') ? options.selColor : this.origColor);
                            }
                        }
                    });
                    event.stopPropagation();
                });
            }
        }
        $(this).find('tr').each(function() {
            $(this).find('td').css('cursor', 'hand');
            this.origColor = $(this).find('td').css('backgroundColor');
            if (options.useClick) {
                $(this).bind('click', function(event) {
                    var $checkBox = $(this).find('td :checkbox');
                    var $target = $(event.target);
                    if ($checkBox.length == 1) {
                        if ($checkBox.attr('checked')) {
                            $(this).find('td').css('backgroundColor', this.origColor);
                        } else {
                            $(this).find('td').css('backgroundColor', options.selColor);
                        }
                        if ($target.attr('type') != 'checkbox')
                            $checkBox.attr('checked', $checkBox.attr('checked') ? false : true);
                    }
                    else {
                        var $radioBox = $(this).find('td :radio');
                        if ($radioBox.length == 1) {
                            if ($target.attr('type') != 'radio') {
                                $(tableThis).find('tr:odd > td').css('backgroundColor', options.oddColor);
                                $(tableThis).find('tr:even > td').css('backgroundColor', options.evenColor);
                                $radioBox.attr('checked', true);
                                $(this).find('td').css('backgroundColor', options.selColor);
                            }
                        }
                    }
                    event.stopPropagation();
                });
            }
            $(this).mouseover(function() {
                $(this).find('td').css('backgroundColor', options.overColor);
            });
            $(this).mouseout(function() {
                var $checkBox = $(this).find('td :checkbox');
                var checkStatus = false;
                if ($checkBox.length == 1) {
                    checkStatus = $checkBox.attr('checked') ? true : false;
                }
                else {
                    var $radioBox = $(this).find('td :radio');
                    if ($radioBox.length == 1) {
                        checkStatus = $radioBox.attr('checked') ? true : false;
                    }
                }
                if (checkStatus) {
                    $(this).find('td').css('backgroundColor', options.selColor);
                } else {
                    $(this).find('td').css('backgroundColor', this.origColor);
                }
            });
        });
    });
};
