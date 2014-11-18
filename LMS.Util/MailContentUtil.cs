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
		/// ���ر���ͷDIV
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static string GetTitleDiv(string t)
		{
			return string.Format("<div><br/><br/><b>{0}</b><br/></div>", t);
		}

		/// <summary>
		/// һ����Ϣ��ʽ
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		public static string GetRowDiv(string msg)
		{
			return string.Format("<p>{0}</p>", msg);
		}

		/// <summary>
		/// ���ݱ����ݻ�ȡHTML��ʽ���ַ�����ʾ��
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
        /// Vancl�ʼ����ͷ�����
        /// </summary>
        public const string VANCL_SMTP = "smtpsrv02.vancloa.cn";

      

        #region  Mail Members

        /// <summary>
        /// �����ʼ�
        /// </summary>
        /// <param name="mailServer">��������ַ</param>
        /// <param name="credential">NetworkCredential</param>
        /// <param name="mail">MailMessage</param>
        public static void SendMail(string mailServer, NetworkCredential credential, MailMessage mail)
        {
            var sc = new SmtpClient(mailServer) { Credentials = credential, DeliveryMethod = SmtpDeliveryMethod.Network };
            sc.Send(mail);
        }

        /// <summary>
        /// �����ʼ�
        /// </summary>
        /// <param name="mailServer">��������ַ</param>
        /// <param name="port">�˿�</param>
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
        /// �����ʼ�
        /// </summary>
        /// <param name="mailServer">�ʼ���������ַ</param>
        /// <param name="mailUser">�������ʺ�</param>
        /// <param name="mailPwd">����������</param>
        /// <param name="mailAddress">�ռ���Email��ַ(����ԷֺŸ���)</param>
        /// <param name="mailFrom">������Email��ַ</param>
        /// <param name="subject">����</param>
        /// <param name="body">����</param>
        public static void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailFrom,
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
        public static void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string cc, string bcc,
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
        /// ���ʹ�����������ʼ�
        /// ���ʹ������ʼ��ʼ�
        /// </summary>
        /// <param name="mailAddress"></param>
        /// <param name="mailSubject"></param>
        /// <param name="mailBody"></param>
        /// <param name="filePathes">������ַ�б�</param>
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
        /// ����ϵͳ�ʼ�
        /// </summary>
        /// <param name="mailSubject">�ʼ�����</param>
        /// <param name="mailContent">�ʼ�����</param>
        /// <param name="mailAddress">�ʼ���ַ(��;�ָ�)</param>
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
        /// ����ϵͳ�����ʼ�
        /// </summary>
        /// <param name="mailSubject">�ʼ�����</param>
        /// <param name="mailContent">�ʼ�����</param>
        /// <param name="mailAddress">�ʼ���ַ(��;�ָ�)</param>
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
        /// ��Ա�������ʼ�
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
        /// ʧ��ʱ�����ʼ���ַ
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