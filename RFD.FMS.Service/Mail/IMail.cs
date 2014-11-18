using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using Common.Logging;

namespace RFD.FMS.Service.Mail
{
    public interface IMail
    {
        void SendMail(string mailServer, int port, System.Net.NetworkCredential credential, System.Net.Mail.MailMessage mail);
        void SendMail(string mailServer, int port, string mailUser, string mailPwd, string mailAddress, string mailFrom, string subject, string body, bool isHtml);
        void SendMail(string mailServer, System.Net.NetworkCredential credential, System.Net.Mail.MailMessage mail);
        void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string cc, string bcc, string mailFrom, string subject, string body);
        void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailCc, string mailBcc, string mailFrom, string subject, string body, bool isHtml);
        void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailFrom, string subject, string body);
        void SendMail(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailFrom, string subject, string body, bool isHtml);
        void SendMailByAttachments(string mailServer, string mailUser, string mailPwd, string mailAddress, string mailFrom, string mailSubject, string mailBody, System.Collections.Generic.IList<string> filePathes);
        bool SendMailToUser(string mailSubject, string mailContent, string mailAddress);
        bool SendMailToUser(string mailSubject, string mailContent, string mailAddress, List<string>appendixPath);
        bool SendSystemErrorMail(string mailSubject, string mailContent, string mailAddress);
        bool SendSystemMail(string mailSubject, string mailContent, string mailAddress);
    }
}
