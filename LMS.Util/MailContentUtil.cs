using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RFD.FMS.Util
{
	public class MailContentUtil
	{
		public const string SplitLine = "----------------------------------------------------------------------------------";

		/// <summary>
		/// 返回标题头DIV
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static string GetTitleDiv(string t)
		{
			return string.Format("<div><br/><br/><b>{0}</b><br/></div>", t);
		}

		/// <summary>
		/// 一行信息格式
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		public static string GetRowDiv(string msg)
		{
			return string.Format("<p>{0}</p>", msg);
		}

		/// <summary>
		/// 根据表数据获取HTML格式的字符串表示；
		/// </summary>
		/// <param name="dtResult"></param>
		/// <returns></returns>
		public static string GetHtmlTableResult(DataTable dtResult)
		{
			string result = string.Empty;

			if (dtResult.Rows.Count > 0)
			{
				string header = string.Empty;
				for (int i = 0; i < dtResult.Columns.Count; i++)
				{
					header += "<td>" + dtResult.Columns[i].ColumnName + "</td>";
				}
				result += "<table><tr>" + header + "</tr>";

				for (int i = 0; i < dtResult.Rows.Count; i++)
				{
					string rowContent = "<tr>";
					for (int j = 0; j < dtResult.Columns.Count; j++)
					{
						rowContent += "<td align='center'>" + dtResult.Rows[i][j] + "</td>";
					}
					result += rowContent + "</tr>";
				}
				result += "</table>";
			}
			return result;
		}

		public static string GetHtmlHeaderOfTable(DataTable dtResult)
		{
			string result = string.Empty;

			if (dtResult.Rows.Count > 0)
			{
				string header = string.Empty;
				for (int i = 0; i < dtResult.Columns.Count; i++)
				{
					header += "<td>" + dtResult.Columns[i].ColumnName + "</td>";
				}
				result += "<table><tr>" + header + "</tr>{0}</table>";
			}
			return result;
		}

		public static string GetHtmlRowContentOfTable(DataRow dr)
		{
			string result = "<tr>";
			for (int j = 0; j < dr.ItemArray.Length; j++)
			{
				result += "<td align='center'>" + dr[j] + "</td>";
			}
			result += "</tr>";
			return result;
		}


        /// <summary>
        /// Vancl邮件发送服务器
        /// </summary>
        public const string VANCL_SMTP = "smtpsrv02.vancloa.cn";

      

        #region  Mail Members

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailServer">服务器地址</param>
        /// <param name="credential">NetworkCredential</param>
        /// <param name="mail">MailMessage</param>
        public static void SendMail(string mailServer, NetworkCredential credential, MailMessage mail)
        {
            var sc = new SmtpClient(mailServer) { Credentials = credential, DeliveryMethod = SmtpDeliveryMethod.Network };
            sc.Send(mail);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailServer">服务器地址</param>
        /// <param name="port">端口</param>
        /// <param name="credential">NetworkCredential</param>
        /// <param name="mail">MailMessage</param>
        public static void SendMail(string mailServer, int port, NetworkCredential credential, MailMessage mail)
        {
            var sc = new SmtpClient(mailServer, port)
            {
                Credentials = credential,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            sc.SendAsync(mail, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailServer">邮件服务器地址</param>
        /// <param name="mailUser">发件人帐号</param>
        /// <param name="mailPwd">发件人密码</param>
        /// <param name="mailAddress">收件人Email地址(多个以分号隔开)</param>
        /// <param name="mailFrom">发件人Email地址</param>
        /// <param name="subject">标题</param>
        /// <param name="body">正文</param>
        /// <param name="isHtml">正文是否HTML格式</param>
        public static void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailFrom,
                             string subject, string body, bool isHtml)
        {
            var mail = new MailMessage();
            var fromAddress = new MailAddress(mailFrom);
            mail.From = fromAddress;
            mail.Subject = subject;
            mail.Body = body;
            string[] arrAddress = mailAddress.Split(';');
            foreach (string address in arrAddress)
            {
                if (!string.IsNullOrEmpty(address))
                {
                    mail.To.Add(address);
                }
            }
            mail.IsBodyHtml = isHtml;
            mail.SubjectEncoding = Encoding.GetEncoding("gb2312");
            mail.BodyEncoding = Encoding.GetEncoding("gb2312");
            var credential = new NetworkCredential(mailUser, mailPwd);
            SendMail(mailServer, credential, mail);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailServer">邮件服务器地址</param>
        /// <param name="mailUser">发件人帐号</param>
        /// <param name="mailPwd">发件人密码</param>
        /// <param name="mailAddress">收件人Email地址(多个以分号隔开)</param>
        /// <param name="mailFrom">发件人Email地址</param>
        /// <param name="subject">标题</param>
        /// <param name="body">正文</param>
        public static void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailFrom,
                             string subject, string body)
        {
            SendMail(mailServer, mailUser, mailPwd, mailAddress, mailFrom, subject, body, false);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailServer">邮件服务器地址</param>
        /// <param name="mailUser">发件人帐号</param>
        /// <param name="mailPwd">发件人密码</param>
        /// <param name="mailAddress">收件人Email地址(多个以分号隔开)</param>
        /// <param name="mailFrom">发件人Email地址</param>
        /// <param name="cc">抄送</param>
        /// <param name="bcc">密送</param>
        /// <param name="subject">标题</param>
        /// <param name="body">正文</param>
        public static void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string cc, string bcc,
                             string mailFrom,
                             string subject, string body)
        {
            SendMail(mailServer, mailUser, mailPwd, mailAddress, cc, bcc, mailFrom, subject, body, false);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailServer">邮件服务器地址</param>
        /// <param name="port">端口</param>
        /// <param name="mailUser">发件人帐号</param>
        /// <param name="mailPwd">发件人密码</param>
        /// <param name="mailAddress">收件人Email地址(多个以分号隔开)</param>
        /// <param name="mailFrom">发件人Email地址</param>
        /// <param name="subject">标题</param>
        /// <param name="body">正文</param>
        /// <param name="isHtml"></param>
        public static void SendMail(string mailServer, int port, string mailUser, string mailPwd, string mailAddress, string mailFrom,
                             string subject, string body, bool isHtml)
        {
            var mail = new MailMessage();
            var fromAddress = new MailAddress(mailFrom);
            mail.From = fromAddress;
            mail.Subject = subject;
            mail.Body = body;
            string[] arrAddress = mailAddress.Split(';');
            foreach (string address in arrAddress)
            {
                mail.To.Add(address);
            }
            mail.IsBodyHtml = isHtml;
            mail.SubjectEncoding = Encoding.GetEncoding("gb2312");
            mail.BodyEncoding = Encoding.GetEncoding("gb2312");
            var credential = new NetworkCredential(mailUser, mailPwd);
            SendMail(mailServer, port, credential, mail);
        }


        /// <summary>
        /// 发送带多个附件的邮件
        /// 发送带附件邮件邮件
        /// </summary>
        /// <param name="mailPwd"></param>
        /// <param name="mailAddress"></param>
        /// <param name="mailFrom"></param>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        /// <param name="filePathes">附件地址列表</param>
        /// <param name="mailServer"></param>
        /// <param name="mailUser"></param>
        public static void SendMailByAttachments(string mailServer, string mailUser, string mailPwd,
                                          string mailAddress, string mailFrom, string mailSubject, string mailBody,
                                          IList<string> filePathes)
        {
            var mail = new MailMessage();
            var fromAddress = new MailAddress(mailFrom);
            mail.From = fromAddress;
            string[] arrTo = mailAddress.Split(';');
            foreach (string t in arrTo)
            {
                mail.To.Add(new MailAddress(t));
            }
            mail.Subject = mailSubject;
            mail.Body = mailBody;
            mail.IsBodyHtml = true;

            foreach (string path in filePathes)
            {
                mail.Attachments.Add(new Attachment(path));
            }

            mail.SubjectEncoding = Encoding.GetEncoding("gb2312");
            mail.BodyEncoding = Encoding.GetEncoding("gb2312");
            var credential = new NetworkCredential(mailUser, mailPwd);
            SendMail(mailServer, credential, mail);
        }
        /// <summary>
        /// 发送带多个附件的邮件
        /// 发送带附件邮件邮件
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        /// <param name="filePathes">附件地址列表</param>
        public static void SendMailByAttachments(string mailAddress, string mailSubject, string mailBody,
                                          IList<string> filePathes)
        {
            var mail = new MailMessage();
            var fromAddress = new MailAddress("fms.wuliusys.com@vancl.cn");
            mail.From = fromAddress;
            string[] arrTo = mailAddress.Split(';');
            foreach (string t in arrTo)
            {
                mail.To.Add(new MailAddress(t));
            }
            mail.Subject = mailSubject;
            mail.Body = mailBody;
            mail.IsBodyHtml = true;

            foreach (string path in filePathes)
            {
                mail.Attachments.Add(new Attachment(path));
            }

            mail.SubjectEncoding = Encoding.GetEncoding("gb2312");
            mail.BodyEncoding = Encoding.GetEncoding("gb2312");
            var credential = new NetworkCredential("fms.wuliusys.com@vancl.cn", "");
            SendMail(VANCL_SMTP, credential, mail);
        }
        /// <summary>
        /// 发送系统邮件
        /// </summary>
        /// <param name="mailSubject">邮件标题</param>
        /// <param name="mailContent">邮件内容</param>
        /// <param name="mailAddress">邮件地址(以;分隔)</param>
        public static bool SendSystemMail(string mailSubject, string mailContent, string mailAddress)
        {
            try
            {
                SendMail(VANCL_SMTP, "fms.wuliusys.com@vancl.cn", "", mailAddress,
                         "fms.wuliusys.com@vancl.cn", mailSubject, mailContent);
            }
            catch (Exception e)
            {
                
                return false;
            }
            return true;
            //SendSystemErrorMail(mailSubject, mailContent, mailAddress);
        }

        /// <summary>
        /// 发送系统错误邮件
        /// </summary>
        /// <param name="mailSubject">邮件标题</param>
        /// <param name="mailContent">邮件内容</param>
        /// <param name="mailAddress">邮件地址(以;分隔)</param>
        public static bool SendSystemErrorMail(string mailSubject, string mailContent, string mailAddress)
        {
            try
            {
                SendMail(VANCL_SMTP, "fms.wuliusys.com@vancl.cn", "", mailAddress,
                         "fms.wuliusys.com@vancl.cn", mailSubject, mailContent);
            }
            catch (Exception e)
            {
               
                return false;
            }
            return true;
        }

        /// <summary>
        /// 给员工发送邮件
        /// </summary>
        /// <param name="mailSubject"></param>
        /// <param name="mailContent"></param>
        /// <param name="mailAddress"></param>
        public static bool SendMailToUser(string mailSubject, string mailContent, string mailAddress)
        {
            try
            {
                SendMail(VANCL_SMTP, "fms.wuliusys.com@vancl.cn", "", mailAddress,
                         "fms.wuliusys.com@vancl.cn", mailSubject, mailContent, true);
            }
            catch (Exception e)
            {
              
                return false;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailServer">邮件服务器地址</param>
        /// <param name="mailUser">发件人帐号</param>
        /// <param name="mailPwd">发件人密码</param>
        /// <param name="mailAddress">收件人Email地址(多个以分号隔开)</param>
        /// <param name="mailCc">抄送</param>
        /// <param name="mailBcc">密送</param>
        /// <param name="mailFrom">发件人Email地址</param>
        /// <param name="subject">标题</param>
        /// <param name="body">正文</param>
        /// <param name="isHtml">正文是否HTML格式</param>
        public static void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailCc,
                             string mailBcc, string mailFrom,
                             string subject, string body, bool isHtml)
        {
            var mail = new MailMessage();
            var fromAddress = new MailAddress(mailFrom);
            mail.From = fromAddress;
            mail.Subject = subject;
            mail.Body = body;
            string[] arrAddress = mailAddress.Split(';');
            foreach (string address in arrAddress)
            {
                if (!string.IsNullOrEmpty(address))
                {
                    mail.To.Add(address);
                }
            }
            string[] arrCc = mailCc.Split(';');
            foreach (string cc in arrCc)
            {
                if (!string.IsNullOrEmpty(cc))
                {
                    mail.CC.Add(cc);
                }
            }
            string[] arrBcc = mailBcc.Split(';');
            foreach (string bcc in arrBcc)
            {
                if (!string.IsNullOrEmpty(bcc))
                {
                    mail.Bcc.Add(bcc);
                }
            }

            mail.IsBodyHtml = isHtml;
            mail.SubjectEncoding = Encoding.GetEncoding("gb2312");
            mail.BodyEncoding = Encoding.GetEncoding("gb2312");
            var credential = new NetworkCredential(mailUser, mailPwd);
            SendMail(mailServer, credential, mail);
        }

       
        
        /// <summary>
        /// 失败时发送邮件地址
        /// </summary>
        public static string ErrorToEmailList
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ErrorToEmailList"];
                }
                catch (Exception ex)
                {
                    return "Jishu_PS.list@vancl.cn";
                }
            }
        }
	}
}