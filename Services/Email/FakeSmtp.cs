using System;
using System.IO;
using System.Linq;
using System.Net.Mail;

namespace Services.Email
{
    public class FakeSmtp : ISmtp
    {
        private readonly string _host;
        private readonly string _port;
        private readonly string _sender;
        public string FilePath => Path.Combine(Directory.GetCurrentDirectory(), $"{_host}_{_port}_");
        public string Sender() => _sender;

        public FakeSmtp(string host, string port, string hostEmail)
        {
            _host = host;
            _port = port;
            _sender = hostEmail;
        }
        public void Send(MailMessage message)
        {
            var file = new FileStream(FilePath + message.Subject + ".html", FileMode.OpenOrCreate);
            var bytearray = message.Body.ToCharArray()
                                   .Select(c => Convert.ToByte(c))
                                   .ToArray();
            file.Write(bytearray, 0, bytearray.Length);
            file.Close();
        }
    }
}
