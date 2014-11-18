using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Common.Logging;
using RFD.FMS.Service;
using RFD.FMS.Service.Mail;
using RFD.FMS.Util;
using System.Windows.Forms;
using System.IO;

namespace ServiceForAreaExpressLevel
{
    public class Common
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Common));
        public static string FailedSubject = "区域类型审批生效服务";

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
        /// 每次执行记录数
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
        /// 支出是否启动
        /// </summary>
        public static bool ExpendIsRun
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ExpendIsRun"] == "true" ? true : false;
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 ExpendIsRun 出错！", ex);

                    return false;
                }
            }
        }

        /// <summary>
        /// 支出计算间隔
        /// </summary>
        public static double ExpendInterval
        {
            get
            {
                try
                {
                    return double.Parse(ConfigurationManager.AppSettings["ExpendInterval"]);
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 ExpendInterval 出错！", ex);

                    return 1000;
                }
            }
        }

        /// <summary>
        /// 支出计算时间
        /// </summary>
        public static string ExpendStartTime
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ExpendStartTime"];
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 ExpendStartTime 出错！", ex);
                    return "00:00:01";
                }
            }
        }

        /// <summary>
        /// 支出计算执行小时范围
        /// </summary>
        public static string ExpendRunHour
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ExpendRunHour"];
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 ExpendRunHour 出错！", ex);
                    return "01";
                }
            }
        }

        /// <summary>
        /// 收入是否启动
        /// </summary>
        public static bool IncomeIsRun
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["IncomeIsRun"] == "true" ? true : false;
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 IncomeIsRun 出错！", ex);

                    return false;
                }
            }
        }

        /// <summary>
        /// 收入计算间隔
        /// </summary>
        public static double IncomeInterval
        {
            get
            {
                try
                {
                    return double.Parse(ConfigurationManager.AppSettings["IncomeInterval"]);
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 IncomeInterval 出错！", ex);

                    return 1000;
                }
            }
        }

        /// <summary>
        /// 收入计算时间
        /// </summary>
        public static string IncomeStartTime
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["IncomeStartTime"];
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 IncomeStartTime 出错！", ex);
                    return "00:00:01";
                }
            }
        }

        /// <summary>
        /// 收入计算执行小时范围
        /// </summary>
        public static string IncomeRunHour
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["IncomeRunHour"];
                }
                catch (Exception ex)
                {
                    Logger.Error("读取配置节点 IncomeRunHour 出错！", ex);
                    return "01";
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
            try
            {
                IMail mail = new RFD.FMS.ServiceImpl.Mail.Mail();
                mail.SendMailToUser(mailSubject, mailBody, FailedMailAdress);
                Logger.Info(string.Format("Send FailedMail at {0}", mailBody));
            }
            catch (Exception ex)
            {

            }
        }

        #region 邮件发送
        //public static void SendMails(string mailSubject, string mailBody)
        //{
        //    string formto = "crm@vancl.cn";
        //    string to = "pototallxg@qq.com";   //接收邮箱
        //    string content = mailSubject;
        //    string body = mailBody;
        //    string name = "@vancl.cn";
        //    string upass = "lxg@578694";
        //    string smtp = "cas.vancloa.cn";
        //    SmtpClient _smtpClient = new SmtpClient();
        //    _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
        //    _smtpClient.Host = smtp; //指定SMTP服务器
        //    _smtpClient.Credentials = new System.Net.NetworkCredential(name, upass);//用户名和密码
        //    MailMessage _mailMessage = new MailMessage();
        //    //发件人，发件人名 
        //    _mailMessage.From = new MailAddress(formto, "crm");
        //    //收件人 
        //    _mailMessage.To.Add(to);
        //    _mailMessage.SubjectEncoding = System.Text.Encoding.GetEncoding("gb2312");
        //    _mailMessage.Subject = content;//主题

        //    _mailMessage.Body = body;//内容
        //    _mailMessage.BodyEncoding = System.Text.Encoding.GetEncoding("gb2312");//正文编码
        //    _mailMessage.IsBodyHtml = true;//设置为HTML格式
        //    _mailMessage.Priority = MailPriority.High;//优先级   
        //    try
        //    {
        //        _smtpClient.Send(_mailMessage);
        //    }
        //    catch { }
        //}
        #endregion

        /// <summary>
        /// 时间点启动timer
        /// </summary>
        /// <returns></returns>
        public static bool JudgeRunTime(string hours)
        {
            if (string.IsNullOrEmpty(hours))
                return true;

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

        public static void WriteText(string tips)
        {
            string path = Application.StartupPath + "/日志";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fs = new FileStream(path + "/" + DateTime.Now.ToString("yyyy年MM月dd") + "logData.txt", FileMode.OpenOrCreate, FileAccess.Write);
            var mStreamWriter = new StreamWriter(fs);
            mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
            mStreamWriter.WriteLine(tips);
            mStreamWriter.Flush();
            mStreamWriter.Close();
            fs.Close();
        }
    }
}
