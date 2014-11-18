var downloadURL = function downloadURL(url) {
    var iframe;
    var hiddenIFrameId = 'hiddenDownloader';
    iframe = document.getElementById(hiddenIFrameId);
    if (iframe === null) {
        iframe = document.createElement('iframe');
        iframe.id = hiddenIFrameId;
        iframe.style.display = 'none';
        document.body.appendChild(iframe);
    }
    iframe.src = url;
};


var fileDownloadCheckTimer;
function blockUIForDownload(value, name) {
    var token = new Date().getTime(); //use the current timestamp as the token value
    value.val(token);
    name.val(token + "token");
    fileDownloadCheckTimer = window.setInterval(function () {
        var cookieValue = $.cookie(name.val());
        if (cookieValue == token)
            finishDownload(name);
    }, 1000);
}

function finishDownload(name) {
    window.clearInterval(fileDownloadCheckTimer);
    $.cookie(name.val(), null);
    $(document).progressDialog.hideDialog();
    alert("导出查询完成，点击保存");
}  