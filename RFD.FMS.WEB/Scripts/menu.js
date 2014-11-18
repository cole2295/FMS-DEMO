OldObj=null;
function chg(id1,id2) 
{
    var chnl = document.getElementById(id1);
    var Obj = document.getElementById(id2);

	if(document.all)
	{
		
		if(chnl!=OldObj)
		{
			Obj.background='../Scripts/imgs/Menu.gif';
			chnl.style.display='';
			if(OldObj!=null){OldObj.style.display='none';}
			OldObj=chnl;
		}
		else
		{
		Obj.background='../Scripts/imgs/Menu.gif';
			chnl.style.display=chnl.style.display=='none'?'':'none';
		}
	}
}
			
