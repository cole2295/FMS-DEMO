using System;
using System.Configuration;
using RFD.FMS.WEBLOGIC.Mail;

namespace WindowsServiceStatusChangeNotifyForFMS
{
    public class Common
    {
        public static readonly string FailedSubject = DateTime.Now + "财务运单状态变更通知服务失败！";
        public static readonly string SuccedSubject = DateTime.Now + "财务运单状态变更通知服务！";

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
                    return "zhangrongrong@vancl.cn";
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
            Mail mail = new Mail();

            mail.SendMailToUser(mailSubject, mailBody, FailedMailAdress);
        }
    }
}
