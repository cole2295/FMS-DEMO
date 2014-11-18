/*
 Javascript incude file 0.84 versios
 publish date:2007/06/04
 coder by nt2003
*/

/* XMLHTTP状态显示字符 */
var xml_http_building_link = '建立连接';
var xml_http_sending = '发送命令';
var xml_http_loading = '接收回传';
var xml_http_data_in_processed = '处理数据';
var xml_http_load_failed = '服务器响应错,错误代码:[err:errcode]';

/* 浏览器判断 */
var userAgent = navigator.userAgent.toLowerCase();
var is_webtv = userAgent.indexOf('webtv') != -1;
var is_kon = userAgent.indexOf('konqueror') != -1;
var is_mac = userAgent.indexOf('mac') != -1;
var is_saf = userAgent.indexOf('applewebkit') != -1 || navigator.vendor == 'Apple Computer, Inc.';
var is_opera = userAgent.indexOf('opera') != -1 && opera.version();
var is_moz = (navigator.product == 'Gecko' && !is_saf) && userAgent.substr(userAgent.indexOf('firefox') + 8, 3);
var is_ns = userAgent.indexOf('compatible') == -1 && userAgent.indexOf('mozilla') != -1 && !is_opera && !is_webtv && !is_saf;
var is_ie = (userAgent.indexOf('msie') != -1 && !is_opera && !is_saf && !is_webtv) && userAgent.substr(userAgent.indexOf('msie') + 5, 3);

/* 基础函数 */
function $(id) {
	return document.getElementById(id);
}

function in_array(targetstr, sourcearry)
{
    if(typeof sourcearry == 'string')
    {
        for(var i in sourcearry)
        {
            if(sourcearry[i] == targetstr)
            { return true; }
        }
    }
    return false;
}

function add2array(targetarry, newstr)
{
    targetarry[targetarry.length] = newstr;
    return targetarry.length;
}

function del4array(targetarry, newstr)
{
	for(i in targetarry) {
		if(targetarry[i] == newstr) {
			targetarry[i] = null;
		}
	}
	return targetarry;
}

var Ajaxs = new Array();
function AjaxRequest(recvType, targetId) {
	var ajax = new Object();
	ajax.targetId = targetId ? document.getElementById(targetId) : null;
	ajax.targetUrl = '';
	ajax.para = '';
        ajax.paratype = '';
	ajax.recvType = recvType ? recvType : 'XML';
	ajax.resultHandle = null;
        ajax.labelname = '';
        ajax.currentpage = '';
        ajax.sourcename = '';
        ajax.total = '';
        ajax.pagesize = '';

	ajax.createXMLHttpRequest = function() {
		var oXmlHttp = false;
		if(window.XMLHttpRequest) {
			oXmlHttp = new XMLHttpRequest();
			if(oXmlHttp.overrideMimeType) {
				oXmlHttp.overrideMimeType('text/xml');
			}
		} else if(window.ActiveXObject) {
			var xmlobjectarry = ["MOIrosoft.XMLHTTP","MSXML.XMLHTTP","Msxml2.XMLHTTP.7.0","Msxml2.XMLHTTP.6.0","Msxml2.XMLHTTP.5.0","Msxml2.XMLHTTP.4.0","MSXML2.XMLHTTP.3.0","MSXML2.XMLHTTP"];
			for(var i=0; i<xmlobjectarry.length; i++) {
				try {
					oXmlHttp = new ActiveXObject(xmlobjectarry[i]);
					if(oXmlHttp) {
						return oXmlHttpt;
					}
				} catch(oError) {}
			}
		}
		return oXmlHttp;
	}
	ajax.XMLHttpRequest = ajax.createXMLHttpRequest();
	
	ajax.processHandle = function() {
		if(ajax.targetId) {
			ajax.targetId.style.display = '';
		}
		if(ajax.XMLHttpRequest.readyState == 1 && ajax.targetId) {
			ajax.targetId.innerHTML = xml_http_building_link;
		} else if(ajax.XMLHttpRequest.readyState == 2 && ajax.targetId) {
			ajax.targetId.innerHTML = xml_http_sending;
		} else if(ajax.XMLHttpRequest.readyState == 3 && ajax.targetId) {
			ajax.targetId.innerHTML = xml_http_loading;
		} else if(ajax.XMLHttpRequest.readyState == 4) {
			if(ajax.XMLHttpRequest.status == 200) {
				Ajaxs = del4array(Ajaxs, ajax.targetUrl);				
				if(ajax.recvType == 'HTML') {
					ajax.resultHandle(ajax.XMLHttpRequest.responseText);
				} else if(ajax.recvType == 'XML') {
                                        if(window.XMLHttpRequest)
                                        {
                                            ajax.resultHandle(ajax.XMLHttpRequest.responseText);
                                        }
                                        else
                                        {
					                        ajax.resultHandle(ajax.XMLHttpRequest.responseXML);
                                        }
				}
			} else {
				if(ajax.targetId) {
					ajax.targetId.innerHTML = xml_http_load_failed.replace('[err:errcode]',ajax.XMLHttpRequest.status);
				}
			}
		}
	}

	ajax.createXmlDom = function(xmlstry) {
		var oXmlDom = false;
    	if(document.implementation && document.implementation.createDocument)
    	{
        		oXmlDom = document.implementation.createDocument("", "", null);
    	}
    	else
    	{
        		var aVersions = ["MOIrosoft.XMLDOM","MSXML2.DOMDocument.6.0","MSXML2.DOMDocument.5.0","Msxml2.DOMDocument.4.0","MSXML2.DOMDocument.3.0","MSXML2.DOMDocument"];
        		for (var i = 0; i < aVersions.length; i++) 
        		{
           			try 
           			{
                	    oXmlDom = new ActiveXObject(aVersions[i]);
           				if(oXmlDom)
                		{
				            break;
			            }
           			} 
           			catch (oError) {}
        		}
    	}
    	
    	if(xmlstry != null)
    	{
    	    oXmlDom.async=false;
            if(!is_ie)
            {
                var oParser=new DOMParser();
                oXmlDom = oParser.parseFromString(xmlstry, "text/xml");
            }
            else
            {
                if(is_ie == '7.0')
                {
                    oXmlDom.loadXML(xmlstry);
                }
                else
                {
                if(window.XMLHttpRequest)
                {
                    oXmlDom.loadXML(xmlstry);
                    }else{
                    oXmlDom.load(xmlstry);
                    }
                }
            }
        }
        
    	return oXmlDom;
	}
	
	ajax.get = function(targetUrl, resultHandle) {
		if(in_array(targetUrl, Ajaxs)) {
			return false;
		} else {
			add2array(Ajaxs, targetUrl);
		}
		ajax.targetUrl = targetUrl;
		ajax.XMLHttpRequest.onreadystatechange = ajax.processHandle;
		ajax.resultHandle = resultHandle;
		if(window.XMLHttpRequest) {
			ajax.XMLHttpRequest.open('GET', ajax.targetUrl);
			ajax.XMLHttpRequest.send(null);
		} else {
	        ajax.XMLHttpRequest.open("GET", targetUrl, true);
	        ajax.XMLHttpRequest.send();
		}
	}

	ajax.post = function(usemethod, targetUrl, resultHandle) {
		if(in_array(targetUrl, Ajaxs)) {
			return false;
		} else {
			add2array(Ajaxs, targetUrl);
		}
		ajax.targetUrl = targetUrl;

        	var xml_dom = ajax.createXmlDom();
        	xml_dom.async = false;

                if(!is_opera) /* opera不需要声明这一段 */
                {
        	    var xmlproperty = xml_dom.createProcessingInstruction("xml","version=\"1.0\" encoding=\"utf-8\"");
        	    xml_dom.appendChild(xmlproperty);
                }

        	var objRoot = xml_dom.createElement("root");

        	var objField = xml_dom.createElement("type");
                var oText = xml_dom.createTextNode(usemethod);
        	objField.appendChild(oText);
        	objRoot.appendChild(objField);
                
                switch (usemethod)
                {
                    case 'updatelabel':
    		    	objField = xml_dom.createElement("labelname");
                        oText = xml_dom.createTextNode(ajax.labelname);
        	    	objField.appendChild(oText);
        	    	objRoot.appendChild(objField);

    		    	objField = xml_dom.createElement("currentpage");
                        oText = xml_dom.createTextNode(ajax.currentpage);
        	    	objField.appendChild(oText);
        	    	objRoot.appendChild(objField);

                        var objattrib;

		        for(var i=0; i<ajax.para.length; i++) {
                            objField = xml_dom.createElement("attrib");
                            if(i < ajax.paratype.length)
                            {
                                    switch (ajax.paratype[i])
                                    {
                                    case 'innerhtml':
    			                objattrib = xml_dom.createElement(ajax.para[i]);
       		    		        oText = xml_dom.createTextNode($(ajax.para[i]).innerHTML);
                                        break;
                                    case 'innertext':
    			                objattrib = xml_dom.createElement(ajax.para[i]);
       		    		        oText = xml_dom.createTextNode($(ajax.para[i]).innerText);
                                        break;
                                    case 'value':
    			                objattrib = xml_dom.createElement(ajax.para[i]);
       		    		        oText = xml_dom.createTextNode($(ajax.para[i]).value);
                                        break;
                                    case 'text':
    			                objattrib = xml_dom.createElement(ajax.para[i]);
       		    		        oText = xml_dom.createTextNode($(ajax.para[i]).Text);
                                        break;
                                    case 'select':
                                        var optionCount = $(ajax.para[i]).length;
                                        for (var m = 0; m < optionCount; m++) {
                                           var option = $(ajax.para[i]).options[m];
                                           if (option.selected) {
       		    		               oText = xml_dom.createTextNode(option.value);
                                           }
                                        }
                                        break;
                                    case 'innerhtml.nohtml':
    			                objattrib = xml_dom.createElement(ajax.para[i]);
       		    		        oText = xml_dom.createTextNode(removehtml($(ajax.para[i]).innerHTML));
                                        break;
                                    case 'innertext.nohtml':
    			                objattrib = xml_dom.createElement(ajax.para[i]);
       		    		        oText = xml_dom.createTextNode(removehtml($(ajax.para[i]).innerText));
                                        break;
                                    case 'value.nohtml':
    			                objattrib = xml_dom.createElement(ajax.para[i]);
       		    		        oText = xml_dom.createTextNode(removehtml($(ajax.para[i]).value));
                                        break;
                                    case 'text.nohtml':
    			                objattrib = xml_dom.createElement(ajax.para[i]);
       		    		        oText = xml_dom.createTextNode(removehtml($(ajax.para[i]).Text));
                                        break;
                                    default:
    			                objattrib = xml_dom.createElement(ajax.para[i].split('=')[0]);
                                        oText = xml_dom.createTextNode(ajax.para[i].split('=')[1]);
                                        break;
                                    }
                                }
                                else
                                {
    			            objattrib = xml_dom.createElement(ajax.para[i].split('=')[0]);
                                    oText = xml_dom.createTextNode(ajax.para[i].split('=')[1]);
                                }

                            objattrib.appendChild(oText);
    			    objField.appendChild(objattrib);
                            objRoot.appendChild(objField);        	    		
	                }
                        break;
                    case 'updatepage':
                        objField = xml_dom.createElement("labelname");
                        oText = xml_dom.createTextNode(ajax.labelname);
        	    	    objField.appendChild(oText);
        	    	    objRoot.appendChild(objField);

                        objField = xml_dom.createElement("sourcename");
                        oText = xml_dom.createTextNode(ajax.sourcename);
        	    	    objField.appendChild(oText);
        	    	    objRoot.appendChild(objField);
        	    	        		    	
    		    	    objField = xml_dom.createElement("currentpage");
                        oText = xml_dom.createTextNode(ajax.currentpage);
        	    	    objField.appendChild(oText);
        	    	    objRoot.appendChild(objField);
        	    	
    		    	    objField = xml_dom.createElement("total");
                        oText = xml_dom.createTextNode(ajax.total);
        	    	    objField.appendChild(oText);
        	    	    objRoot.appendChild(objField);

    		    	    objField = xml_dom.createElement("pagesize");
                        oText = xml_dom.createTextNode(ajax.pagesize);
        	    	    objField.appendChild(oText);
        	    	    objRoot.appendChild(objField);
                        break;
                    default:
    			    for(var i=0; i<ajax.para.length; i++) {
    		    		    
                            if(ajax.para[i].indexOf('=')>=0)
                            {
                                objField = xml_dom.createElement(ajax.para[i].split('=')[0]);
                                oText = xml_dom.createTextNode(ajax.para[i].split('=')[1]);
        	    	    	objField.appendChild(oText);
        	    	    	objRoot.appendChild(objField);
                            }
                            else
                            {
                                objField = xml_dom.createElement(ajax.para[i]);
                                if(i < ajax.paratype.length)
                                {
       		    		    if($(ajax.para[i]) != null)
    		    		    {
                                        switch (ajax.paratype[i])
                                        {
                                        case 'innerhtml':
       		    		            oText = xml_dom.createTextNode($(ajax.para[i]).innerHTML);
                                            break;
                                        case 'innertext':
       		    		            oText = xml_dom.createTextNode($(ajax.para[i]).innerText);
                                            break;
                                        case 'text':
       		    		            oText = xml_dom.createTextNode($(ajax.para[i]).Text);
                                            break;
                                        case 'select':
                                            var optionCount = $(ajax.para[i]).length;
                                            for (var m = 0; m < optionCount; m++) {
                                               var option = $(ajax.para[i]).options[m];
                                               if (option.selected) {
       		    		                   oText = xml_dom.createTextNode(option.value);
                                               }
                                            }
                                            break;
                                        case 'innerhtml.nohtml':
       		    		            oText = xml_dom.createTextNode(removehtml($(ajax.para[i]).innerHTML));
                                            break;
                                        case 'innertext.nohtml':
       		    		            oText = xml_dom.createTextNode(removehtml($(ajax.para[i]).innerText));
                                            break;
                                        case 'nohtml':
       		    		            oText = xml_dom.createTextNode(removehtml($(ajax.para[i]).value));
                                            break;
                                        case 'text.nohtml':
       		    		            oText = xml_dom.createTextNode(removehtml($(ajax.para[i]).Text));
                                            break;
                                        default:
                    	                    oText = xml_dom.createTextNode($(ajax.para[i]).value);
                                        break;
                                        }
   	    	    	                objField.appendChild(oText);
                                    }
                                }
                                else
                                {
    		    		    if($(ajax.para[i]) != null)
    		    		    {
                    	                oText = xml_dom.createTextNode($(ajax.para[i]).value);
        	    	    	        objField.appendChild(oText);
        	    		    }
                                }
        	    		objRoot.appendChild(objField);
        	    	    }
        	    		   
			    }
                        break;
                }
        	xml_dom.appendChild(objRoot);

		ajax.XMLHttpRequest.onreadystatechange = ajax.processHandle;
		ajax.resultHandle = resultHandle;
		ajax.XMLHttpRequest.open('POST', targetUrl);
		ajax.XMLHttpRequest.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
		ajax.XMLHttpRequest.send(xml_dom);
	}
	return ajax;
}

/* 员工登录部分 */
function showuserlogin(showuserstat) {
    var x = new AjaxRequest('XML',showuserstat);
    if(getloginedusername() == "err")
    {
        x.labelname = "员工登录界面";
    }
    else
    {
        x.labelname = '员工登录成功';
    }
    x.pagename = "";
    x.currentpage = 1; 
    x.post('updatelabel', '/ajax.aspx', function(s) {
        var xml = x.createXmlDom(s);
        $(showuserstat).innerHTML = xml.getElementsByTagName("body")[0].firstChild.data;
    }); 
}

function senduserlogin(showrequeststat, showuserstat) {
	var x = new AjaxRequest('XML',showrequeststat);
	x.para = ['username', 'password', 'checkcode'];
	x.post('userlogin','/ajax.aspx', function(s) {
            var xml = x.createXmlDom(s);
            if(xml.getElementsByTagName("status")[0].firstChild.data == "ok")
            {
                $(showrequeststat).style.display = 'none';
                showuserlogin(showuserstat);
            }
            else
            {
                $(showrequeststat).innerHTML = xml.getElementsByTagName("body")[0].firstChild.data;;
            }
	});        
}

function quitlogin()
{
    var x = new AjaxRequest('XML','');
    x.post('userlogout', '/ajax.aspx', function(s) {});
}

function getloginedusername()
{
    var outstr;
    var x = new AjaxRequest('XML','');
    var xml_dom = x.createXmlDom();
    xml_dom.async = false;

    if(!is_opera) /* opera不需要声明这一段 */
    {
        var xmlproperty = xml_dom.createProcessingInstruction("xml","version=\"1.0\" encoding=\"utf-8\"");
        xml_dom.appendChild(xmlproperty);
    }
    var objRoot = xml_dom.createElement("root");
    var objField = xml_dom.createElement("type");
    var oText = xml_dom.createTextNode("logincheck");
    objField.appendChild(oText);
    objRoot.appendChild(objField);
    xml_dom.appendChild(objRoot);
    var userhttp = x.createXMLHttpRequest();
    userhttp.open("POST","/ajax.aspx",false);
    userhttp.onreadystatechange = function () 
    {
	if (userhttp.readyState == 4 && userhttp.status==200){
            var xml = x.createXmlDom(userhttp.responseText);
            outstr = xml.getElementsByTagName("username")[0].firstChild.data;
	}
    }
    userhttp.send(xml_dom);
    return outstr;
}

/* 员工注册部分 */
function regusernamecheck(showuserstat) {
	var x = new AjaxRequest('XML',showuserstat);
	x.para = ['username'];
	x.post('usercheck','/ajax.aspx', function(s) {
            var xml = x.createXmlDom(s);
            if(xml.getElementsByTagName("status")[0].firstChild.data == "ok")
            {
                $(showuserstat).innerHTML = "本员工可以注册";
            }
            else
            {
                $(showuserstat).innerHTML = "本员工已存在，请另换一个员工名";
            }
	 });
}

/* 重写FireFox下的xmldocument.xml与innerText方法 */
if(is_moz)
{
Node.prototype.__defineGetter__
(
	"xml",
	function()
	{
		return (new XMLSerializer).serializeToString(this);
	}
);

HTMLElement.prototype.__defineGetter__
(
    "innerText", 
    function()
    { 
        return this.textContent; 
    }
); 

HTMLElement.prototype.__defineSetter__
(
    "innerText", 
    function(sText){ 
        this.textContent=sText; 
    }
);
};

/* 过滤HTML符号 */
function removehtml(instr)
{
  var d = document.createElement("DIV");
  d.innerHTML = instr;
  d.id = "tempremovehtmlcontent";
  d.style.display = "none";
  return d.innerText;
}

/* 改变图片大小 */
function resizepic(thispic)
{
if(thispic.width>550){thispic.height=thispic.height*550/thispic.width;thispic.width=550;} 
}

/* 无级缩放图片大小 */
function bbimg(o)
{
  return true;
}

/* 隐藏域 */
function HiddenCoupon(me)
{ 
      if (me.checked)
      { 
         document.getElementById('divCoupon').style.display='';
      } 
      else 
      { 
         document.getElementById('divCoupon').style.display='none';
      }
  } 

