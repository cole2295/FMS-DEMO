using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Util.ControlHelper;
using RFD.FMS.WEB.Main;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.Service.FinancialManage;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.FinancialManage
{
    public partial class ExpressPriceManage : System.Web.UI.Page
    {
        IExpressPriceService expressPriceService = ServiceLocator.GetService<IExpressPriceService>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                bindGV();
            }
        }
        private void  bindGV()
        {
            string strNO = txtCityID.Text.Trim();
            string strName = txtCityName.Text.Trim();
            gvData.DataSource = expressPriceService.GetAreaInfo(strNO, strName);
            gvData.DataBind();
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            bindGV();
        }

        protected void gvData_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }
    }
}
