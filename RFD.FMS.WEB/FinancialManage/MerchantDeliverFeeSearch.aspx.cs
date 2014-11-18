using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;

namespace RFD.FMS.WEB.FinancialManage
{
	public partial class MerchantDeliverFeeSearch : BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            Master.DistributionCode = base.DistributionCode;
            if (!IsPostBack)
            {
            }
            
		}
	}
}
