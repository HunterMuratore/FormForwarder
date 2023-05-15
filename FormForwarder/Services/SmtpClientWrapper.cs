using FormForwarder.Interfaces;
using System.Net.Mail;

namespace FormForwarder.Services
{
    public class SmtpClientWrapper : ISmtpClient
    {
        private readonly SmtpClient _smtpClient;

        public SmtpClientWrapper(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public void Send(MailMessage mailMessage) => _smtpClient.Send(mailMessage);
    }
}
