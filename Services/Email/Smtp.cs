using System;
using System.Net.Mail;

namespace Services.Email
{
    public class Smtp : ISmtp
    {
        private readonly SmtpClient _smtp;
        private readonly string _sender;
        public string Sender() => _sender;
        public Smtp(string host, string port, string hostEmail, string hostPassword)
        {
            _sender = hostEmail;
            _smtp = new SmtpClient
            {
                Host = host,
                Port = Convert.ToInt32(port),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(hostEmail, hostPassword)
            };
        }

        public void Send(MailMessage message) => _smtp.Send(message);
    }
}
