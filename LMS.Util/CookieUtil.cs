using System;
using System.Linq;
using System.Text;
using System.Web;
using RFD.FMS.Util.Security;
using Org.BouncyCastle.Asn1.Ocsp;

namespace RFD.FMS.Util
{
    /*
 * (C)Copyright 2011-2012 如风达信息管理系统
 * 
 * 模块名称：Cookie处理类
 * 说明：添加、删除Cookie等方法。
 * 作者：杨来旺
 * 创建日期：2011-03-0212:11:00
 * 修改人：
 * 修改时间：
 * 修改记录：
 */

    public class CookieUtil
    {
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="CookieName">Cookie名称</param>
        /// <param name="CookieValue">Cookie值</param>
        /// <param name="ExpiresTime">有效时间</param>
        public static void AddCookie(string CookieName, string CookieValue, int ExpiresTime)
        {
            CookieValue = HttpContext.Current.Server.UrlEncode(CookieValue);
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                ClearCookie(CookieName);//如果存在则清空
            }

            var MyCookie = new HttpCookie(CookieName)
                               {
                                   Value = CookiesEncrypt.Encrypt(CookieValue),
                                   Expires = DateTime.Now.AddHours(ExpiresTime),
                                   //#if !Debug
                                   //Domain = ".rufengda.com"
                                   //#endif
                               };

            HttpContext.Current.Response.Cookies.Add(MyCookie);
        }

        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="CookieName">Cookie名称</param>
        /// <param name="CookieValue">Cookie值</param>
        /// <param name="ExpiresTime">有效时间</param>
        public static void AddCookie(string CookieName, string CookieValue, int ExpiresTime, bool isDecrypt)
        {
            CookieValue = HttpContext.Current.Server.UrlEncode(CookieValue);
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                ClearCookie(CookieName);//如果存在则清空
            }

            var MyCookie = new HttpCookie(CookieName)
            {
                Value = isDecrypt ? CookiesEncrypt.Encrypt(CookieValue) : CookieValue,
                Expires = DateTime.Now.AddHours(ExpiresTime),
                Path="/"
                //#if !Debug
                //Domain = ".rufengda.com"
                //#endif
            };

            HttpContext.Current.Response.Cookies.Add(MyCookie);
        }

        public static void AddCookie(string CookieName, string CookieValue, int ExpiresTime, string domain)
        {
            CookieValue = HttpContext.Current.Server.UrlEncode(CookieValue);
            if (HttpContext.Current.Request.Cookies[CookieName] != null)
            {
                ClearCookie(CookieName);//如果存在则清空
            }

            var MyCookie = new HttpCookie(CookieName)
            {
                Value = CookiesEncrypt.Encrypt(CookieValue),
                Expires = DateTime.Now.AddHours(ExpiresTime),

                Domain = domain
                
            };

            HttpContext.Current.Response.Cookies.Add(MyCookie);
        }


        /// <summary>
        /// 根据Cookie名称获取Cookie的值
        /// </summary>
        /// <param name="CookieName"></param>
        public static string GetCookie(string CookieName)
        {
            try
            {
                return HttpContext.Current.Server.UrlDecode(CookiesEncrypt.Decrypt(HttpContext.Current.Request.Cookies[CookieName].Value));
            }
            catch (Exception ex)
            {
                throw new Exception("未找到该cookie值！" + CookieName + DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
        }

        /// <summary>
        /// 根据Cookie名称获取Cookie的值
        /// </summary>
        /// <param name="CookieName"></param>
        public static string GetCookie(string CookieName, bool isDecrypt)
        {
            try
            {
                return HttpContext.Current.Server.UrlDecode(isDecrypt ? CookiesEncrypt.Decrypt(HttpContext.Current.Request.Cookies[CookieName].Value) 
                    : HttpContext.Current.Request.Cookies[CookieName].Value);
            }
            catch (Exception ex)
            {
                throw new Exception("未找到该cookie值！" + CookieName + DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
        }

        /// <summary>
        /// 清理Cookie
        /// </summary>
        public static void ClearCookie(string CookieName)
        {
            try
            {
                if (HttpContext.Current.Request.Cookies[CookieName] != null)
                {
                    HttpCookie mycookie = HttpContext.Current.Request.Cookies[CookieName];
                    var ts = new TimeSpan(0, 0, 0, 0); //时间跨度 
                    mycookie.Expires = DateTime.Now.Add(ts); //立即过期 
                    HttpContext.Current.Response.Cookies.Remove(CookieName); //清除 
                    HttpContext.Current.Response.Cookies.Add(mycookie); //写入立即过期的*/
                    HttpContext.Current.Response.Cookies[CookieName].Expires = DateTime.Now.AddDays(-1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("操作cookie！" + CookieName + DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
        }

        public static bool ExistCookie(string cookieName)
        {
            return HttpContext.Current.Request.Cookies.AllKeys.ToList().Exists(item => item == cookieName);
        }

        public static void AddCookie<T>(string name, T value)
        {
            ClearCookie(name);
            AddCookie(name, value.ToString(), 1);
        }

        public static void AddCookie<T>(string name, T value, bool isDecrypt)
        {
            ClearCookie(name);
            AddCookie(name, value.ToString(), 1, isDecrypt);
        }

        public static void AddCookie<T>(string name, T value, int expiresTime)
        {
            ClearCookie(name);

            AddCookie(name, value.ToString(), expiresTime);
        }

        public static T GetCookie<T>(string name)
        {
            try
            {
                return DataConvert.ToValue<T>(GetCookie(name));
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static T GetCookie<T>(string name, bool isDecrypt)
        {
            try
            {
                return DataConvert.ToValue<T>(GetCookie(name, isDecrypt));
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static string GetLastCookie(HttpCookie cookies)
        {
            try
            {
                return HttpContext.Current.Server.UrlDecode(CookiesEncrypt.Decrypt(cookies.Value));
            }
            catch (Exception ex)
            {
                throw new Exception("未找到该cookie值！" + cookies.Name + DateTime.Now + ex.Message + ex.Source + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取客户端信息已经服务的错误信息
        /// </summary>
        /// <returns></returns>
        public static string GetHttpRequestAndError(Exception exception)
        {
            StringBuilder RequestAndErrorMsg = new StringBuilder();
            if (HttpContext.Current.Request.Cookies["RFDLMSUserID"] != null)
            {
                RequestAndErrorMsg.Append("访问时间：" + DateTime.Now + "<br>");
                RequestAndErrorMsg.Append("员工ID：" + CookieUtil.GetCookie("RFDLMSUserID") + "<br>");
                RequestAndErrorMsg.Append("员工名称：" + CookieUtil.GetCookie("RFDLMSUserName") + "<br>");
                RequestAndErrorMsg.Append("员工代号：" + CookieUtil.GetCookie("RFDLMSUserCode") + "<br>");
                RequestAndErrorMsg.Append("所在部门ID：" + Convert.ToInt32(CookieUtil.GetCookie("RFDLMSExpressID")) + "<br>");
                RequestAndErrorMsg.Append("所在部门名称：" + CookieUtil.GetCookie("RFDLMSExpressName") + "<br>");
                RequestAndErrorMsg.Append("配送商编号：" + CookieUtil.GetCookie("DistributionCode") + "<br>");
            }
            string user_IP = "";
            //HttpContext.Current.Request.
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                user_IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                user_IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }

            RequestAndErrorMsg.Append("客户端IP：" + user_IP + "<br>");
            RequestAndErrorMsg.Append("客户端DNS主机名：" + HttpContext.Current.Request.UserHostName + "<br>");
            RequestAndErrorMsg.Append("客户端使用平台：" + HttpContext.Current.Request.Browser.Platform + "<br>");
            RequestAndErrorMsg.Append("客户端使用浏览器：" + HttpContext.Current.Request.Browser.Type + "<br>");
            RequestAndErrorMsg.Append("客户端浏览器版本号：" + HttpContext.Current.Request.Browser.Version + "<br>");
            RequestAndErrorMsg.Append("客户端请求URL：" + HttpContext.Current.Request.Url + "<br>");
            RequestAndErrorMsg.Append(string.Format("错误页面：{0}\r\nMessage:{1}\r\nSource:{2}\r\nTrace:{3}",
                                                    HttpContext.Current.Request.Url, exception.Message, exception.Source, exception.StackTrace));
            return RequestAndErrorMsg.ToString();
        }

        /// <summary>
        /// 获取客户端信息已经服务的错误信息
        /// </summary>
        /// <returns></returns>
        public static string GetHttpRequestForLogin(Exception exception)
        {
            StringBuilder RequestAndErrorMsg = new StringBuilder();
            
            string user_IP = "";
            //HttpContext.Current.Request.
            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    user_IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
            }
            else
            {
                if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
                {
                    user_IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                }
            }

            RequestAndErrorMsg.Append("客户端IP：" + user_IP + "<br>");
            RequestAndErrorMsg.Append("客户端DNS主机名：" + HttpContext.Current.Request.UserHostName + "<br>");
            RequestAndErrorMsg.Append("客户端使用平台：" + HttpContext.Current.Request.Browser.Platform + "<br>");
            RequestAndErrorMsg.Append("客户端使用浏览器：" + HttpContext.Current.Request.Browser.Type + "<br>");
            RequestAndErrorMsg.Append("客户端浏览器版本号：" + HttpContext.Current.Request.Browser.Version + "<br>");
            return RequestAndErrorMsg.ToString();
        }
    }
}