using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Common.Logging;
using System.Configuration;
using RFD.FMS.Service;
using RFD.FMS.Util;
using RFD.FMS.Service.Mail;

namespace ServiceForCalculateDedust
{
    public class Common
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Common));
        public static string FailedSubject = "配送员提成计算服务";

        /// <summary>
        /// 执行时间间隔
        /// </summary>
        public static double ServiceInterval
        {
            get
            {
                try
                {
                    return double.Parse(ConfigurationManager.AppSettings["ServiceInterval"]);
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 ServiceInterval 出错！", ex);

                    return 10 * (60 * 1000);
                }
            }
        }

        /// <summary>
        /// 失败时发送邮件地址
        /// </summary>
        public static string FailedMailAdress
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["FailedMailAdress"];
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 FailedMailAdress 出错！", ex);

                    return "Jishu_PS.list@vancl.cn";
                }
            }
        }

        /// <summary>
        /// 发送错误邮件
        /// </summary>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        public static void SendFailedMail(string mailSubject, string mailBody)
        {
            IMail mail = ServiceLocator.GetService<IMail>();

            mail.SendMailToUser(mailSubject, mailBody, FailedMailAdress);

            Logger.Info(string.Format("Send FailedMail at {0}", mailBody));
        }
    }
}
