using Services.Email.Models;
using System.Net.Mail;

namespace Services.Email
{
    public class EmailWorker : IEmailWorker
    {
        public EmailWorker(ISmtp smtp) => _smtp = smtp;

        private readonly ISmtp _smtp;

        public ISmtp Smtp() => _smtp;
        public void Send(string htmlString, EmailModel model)
        {
            var mail = new MailMessage(_smtp.Sender(), model.RecipientEmail, model.Title, htmlString) { IsBodyHtml = true };
            _smtp.Send(mail);
        }
    }
}
