using System.Net.Mail;

namespace Iridio.ReadModel.Abstracts
{
  public interface ISmtpClient
  {
    void Send(MailMessage message);
  }
}
