using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util
{
	public class UICustomerException:Exception
	{
		public UICustomerException(string customerMsg)
		{
			this.customerMsg = customerMsg;
		}
		public string customerMsg { get; set; }
		
	}
	public class LogicException : Exception
	{
		public LogicException(string customerMsg)
		{
			this.customerMsg = customerMsg;
		}
		public LogicException(string customerMsg,string typeName)
		{
			this.customerMsg = customerMsg;
			this.typeName = typeName;
		}
		public string customerMsg { get; set; }
		public string typeName{get;set;}
	}
}
