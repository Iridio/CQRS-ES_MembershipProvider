using System.Collections.Generic;
using System.Net.Mail;
using Iridio.ReadModel.Abstracts;

namespace Iridio.ReadModel.Services
{
  public class EmailService : ServiceBase, IEmailService
  {
    private string mailFrom;
    private readonly string mailAlias;
    private readonly ISmtpClient smtpClient;

    public EmailService(ISmtpClient smtpClient, string mailFrom, string mailAlias)
    {
      this.smtpClient = smtpClient;
      this.mailFrom = mailFrom;
      this.mailAlias = mailAlias;
    }

    public bool Send(string mailTo, string subject, string body, bool html)
    {
      return Send(mailTo, subject, body, html, new List<Attachment>());
    }

    public bool Send(string mailTo, string subject, string body, bool html, IList<Attachment> attachments)
    {
      var from = new MailAddress(mailFrom, mailAlias);
      var to = new MailAddress(mailTo);
      var message = new MailMessage(from, to) { Subject = subject, Body = body, IsBodyHtml = html };
      foreach (Attachment item in attachments)
        message.Attachments.Add(item);
      return SendMail(message);
    }

    private bool SendMail(MailMessage message)
    {
      try
      {
        smtpClient.Send(message);
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}