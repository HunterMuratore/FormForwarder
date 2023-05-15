using System.Net.Mail;

namespace FormForwarder.Interfaces
{
    public interface ISmtpClient
    {
        void Send(MailMessage mailMessage);
    }
}
