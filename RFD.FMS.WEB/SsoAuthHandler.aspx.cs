using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using RFD.FMS.Util;
using RFD.SSO.WebClient;

namespace RFD.FMS.WEB
{
    public partial class SsoAuthHandler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoginType.IsSsoLogin = true;

            if (!IsPostBack)
            {
                SsoClient ssoClinet = new SsoClient();
                ProcessLogin processLogin = new ProcessLogin();

                if (!ssoClinet.Login(ssoClinet.QueryUidFromUrl(),
                    ssoClinet.QueryTokenFromUrl(),
                    processLogin))
                {
                    ssoClinet.RedirectToLoginPage();
                }
                Response.Redirect(string.Format("index.aspx?ssotoken={0}&ssouid={1}",
                       HttpContext.Current.Request.QueryString["ssotoken"],
                       HttpContext.Current.Request.QueryString["ssouid"]), true);
            }
        }
    }

    public class ProcessLogin : IProcessLoginInfo
    {
        public void ProcessLoginInfo(SsoResponse loginInfo)
        {
            var userid = loginInfo.EmployeeID.ToString();
            var userCode = loginInfo.EmployeeCode;
            var userName = loginInfo.EmployeeName;
            var expressId = loginInfo.StationID;
            var expressName = loginInfo.Companyname;
            var distributionCode = loginInfo.DistributionCode;
            var sysManager = loginInfo.SysManager;

            CookieUtil.AddCookie("RFDLMSUserID", userid, 18);
            CookieUtil.AddCookie("RFDLMSUserCode", userCode, 18);
            CookieUtil.AddCookie("RFDLMSUserName", userName, 18);
            CookieUtil.AddCookie("RFDLMSExpressID", expressId, 18);
            CookieUtil.AddCookie("RFDLMSExpressName", expressName, 18);
            CookieUtil.AddCookie("DistributionCode", distributionCode, 18);
            CookieUtil.AddCookie("SysManager", sysManager, 18);
        }
    }
}

