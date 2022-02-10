using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using static BuisnessLayer.Senders.SmtpOptions;

namespace BuisnessLayer.Senders
{
    public class SmtpSender : IEmailSender
    {
        SmtpClient smtpClient;
        string addressFrom;
        public SmtpSender(string hostName, int port, string userName, string userPassword)
        {
            // smtp server address and port
            smtpClient = new SmtpClient(hostName, port);

            smtpClient.Credentials = new NetworkCredential(userName, userPassword);
            smtpClient.EnableSsl = true;
            addressFrom = userName;
        }

        public SmtpSender() : this(HOST_NAME, PORT, USER_NAME, USER_PASSWORD) { }

        public void SendMess(string addressTo, string messageTheme, string messageBody)
        {
            MailAddress from = new MailAddress(addressFrom);
            MailAddress to = new MailAddress(addressTo);

            MailMessage m = new MailMessage(from, to);
            //message theme
            m.Subject = messageTheme;
            //message text
            m.Body = messageBody;

            smtpClient.Send(m);
        }
    }
}
