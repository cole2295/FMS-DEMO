using System;
using System.Web.UI;

namespace RFD.FMS.WEB.Main
{
    public partial class Frame : Page
    {
        protected string ssouid = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            ssouid = GetQueryString("ssouid");
        }

        protected string GetQueryString(string key)
        {
            return Request.QueryString[key] ?? string.Empty;
        }
    }
}