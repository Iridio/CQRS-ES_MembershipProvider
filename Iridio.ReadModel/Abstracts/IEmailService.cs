using System.Collections.Generic;
using System.Net.Mail;

namespace Iridio.ReadModel.Abstracts
{
  public interface IEmailService
  {
    bool Send(string mailTo, string subject, string body, bool html);
    bool Send(string mailTo, string subject, string body, bool html, IList<Attachment> attachments);
  }
}
