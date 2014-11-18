using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mime;
using RFD.FMS.Service.Mail;
using RFD.FMS.Util;
using RFD.FMS.Service;
using System.Windows.Forms;


namespace ServiceForWmsVsFMSDailyCheck
{
    public class Common
    {
        public static string FailedSubject = "凡客如风达财务核对服务";

        /// <summary>
        /// 执行时间间隔
        /// </summary>
        public static int ServiceInterval
        {
            get
            {
                try
                {
                    return DataConvert.ToInt(ConfigurationManager.AppSettings["ServiceInterval"]);
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
                    return "gaopengxiang@vancl.cn";
                }
            }
        }

        /// <summary>
        /// 失败时发送邮件地址
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
                    return "gaopengxiang@vancl.cn";
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
        /// topcount
        /// </summary>
        public static int TopCount
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["topCount"]);
                }
                catch (Exception ex)
                {
                    return 1;
                }
            }
        }

        public static  bool ExistInCancelStation(string id)
        {
            string cancelStation = ConfigurationManager.AppSettings["CancelStationID"];
            string []stationArray = cancelStation.Split(',');
            foreach (var s in stationArray)
            {
                if (s == id)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 发送邮件通知
        /// </summary>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        public static void SendMail(string mailSubject, string mailBody)
        {
            IMail mail = ServiceLocator.GetService<IMail>();

            mail.SendMailToUser(mailSubject, mailBody, MailAdress);
        }

        public static void SendMailByAttachment(string mailSubject,string mailBody, List<string> appendixPath)
        {
            IMail mail = ServiceLocator.GetService<IMail>();
            mail.SendMailToUser(mailSubject,mailBody,MailAdress,appendixPath);
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

        public static DateTime GetstartTime
        {
            get 
            {
                try 
                {
                    string time = ConfigurationManager.AppSettings["startDate"];
                    return !string.IsNullOrEmpty(time) ?
                        Convert.ToDateTime(ConfigurationManager.AppSettings["startDate"])
                        : DateTime.Now.AddDays(-1);
                }   
                catch(Exception ex)
                {
                    return DateTime.Now;
                }
            }
        }

         public static DateTime GetendTime
        {
            get 
            {
                try 
                {
                    string time = ConfigurationManager.AppSettings["endDate"];
                    return !string.IsNullOrEmpty(time)?
                        Convert.ToDateTime(ConfigurationManager.AppSettings["endDate"])
                        : DateTime.Now.AddDays(-1);
                }   
                catch(Exception ex)
                {
                    return DateTime.Now.AddDays(-1);
                }
            }
        }

       
         public static void WriteTest(string tips)
         {
             string Logpath = Application.StartupPath + "\\日志\\";
             if (!Directory.Exists(Logpath))
             {
                 Directory.CreateDirectory(Logpath);
             }
             var fs = new FileStream(Logpath +
                                     DateTime.Now.ToString("yyyy年MM月dd") + "logData.txt",
                                     FileMode.OpenOrCreate, FileAccess.Write);
             var mStreamWriter = new StreamWriter(fs);
             mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
             mStreamWriter.WriteLine(tips);
             mStreamWriter.Flush();
             mStreamWriter.Close();
             fs.Close();
         }

         public static string  WriteHtml(string tips)
         {
             string Logpath = Application.StartupPath + "\\日志\\";
             if (!Directory.Exists(Logpath))
             {
                 Directory.CreateDirectory(Logpath);
             }

             if (File.Exists(Logpath +
                                      "Vancl收款核对附件.html"))
             {
                 File.Delete(Logpath +
                                     "Vancl收款核对附件.html");
             }

             var fs = new FileStream(Logpath +
                                      "Vancl收款核对附件.html",
                                     FileMode.OpenOrCreate, FileAccess.Write);
             var mStreamWriter = new StreamWriter(fs,System.Text.Encoding.GetEncoding("GB2312"));
             mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
             mStreamWriter.WriteLine(tips);
             mStreamWriter.Flush();
             mStreamWriter.Close();
             fs.Close();
             return Logpath +
                   "Vancl收款核对附件.html";
         }

         public static bool ExistInCancelStationEx(string id)
         {
             string cancelStation = ConfigurationManager.AppSettings["CancelStationIDEX"];
             string[] stationArray = cancelStation.Split(',');
             foreach (var s in stationArray)
             {
                 if (s == id)
                     return true;
             }
             return false;
         }
    }
}