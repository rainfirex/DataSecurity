using System;
using System.Net.Mail;
using System.Threading;

namespace DataSecurity.Modules
{
    public delegate void SendError(string result);

    class SendMail
    {
        SmtpClient smtpClient;

        private MailMessage _message;

        private SendError _sendError;

        public static void sendOne(string from, string fromName, string to, string subject, string htmlBody, string smtpHost, int port, string user, string password, string attach = "", bool isSSL = false)
        {
            MailAddress _from = new MailAddress(from, fromName);
            MailAddress _to = new MailAddress(to);
            MailMessage message = new MailMessage(_from, _to);
            message.IsBodyHtml = true;
            message.Subject = subject;
            message.Body = htmlBody;
            if (!String.IsNullOrEmpty(attach))
                message.Attachments.Add(new Attachment(attach));

            SmtpClient smtpClient = new SmtpClient(smtpHost, port);
            smtpClient.Credentials = new System.Net.NetworkCredential(user, password);
            smtpClient.EnableSsl = isSSL;
            smtpClient.Send(message);
        }

        public SendMail(string smtpHost, int port, string user, string password, int timeout = 50, bool isSsl = false, SendError sendError = null)
        {
            smtpClient = new SmtpClient(smtpHost, port);
            smtpClient.Timeout = timeout;
            smtpClient.EnableSsl = isSsl;
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = user,
                Password = password
                
            };
            _sendError = sendError;
        }

        public SendMail InitAddress(string from, string fromName, string to, bool isBodyHtml = true)
        {
            _message = new MailMessage(new MailAddress(from, fromName), new MailAddress(to));
            _message.IsBodyHtml = isBodyHtml;
            return this;
        }

        public SendMail AddText(string subject, string htmlBody)
        {            
            _message.Subject = subject;
            _message.Body = htmlBody;
            return this;
        }

        public SendMail AddFile(string attachPath = "")
        {
            if (!String.IsNullOrEmpty(attachPath))
                _message.Attachments.Add(new Attachment(attachPath));

            return this;
        }

        public void send()
        {
            Thread th = new Thread(ThSend);
            th.IsBackground = true;
            th.Start();
        }

        private void ThSend()
        {
            try
            {
                smtpClient.Send(_message);
            }
            catch (Exception error)
            {
                if (_sendError != null)
                    _sendError(error.Message);
            }
        }
    }
}
