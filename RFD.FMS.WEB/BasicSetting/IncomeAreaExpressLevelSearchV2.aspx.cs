using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class IncomeAreaExpressLevelSearchV2 : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Master.DistributionCode = base.DistributionCode;
            }
        }

    }
}