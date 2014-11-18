using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Common.Logging;
using System.Configuration;
using RFD.FMS.Service;
using RFD.FMS.Service.Mail;
using RFD.FMS.Util;

namespace ServiceForRepairDedust
{
    public class Common
    {
        public static string FailedSubject = "提成数据修复服务";

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
        }
    }
}
