using System.Net.Mail;

namespace Services.Email
{
    public interface ISmtp
    {
        void Send(MailMessage message);
        string Sender();
    }
}
