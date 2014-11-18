using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Util;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB.BasicSetting
{
    public partial class AreaExpressLevelIncomeViewLog : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadLog();
        }

        private string AreaID
        {
            get { return GetQueryString("areaId"); }
        }

        private string ExpressCompanyID
        {
            get { return GetQueryString("expressCompanyId"); }
        }

        private string WareHouseID
        {
            get { return GetQueryString("wareHouseId"); }
        }

        private string MerchantID
        {
            get { return GetQueryString("merchantId"); }
        }

        private string DistributionCode
        {
            get { return GetQueryString("distributionCode"); }
        }

        private void LoadLog()
        {
            var dal = ServiceLocator.GetService<IAreaExpressLevelIncomeService>();
            gvList.DataSource = dal.SearchAreaTypeLog(AreaID, ExpressCompanyID, WareHouseID, MerchantID, DistributionCode);
            gvList.DataBind();
        }
    }
}