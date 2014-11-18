document.request = null;
Xml = new Object();
Xml.Request = function(url, data, command, args, breakOther)
{	
	if (breakOther == true && document.request != null) document.request.abort();
	var rex = new RegExp('^http:/' + '/');
	rex.ignoreCase = true;
	if(rex.test(url))
	{
		var domain = url.replace(rex, '').toLowerCase();
		var index = domain.indexOf('/');
		if(index > -1) domain = domain.substring(0, index);
		if(domain != window.location.host)
		{
			var scripts = document.getElementsByTagName('SCRIPT');
			var path;
			for (var i=0; i<scripts.length; i++)
			{
				if (scripts[i].src.indexOf('Root.Xml.js') > -1)
				{
					path = scripts[i].src;
					break;
				}
			}			
			path = path.substring(0, path.lastIndexOf('/') + 1);
			url = path + 'Root.Xml.aspx?url=' + escape(url);
		}
	}
	url += (url.indexOf('?') > 0)? '&':'?';
	url += Math.random();
	var xmlRequest;
	if (window.XMLHttpRequest)
	{
		xmlRequest = new XMLHttpRequest();
		if(xmlRequest.overrideMimeType)
		{
			xmlRequest.overrideMimeType('text/xml');
		}
	}
	else if (window.ActiveXObject)
	{
		var MSXML = new Array('MSXML2.XMLHTTP', 'Microsoft.XMLHTTP', 'MSXML2.XMLHTTP.3.0', 'MSXML2.XMLHTTP.4.0', 'MSXML2.XMLHTTP.5.0');
		for (var i=0; i<MSXML.length; i++)
		{
			try
			{		
				xmlRequest = new ActiveXObject(MSXML[i]);				
				break;
			}
			catch(e)
			{
				xmlRequest = null;
			}	
		}
	}
	if (xmlRequest != null)
	{
		xmlRequest.onreadystatechange = function()
		{
			if (xmlRequest.readyState == 4)
			{	
				if(xmlRequest.status == 0 || (xmlRequest.status >= 200 && xmlRequest.status < 300))
				{
					if (command != null)
					{				
						if (typeof(command) == 'function')
						{
							if(args != null &&  args instanceof Array)
							{
								command.apply(xmlRequest, args);
							}
							else
							{
								command.call(xmlRequest, args);
							}
						}							
						else if (typeof(command) == 'string')
						{
							command = command.replace('[Xml]', 'xmlRequest.responseXML');
							command = command.replace('[Text]', 'xmlRequest.responseText');					
							eval(command);								
						}
						else if (typeof(command) == 'object')
						{
							if(args == null)
							{
								if(command.id && command.innerHTML)
								{
									command.innerHTML = xmlRequest.responseText;
								}
							}
							else if(command[args])
							{
								command[args](xmlRequest);									
							}
						}											
					}
				}
				else
				{
					window.alert('ErrorCode:' + xmlRequest.status + '\r\n' + 'ErrorMessage:' + xmlRequest.statusText);
					window.open(url);
				}
			}
		};
		if (data == null)
		{			
			xmlRequest.open('GET', url, true);
			xmlRequest.send(null);
		}
		else
		{	
			xmlRequest.open('POST',url,true);
			xmlRequest.setRequestHeader("Content-Type","application/x-www-form-urlencoded;");
			xmlRequest.send(data);
		}
		document.request = xmlRequest;
	}
	else
	{
		window.alert('GOD SAY: YOUR BROWSER IS TOO OLD!');
	}
}