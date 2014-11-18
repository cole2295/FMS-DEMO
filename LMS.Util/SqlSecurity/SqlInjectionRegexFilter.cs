using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace RFD.FMS.Util.SqlSecurity
{
    /// <summary>
    /// 防SQL注入漏洞的HttpModule
    /// </summary>
    public class SqlInjectionRegexFilter : IHttpModule
    {
        #region IHttpModule 成员

        private readonly List<string> _ignoreList = new List<string>
                                                        {
                                                            "__VIEWSTATE"
                                                        };

        private string _redirectPageUrl;
        private Regex _sqlInjectionRegex;

        public void Dispose()
        {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            _sqlInjectionRegex = new Regex(ConfigurationManager.AppSettings["SqlInjectionRegex"],RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _redirectPageUrl = ConfigurationManager.AppSettings["RedirectPageUrl"];
            context.BeginRequest += ContextBeginRequest;
        }

        /// <summary>
        /// 截获每个请求并分析其Request参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextBeginRequest(object sender, EventArgs e)
        {
            try
            {
                var application = (HttpApplication)sender;
                HttpContext ctx = application.Context;
                if (ctx.Request.Url.ToString().ToLower().IndexOf(_redirectPageUrl.ToLower()) >= 0)
                    return;

                foreach (var key in ctx.Request.Form.Keys)
                {
                    if (_ignoreList.Contains(key.ToString()))
                        continue;
                    var value = ctx.Request[key.ToString()];
                    if (value.Length > 4000)
                        continue;
                    var match = _sqlInjectionRegex.Match(value);
                    if (match.Success)
                    {
                        application.CompleteRequest();
                        ctx.Response.Redirect(string.Format(@"~/{0}?Type=sql&Info={1}", _redirectPageUrl,
                                                            HttpContext.Current.Server.UrlEncode(match.Value)), true);
                        return;
                    }
                }
                foreach (var key in ctx.Request.QueryString.Keys)
                {

                    if (_ignoreList.Contains(key.ToString()))
                        continue;
                    var value = ctx.Request[key.ToString()];
                    if (value.Length > 4000)
                        continue;
                    var match = _sqlInjectionRegex.Match(value);
                    if (match.Success)
                    {
                        application.CompleteRequest();
                        ctx.Response.Redirect(string.Format(@"~/{0}?Type=sql&Info={1}", _redirectPageUrl,
                                                            HttpContext.Current.Server.UrlEncode(match.Value)), true);
                        return;
                    }
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch { }
            // ReSharper restore EmptyGeneralCatchClause

        }

        #endregion
    }
}