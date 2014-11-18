using System;
using System.Configuration;
using Common.Logging;
using RFD.FMS.Service.Mail;
using RFD.FMS.Util;

namespace ServiceForSortingFee
{
    public class Common
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Common));
        public static readonly string FailedSubject = DateTime.Now + "拣运费用生效服务！";
        public static readonly string SuccedSubject = DateTime.Now + "拣运费用生效服务已启动！";

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
        /// 处理条数
        /// </summary>
        public static int RowCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["RowCount"]);
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 RowCount 出错！", ex);
                    return 50;
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

                    return "xueyi@vancl.cn;zengwei@vancl.cn";
                }
            }
        }


        /// <summary>
        /// 生效服务否启动
        /// </summary>
        public static bool EffectIsRun
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["EffectIsRun"] == "true";
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 EffectIsRun 出错！", ex);

                    return false;
                }
            }
        }

        /// <summary>
        /// 生效服务间隔
        /// </summary>
        public static double EffectInterval
        {
            get
            {
                try
                {
                    return double.Parse(ConfigurationManager.AppSettings["EffectInterval"]);
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 EffectInterval 出错！", ex);

                    return 1000;
                }
            }
        }

        /// <summary>
        /// 生效服务启动时间
        /// </summary>
        public static string EffectStartTime
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["EffectStartTime"];
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 EffectStartTime 出错！", ex);
                    return "00:00:01";
                }
            }
        }

        /// <summary>
        /// 生效服务执行小时范围
        /// </summary>
        public static string EffectRunHour
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["EffectRunHour"];
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 EffectRunHour 出错！", ex);
                    return "01";
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
        /// 发送错误邮件
        /// </summary>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        public static void SendFailedMail(string mailSubject, string mailBody)
        {
            try
            {
                IMail mail = ServiceLocator.GetService<IMail>();
                mail.SendMailToUser(mailSubject, mailBody, FailedMailAdress);
                Logger.Info(string.Format("Send FailedMail at {0}", mailBody));
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
