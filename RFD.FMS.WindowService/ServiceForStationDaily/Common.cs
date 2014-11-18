using System;
using System.Configuration;
using RFD.LMS.ServiceImpl;
using System.IO;

namespace ServiceForStationDaily
{
    public class Common
    {
        public static string FailedSubject = "如风达配送报表数据";

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
                    return "liuxiaogang@vancl.cn";
                }
            }
        }

        /// <summary>
        /// 日志路径
        /// </summary>
        public static string LogicFilePath
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["LogFilePath"];
                }
                catch (Exception ex)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 服务执行时间
        /// </summary>
        public static string StartTime
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["StartTime"];
                }
                catch (Exception ex)
                {
                    return "00:10";
                }
            }
        }

        /// <summary>
        /// 服务启动时间（小时）
        /// </summary>
        public static int StartHour
        {
            get
            {
                try
                {
                    return int.Parse(StartTime.Substring(0, StartTime.IndexOf(":")));
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 服务启动时间（分钟）
        /// </summary>
        public static int StartMinute
        {
            get
            {
                try
                {
                    return int.Parse(StartTime.Substring(StartTime.IndexOf(":") + 1));
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 几天前的报表数据
        /// </summary>
        public static int PreDayCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["PreDayCount"]);
                }
                catch (Exception ex)
                {
                    return 1;
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
            var mail = new Mail();
            mail.SendMailToUser(mailSubject, mailBody, FailedMailAdress);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="tips"></param>
        public static void WriteTest(string tips)
        {
            var fs = new FileStream(Common.LogicFilePath +
                                    DateTime.Now.ToString("yyyy年MM月dd") + "logData.txt",
                                    FileMode.OpenOrCreate, FileAccess.Write);
            var mStreamWriter = new StreamWriter(fs);
            mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            mStreamWriter.WriteLine(tips);
            mStreamWriter.Flush();
            mStreamWriter.Close();
            fs.Close();
        }
    }
}