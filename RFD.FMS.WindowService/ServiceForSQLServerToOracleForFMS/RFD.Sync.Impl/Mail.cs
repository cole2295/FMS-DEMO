using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RFD.Sync.Impl
{
	/// <summary>
	/// 邮件发送类
	/// </summary>
	public class Mail
	{
		/// <summary>
		/// Vancl邮件发送服务器
		/// </summary>
        public const string VANCL_SMTP = "smtpsrv02.vancloa.cn";

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailServer">服务器地址</param>
        /// <param name="credential">NetworkCredential</param>
        /// <param name="mail">MailMessage</param>
        public void SendMailDefault(string mailServer, NetworkCredential credential, MailMessage mail)
        {
            var sc = new SmtpClient(mailServer) { Credentials = credential, DeliveryMethod = SmtpDeliveryMethod.Network };
            sc.UseDefaultCredentials = true;
            sc.Send(mail);
        }

		/// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="mailServer">服务器地址</param>
		/// <param name="credential">NetworkCredential</param>
		/// <param name="mail">MailMessage</param>
		public void SendMail(string mailServer, NetworkCredential credential, MailMessage mail)
		{
			var sc = new SmtpClient(mailServer) {Credentials = credential, DeliveryMethod = SmtpDeliveryMethod.Network};
            sc.Send(mail);
		}

		/// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="mailServer">服务器地址</param>
		/// <param name="port">端口</param>
		/// <param name="credential">NetworkCredential</param>
		/// <param name="mail">MailMessage</param>
		public void SendMail(string mailServer, int port, NetworkCredential credential, MailMessage mail)
		{
			var sc = new SmtpClient(mailServer, port)
			         	{
			         		Credentials = credential,
			         		DeliveryMethod = SmtpDeliveryMethod.Network
			         	};
			sc.SendAsync(mail,null);
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
		public void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailFrom,
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
		public void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailFrom,
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
		public void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string cc, string bcc,
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
		public void SendMail(string mailServer, int port, string mailUser, string mailPwd, string mailAddress, string mailFrom,
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
        public void SendMailByAttachmentsByDefault(string mailServer, string mailUser, string mailPwd,
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
            SendMailDefault(mailServer, credential, mail);
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
		public void SendMailByAttachments(string mailServer, string mailUser, string mailPwd,
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
        public void SendMailByAttachments(string mailAddress, string mailSubject, string mailBody,
                                          IList<string> filePathes)
        {
            var mail = new MailMessage();
            var fromAddress = new MailAddress("webmaster@vancl.cn");
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
            var credential = new NetworkCredential("crm@vancl.cn", ".654sy56kj67dgb577ks");
            SendMail(VANCL_SMTP, credential, mail);
        }
		/// <summary>
		/// 发送系统邮件
		/// </summary>
		/// <param name="mailSubject">邮件标题</param>
		/// <param name="mailContent">邮件内容</param>
		/// <param name="mailAddress">邮件地址(以;分隔)</param>
		public bool SendSystemMail(string mailSubject, string mailContent, string mailAddress)
		{
			try
			{
				SendMail(VANCL_SMTP, "webmaster@vancl.cn", ".u8x9y1m4k7r3h6", mailAddress,
				         "webmaster@vancl.cn", mailSubject, mailContent);
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
		public bool SendSystemErrorMail(string mailSubject, string mailContent, string mailAddress)
		{
			try
			{
				SendMail("mail1.vancl.com", "systemmail@mail1.vancl.com", "vancl1234567890", mailAddress,
				         "systemmail@mail1.vancl.com", mailSubject, mailContent);
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
		public bool SendMailToUser(string mailSubject, string mailContent, string mailAddress)
		{
			try
			{
				SendMail(VANCL_SMTP, "crm@vancl.cn", ".654sy56kj67dgb577ks", mailAddress,
				         "lms.wuliusys.com@vancl.cn", mailSubject, mailContent, true);
			}
			catch (Exception e)
			{
				return false;
			}
			return true;
		}

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
		public void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailCc,
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
	}
}