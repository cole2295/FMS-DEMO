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
	public partial class DeliveryPriceLog : BasePage
	{
        IDeliveryPriceService service = ServiceLocator.GetService<IDeliveryPriceService>();
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
                if (string.IsNullOrEmpty(Request.QueryString["lineNo"]))
                    ViewState.Add("lineNo", "");
                else
                    ViewState.Add("lineNo", Request.QueryString["lineNo"].ToString());

				txtBeginTime.Text = string.Format("{0:s}", DateTime.Now.AddDays(-1).ToString());
				txtEndTime.Text = string.Format("{0:s}", DateTime.Now.ToString());

                gvList.DataSource = service.GetDeliveryPriceLog(CodLineNO, "", "", base.DistributionCode);
                gvList.DataBind();
			}
		}

		protected void btSearch_Click(object sender, EventArgs e)
		{
            CodLineNO = txtLineNO.Text.Trim();
            string dateStr=txtBeginTime.Text.Trim();
            string dateEnd=txtEndTime.Text.Trim();
            
            gvList.DataSource = service.GetDeliveryPriceLog(CodLineNO, dateStr, dateEnd, base.DistributionCode);
			gvList.DataBind();
		}

        public string CodLineNO
        {
            get {
                return ViewState["lineNo"].ToString();
            }
            set { ViewState.Add("lineNo", value); }
        }
	}
}
