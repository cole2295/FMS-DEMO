using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Common.Logging;
using System.Configuration;
using RFD.FMS.WEBLOGIC;

namespace ServiceIncomeAccount
{
    public class Common
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Common));
        public static string FailedSubject = "收入结算日统计服务";
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        public static string GetMachineIp()
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
            string strAddr = ipEntry.AddressList[0].ToString(); //假设本地主机为单网卡
            return strAddr;
        }

       

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
        /// 执行指定小时
        /// </summary>
        public static string RunHour
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["RunHour"].ToString();
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 ServiceInterval 出错！", ex);
                    return "20,11";
                }
            }
        }

        /// <summary>
        /// 邮件发送时间
        /// </summary>
        public static string MailSendTime
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["MailSendTime"].ToString();
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 MailSendTime 出错！", ex);
                    return "22:00";
                }
            }
        }

        /// <summary>
        /// 时间点启动timer
        /// </summary>
        /// <returns></returns>
        public static bool JudgeRunTime(string hours)
        {
            string[] notSubmits = hours.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string hour = DateTime.Now.Hour.ToString();
            for (int n = 0; n < notSubmits.Length; n++)
            {
                if (notSubmits[n] == hour)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检测订单的日期范围
        /// </summary>
        public static int DateLimit
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["DateLimit"]);
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 DateLimit 出错！", ex);
                    return 30;
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
                    return "wangxueg@vancl.cn";
                }
            }
        }

        /// <summary>
        /// 服务器列表
        /// </summary>
        public static string ServiceListPath
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ServiceListPath"];
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 ServiceListPath 出错！", ex);
                    return string.Empty;
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
            EmailService.Mail mail = new EmailService.Mail();
            mail.SendMailToUser(mailSubject, mailBody, FailedMailAdress);
            Logger.Info(string.Format("Send FailedMail at {0}", mailBody));
        }
    }
}
