using System;
using RFD.FMS.Util;
using RFD.SSO.WebClient;
using RFD.FMS.Service.BasicSetting;

namespace RFD.FMS.WEB
{
    public partial class Index : System.Web.UI.Page
    {
        protected string ssouid = string.Empty;
        protected string UserName = string.Empty;
        protected string CompanyName = string.Empty;
        protected string DistributionName = string.Empty;
        protected string SystemName = "FMS";
        protected string SyslistHtml = string.Empty;
        protected string SystemTitle = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            ssouid = GetQueryString("ssouid");
            UserName = LoginUser.UserName;
            CompanyName = LoginUser.ExpressName;
            DistributionName = GetDistributionName();
            SystemTitle = DistributionName + "财务管理系统";
            SyslistHtml = BuiltNavigation();
        }
        private static string GetDistributionName()
        {
            var ecDao = ServiceLocator.GetService<IExpressCompanyService>();
            return ecDao.GetDistributionNameByCode(LoginUser.DistributionCode);
        }
        protected string GetQueryString(string key)
        {
            return Request.QueryString[key] == null ? string.Empty : Request.QueryString[key].ToString();
        }
        /// <summary>
        /// 生成SSO站点链接
        /// </summary>
        private static string BuiltNavigation()
        {
            SsoClient ssoClient = new SsoClient();
            return ssoClient.GetNavigationHtml();
        }
    }
}
