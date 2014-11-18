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
    public partial class AreaExpressLevelViewLog : BasePage
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

        private string WareHouseType
        {
            get { return GetQueryString("wareHouseType"); }
        }

        private string MerchantID
        {
            get { return GetQueryString("merchantId"); }
        }

        private string ProductID
        {
            get { return GetQueryString("productId"); }
        }

        private void LoadLog()
        {
            var dal = ServiceLocator.GetService<IAreaExpressLevelService>();
            gvList.DataSource = dal.SearchAreaTypeLog(AreaID, ExpressCompanyID, WareHouseID, WareHouseType, MerchantID, ProductID,base.DistributionCode);
            gvList.DataBind();
        }
    }
}