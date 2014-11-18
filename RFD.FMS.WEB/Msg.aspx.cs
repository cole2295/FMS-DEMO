using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFD.FMS.WEB
{
	public partial class Msg : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			this.Label1.Text = Request["msg"];
		}
	}
}
