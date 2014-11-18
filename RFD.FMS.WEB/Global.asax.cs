using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using RFD.FMS.Util;
using RFD.FMS.WEB.Old_App_Code;
using System.Configuration;
using RFD.FMS.WEBLOGIC;
using System.Text;
using RFD.FMS.Service.Mail;

namespace RFD.FMS.WEB
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{

		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{
            //update by zengwei
            LogExceptionToDb();
		}

        private void LogExceptionToDb()
        {
            Exception objErr = Server.GetLastError().GetBaseException();
            log4net.ILog log = log4net.LogManager.GetLogger(Request.Url.ToString());
            if (objErr.StackTrace.IndexOf("FromBase64String") >= 0)
            {
                return;
            }
            if (objErr.Message.IndexOf("无法在流的结尾之外进行读取") >= 0)
            {
                return;
            }

            if (objErr.Message.IndexOf("无效的脚本资源") >= 0 || objErr.Message.IndexOf("invalid script resource") >= 0)
            {
                return;
            }

            if (Request.Url.ToString().IndexOf("localhost") >= 0)
            {
                return;
            }
            log.ErrorFormat("ErrorPage:{0}\r\nMessage:{1}\r\nSource:{2}\r\nTrace:{3}", Request.Url.ToString(), objErr.Message, objErr.Source, objErr.StackTrace);
            SendEmail(GetHttpRequestAndError(Request, objErr));
            Server.ClearError();
            Response.Redirect(WebSiteConst.ErrorPage);
        }

        private void LogExceptionToDb(UnhandledExceptionEventArgs e)
        {
            string title = "LMS毁灭性(Fatal)错误,请优先处理";
            SendEmail(e.ExceptionObject.ToString(), title, FatalErrorToEmailList);
            Server.ClearError();
            Response.Redirect(WebSiteConst.ErrorPage);
        }

        /// <summary>
        /// 获取客户端信息已经服务的错误信息
        /// </summary>
        /// <returns></returns>
        private string GetHttpRequestAndError(HttpRequest request, Exception exception)
        {
            StringBuilder RequestAndErrorMsg = new StringBuilder();
            //if (HttpContext.Current.Request.Cookies["RFDLMSUserID"].Value != null)
            //{
            RequestAndErrorMsg.Append("访问时间：" + DateTime.Now + "<br>");
            RequestAndErrorMsg.Append("员工ID：" + CookieUtil.GetCookie("RFDLMSUserID") + "<br>");
            RequestAndErrorMsg.Append("员工名称：" + CookieUtil.GetCookie("RFDLMSUserName") + "<br>");
            RequestAndErrorMsg.Append("员工代号：" + CookieUtil.GetCookie("RFDLMSUserCode") + "<br>");
            RequestAndErrorMsg.Append("所在部门ID：" + Convert.ToInt32(CookieUtil.GetCookie("RFDLMSExpressID")) + "<br>");
            RequestAndErrorMsg.Append("所在部门名称：" + CookieUtil.GetCookie("RFDLMSExpressName") + "<br>");
            //RequestAndErrorMsg.Append("配送商编号：" + CookieUtil.GetCookie("DistributionCode") + "<br>");
            //}
            string user_IP = "";
            if (Request.ServerVariables["HTTP_VIA"] != null)
            {
                user_IP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                user_IP = Request.ServerVariables["REMOTE_ADDR"].ToString();
            }

            RequestAndErrorMsg.Append("客户端IP：" + user_IP + "<br>");
            RequestAndErrorMsg.Append("客户端DNS主机名：" + request.UserHostName + "<br>");
            RequestAndErrorMsg.Append("客户端使用平台：" + request.Browser.Platform + "<br>");
            RequestAndErrorMsg.Append("客户端使用浏览器：" + request.Browser.Type + "<br>");
            RequestAndErrorMsg.Append("客户端浏览器版本号：" + request.Browser.Version + "<br>");
            RequestAndErrorMsg.Append("客户端请求URL：" + request.Url + "<br>");
            RequestAndErrorMsg.Append(string.Format("错误页面：{0}\r\nMessage:{1}\r\nSource:{2}\r\nTrace:{3}",
                                                    Request.Url, exception.Message, exception.Source, exception.StackTrace));
            if (exception.InnerException != null)
                RequestAndErrorMsg.Append(string.Format("\r\nInnerException：\r\nMessage:{0}\r\nSource:{1}\r\nTrace:{2}",
                                                        exception.InnerException.Message, exception.InnerException.Source, exception.InnerException.StackTrace));
            return RequestAndErrorMsg.ToString();
        }

		protected void Session_End(object sender, EventArgs e)
		{

		}

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="ErrorString"></param>
        private static void SendEmail(string ErrorString, string title, string EmailList)
        {
            var mail=ServiceLocator.GetService<IMail>();
            mail.SendMailToUser(title, ErrorString, EmailList);
        }
        private static void SendEmail(string ErrorString)
        {
            SendEmail(ErrorString, "如风达配送系统错误报告：", ErrorToEmailList);
        }

        /// <summary>
        /// 失败时发送邮件地址
        /// </summary>
        public static string ErrorToEmailList
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ErrorToEmailList"];
                }
                catch (Exception ex)
                {
                    return "Jishu_PS.list@vancl.cn";
                }
            }
        }
        /// <summary>
        /// 失败时发送邮件地址
        /// </summary>
        public static string FatalErrorToEmailList
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["FatalErrorToEmailList"];
                }
                catch (Exception ex)
                {
                    return "Jishu_PS.list@vancl.cn";
                }
            }
        }

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}