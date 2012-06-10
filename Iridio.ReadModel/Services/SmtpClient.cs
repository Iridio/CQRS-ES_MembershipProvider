using System.Net.Mail;

namespace Iridio.ReadModel.Abstracts
{
  public class SmtpClient : System.Net.Mail.SmtpClient, ISmtpClient
  {
    public SmtpClient(string host, int port)
      : base(host, port)
    {
    }

    public new void Send(MailMessage message)
    {
      base.Send(message);
    }
  }
}
