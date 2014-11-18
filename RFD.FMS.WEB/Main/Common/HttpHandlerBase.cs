using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RFD.FMS.Util;

namespace RFD.FMS.WEB.Main
{
    public class HttpHandlerBase
    {
        /// <summary>
        /// 请求地址，子类重写来源地址
        /// </summary>
        protected virtual string RequestUrl
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckPermission(HttpContext context)
        {

            if (!CheckIsAllowUrl(context) || !CheckIsLogin(context))
                return false;
            return true;
        }

        /// <summary>
        /// 判断是否已经登录
        /// </summary>
        /// <returns></returns>
        private bool CheckIsLogin(HttpContext context)
        {
            return context.Request.Cookies.AllKeys.ToList().Exists(item => item.ToLower() == "RFDLMSUserID".ToLower());
        }

        /// <summary>
        /// 判断是否由指定页面访问
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool CheckIsAllowUrl(HttpContext context)
        {
            if (!string.IsNullOrEmpty(RequestUrl))
                return context.Request.UrlReferrer.PathAndQuery.IndexOf(this.RequestUrl) > 0;
            return true;
        }
    }
}
