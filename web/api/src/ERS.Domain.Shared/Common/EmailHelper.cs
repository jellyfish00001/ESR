using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Volo.Abp.DependencyInjection;

namespace ERS.Common
{
    public class EmailHelper : IScopedDependency
    {
        private IConfiguration _Configuretion;

        public EmailHelper(IConfiguration Configuretion)
        {
            _Configuretion = Configuretion;
        }
        void Send(string to, string cc, string body, string subject = null, Stream file = null, string fileName = null, Action<object, AsyncCompletedEventArgs> callBack = null, bool isHtml = true, int port = 25)
        {
            IConfiguration config = _Configuretion.GetSection("Email");
            if (config["isSend"] == "false")
                return;
            SmtpClient client = new SmtpClient();
            client.Host = config["host"];
            client.Port = port;
            if (config["isCredential"] == "true")
                client.Credentials = new NetworkCredential(config["userName"], config["password"]);

            if (callBack != null)
            {
                client.SendCompleted += new SendCompletedEventHandler(callBack);
            }

            if (string.IsNullOrEmpty(subject)) subject = config["subject"];

            MailMessage message = new MailMessage();
            message.From = new MailAddress(config["address"]);
            string[] array = to.Split(';');
            foreach (string text in array)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    message.To.Add(new MailAddress(text));
                }
            }
            array = cc.Split(';');
            foreach (string text2 in array)
            {
                if (!string.IsNullOrEmpty(text2))
                {
                    message.CC.Add(new MailAddress(text2));
                }
            }
            message.Subject = subject;
            message.SubjectEncoding = Encoding.UTF8;

            Attachment at;
            if (file != null)
            {
                at = new Attachment(file, fileName);
                message.Attachments.Add(at);
            }

            message.Body = body;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = isHtml;
            client.SendAsync(message, "userToken");

            //异步发送不能直接Dispose掉client否则发送不成功
            //client.Dispose();
            //message.Dispose();

        }

        public void SendEmail(string subject, string to, string cc, string body, Stream file = null, string fileName = null)
        {
            this.Send(to, cc, body, subject, file, fileName, callBack: (object sender, AsyncCompletedEventArgs e) =>
            {
                SmtpClient client = sender as SmtpClient;
                client.Dispose();
            });
        }
        public void SendEmail(string to, string cc, string body, Stream file = null, string fileName = null)
        {
            this.Send(to, cc, body, null, file, fileName, callBack: (object sender, AsyncCompletedEventArgs e) =>
            {
                SmtpClient client = sender as SmtpClient;
                client.Dispose();
            });
        }
    }
}
