function datait(dataName, btnId, dataNumber) {
    for (i = 0; i < dataNumber; i++) {
        document.getElementById(dataName + "_div" + i).style.display = "none";
        document.getElementById(dataName + "_btn" + i).className = dataName + "_off";
    }
    document.getElementById(dataName + "_div" + btnId).style.display = "block";
    document.getElementById(dataName + "_btn" + btnId).className = dataName + "_on";

}


function setTab(m, n) {
    var tli = document.getElementById("menu" + m).getElementsByTagName("li");
    var mli = document.getElementById("main" + m).getElementsByTagName("ul");
    for (i = 0; i < tli.length; i++) {
        tli[i].className = i == n ? "hover" : "";
        mli[i].style.display = i == n ? "block" : "none";
    }
}