using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEB.Main;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.COD
{
	public partial class DeliveryPriceHistory : BasePage
	{
        IDeliveryPriceService deliveryPriceService = ServiceLocator.GetService<IDeliveryPriceService>();
		protected void Page_Load(object sender, EventArgs e)
		{
            BindData();
		}

        public void BindData()
        {
            var dt = deliveryPriceService.GetDeliveryPriceHistoryList(CodLineNO);
            if (dt == null || dt.Rows.Count <= 0)
            {
                RunJS("alert('没有找到价格历史');window.close();");
                return;
            }
            gvList.DataSource = dt;
            gvList.DataBind();
        }

		public string CodLineNO
		{
			get { return string.IsNullOrEmpty(Request.QueryString["lineNo"]) ? null : Request.QueryString["lineNo"].ToString(); }
		}
	}
}
