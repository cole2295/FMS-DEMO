    var FileAttributeDialog = "dialogHeight:413px;dialogWidth:365px;maximize:no;scroll:no;status:no;resizable:no;unadorned:no;help:no;center:yes";
    
    function btnModifyClick(prefix)
    {
        //alert(prefix);
        var args = new Array(3);
        var url;
	    var ret;
	    
        args[0] = "a";
        args[1] = "b";
        args[2] = "c";
        
//        var param = document.getElementById(prefix+"_PartParam").value;
//        url = "type="+param;
        //alert(param);
        today   =   new   Date(); 
        d="&d="+today.toTimeString()
        ret = window.showModalDialog("../Common/StationChoose.aspx?" + url + d, args, FileAttributeDialog);
        if (ret != null)
        {
            var retvalue = ret.split("::");
            
            document.getElementById(prefix+"_ExpressCompanyID").value = retvalue[0];
            document.getElementById(prefix+"_ExpressCompanyCode").value = retvalue[1];
            document.getElementById(prefix+"_CompanyName").value = retvalue[2];
            //alert(retvalue[2]);
            
        }
        
        //document.forms[0].submit();
        __doPostBack(event.srcElement.id,"")
        return true;
    }
    
    //设置文本框焦点获取事件
    function SetIndexFocus()
    {
	    try
	    {
	        var src = event.srcElement;
		    if(src.value =="" ||src.type!="text")
		    {
		        return false;
		    }
	        //alert(src.id+'  3');
		    var vsClass = src.id.replace("Show","");
		    
		    //alert(src);
		    src.value = document.getElementById(vsClass+"Code").value;
		    //src.select();
		    return false;
	    }
	    catch(err)
	    {
		    alert(err.description);
		    return false;
	    }
    }


    //设置文本框焦点获取事件
    function SetIndexBlur()
    {
	    try
	    {
		    var src = event.srcElement;
		    //alert(src.id);
		    if (src.value == "" || src.id == "ExpressCompany" || src.type != "text")
		    {
		        return false;
		    }
		    var vsClass = src.id.replace("Show","");
		    //alert('2');
		    src.value = document.getElementById(vsClass+"Name").value;
		    src.blur();
		    return false;
	    }
	    catch(err)
	    {
		    alert(err.description);
		    return false;
	    }
    }