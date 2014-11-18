using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.WEBLOGIC;
using RFD.FMS.AdoNet.UnitOfWork;
using System.Transactions;
using RFD.FMS.Util;
using RFD.FMS.Service.Mail;


namespace RFD.FMS.WEB.test
{
    public partial class WebForm11 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            
        }

        protected void btShow_Click(object sender, EventArgs e)
        {
            lbShow.Text = UCMerchantSourceTV.SelectMerchantID;
        }

        protected void btnMailTest_Click(object sender, EventArgs e)
        {
            string str = "test\r\n test";
            var mailService=ServiceLocator.GetService<IMail>();
            mailService.SendMailToUser("test",str,"zengwei@vancl.cn");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label1.Text = "SelectAccountPeriodID:" + UCAccountPeriodTV.SelectAccountPeriodID
                + "<br>SelectAccountExpressID:" + UCAccountPeriodTV.SelectAccountExpressID
                + "<br>SelectAccountPeriodObjectID:" + UCAccountPeriodTV.SelectAccountPeriodObjectID
                + "<br>ExpressType:" + UCAccountPeriodTV.ExpressType
                + "<br>AccountDate:" + UCAccountPeriodTV.AccountDate
                + "<br>AccountDateStart:" + UCAccountPeriodTV.AccountDateStart
                + "<br>AccountDateEnd:" + UCAccountPeriodTV.AccountDateEnd;
        }
    }
}
