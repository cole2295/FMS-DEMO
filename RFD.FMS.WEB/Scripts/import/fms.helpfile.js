
function GetHelpFile() {
    var help = $("#helpfile a.help:first");
    //var $helpfilea = $helpfile.find('a.help');
    //var help = $helpfilea.first();
    help.attr('disabled', 'disabled');

    var sysname = help.attr("sysname");
    var menuname = help.attr("menuname");

    $.ajax({
        type: "POST",
        contentType: "application/json",
        url: "../BasicSetting/GenerateFile.aspx/GetHelpFile",
        data: "{type:'help',sysname:'" + sysname + "',menuname:'" + menuname + "'}",
        dataType: 'json',
        success: function(result) {
            if (result.d == null) {
                alert('服务器无响应');
            }
            if (result.d.done) {
                help.removeAttr('disabled');
                HideHelpProgress("help", sysname, menuname, result.d.ftpfile);

            } else {

                alert('未添加帮助文件');
            }

        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            help.removeAttr('disabled');
            alert('操作超时，服务器无响应' + textStatus + errorThrown);

        }
    });

    return false;
}



function HideHelpProgress(type, sysname, menuname, ftpfile) {
    document.getElementById("helpdiv").innerHTML =
            '<iframe name="helpiframe" style="display:none;" src="../BasicSetting/GenerateFile.aspx?type=' + type + '&sysname=' + sysname + '&menuname=' + menuname + '&ftpfile=' + ftpfile + '"></iframe>';

}