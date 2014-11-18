using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RFD.FMS.WEB
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(Request["msg"]);
            DisplayErrorInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayErrorInfo()
        {
            var type = Request["Type"];
            if (type != null && type.ToLower().Trim() == "sql")
            {
                Response.Write(string.Format(@"输入有误，请确认内容：{0}", Request["Info"]));
            }
        }
    }
}
