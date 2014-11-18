using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Configuration;
using Common.Logging;

namespace WindowsServiceFactory
{
  
    /// <summary>
    /// 获得配置文件相关参数
    /// </summary>
    public class AppConfig
    {
        private static readonly ILog logger = GetFactoryLogger();
        private static readonly ILog loggeremail = GetFactoryEmail();

        private static int _FactoryFlashInterval;
        public static ILog GetFactoryLogger()
        {
            return LogManager.GetLogger(FactoryLog);
        }
        public static ILog GetFactoryEmail()
        {
            return LogManager.GetLogger(FactoryEMail);
        }
        public static ILog GetFactoryEmail(string arg)
        {
            return LogManager.GetLogger(arg);
        }

        public static string GetIP()
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostByName(strHostName); //取得本机IP
            string strAddr = ipEntry.AddressList[0].ToString(); //假设本地主机为单网卡
            return strAddr;
        }
        /// <summary>
        /// 工厂每次刷新时间
        /// </summary>
        public static int FactoryFlashInterval
        {
            get
            {
                try
                {
                     int.TryParse(System.Configuration.ConfigurationManager.AppSettings["FactoryFlashInterval"],out _FactoryFlashInterval);
                    return _FactoryFlashInterval;
                }
                catch (Exception ex)
                {
                    return 6000;
                }
            }
        }

        /// <summary>
        /// 工厂日志
        /// </summary>
        public static string FactoryLog
        {
            get
            {
                try
                {
                    return System.Configuration.ConfigurationManager.AppSettings["FactoryLog"];
                }
                catch (Exception ex)
                {
                    return "default_Log";
                }
            }
        }
        /// <summary>
        /// 工厂邮件
        /// </summary>
        public static string FactoryEMail
        {
            get
            {
                try
                {
                    return System.Configuration.ConfigurationManager.AppSettings["FactoryEMail"];
                }
                catch (Exception ex)
                {
                    return "default_EMail";
                }
            }
        }
    }
}
