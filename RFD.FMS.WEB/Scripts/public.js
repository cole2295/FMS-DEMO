$(function() {
    $(".Wdate").bind("focus", function() {
        WdatePicker({ skin: 'whyGreen', dateFmt: 'yyyy-MM-dd' });
    });
    if ($('.GridView').length > 0 && $('.GridView').tablegrid != undefined)
        $('.GridView').tablegrid({ oddColor: '#F5FAFC', evenColor: '#FFFFFF', overColor: '#FFF8D2', selColor: '#FFCC99', useClick: true });
})
$(function() {
    $(".Wyearmonth").focus(function() { WdatePicker({ skin: 'whyGreen', dateFmt: 'yyyy-MM' }); });
})
function reBindDate() {
    $(".Wdate").focus(function() { WdatePicker({ skin: 'whyGreen', dateFmt: 'yyyy-MM-dd HH:mm:ss' }); });
}

function fnOpenNewPage(pageId, pageTitle, url, showClose) {
    var main = window.parent.parent.frames['main'];
    if (main && main.SetPage) {
        main.SetPage(pageId, pageTitle, url, showClose);
    }
}
function fnCloseNewPage(pageId) {
    var main = window.parent.parent.frames['main'];
    if (main && main.ClosePage) {
        main.ClosePage(pageId);
    }
}
