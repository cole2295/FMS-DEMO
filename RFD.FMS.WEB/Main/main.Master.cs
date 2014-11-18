using System;
using System.Web;
using System.Web.UI;

namespace RFD.FMS.WEB.Main
{
    public partial class main : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var sysName = HttpContext.Current.Request.QueryString["sysname"];
                var menuName = HttpContext.Current.Request.QueryString["menuname"];
                help.Attributes["sysname"] = sysName;
                help.Attributes["menuName"] = menuName;
                help.NavigateUrl = "javascript:void(0)";
            }
        }
    }
}