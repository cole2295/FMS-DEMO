using System;
using System.Configuration;
using Common.Logging;
using RFD.FMS.Service;
using RFD.FMS.Service.Mail;
using RFD.FMS.Util;

namespace ServiceForLmsSynFms
{
    class Common
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Common));
        public static readonly string FailSubject =
            DateTime.Now + "  LMS同步FMS服务失败！";
        public static readonly string SuccessSubject =
            DateTime.Now + "  LMS同步FMS服务成功！";
        public static readonly string StartSubject =
            DateTime.Now + "  LMS同步FMS服务已启动！";
        public static readonly string StopSubject =
            DateTime.Now + "  LMS同步FMS服务已停止！";
        public static readonly string MediumNoSynFms =
            "中间表未向FMS同步订单数量";
        public static readonly string MediumSynFmsFailed =
            "中间表向FMS同步未成功订单数量";

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
                    return 500;
                }
            }
        }


        public static double Service2Interval
        {
            get
            {
                try
                {
                    return double.Parse(ConfigurationManager.AppSettings["Service2Interval"]);
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 ServiceInterval 出错！", ex);
                    return 5000;
                }
            }
        }

        /// <summary>
        /// 执行时间间隔
        /// </summary>
        public static double Service3Interval
        {
            get
            {
                try
                {
                    return double.Parse(ConfigurationManager.AppSettings["Service3Interval"]);
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 Service3Interval 出错！", ex);
                    return 10000;
                }
            }
        }

        /// <summary>
        /// 发送邮件地址
        /// </summary>
        public static string MailAdress
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["MailAdress"];
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 MailAdress 出错！", ex);
                    return "mazhonghua@vancl.cn;zengwei@vancl.cn";
                }
            }
        }

        /// <summary>
        /// 发送错误邮件
        /// </summary>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        public static void SendMail(string mailSubject, string mailBody)
        {
            IMail mail = ServiceLocator.GetService<IMail>();
            mail.SendMailToUser(mailSubject, mailBody, MailAdress);
            Logger.Info(string.Format("Send FailedMail at {0}", mailBody));
        }
    }
}
