using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RFD.FMS.Service.BasicSetting;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.test
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (TextBox2.Text.Trim().IndexOf("where") < 0 && TextBox2.Text.Trim().IndexOf("insert") > 0)
                {
                    Response.Write("没有where 条件");
                    return;
                }

                IExpressCompanyService expressCompanyService = ServiceLocator.GetService<IExpressCompanyService>();
                Response.Write(expressCompanyService.ExecSql(TextBox2.Text.Trim()));
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}