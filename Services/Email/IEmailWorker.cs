using Services.Email.Models;

namespace Services.Email
{
    public interface IEmailWorker
    {
        ISmtp Smtp();
        void Send(string htmlString, EmailModel model);
    }
}
