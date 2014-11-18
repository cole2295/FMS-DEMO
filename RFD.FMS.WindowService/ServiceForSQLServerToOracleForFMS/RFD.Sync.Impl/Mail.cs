using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace RFD.Sync.Impl
{
	/// <summary>
	/// �ʼ�������
	/// </summary>
	public class Mail
	{
		/// <summary>
		/// Vancl�ʼ����ͷ�����
		/// </summary>
        public const string VANCL_SMTP = "smtpsrv02.vancloa.cn";

        /// <summary>
        /// �����ʼ�
        /// </summary>
        /// <param name="mailServer">��������ַ</param>
        /// <param name="credential">NetworkCredential</param>
        /// <param name="mail">MailMessage</param>
        public void SendMailDefault(string mailServer, NetworkCredential credential, MailMessage mail)
        {
            var sc = new SmtpClient(mailServer) { Credentials = credential, DeliveryMethod = SmtpDeliveryMethod.Network };
            sc.UseDefaultCredentials = true;
            sc.Send(mail);
        }

		/// <summary>
		/// �����ʼ�
		/// </summary>
		/// <param name="mailServer">��������ַ</param>
		/// <param name="credential">NetworkCredential</param>
		/// <param name="mail">MailMessage</param>
		public void SendMail(string mailServer, NetworkCredential credential, MailMessage mail)
		{
			var sc = new SmtpClient(mailServer) {Credentials = credential, DeliveryMethod = SmtpDeliveryMethod.Network};
            sc.Send(mail);
		}

		/// <summary>
		/// �����ʼ�
		/// </summary>
		/// <param name="mailServer">��������ַ</param>
		/// <param name="port">�˿�</param>
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
		/// �����ʼ�
		/// </summary>
		/// <param name="mailServer">�ʼ���������ַ</param>
		/// <param name="mailUser">�������ʺ�</param>
		/// <param name="mailPwd">����������</param>
		/// <param name="mailAddress">�ռ���Email��ַ(����ԷֺŸ���)</param>
		/// <param name="mailFrom">������Email��ַ</param>
		/// <param name="subject">����</param>
		/// <param name="body">����</param>
		/// <param name="isHtml">�����Ƿ�HTML��ʽ</param>
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
		/// �����ʼ�
		/// </summary>
		/// <param name="mailServer">�ʼ���������ַ</param>
		/// <param name="mailUser">�������ʺ�</param>
		/// <param name="mailPwd">����������</param>
		/// <param name="mailAddress">�ռ���Email��ַ(����ԷֺŸ���)</param>
		/// <param name="mailFrom">������Email��ַ</param>
		/// <param name="subject">����</param>
		/// <param name="body">����</param>
		public void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailFrom,
		                     string subject, string body)
		{
			SendMail(mailServer, mailUser, mailPwd, mailAddress, mailFrom, subject, body, false);
		}

		/// <summary>
		/// �����ʼ�
		/// </summary>
		/// <param name="mailServer">�ʼ���������ַ</param>
		/// <param name="mailUser">�������ʺ�</param>
		/// <param name="mailPwd">����������</param>
		/// <param name="mailAddress">�ռ���Email��ַ(����ԷֺŸ���)</param>
		/// <param name="mailFrom">������Email��ַ</param>
		/// <param name="cc">����</param>
		/// <param name="bcc">����</param>
		/// <param name="subject">����</param>
		/// <param name="body">����</param>
		public void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string cc, string bcc,
		                     string mailFrom,
		                     string subject, string body)
		{
			SendMail(mailServer, mailUser, mailPwd, mailAddress, cc, bcc, mailFrom, subject, body, false);
		}

		/// <summary>
		/// �����ʼ�
		/// </summary>
		/// <param name="mailServer">�ʼ���������ַ</param>
		/// <param name="port">�˿�</param>
		/// <param name="mailUser">�������ʺ�</param>
		/// <param name="mailPwd">����������</param>
		/// <param name="mailAddress">�ռ���Email��ַ(����ԷֺŸ���)</param>
		/// <param name="mailFrom">������Email��ַ</param>
		/// <param name="subject">����</param>
		/// <param name="body">����</param>
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
        /// ���ʹ�����������ʼ�
        /// ���ʹ������ʼ��ʼ�
        /// </summary>
        /// <param name="mailPwd"></param>
        /// <param name="mailAddress"></param>
        /// <param name="mailFrom"></param>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        /// <param name="filePathes">������ַ�б�</param>
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
		/// ���ʹ�����������ʼ�
		/// ���ʹ������ʼ��ʼ�
		/// </summary>
		/// <param name="mailPwd"></param>
		/// <param name="mailAddress"></param>
		/// <param name="mailFrom"></param>
		/// <param name="mailSubject"></param>
		/// <param name="mailBody"></param>
		/// <param name="filePathes">������ַ�б�</param>
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
        /// ���ʹ�����������ʼ�
        /// ���ʹ������ʼ��ʼ�
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        /// <param name="filePathes">������ַ�б�</param>
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
		/// ����ϵͳ�ʼ�
		/// </summary>
		/// <param name="mailSubject">�ʼ�����</param>
		/// <param name="mailContent">�ʼ�����</param>
		/// <param name="mailAddress">�ʼ���ַ(��;�ָ�)</param>
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
		/// ����ϵͳ�����ʼ�
		/// </summary>
		/// <param name="mailSubject">�ʼ�����</param>
		/// <param name="mailContent">�ʼ�����</param>
		/// <param name="mailAddress">�ʼ���ַ(��;�ָ�)</param>
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
		/// ��Ա�������ʼ�
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
		/// �����ʼ�
		/// </summary>
		/// <param name="mailServer">�ʼ���������ַ</param>
		/// <param name="mailUser">�������ʺ�</param>
		/// <param name="mailPwd">����������</param>
		/// <param name="mailAddress">�ռ���Email��ַ(����ԷֺŸ���)</param>
		/// <param name="mailCc">����</param>
		/// <param name="mailBcc">����</param>
		/// <param name="mailFrom">������Email��ַ</param>
		/// <param name="subject">����</param>
		/// <param name="body">����</param>
		/// <param name="isHtml">�����Ƿ�HTML��ʽ</param>
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