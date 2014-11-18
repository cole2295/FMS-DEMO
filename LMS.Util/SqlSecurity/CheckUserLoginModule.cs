using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RFD.FMS.Util.SqlSecurity
{
    /// <summary>
    /// 防止没有登录用户操作系统
    /// </summary>
    public class CheckUserLoginModule : IHttpModule
    {
        #region IHttpModule 成员

        private readonly List<string> _ignoreList = new List<string>
                                                        {
                                                            "Login.aspx",
                                                            "Validate.aspx",
                                                            "Error.aspx",
                                                            "SsoAuthHandler.aspx",
                                                            "Msg.aspx",
                                                            "Default.aspx"                                                            
                                                        };

        public void Dispose()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest -= new EventHandler(context_BeginRequest);
            context.BeginRequest += new EventHandler(context_BeginRequest);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                var application = (HttpApplication)sender;

                HttpContext ctx = application.Context;

                if (CheckPageExtension(ctx) == false)
                    return;

                if (CheckIsIgnorePage(ctx))
                    return;

                if (CheckIsLogin(ctx) == false)
                {
                    ctx.Response.Redirect(@"~/Login.aspx", true);
                }

            }
            catch
            {

            }
        }

        /// <summary>
        /// 是否是Aspx页面
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private bool CheckPageExtension(HttpContext ctx)
        {
            bool isAspPage = ctx.Request.AppRelativeCurrentExecutionFilePath.ToLower().Trim().IndexOf(".aspx") != -1;
            //bool isAshxPage = ctx.Request.AppRelativeCurrentExecutionFilePath.ToLower().Trim().IndexOf(".ashx") != -1;
            return isAspPage;
        }

        /// <summary>
        /// 是否是忽略页面
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private bool CheckIsIgnorePage(HttpContext ctx)
        {
            return
                _ignoreList.Any(
                    ignore =>
                    ctx.Request.AppRelativeCurrentExecutionFilePath.ToLower().Trim().IndexOf(ignore.ToLower().Trim()) != -1);
        }

        /// <summary>
        /// 是否登陆用户
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        private bool CheckIsLogin(HttpContext ctx)
        {
            bool isLogin = ctx.Request.Cookies.AllKeys.ToList().Exists(item => item.ToLower() == "RFDLMSUserID".ToLower());
            return isLogin;
        }
        #endregion
    }
}